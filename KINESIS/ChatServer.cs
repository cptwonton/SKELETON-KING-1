namespace KINESIS;

public class ChatServer
{
    private readonly IDbContextFactory<BountyContext> _dbContextFactory;
    private readonly IProtocolRequestFactory<ConnectedClient> _clientProtocolRequestFactory;
    private readonly IProtocolRequestFactory<ConnectedServer> _serverProtocolRequestFactory;
    private readonly IProtocolRequestFactory<ConnectedManager> _managerProtocolRequestFactory;
    private readonly TcpListener _clientListener;
    private readonly TcpListener _serverListener;
    private readonly TcpListener _managerListener;

    public static readonly ConcurrentDictionary<int, ConnectedClient> ConnectedClientsByAccountId = new();
    public static readonly ConcurrentDictionary<int, ConnectedServer> ConnectedServersByServerId = new();
    public static readonly ConcurrentDictionary<int, ConnectedManager> ConnectedManagersByAccountId = new();
    public static readonly ConcurrentDictionary<int, Client.ChatChannel> ChatChannelsByChannelId = new();
    public static readonly ConcurrentDictionary<string, Client.ChatChannel> ChatChannelsByUpperCaseName = new();
    public static readonly ConcurrentDictionary<string, ConcurrentDictionary<ConnectedServer,bool>> IdleServersByRegion = new();
    public static readonly ConcurrentDictionary<int, ConcurrentDictionary<ConnectedServer, bool>> IdleServersByManager = new();
    public static readonly ConcurrentDictionary<ConnectedServer, bool> ConnectedServers = new();
    public static readonly ConcurrentDictionary<ConnectedManager, bool> ConnectedManagers = new();

    public static Matchmaking.MatchmakingSettingsResponse MatchmakingSettingsResponse = null!;

    public ChatServer(IDbContextFactory<BountyContext> dbContextFactory, ChatServerConfiguration chatServerConfiguration, IConfiguration configuration)
    {
        _dbContextFactory = dbContextFactory;

        _clientListener = new TcpListener(IPAddress.Any, chatServerConfiguration.ClientPort);
        _serverListener = new TcpListener(IPAddress.Any, chatServerConfiguration.ServerPort);
        _managerListener = new TcpListener(IPAddress.Any, chatServerConfiguration.ManagerPort);

        _clientProtocolRequestFactory = new ClientProtocolRequestFactory();
        _serverProtocolRequestFactory = new ServerProtocolRequestFactory();
        _managerProtocolRequestFactory = new ManagerProtocolRequestFactory();

        MatchmakingSettingsResponse = CreateMatchmakingSettingsResponse(configuration);
    }

    public void Start()
    {
        _clientListener.Start();
        _clientListener.BeginAcceptSocket(AcceptClientSocketCallback, this);

        _serverListener.Start();
        _serverListener.BeginAcceptSocket(AcceptServerSocketCallback, this);

        _managerListener.Start();
        _managerListener.BeginAcceptSocket(AcceptManagerSocketCallback, this);
    }

    public void Stop()
    {
        _clientListener.Stop();
    }

    public static void AcceptClientSocketCallback(IAsyncResult result)
    {
        ChatServer chatServer = (result.AsyncState as ChatServer)!;

        TcpListener clientListener = chatServer._clientListener;
        Socket socket;
        try
        {
            socket = clientListener.EndAcceptSocket(result);

            ConnectedClient client = new(socket, chatServer._clientProtocolRequestFactory, chatServer._dbContextFactory);
            client.Start();
        }
        finally
        {
            // Prepare for another connection.
            clientListener.BeginAcceptSocket(AcceptClientSocketCallback, chatServer);
        }
    }

    public static void AcceptServerSocketCallback(IAsyncResult result)
    {
        ChatServer chatServer = (result.AsyncState as ChatServer)!;

        TcpListener serverListener = chatServer._serverListener;
        Socket socket;
        try
        {
            socket = serverListener.EndAcceptSocket(result);

            ConnectedServer server = new(socket, chatServer._serverProtocolRequestFactory, chatServer._dbContextFactory);
            server.Start();
        }
        finally
        {
            // Prepare for another connection.
            serverListener.BeginAcceptSocket(AcceptServerSocketCallback, chatServer);
        }
    }

    public static void AcceptManagerSocketCallback(IAsyncResult result)
    {
        ChatServer chatServer = (result.AsyncState as ChatServer)!;

        TcpListener managerListener = chatServer._managerListener;
        Socket socket;
        try
        {
            socket = managerListener.EndAcceptSocket(result);

            ConnectedManager manager = new(socket, chatServer._managerProtocolRequestFactory, chatServer._dbContextFactory);
            manager.Start();
        }
        finally
        {
            // Prepare for another connection.
            managerListener.BeginAcceptSocket(AcceptManagerSocketCallback, chatServer);
        }
    }

    private static Matchmaking.MatchmakingSettingsResponse CreateMatchmakingSettingsResponse(IConfiguration configuration)
    {
        string BasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Matchmaking");
        Matchmaking.MatchmakingConfiguration matchmakingConfiguration = System.Text.Json.JsonSerializer.Deserialize<Matchmaking.MatchmakingConfiguration>(File.ReadAllText(Path.Combine(BasePath, "MatchmakingConfiguration.JSON")))!;

        Matchmaking.MatchmakingMapConfiguration caldavar = matchmakingConfiguration.Caldavar;
        Matchmaking.MatchmakingMapConfiguration midwars = matchmakingConfiguration.MidWars;

        string enabledRegions = string.Join('|', caldavar.Regions);

        // This list appears to only be used by the old UI and must match
        // the maps/modes enabled below.
        List<GameFinder.TMMGameType> enabledGameTypes = new()
        {
            GameFinder.TMMGameType.MIDWARS,
            GameFinder.TMMGameType.CAMPAIGN_NORMAL
        };

        List<string> enabledMaps = new();
        if (caldavar.Modes.Length != 0)
        {
            enabledMaps.Add("caldavar");
        }
        if (midwars.Modes.Length != 0)
        {
            enabledMaps.Add("midwars");
        }

        HashSet<string> enabledGameModes = new();
        foreach (string mode in caldavar.Modes)
        {
            enabledGameModes.Add(mode);
        }
        foreach (string mode in midwars.Modes)
        {
            enabledGameModes.Add(mode);
        }

        List<string> unsupportedCombinations = new();
        foreach (string mode in enabledGameModes)
        {
            if (!midwars.Modes.Contains(mode))
            {
                unsupportedCombinations.Add("con->" + mode);
            }
            if (!midwars.Modes.Contains(mode))
            {
                unsupportedCombinations.Add("midwars->" + mode);
            }
        }

        int numberOfEnabledRegions = 0;
        if (enabledRegions.Length != 0)
        {
            numberOfEnabledRegions = enabledRegions.Count(c => c == '|') + 1;
        }

        return new Matchmaking.MatchmakingSettingsResponse(
            matchmakingAvailability: numberOfEnabledRegions == 0 ? (byte)0 : (byte)1,
            availableMaps: string.Join("|", enabledMaps),
            gameTypes: string.Join("|", enabledGameTypes),
            gameModes: string.Join("|", enabledGameModes),
            availableRegions: enabledRegions,
            disabledGameModesByGameType: "",
            disabledGameModesByRankType: "",
            disabledGameModesByMap: string.Join("|", unsupportedCombinations),
            restrictedRegions: "",
            clientCountryCode: "",
            legend: "maps:modes:regions:",
            popularityByGameMap: new byte[enabledMaps.Count],
            popularityByGameType: new byte[enabledGameTypes.Count],
            popularityByGameMode: new byte[enabledGameModes.Count],
            popularityByRegion: new byte[numberOfEnabledRegions],
            customMapRotationTime: 0
        );
    }
}
