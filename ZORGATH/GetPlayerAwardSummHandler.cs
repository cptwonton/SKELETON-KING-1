namespace ZORGATH;

internal record GetPlayerAwardSummData(int AccountId, PlayerAwardSummary AwardSummary);

public class GetPlayerAwardSummHandler : IClientRequestHandler
{
    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
        GetPlayerAwardSummData? data = await bountyContext.Accounts
            .Where(account => account.Name == formData["nickname"])
            .Select(account => new GetPlayerAwardSummData(account.AccountId, account.PlayerSeasonStatsRanked.PlayerAwardSummary))
            .FirstOrDefaultAsync();

        if (data == null) { return new OkObjectResult(""); }

        Dictionary<string, int> awards = new();
        awards["awd_hcs"] = data.AwardSummary.TopCreepScore;
        awards["awd_ledth"] = data.AwardSummary.LeastDeaths;
        awards["awd_lgks"] = data.AwardSummary.BestKillStreak;
        awards["awd_mann"] = data.AwardSummary.TopAnnihilations;
        awards["awd_masst"] = data.AwardSummary.MostAssists;
        awards["awd_mbdmg"] = data.AwardSummary.TopSiegeDamage;
        awards["awd_mhdd"] = data.AwardSummary.TopHeroDamage;
        awards["awd_mkill"] = data.AwardSummary.MostKills;
        awards["awd_mqk"] = data.AwardSummary.MostQuadKills;
        awards["awd_msd"] = data.AwardSummary.MostSmackdowns;
        awards["mvp"] = data.AwardSummary.MVP;
        awards["awd_mwk"] = data.AwardSummary.MostWardsKilled;
        awards["account_id"] = data.AccountId;

        return new OkObjectResult(PHP.Serialize(awards));
    }
}
