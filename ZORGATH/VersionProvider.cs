namespace ZORGATH;

public class VersionProvider
{
    private class VersionInfo
    {
        public string VersionIgnoringHotfix; // e.g. 4.10.6
        public string Version;               // e.g. 4.10.6.1
        public long RefreshTime;

        public VersionInfo(string versionIgnoringHotfix, string version, long refreshTime)
        {
            VersionIgnoringHotfix = versionIgnoringHotfix;
            Version = version;
            RefreshTime = refreshTime;
        }
    }

    private readonly HttpClient _httpClient;
    private readonly string _path;
    private readonly long _refreshPeriod = Stopwatch.Frequency * 60 * 5; // once every 5 minutes.
    private readonly ConcurrentDictionary<string, VersionInfo> _versions = new();

    public VersionProvider(string versionHost, string path)
    {
        _httpClient = new()
        {
            BaseAddress = new Uri(versionHost),
        };
        _path = path;

        long refreshTime = Stopwatch.GetTimestamp() + _refreshPeriod;

        _versions["was-crIac6LASwoafrl8FrOa/x86_64"] = new VersionInfo("0.0.0", "0.0.0.0", refreshTime);
        ScheduleRefresh("was-crIac6LASwoafrl8FrOa/x86_64");

        _versions["wxs-crIac6LASwoafrl8FrOa/x86_64"] = new VersionInfo("0.0.0", "0.0.0.0", refreshTime);
        ScheduleRefresh("wxs-crIac6LASwoafrl8FrOa/x86_64");

        _versions["las-crIac6LASwoafrl8FrOa/x86-biarch"] = new VersionInfo("0.0.0", "0.0.0.0", refreshTime);
        ScheduleRefresh("las-crIac6LASwoafrl8FrOa/x86-biarch");

        _versions["wac/x86_64"] = new VersionInfo("0.0.0", "0.0.0.0", refreshTime);
        ScheduleRefresh("wac/x86_64");

        _versions["wxc/x86_64"] = new VersionInfo("0.0.0", "0.0.0.0", refreshTime);
        ScheduleRefresh("wxc/x86_64");

        _versions["lac/x86-biarch"] = new VersionInfo("0.0.0", "0.0.0.0", refreshTime);
        ScheduleRefresh("lac/x86-biarch");

        _versions["mac/universal-64"] = new VersionInfo("0.0.0", "0.0.0.0", refreshTime);
        ScheduleRefresh("mac/universal-64");
    }

    public string ObtainGameVersionIgnoringHotfix(string os, string arch)
    {
        long now = Stopwatch.GetTimestamp();
        string key = os + '/' + arch;
        if (_versions.TryGetValue(key, out var versionInfo))
        {
            lock (versionInfo)
            {
                if (now > versionInfo.RefreshTime)
                {
                    versionInfo.RefreshTime = now + _refreshPeriod;
                    ScheduleRefresh(key);
                }
            }
            return versionInfo.VersionIgnoringHotfix;
        }
        else
        {
            // unknown version.
            return "0.0.0";
        }
    }

    public string ObtainGameVersion(string os, string arch)
    {
        long now = Stopwatch.GetTimestamp();
        string key = os + '/' + arch;
        if (_versions.TryGetValue(key, out var versionInfo))
        {
            lock (versionInfo)
            {
                if (now > versionInfo.RefreshTime)
                {
                    versionInfo.RefreshTime = now + _refreshPeriod;
                    ScheduleRefresh(key);
                }
            }
            return versionInfo.Version;
        }
        else
        {
            // unknown version.
            return "0.0.0";
        }
    }

    private void ScheduleRefresh(string key)
    {
        Task.Run(async () => await Refresh(key));
    }

    private async Task Refresh(string key)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(_path + key + "/version.cfg");
        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            string[] values = result.Split(';');
            string version = values[0];
            string versionIgnoringHotfix;
            if (version.Split('.').Length == 4)
            {
                // includes a hotfix, drop the hotfix
                versionIgnoringHotfix = version.Substring(0, version.LastIndexOf('.'));
            }
            else
            {
                versionIgnoringHotfix = version;
            }


            var versionInfo = _versions[key];
            lock (versionInfo)
            {
                versionInfo.VersionIgnoringHotfix = versionIgnoringHotfix;
                versionInfo.Version = version;
            }
        }
    }
}
