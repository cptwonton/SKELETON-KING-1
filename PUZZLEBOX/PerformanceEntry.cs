namespace PUZZLEBOX;

public class PerformanceEntry
{
    [Key]
    public int Id { get; set; }

    // broad category of the entry (e.g. ClientRequester, ServerRequester, ChatServer etc)
    [Required]
    public string Category { get; set; } = null!;

    // subcategory within broader scope (e.g. server_list, pre_auth, srpAuth etc).
    [Required]
    public string Subcategory { get; set; } = null!;

    // When this entry was collected.
    public DateTime Date { get; set; }

    // overall duration of the request
    public long Duration { get; set; }

    // ChatServer specific extensions.
    public long BeforeProcessDuration { get; set; }

    public long ProcessDuration { get; set; }

    public long AfterProcessDuration { get; set; }

    // How many times this particular method has been invoked during the accumulation period.
    public int TimesCalled { get; set; }
}
