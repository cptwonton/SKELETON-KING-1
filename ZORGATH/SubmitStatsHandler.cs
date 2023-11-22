namespace ZORGATH;

public class SubmitStatsHandler : IStatsRequestHandler
{
    private readonly EconomyConfiguration _economy;

    public SubmitStatsHandler(EconomyConfiguration economy)
    {
        _economy = economy;
    }

    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, BountyContext bountyContext, MatchStatsForSubmission stats)
    {
        if (!await stats.PopulateMatchData(bountyContext, _economy))
        {
            return new BadRequestResult();
        }

        Dictionary<string, string> response = new()
        {
            { "match_id", stats.match_stats.match_id.ToString() },
            { "match_info", "OK" },
            { "match_summ", "OK" },
            { "match_stats", "OK" },
            { "match_history", "OK" }
        };
        return new OkObjectResult(PHP.Serialize(response));
    }
}
