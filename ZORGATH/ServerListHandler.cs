namespace ZORGATH;

internal class ServerForCreateListResponse
{
    internal ServerForCreateListResponse(string cookie, string? region)
    {
        // Only add servers that we know are idle.
        switch (region)
        {
            case null:
                // Blank region indicates ALL servers.
                AddServers(KINESIS.ChatServer.ConnectedServers);
                break;

            case "AUTO":
                foreach (var kvp in KINESIS.ChatServer.ConnectedManagers)
                {
                    var connectedManager = kvp.Key;
                    var managerState = connectedManager.ManagerState;

                    Servers.Add(new ServerInfo(managerState.AccountId, managerState.Address, managerState.Port, managerState.Name));
                }
                break;

            case "US":
                // US combines USE and USW
                if (KINESIS.ChatServer.IdleServersByRegion.TryGetValue("USE", out var useServers))
                {
                    AddServers(useServers);
                }
                if (KINESIS.ChatServer.IdleServersByRegion.TryGetValue("USW", out var uswServers))
                {
                    AddServers(uswServers);
                }
                break;
            default:
                // Only use a specific region.
                if (KINESIS.ChatServer.IdleServersByRegion.TryGetValue(region, out var servers))
                {
                    AddServers(servers);
                }
                break;
        }

        // TODO: restrict how frequently accountKey can be obtained.
        AccountKey = Guid.NewGuid().ToString("N");
        AccountKeyHash = AuthResponse.ComputeAuthHash(AccountKey, cookie);

        // TODO: register the token.
        // RegisterGameHostingPermissionToken(cookie, AccountKey);
    }

    [PhpProperty("server_list")]
    public readonly List<ServerInfo> Servers = new();

    [PhpProperty("acc_key")]
    public readonly string AccountKey;

    [PhpProperty("acc_key_hash")]
    public readonly string AccountKeyHash;

    [PhpProperty(0)]
    public readonly bool Zero = true;

    private void AddServers(ConcurrentDictionary<KINESIS.ConnectedServer,bool> servers)
    {
        foreach (var entry in servers)
        {
            var connectedServer = entry.Key;
            var serverState = connectedServer.ServerState;

            // Only add Idle servers.
            if (serverState.Status == KINESIS.Server.ServerStatus.Idle)
            {
                Servers.Add(new ServerInfo(serverState.ServerId, serverState.Address, serverState.Port, serverState.Location));
            }
        }
    }
}

public struct ServerInfo
{
    public ServerInfo(int id, string ip, short port, string location)
    {
        Id = id;
        Ip = ip;
        Port = port;
        Location = location;
    }

    [PhpProperty("server_id")]
    public readonly int Id;

    [PhpProperty("ip")]
    public readonly string Ip;

    [PhpProperty("port")]
    public readonly short Port;

    [PhpProperty("location")]
    public readonly string Location;

    [PhpProperty("class")]
    public readonly int Category = 1;
}

internal class ServerForJoinListResponse
{
    public ServerForJoinListResponse()
    {
        int GAME_PHASE_LOBBY = 1;
        // Only add servers that we believe are joinable. These servers are either currently loading resources for a
        // game that will soon start, or the game started but is currently in a lobby phase where anyone can join.
        // Normally, we would exclude servers with a Loading status, but while Loading status is dispatched
        // immediately when a game server hosts a game, the "Loading Finished and game is not in the Lobby phase" may
        // be delayed.
        foreach (var kvp in KINESIS.ChatServer.ConnectedServers)
        {
            var connectedServer = kvp.Key;
            var serverState = connectedServer.ServerState;
            if (serverState.Status == KINESIS.Server.ServerStatus.Loading || (serverState.Status == KINESIS.Server.ServerStatus.Active && serverState.GamePhase == GAME_PHASE_LOBBY))
            {
                Servers.Add(new ServerInfo(serverState.ServerId, serverState.Address, serverState.Port, serverState.Location));
            }
        }
    }
    [PhpProperty("server_list")]
    public readonly List<ServerInfo> Servers = new();

    [PhpProperty(0)]
    public readonly bool Zero = true;
}

public class ServerListHandler : IClientRequestHandler
{
    public Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        IActionResult result;

        formData.TryGetValue("gametype", out var gametype);
        formData.TryGetValue("cookie", out var cookie);

        if (cookie == null)
        {
            result = new UnauthorizedResult();
        }
        else if (gametype == "10")
        {
            result = new OkObjectResult(PHP.Serialize(new ServerForJoinListResponse()));
        }
        else if (gametype == "90")
        {
            formData.TryGetValue("region", out var region);
            result = new OkObjectResult(PHP.Serialize(new ServerForCreateListResponse(cookie, region)));
        }
        else
        {
            result = new BadRequestResult();
        }

        return Task.FromResult(result);
    }
}
