namespace ZORGATH;

internal record NewSessionAuthData(int AccountId, AccountType accountType);

public class NewSessionHandler : IServerRequestHandler
{
    private readonly VersionProvider _versionProvider;
    private readonly string _chatServerAddress;
    private readonly short _chatServerPort;

    public NewSessionHandler(VersionProvider versionProvider, string chatServerAddres, short chatServerPort)
    {
        _versionProvider = versionProvider;
        _chatServerAddress = chatServerAddres;
        _chatServerPort = chatServerPort;
    }

    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        string? userAgent = controllerContext.HttpContext.Request.Headers["User-Agent"];
        if (userAgent == null)
        {
            return new UnauthorizedResult();
        }

        string[] parameters = userAgent.Split('/');
        string version = parameters[2];

        // Only allow server if it's matching Windows client version (ignoring the hotfix, e.g. 4.10.6.x)
        string allowedVersion = _versionProvider.ObtainGameVersionIgnoringHotfix("wac", "x86_64");
        if (!version.StartsWith(allowedVersion))
        {
            // Suppress servers that don't match desired version.
            return new UnauthorizedResult();
        }

        string[] accountNameWithInstanceId = formData["login"].Split(':');
        string accountName = accountNameWithInstanceId[0];

        // Instance index can be obtained like this:
        // int instanceId = int.Parse(accountNameWithInstanceId[1]);

        using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
        NewSessionAuthData? authData = await bountyContext.Accounts
                .Where(account => account.Name == accountName && (account.AccountType == AccountType.Staff || account.AccountType == AccountType.RankedMatchHost || account.AccountType == AccountType.UnrankedMatchHost))
                .Select(account => new NewSessionAuthData(account.AccountId, account.AccountType))
                .FirstOrDefaultAsync();
        if (authData == null)
        {
            // TODO: return "error" => message instead?
            return new UnauthorizedResult();
        }

        int accountId = authData.AccountId;
        string address = formData["ip"];
        short port = short.Parse(formData["port"]);
        string location = formData["location"];
        string name = formData["name"];

        int serverId;
        string cookie = Guid.NewGuid().ToString("N");
        int? existingGameServerId = await bountyContext.GameServers
                .Where(server => server.Account.AccountId == accountId && server.Address == address && server.Port == port && server.Location == location && server.Name == name)
                .Select(gameServer => gameServer.GameServerId)
                .FirstOrDefaultAsync();
        if (existingGameServerId == null)
        {
            // Add new one.
            serverId = (await bountyContext.GameServers.MaxAsync(x => (int?)x.GameServerId) ?? 0) + 1;
            GameServer gameServer = new GameServer(
                // Poor man's auto-increment.
                gameServerId: serverId,
                accountId: accountId,
                timestampCreated: DateTime.UtcNow,
                timestampLastSession: null,
                address: address,
                port: port,
                location: location,
                name: name,
                cookie: cookie
            );
            bountyContext.GameServers.Add(gameServer);
            await bountyContext.SaveChangesAsync();
        }
        else
        {
            // Update existing one.
            serverId = existingGameServerId.Value;
            bountyContext.GameServers
                .Where(gameServer => gameServer.GameServerId == serverId)
                .ExecuteUpdate(s => s.SetProperty(b => b.Cookie, b => cookie).SetProperty(b => b.TimestampLastSession, b => DateTime.UtcNow));
        }

        Dictionary<string, object> response = new()
        {
            ["session"] = cookie,
            ["server_id"] = serverId,
            ["chat_address"] = _chatServerAddress,
            ["chat_port"] = _chatServerPort,
            ["leaverthreshold"] = 0.05
        };

        return new OkObjectResult(PHP.Serialize(response));
    }
}
