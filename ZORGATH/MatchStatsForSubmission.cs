namespace ZORGATH;

internal class ExistingMatchData
{
    public readonly DateTime MatchStartTime;
    public readonly string? Name;

    public ExistingMatchData(DateTime matchStartTime, string? name)
    {
        MatchStartTime = matchStartTime;
        Name = name;
    }
}

internal class AccountStatsInfo
{
    public readonly float Rating;
    public string PlacementMatchDetails;
    public readonly Dictionary<string, int> HeroUsage;

    public AccountStatsInfo(float rating, string placementMatchDetails, string serializedHeroUsage)
    {
        Rating = rating;
        PlacementMatchDetails = placementMatchDetails;

        Dictionary<string, int>? heroUsage = null;
        try
        {
            heroUsage = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(serializedHeroUsage);
        }
        catch
        {
        }

        HeroUsage = heroUsage ?? new Dictionary<string, int>();
    }

    public string UpdateHeroUsage(string heroCode)
    {
        HeroUsage.TryGetValue(heroCode, out var timesUsed);
        HeroUsage[heroCode] = timesUsed + 1;

        return System.Text.Json.JsonSerializer.Serialize(HeroUsage);
    }
}

internal class AccountMatchInfo
{
    public readonly int TotalMatchesPlayed;
    public readonly int StatResetCount;
    public readonly bool IsMainAccount;

    public AccountMatchInfo(int totalMatchesPlayed, int statResetCount, bool isMainAccount)
    {
        TotalMatchesPlayed = totalMatchesPlayed;
        StatResetCount = statResetCount;
        IsMainAccount = isMainAccount;
    }
}

public class MatchStatsForSubmission
{
    public string f { get; set; } = string.Empty;
    public string session { get; set; } = string.Empty;
    public string login { get; set; } = string.Empty;
    public string pass { get; set; } = string.Empty;
    public string resubmission_key { get; set; } = string.Empty;
    public int server_id { get; set; } = 0;
    public MatchResults match_stats { get; set; } = new();
    public Dictionary<int, Dictionary<string, PlayerMatchResults>> player_stats { get; set; } = new();
    public Dictionary<int, Dictionary<string, string>> inventory { get; set; } = new();

    public async Task<Boolean> PopulateMatchData(BountyContext bountyContext, EconomyConfiguration economy)
    {
        ExistingMatchData? existingMatchData = await bountyContext.MatchResults.Where(matchResults => matchResults.match_id == match_stats.match_id)
            .Select(matchResults => new ExistingMatchData(matchResults.datetime, matchResults.name))
            .FirstOrDefaultAsync();
        if (existingMatchData == null)
        {
            return false;
        }

        // Populate data, time and inventory before writing to the database.
        // We will need these values when retrieving match stats.
        DateTime matchStartTime = existingMatchData.MatchStartTime;
        match_stats.date = matchStartTime.ToShortDateString();
        match_stats.time = TimeSpan.FromSeconds(matchStartTime.TimeOfDay.TotalSeconds).ToString();
        match_stats.timestamp = (long)matchStartTime.Subtract(DateTime.UnixEpoch).TotalSeconds;
        match_stats.datetime = matchStartTime;

        // We must re-map inventory indexes. This is because while the game server is sending us items indexed
        // as "0", "1", "2" .. "5", the game client expect them to be indexes as "slot_1", "slot_2" .. "slot6".
        foreach (Dictionary<string, string>? playerInventory in inventory.Select(entry => entry.Value))
        {
            for (int i = 0; i < 7; ++i)
            {
                if (playerInventory.Remove(i.ToString(), out string? item))
                    playerInventory.Add("slot_" + (i + 1), item);
            }
        }

        // match_stats.name = await bountyContext.GameServers.Where(gameServer => gameServer.GameServerId == match_stats.server_id).Select(gameServer => gameServer.Name).SingleAsync();
        match_stats.inventory = System.Text.Json.JsonSerializer.Serialize(inventory);

        // Only count stats for matches that are longer than 5 minutes to avoid abuse. E.g. you can play up to
        // 20 1v1 same heroes games per hour to reduce leaver % or grind coins. Also exclude botmatches.
        List<float> ratings = new();

        var transaction = await bountyContext.Database.BeginTransactionAsync();
        bool updateStats = match_stats.time_played > 5 * 60 && match_stats.gamemode != "botmatch";

        await UpdatePlayerStats(bountyContext, economy, updateStats);
        bountyContext.MatchResults.Update(match_stats);

        await bountyContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }

    private async Task UpdatePlayerStats(BountyContext bountyContext, EconomyConfiguration economy, bool updateStats)
    {
        List<PlayerMatchResults>[] playerResultsByTeam = new List<PlayerMatchResults>[3] { null!, new(), new() };
        foreach (KeyValuePair<int, Dictionary<string, PlayerMatchResults>> playerStatsEntry in player_stats)
        {
            foreach (KeyValuePair<string, PlayerMatchResults> heroUsedEntry in playerStatsEntry.Value)
            {
                PlayerMatchResults playerMatchResults = heroUsedEntry.Value;
                playerResultsByTeam[playerMatchResults.team].Add(playerMatchResults);
            }
        }

        PlayerMatchResults[] teamResults = new PlayerMatchResults[3]
        {
            null!,
            new PlayerMatchResults(playerResultsByTeam[1]),
            new PlayerMatchResults(playerResultsByTeam[2]),
        };

        foreach (KeyValuePair<int, Dictionary<string, PlayerMatchResults>> playerStatsEntry in player_stats)
        {
            int accountId = playerStatsEntry.Key;

            foreach (KeyValuePair<string, PlayerMatchResults> heroUsedEntry in playerStatsEntry.Value)
            {
                string heroCode = heroUsedEntry.Key;
                PlayerMatchResults playerMatchResults = heroUsedEntry.Value;

                // social_bonus is the size of the group (1..5)
                if (playerMatchResults.wins + playerMatchResults.losses == 1 && updateStats)
                {
                    AccountMatchInfo? accountMatchInfo = await bountyContext.Accounts.Where(account => account.AccountId == accountId)
                            .Select(account => new AccountMatchInfo(
                                    account.PlayerSeasonStatsPublic.AprilFirstWins + account.PlayerSeasonStatsPublic.AprilFirstLosses +
                                    account.PlayerSeasonStatsRanked.AprilFirstWins + account.PlayerSeasonStatsRanked.AprilFirstLosses +
                                    account.PlayerSeasonStatsRankedCasual.AprilFirstWins + account.PlayerSeasonStatsRankedCasual.AprilFirstLosses +
                                    account.PlayerSeasonStatsMidWars.AprilFirstWins + account.PlayerSeasonStatsMidWars.AprilFirstLosses,
                                    account.StatResetCount,
                                    account.Name == account.User.UserName))
                            .FirstOrDefaultAsync();
                    if (accountMatchInfo != null)
                    {
                        Reward reward = GetPostMatchRewards(economy, accountMatchInfo.TotalMatchesPlayed, accountMatchInfo.StatResetCount, accountMatchInfo.IsMainAccount, playerMatchResults.wins == 1, playerMatchResults.social_bonus);
                        bountyContext.Accounts
                            .Where(account => account.AccountId == accountId)
                            .Select(account => account.User)
                            .ExecuteUpdate(s => s.SetProperty(b => b.GoldCoins, b => b.GoldCoins + reward.GoldCoins)
                                                 .SetProperty(b => b.SilverCoins, b => b.SilverCoins + reward.SilverCoins)
                                                 .SetProperty(b => b.PlinkoTickets, b => b.PlinkoTickets + reward.PlinkoTickets));
                    }
                }

                // Fix bug where user disconnecting before the picking phase is over will not be marked as disconnected.
                if (playerMatchResults.secs < 5 * 60)
                {
                    // If the player spend less that 5 minutes in-game, including picking phase, mark them terminated.
                    playerMatchResults.discos = 1;
                }

                float rating = 0;
                if (updateStats)
                {
                    if (playerMatchResults.pub_count == 1)
                    {
                        double ratingGained = playerMatchResults.pub_skill;
                        rating = await UpdateStats(bountyContext.PlayerSeasonStatsPublic, accountId, match_stats, playerMatchResults, teamResults, ratingGained, heroCode);
                    }
                    else
                    {
                        double ratingGained = playerMatchResults.amm_team_rating;
                        if (match_stats.map == "caldavar")
                        {
                            rating = await UpdateStats(bountyContext.PlayerSeasonStatsRanked, accountId, match_stats, playerMatchResults, teamResults, ratingGained, heroCode);
                        }

                        // Ranked TMM Casual
                        else if (match_stats.map == "caldavar_old")
                        {
                            rating = await UpdateStats(bountyContext.PlayerSeasonStatsRankedCasual, accountId, match_stats, playerMatchResults, teamResults, ratingGained, heroCode);
                        }
                        // Ranked MidWars
                        else if (match_stats.map == "midwars")
                        {
                            rating = await UpdateStats(bountyContext.PlayerSeasonStatsMidWars, accountId, match_stats, playerMatchResults, teamResults, ratingGained, heroCode);
                        }
                    }
                }

                // Record player's rating so that we can analyze it later.
                playerMatchResults.gameplaystat9 = rating;

                // Populate additional fields. We will need them later to provide match details.
                playerMatchResults.match_id = match_stats.match_id;
                playerMatchResults.account_id = accountId;
                playerMatchResults.map = match_stats.map;
                playerMatchResults.cli_name = heroCode;
                playerMatchResults.mdt = match_stats.date;
                playerMatchResults.datetime = match_stats.datetime;

                bountyContext.PlayerMatchResults.Add(playerMatchResults);
            }
        }
    }

    private Reward GetPostMatchRewards(EconomyConfiguration economy, int totalGamesPlayed, int statResetCount, bool isMainAccount, bool gameWon, int groupSize)
    {
        Reward reward = groupSize switch
        {
            0 => gameWon ? economy.MatchRewards.Solo.Win : economy.MatchRewards.Solo.Loss,
            1 => gameWon ? economy.MatchRewards.Solo.Win : economy.MatchRewards.Solo.Loss,
            2 => gameWon ? economy.MatchRewards.TwoPersonGroup.Win : economy.MatchRewards.TwoPersonGroup.Loss,
            3 => gameWon ? economy.MatchRewards.ThreePersonGroup.Win : economy.MatchRewards.ThreePersonGroup.Loss,
            4 => gameWon ? economy.MatchRewards.FourPersonGroup.Win : economy.MatchRewards.FourPersonGroup.Loss,
            5 => gameWon ? economy.MatchRewards.FivePersonGroup.Win : economy.MatchRewards.FivePersonGroup.Loss,

            _ => throw new ArgumentOutOfRangeException($@"Unsupported Group Size: {groupSize}")
        };

        if (isMainAccount && statResetCount == 0 && totalGamesPlayed < economy.EventRewards.PostSignupBonus.MatchesCount)
        {
            return new Reward(
                    goldCoins: reward.GoldCoins + economy.EventRewards.PostSignupBonus.GoldCoins,
                    silverCoins: reward.SilverCoins + economy.EventRewards.PostSignupBonus.SilverCoins,
                    plinkoTickets: reward.PlinkoTickets + economy.EventRewards.PostSignupBonus.PlinkoTickets);
        }
        return reward;
    }

    static async Task<float> UpdateStats<T>(DbSet<T> table, int accountId, MatchResults matchResults, PlayerMatchResults playerMatchResults, PlayerMatchResults[] teamResults, double ratingGained, string heroCode) where T : PlayerSeasonStats
    {
        AccountStatsInfo? statsInfo = await table
                            .Where(stats => stats.AccountId == accountId)
                            .Select(stats => new AccountStatsInfo(stats.Rating, stats.PlacementMatchesDetails, stats.SerializedHeroUsage))
                            .FirstOrDefaultAsync();
        if (statsInfo == null)
        {
            return 0;
        }

        if (statsInfo.PlacementMatchDetails.Length < PlayerSeasonStats.NumPlacementMatches)
        {
            ratingGained = CalculateMmrGain(playerMatchResults, teamResults[playerMatchResults.team]);
            statsInfo.PlacementMatchDetails = statsInfo.PlacementMatchDetails + (playerMatchResults.wins == 0 ? "0" : "1");
        }

        T? stats = await table.Where(stats => stats.AccountId == accountId).FirstOrDefaultAsync();
        if (stats != null)
        {
            stats.Rating = (float)Math.Max(stats.Rating + ratingGained, 1250);
            stats.PlacementMatchesDetails = statsInfo.PlacementMatchDetails;
            stats.SerializedHeroUsage = statsInfo.UpdateHeroUsage(heroCode);
            stats.UpdateFrom(playerMatchResults);
            stats.SerializedMatchIds = stats.SerializedMatchIds + "|" + matchResults.match_id;

            if (matchResults.mvp == accountId) ++stats.PlayerAwardSummary.MVP;
            if (matchResults.awd_mann == accountId) ++stats.PlayerAwardSummary.TopAnnihilations;
            if (matchResults.awd_mqk == accountId) ++stats.PlayerAwardSummary.MostQuadKills;
            if (matchResults.awd_lgks == accountId) ++stats.PlayerAwardSummary.BestKillStreak;
            if (matchResults.awd_msd == accountId) ++stats.PlayerAwardSummary.MostSmackdowns;
            if (matchResults.awd_mkill == accountId) ++stats.PlayerAwardSummary.MostKills;
            if (matchResults.awd_masst == accountId) ++stats.PlayerAwardSummary.MostAssists;
            if (matchResults.awd_ledth == accountId) ++stats.PlayerAwardSummary.LeastDeaths;
            if (matchResults.awd_mbdmg == accountId) ++stats.PlayerAwardSummary.TopSiegeDamage;
            if (matchResults.awd_mwk == accountId) ++stats.PlayerAwardSummary.MostWardsKilled;
            if (matchResults.awd_mhdd == accountId) ++stats.PlayerAwardSummary.TopHeroDamage;
            if (matchResults.awd_hcs == accountId) ++stats.PlayerAwardSummary.TopCreepScore;

            table.Update(stats);
        }

        return statsInfo.Rating;
    }

    public static double CalculateMmrGain(PlayerMatchResults playerMatchResults, PlayerMatchResults teamMatchResults)
    {
        double allKills = Math.Max(1, teamMatchResults.herokills);
        double percentOfAllKills = playerMatchResults.herokills / allKills;

        double allAssists = Math.Max(1, teamMatchResults.heroassists);
        double percentOfAllAssists = playerMatchResults.heroassists / allAssists;

        double allDeaths = Math.Max(1, teamMatchResults.deaths);
        double percentOfAllDeaths = playerMatchResults.deaths / allDeaths;

        double allWards = Math.Max(1, teamMatchResults.wards);
        double percentOfAllWards = playerMatchResults.wards / allWards;

        double participantion = (percentOfAllKills + percentOfAllAssists + percentOfAllWards / 2 - percentOfAllDeaths) * 100 / 1.5;
        if (playerMatchResults.wins > 0)
        {
            return participantion;
        }
        else
        {
            return participantion - 40;
        }
    }
}
