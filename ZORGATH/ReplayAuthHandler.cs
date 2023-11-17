namespace ZORGATH;

internal record ReplayAuthData(int AccountId, string PasswordSalt, string HashedPassword);

public class ReplayAuthHandler : IServerRequestHandler
{
    private readonly string _chatServerAddress;
    private readonly short _managerPort;
    public ReplayAuthHandler(string chatServerAddress, short managerPort)
    {
        _chatServerAddress = chatServerAddress;
        _managerPort = managerPort;
    }

    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        // ServerManager authentication flow.
        string? login = formData["login"];

        // Login must end with a colon.
        if (login == null || !login.EndsWith(':'))
        {
            // Invalid login.
            return new UnauthorizedResult();
        }

        // Drop the colon.
        login = login.Substring(0, login.Length - 1);
        string? md5Pass = formData["pass"];

        using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
        ReplayAuthData? data = await bountyContext.Accounts
                .Where(account => account.Name == login)
                .Select(account => new ReplayAuthData(account.AccountId, account.User.PasswordSalt, account.User.HashedPassword))
                .FirstOrDefaultAsync();
        if (data == null)
        {
            // Unknown username.
            return new UnauthorizedResult();
        }

        string hashedPassword = RegistrationDataHelper.HashAccountPasswordMD5(md5Pass, data.PasswordSalt);
        if (hashedPassword != data.HashedPassword)
        {
            // Incorrect password.
            return new UnauthorizedResult();
        }

        // Cookie to connect to the ChatServer.
        string sessionCookie = Guid.NewGuid().ToString("N");
        int? existingManagerId = await bountyContext.GameServerManagers
            .Where(manager => manager.GameServerManagerId == data.AccountId)
            .Select(manager => manager.GameServerManagerId)
            .FirstOrDefaultAsync();
        if (existingManagerId == null)
        {
            bountyContext.GameServerManagers.Add(new GameServerManager(data.AccountId, sessionCookie));
            await bountyContext.SaveChangesAsync();
        }
        else
        {
            bountyContext.GameServerManagers
                .Where(manager => manager.GameServerManagerId == existingManagerId.Value)
                .ExecuteUpdate(s => s.SetProperty(b => b.Cookie, b => sessionCookie));
        }

        Dictionary<string, object> response = new()
        {
            ["server_id"] = data.AccountId,
            ["official"] = 1, // if not official, it's considered to be un-authorized.
            ["session"] = sessionCookie
        };

        response["chat_address"] = _chatServerAddress!;
        response["chat_port"] = _managerPort;

        // TODO: investigate how these are used.
        response["cdn_upload_host"] = "kongor.online";
        response["cdn_upload_target"] = "upload";

        // TODO: clear all cookies for this manager?
        return new OkObjectResult(PHP.Serialize(response));
    }
}
