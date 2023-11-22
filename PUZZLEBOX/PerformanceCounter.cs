namespace PUZZLEBOX;

public class PerformanceCounter : IDisposable
{
    // Accumulates PerformanceEntries for a batched insert into a DB.
    private static readonly ConcurrentBag<PerformanceEntry> _performanceEntries = new();
    // Resolution of the high-performance timer.
    private static readonly long _frequency = Stopwatch.Frequency;
    // How frequently we want to write to the database
    private static readonly long _databaseWriteFrequency = _frequency * 60 * 2; // once every hour
    private static long _lastTimeWroteToDatabase = Stopwatch.GetTimestamp();

    private readonly IServiceProvider? _serviceProvider;
    private readonly IDbContextFactory<BountyContext>? _dbContextFactory;
    private readonly long _timestamp = Stopwatch.GetTimestamp();

    public string Category = "Unknown";
    public string Subcategory = "Unknown";

    public PerformanceCounter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public PerformanceCounter(IDbContextFactory<BountyContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public void Dispose()
    {
        FinishCollection();
    }

    private void FinishCollection()
    {
        _performanceEntries.Add(new PerformanceEntry()
        {
            Category = Category,
            Subcategory = Subcategory,
            Duration = Stopwatch.GetTimestamp() - _timestamp,
            TimesCalled = 1,
        });

        MaybeSaveToDatabase();
    }

    private void MaybeSaveToDatabase()
    {
        long now = _timestamp;
        long lastTimeWroteToDatabase = _lastTimeWroteToDatabase;
        if (now - lastTimeWroteToDatabase < _databaseWriteFrequency)
        {
            // Not enough time has passed.
            return;
        }

        // Now is a good time. Atomically swap the value so that 2 threads don't try to do the write
        // at the exact same time.
        if (Interlocked.CompareExchange(ref _lastTimeWroteToDatabase, now, lastTimeWroteToDatabase) != lastTimeWroteToDatabase)
        {
            // CompareExchange failed, some other thread did this first.
            return;
        }

        // Go over the collected entries and aggregate them to reduce the amount of data stored:
        // countersByCategoryAndSubcategory[category][subcategory] = aggregatedEntry;
        // Then flatten the Dictionary of Dictionaries into a list and saved them into the database.
        Dictionary<string, Dictionary<string, PerformanceEntry>> countersByCategoryAndSubcategory = new();

        // Note that the PerformanceEntries entries are still in timer-specific ticks.
        // They will be converted into nanoseconds before inserting them into a database
        // to avoid converting each individual entry int nanoseconds format.
        int numberOfDistinctEntries = 0;
        while (_performanceEntries.TryTake(out var performanceEntry))
        {
            string category = performanceEntry.Category;
            string subcategory = performanceEntry.Subcategory;

            Dictionary<string, PerformanceEntry> countersBySubcategory;
            if (countersByCategoryAndSubcategory.TryGetValue(category, out var countersBySubcategoryTmp))
            {
                countersBySubcategory = countersBySubcategoryTmp;
            }
            else
            {
                countersBySubcategory = new Dictionary<string, PerformanceEntry>();
                countersByCategoryAndSubcategory[category] = countersBySubcategory;
            }

            if (countersBySubcategory.TryGetValue(subcategory, out var aggregatedEntry))
            {
                // Existing entry found. Add to it.
                aggregatedEntry.Duration += performanceEntry.Duration;
                aggregatedEntry.TimesCalled++;
            }
            else
            {
                // No existing entry. Use existing entry as the basis.
                ++numberOfDistinctEntries;
                countersBySubcategory[subcategory] = performanceEntry;
            }
        }

        DateTime date = DateTime.UtcNow;
        using BountyContext bountyContext = _serviceProvider != null ? _serviceProvider.GetRequiredService<BountyContext>() : _dbContextFactory!.CreateDbContext();
        foreach (var countersByCategory in countersByCategoryAndSubcategory)
        {
            foreach (var counters in countersByCategory.Value)
            {
                PerformanceEntry entry = counters.Value;

                entry.Date = date;
                entry.Duration = (entry.Duration / entry.TimesCalled) * 1000000 / _frequency;
                bountyContext.Add(entry);
            }
        }
        bountyContext.SaveChanges();
    }
}
