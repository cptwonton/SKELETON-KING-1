namespace ZORGATH;

internal class GameServerInfo
{
    public readonly int Id;
    public readonly string Name;

    public GameServerInfo(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class StartGameHandler : IServerRequestHandler
{
    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        if (!formData.TryGetValue("session", out var cookie))
        {
            return new UnauthorizedResult();
        }

        using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
        int gameServerId = await bountyContext.GameServers.Where(gameServer => gameServer.Cookie == cookie)
                .Select(gameServer => gameServer.GameServerId)
                .FirstOrDefaultAsync();
        if (gameServerId == 0)
        {
            return new UnauthorizedResult();
        }

        formData.TryGetValue("map", out var map);
        formData.TryGetValue("version", out var version);
        formData.TryGetValue("mname", out var matchName);
        MatchResults matchResults = new MatchResults()
        {
            server_id = gameServerId,
            map = map,
            version = version,
            name = matchName,
            datetime = DateTime.UtcNow
        };

        bountyContext.MatchResults.Add(matchResults);
        await bountyContext.SaveChangesAsync();

        Dictionary<string, object> response = new()
        {
            { "match_id", matchResults.match_id },
            { "match_date", DateTime.UtcNow },
            { "is_recommended", false },
            { "soccer_hero_list", "" },
            { "free_hero_list", "" },
            { "early_access_hero_list", "" },
            { "disabled_hero_list", "" }
        };
        return new OkObjectResult(PHP.Serialize(response));
    }
}
