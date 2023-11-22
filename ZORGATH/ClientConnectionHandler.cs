namespace ZORGATH;

internal class ClientConnectionAccountDetails
{
    public readonly int AccountId;
    public readonly string Name;
    public readonly AccountType AccountType;
    public readonly int ClanId;
    public readonly string? ClanTag;
    public readonly ICollection<string> UnlockedUpgradeCodes;
    public readonly ICollection<string> SelectedUpgradeCodes;
    public readonly AccountStats AccountStats;

    internal ClientConnectionAccountDetails(int accountId, string name, AccountType accountType, int clanId, string? clanTag, ICollection<string> unlockedUpgradeCodes, ICollection<string> selectedUpgradeCodes, AccountStats accountStats)
    {
        AccountId = accountId;
        Name = name;
        AccountType = accountType;
        ClanId = clanId;
        ClanTag = clanTag;
        UnlockedUpgradeCodes = unlockedUpgradeCodes;
        SelectedUpgradeCodes = selectedUpgradeCodes;
        AccountStats = accountStats;
    }
}

public class ClientConnectionHandler : IServerRequestHandler
{
    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
        if (!formData.TryGetValue("cookie", out var cookie))
        {
            return new UnauthorizedResult();
        }

        ClientConnectionAccountDetails? accountDetails = await bountyContext.Accounts
            .Where(account => account.Cookie == cookie)
            .Select(account => new ClientConnectionAccountDetails(
                account.AccountId,
                account.Name,
                account.AccountType,
                account.Clan == null ? -1 : account.Clan.ClanId,
                account.Clan == null ? null : account.Clan.Tag,
                account.User.UnlockedUpgradeCodes,
                account.SelectedUpgradeCodes,
                new AccountStats(
                        /* level: */ 0,
                        /* levelExp: */ 0,
                        /* psr: */ account.PlayerSeasonStatsPublic.Rating,
                        /* normalRankedGamesMMR: */ account.PlayerSeasonStatsRanked.Rating,
                        /* casualModeMMR: */ account.PlayerSeasonStatsRankedCasual.Rating,
                        /* publicGamesPlayed: */ account.PlayerSeasonStatsPublic.Wins + account.PlayerSeasonStatsRankedCasual.Losses,
                        /* normalRankedGamesPlayed: */ account.PlayerSeasonStatsRanked.Wins + account.PlayerSeasonStatsRanked.Losses,
                        /* casualModeGamesPlayed: */ account.PlayerSeasonStatsRankedCasual.Wins + account.PlayerSeasonStatsRankedCasual.Losses,
                        /* midWarsGamesPlayed: */ account.PlayerSeasonStatsMidWars.Wins + account.PlayerSeasonStatsMidWars.Losses,
                        /* allOtherGamesPlayed: */ 0,
                        /* publicGameDisconnects: */ account.PlayerSeasonStatsPublic.TimesDisconnected,
                        /* normalRankedGameDisconnects: */ account.PlayerSeasonStatsRanked.TimesDisconnected,
                        /* casualModeDisconnects: */ account.PlayerSeasonStatsRankedCasual.TimesDisconnected,
                        /* midWarsTimesDisconnected: */ account.PlayerSeasonStatsMidWars.TimesDisconnected,
                        /* allOtherGameDisconnects: */ 0)
                )
            )
            .FirstOrDefaultAsync();
        if (accountDetails == null) return new UnauthorizedResult();

        // bool isCasual = formData["cas"]; // Casual Mode
        // int matchType = formData["new"]; // 1 - Public, 5 MidWars, 11 - CoN (I think).
        // string ip = formData["ip"] // IP Address Of The Client

        // TODO: Create Response Model For This Data
        Dictionary<string, object?> response = new()
        {
            { "cookie", cookie },
            { "account_id", accountDetails.AccountId },
            { "nickname", accountDetails.Name },
            { "super_id", accountDetails.AccountId },
            { "account_type", accountDetails.AccountType },
            { "level", 1 },
            { "clan_id", accountDetails.ClanId },
            { "tag", accountDetails.ClanTag },
            { "infos", new List<AccountStats>() { accountDetails.AccountStats } },
            { "game_cookie", "0123456789" }, // Must Exist; TODO: Generate And Store This Cookie?
            { "my_upgrades", accountDetails.UnlockedUpgradeCodes },
            { "selected_upgrades", accountDetails.SelectedUpgradeCodes }
        };

        return new OkObjectResult(PHP.Serialize(response));
    }
}
