using System.Security.Principal;

using Humanizer;

using PUZZLEBOX;

namespace ZORGATH;

public class ShowStatsHandler : IClientRequestHandler
{
    public async Task<IActionResult> HandleRequest(ControllerContext controllerContext, Dictionary<string, string> formData)
    {
        string nickname = formData["nickname"];
        switch (formData["table"])
        {
            case "campaign":
            {
                using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
                string? response = await bountyContext.Accounts.Where(a => a.Name == nickname).Select(a => GetPlayerStatsNormalSeasonOrMidWars(
                    /* accountId: */ a.AccountId,
                    /* accountName: */ GetAccountNameWithClanTag(a.Name, a.Clan!.Tag),
                    /* clanName: */ a.Clan!.Name,
                    /* wins: */ a.PlayerSeasonStatsRanked.Wins,
                    /* losses: */ a.PlayerSeasonStatsRanked.Losses,
                    /* disconnects: */ a.PlayerSeasonStatsRanked.TimesDisconnected,
                    /* kills: */ a.PlayerSeasonStatsRanked.HeroKills,
                    /* deaths: */ a.PlayerSeasonStatsRanked.Deaths,
                    /* assists: */ a.PlayerSeasonStatsRanked.HeroAssists,
                    /* secondsPlayed: */ a.PlayerSeasonStatsRanked.Secs,
                    /* experienceGained: */ a.PlayerSeasonStatsRanked.Exp,
                    /* timeEarningExp: */ a.PlayerSeasonStatsRanked.TimeEarningExp,
                    /* creepDenies:*/ a.PlayerSeasonStatsRanked.Denies,
                    /* creepKills: */ a.PlayerSeasonStatsRanked.TeamCreepKills,
                    /* neautralKills: */ a.PlayerSeasonStatsRanked.NeutralCreepKills,
                    /* actions: */ a.PlayerSeasonStatsRanked.Actions,
                    /* wardsPlaced: */ a.PlayerSeasonStatsRanked.Wards,
                    /* goldEarned: */ a.PlayerSeasonStatsRanked.Gold,
                    /* ks3Count: */ a.PlayerSeasonStatsRanked.Ks3,
                    /* ks4Count: */ a.PlayerSeasonStatsRanked.Ks4,
                    /* ks5Count: */ a.PlayerSeasonStatsRanked.Ks5,
                    /* ks6Count: */ a.PlayerSeasonStatsRanked.Ks6,
                    /* ks7Count: */ a.PlayerSeasonStatsRanked.Ks7,
                    /* ks8Count: */ a.PlayerSeasonStatsRanked.Ks8,
                    /* ks9Count: */ a.PlayerSeasonStatsRanked.Ks9,
                    /* ks10Count: */ a.PlayerSeasonStatsRanked.Ks10,
                    /* ks15Count: */ a.PlayerSeasonStatsRanked.Ks15,
                    /* smackdownCount: */ a.PlayerSeasonStatsRanked.Smackdown,
                    /* humiliationCount: */ a.PlayerSeasonStatsRanked.Humiliation,
                    /* bloodlustCount: */ a.PlayerSeasonStatsRanked.Bloodlust,
                    /* doublekillCount: */ a.PlayerSeasonStatsRanked.DoubleKill,
                    /* triplekillCount: */ a.PlayerSeasonStatsRanked.TrippleKill,
                    /* quadkillCount: */ a.PlayerSeasonStatsRanked.QuadKill,
                    /* annihilationCount: */ a.PlayerSeasonStatsRanked.Annihilation,
                    /* numberOfPlacementMatchesPlayed: */ a.PlayerSeasonStatsRanked.PlacementMatchesDetails.Length,
                    /* rating: */ a.PlayerSeasonStatsRanked.Rating,
                    /* serializedHeroUsage: */ a.PlayerSeasonStatsRanked.SerializedHeroUsage,
                    /* selectedUpgrades: */ a.SelectedUpgradeCodes,
                    /* publicMatchesPlayed: */ a.PlayerSeasonStatsPublic.Wins + a.PlayerSeasonStatsPublic.Losses,
                    /* publicDisconnects: */ a.PlayerSeasonStatsPublic.TimesDisconnected,
                    /* rankedMatchesPlayed: */ a.PlayerSeasonStatsRanked.Wins + a.PlayerSeasonStatsRanked.Losses,
                    /* rankedDisconnects: */ a.PlayerSeasonStatsRanked.TimesDisconnected,
                    /* casualMatchesPlayed: */ a.PlayerSeasonStatsRankedCasual.Wins + a.PlayerSeasonStatsRankedCasual.Losses,
                    /* casualDisconnects: */ a.PlayerSeasonStatsRankedCasual.TimesDisconnected,
                    /* midWarsMatchesPlayed: */ a.PlayerSeasonStatsMidWars.Wins + a.PlayerSeasonStatsMidWars.Losses,
                    /* midWarsDisconnects: */ a.PlayerSeasonStatsMidWars.TimesDisconnected,
                    /* accountCreated: */ a.TimestampCreated,
                    /* lastActivity: */ a.LastActivity)).FirstOrDefaultAsync();
                return new OkObjectResult(response);
            }

            case "campaign_casual":
                {
                    using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
                    string? response = await bountyContext.Accounts.Where(a => a.Name == nickname).Select(a => GetPlayerStatsCasualSeason(
                   /* accountId: */ a.AccountId,
                   /* accountName: */ GetAccountNameWithClanTag(a.Name, a.Clan!.Tag),
                   /* clanName: */ a.Clan!.Name,
                   /* wins: */ a.PlayerSeasonStatsRankedCasual.Wins,
                   /* losses: */ a.PlayerSeasonStatsRankedCasual.Losses,
                   /* disconnects: */ a.PlayerSeasonStatsRankedCasual.TimesDisconnected,
                   /* kills: */ a.PlayerSeasonStatsRankedCasual.HeroKills,
                   /* deaths: */ a.PlayerSeasonStatsRankedCasual.Deaths,
                   /* assists: */ a.PlayerSeasonStatsRankedCasual.HeroAssists,
                   /* secondsPlayed: */ a.PlayerSeasonStatsRankedCasual.Secs,
                   /* experienceGained: */ a.PlayerSeasonStatsRankedCasual.Exp,
                   /* timeEarningExp: */ a.PlayerSeasonStatsRankedCasual.TimeEarningExp,
                   /* creepDenies:*/ a.PlayerSeasonStatsRankedCasual.Denies,
                   /* creepKills: */ a.PlayerSeasonStatsRankedCasual.TeamCreepKills,
                   /* neautralKills: */ a.PlayerSeasonStatsRankedCasual.NeutralCreepKills,
                   /* actions: */ a.PlayerSeasonStatsRankedCasual.Actions,
                   /* wardsPlaced: */ a.PlayerSeasonStatsRankedCasual.Wards,
                   /* goldEarned: */ a.PlayerSeasonStatsRankedCasual.Gold,
                   /* ks3Count: */ a.PlayerSeasonStatsRankedCasual.Ks3,
                   /* ks4Count: */ a.PlayerSeasonStatsRankedCasual.Ks4,
                   /* ks5Count: */ a.PlayerSeasonStatsRankedCasual.Ks5,
                   /* ks6Count: */ a.PlayerSeasonStatsRankedCasual.Ks6,
                   /* ks7Count: */ a.PlayerSeasonStatsRankedCasual.Ks7,
                   /* ks8Count: */ a.PlayerSeasonStatsRankedCasual.Ks8,
                   /* ks9Count: */ a.PlayerSeasonStatsRankedCasual.Ks9,
                   /* ks10Count: */ a.PlayerSeasonStatsRankedCasual.Ks10,
                   /* ks15Count: */ a.PlayerSeasonStatsRankedCasual.Ks15,
                   /* smackdownCount: */ a.PlayerSeasonStatsRankedCasual.Smackdown,
                   /* humiliationCount: */ a.PlayerSeasonStatsRankedCasual.Humiliation,
                   /* bloodlustCount: */ a.PlayerSeasonStatsRankedCasual.Bloodlust,
                   /* doublekillCount: */ a.PlayerSeasonStatsRankedCasual.DoubleKill,
                   /* triplekillCount: */ a.PlayerSeasonStatsRankedCasual.TrippleKill,
                   /* quadkillCount: */ a.PlayerSeasonStatsRankedCasual.QuadKill,
                   /* annihilationCount: */ a.PlayerSeasonStatsRankedCasual.Annihilation,
                   /* numberOfPlacementMatchesPlayed: */ a.PlayerSeasonStatsRankedCasual.PlacementMatchesDetails.Length,
                   /* rating: */ a.PlayerSeasonStatsRankedCasual.Rating,
                   /* serializedHeroUsage: */ a.PlayerSeasonStatsRankedCasual.SerializedHeroUsage,
                   /* selectedUpgrades: */ a.SelectedUpgradeCodes,
                   /* publicMatchesPlayed: */ a.PlayerSeasonStatsPublic.Wins + a.PlayerSeasonStatsPublic.Losses,
                   /* publicDisconnects: */ a.PlayerSeasonStatsPublic.TimesDisconnected,
                   /* rankedMatchesPlayed: */ a.PlayerSeasonStatsRanked.Wins + a.PlayerSeasonStatsRanked.Losses,
                   /* rankedDisconnects: */ a.PlayerSeasonStatsRanked.TimesDisconnected,
                   /* casualMatchesPlayed: */ a.PlayerSeasonStatsRankedCasual.Wins + a.PlayerSeasonStatsRankedCasual.Losses,
                   /* casualDisconnects: */ a.PlayerSeasonStatsRankedCasual.TimesDisconnected,
                   /* midWarsMatchesPlayed: */ a.PlayerSeasonStatsMidWars.Wins + a.PlayerSeasonStatsMidWars.Losses,
                   /* midWarsDisconnects: */ a.PlayerSeasonStatsMidWars.TimesDisconnected,
                   /* accountCreated: */ a.TimestampCreated,
                   /* lastActivity: */ a.LastActivity)).FirstOrDefaultAsync();
                    return new OkObjectResult(response);
            }

            case "midwars":
                {
                    using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
                    string? response = await bountyContext.Accounts.Where(a => a.Name == nickname).Select(a => GetPlayerStatsNormalSeasonOrMidWars(
                    /* accountId: */ a.AccountId,
                    /* accountName: */ GetAccountNameWithClanTag(a.Name, a.Clan!.Tag),
                    /* clanName: */ a.Clan!.Name,
                    /* wins: */ a.PlayerSeasonStatsMidWars.Wins,
                    /* losses: */ a.PlayerSeasonStatsMidWars.Losses,
                    /* disconnects: */ a.PlayerSeasonStatsMidWars.TimesDisconnected,
                    /* kills: */ a.PlayerSeasonStatsMidWars.HeroKills,
                    /* deaths: */ a.PlayerSeasonStatsMidWars.Deaths,
                    /* assists: */ a.PlayerSeasonStatsMidWars.HeroAssists,
                    /* secondsPlayed: */ a.PlayerSeasonStatsMidWars.Secs,
                    /* experienceGained: */ a.PlayerSeasonStatsMidWars.Exp,
                    /* timeEarningExp: */ a.PlayerSeasonStatsMidWars.TimeEarningExp,
                    /* creepDenies: */ a.PlayerSeasonStatsMidWars.Denies,
                    /* creepKills: */ a.PlayerSeasonStatsMidWars.TeamCreepKills,
                    /* neautralKills: */ a.PlayerSeasonStatsMidWars.NeutralCreepKills,
                    /* actions: */ a.PlayerSeasonStatsMidWars.Actions,
                    /* wardsPlaced: */ a.PlayerSeasonStatsMidWars.Wards,
                    /* goldEarned: */ a.PlayerSeasonStatsMidWars.Gold,
                    /* ks3Count: */ a.PlayerSeasonStatsMidWars.Ks3,
                    /* ks4Count: */ a.PlayerSeasonStatsMidWars.Ks4,
                    /* ks5Count: */ a.PlayerSeasonStatsMidWars.Ks5,
                    /* ks6Count: */ a.PlayerSeasonStatsMidWars.Ks6,
                    /* ks7Count: */ a.PlayerSeasonStatsMidWars.Ks7,
                    /* ks8Count: */ a.PlayerSeasonStatsMidWars.Ks8,
                    /* ks9Count: */ a.PlayerSeasonStatsMidWars.Ks9,
                    /* ks10Count: */ a.PlayerSeasonStatsMidWars.Ks10,
                    /* ks15Count: */ a.PlayerSeasonStatsMidWars.Ks15,
                    /* smackdownCount: */ a.PlayerSeasonStatsMidWars.Smackdown,
                    /* humiliationCount: */ a.PlayerSeasonStatsMidWars.Humiliation,
                    /* bloodlustCount: */ a.PlayerSeasonStatsMidWars.Bloodlust,
                    /* doublekillCount: */ a.PlayerSeasonStatsMidWars.DoubleKill,
                    /* triplekillCount: */ a.PlayerSeasonStatsMidWars.TrippleKill,
                    /* quadkillCount: */ a.PlayerSeasonStatsMidWars.QuadKill,
                    /* annihilationCount: */ a.PlayerSeasonStatsMidWars.Annihilation,
                    /* numberOfPlacementMatchesPlayed: */ a.PlayerSeasonStatsMidWars.PlacementMatchesDetails.Length,
                    /* rating: */ a.PlayerSeasonStatsMidWars.Rating,
                    /* serializedHeroUsage: */ a.PlayerSeasonStatsMidWars.SerializedHeroUsage,
                    /* selectedUpgrades: */ a.SelectedUpgradeCodes,
                    /* publicMatchesPlayed: */ a.PlayerSeasonStatsPublic.Wins + a.PlayerSeasonStatsPublic.Losses,
                    /* publicDisconnects: */ a.PlayerSeasonStatsPublic.TimesDisconnected,
                    /* rankedMatchesPlayed: */ a.PlayerSeasonStatsRanked.Wins + a.PlayerSeasonStatsRanked.Losses,
                    /* rankedDisconnects: */ a.PlayerSeasonStatsRanked.TimesDisconnected,
                    /* casualMatchesPlayed: */ a.PlayerSeasonStatsRankedCasual.Wins + a.PlayerSeasonStatsRankedCasual.Losses,
                    /* casualDisconnects: */ a.PlayerSeasonStatsRankedCasual.TimesDisconnected,
                    /* midWarsMatchesPlayed: */ a.PlayerSeasonStatsMidWars.Wins + a.PlayerSeasonStatsMidWars.Losses,
                    /* midWarsDisconnects: */ a.PlayerSeasonStatsMidWars.TimesDisconnected,
                    /* accountCreated: */ a.TimestampCreated,
                    /* lastActivity: */ a.LastActivity)).FirstOrDefaultAsync();
                    return new OkObjectResult(response);
                }

            case "player":
            {
                using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
                string? response = await bountyContext.Accounts.Where(a => a.Name == nickname).Select(a => GetPlayerStatsPublic(
                    a.AccountId,
                    a.Name,
                    a.Clan!.Name,
                    a.PlayerSeasonStatsPublic.Wins,
                    a.PlayerSeasonStatsPublic.Losses,
                    a.PlayerSeasonStatsPublic.HeroKills,
                    a.PlayerSeasonStatsPublic.Deaths,
                    a.PlayerSeasonStatsPublic.HeroAssists,
                    a.PlayerSeasonStatsPublic.Secs,
                    a.PlayerSeasonStatsPublic.Exp,
                    a.PlayerSeasonStatsPublic.TimeEarningExp,
                    a.PlayerSeasonStatsPublic.Denies,
                    a.PlayerSeasonStatsPublic.TeamCreepKills,
                    a.PlayerSeasonStatsPublic.NeutralCreepKills,
                    a.PlayerSeasonStatsPublic.Actions,
                    a.PlayerSeasonStatsPublic.Wards,
                    a.PlayerSeasonStatsPublic.Gold,
                    a.PlayerSeasonStatsPublic.Rating,
                    a.PlayerSeasonStatsPublic.SerializedHeroUsage,
                    a.SelectedUpgradeCodes,
                    a.PlayerSeasonStatsPublic.TimesDisconnected,
                    a.PlayerSeasonStatsRanked.Wins + a.PlayerSeasonStatsRanked.Losses,
                    a.PlayerSeasonStatsRanked.TimesDisconnected,
                    a.PlayerSeasonStatsRankedCasual.Wins + a.PlayerSeasonStatsRankedCasual.Losses,
                    a.PlayerSeasonStatsRankedCasual.TimesDisconnected,
                    a.PlayerSeasonStatsMidWars.Wins + a.PlayerSeasonStatsMidWars.Losses,
                    a.PlayerSeasonStatsMidWars.TimesDisconnected)).FirstOrDefaultAsync();
                return new OkObjectResult(response);
            }

            case "mastery":
            {
                using BountyContext bountyContext = controllerContext.HttpContext.RequestServices.GetRequiredService<BountyContext>();
                string? response = await bountyContext.Accounts.Where(a => a.Name == nickname).Select(a => GetMastery(a.AccountId, a.Name, a.Clan!.Tag, a.Clan!.Name, a.LastActivity, a.TimestampCreated, a.SelectedUpgradeCodes)).FirstOrDefaultAsync();
                return new OkObjectResult(response);
            }

            default:
                return new OkObjectResult("");
        }
    }

    // This is mostly a stub.
    private static string GetMastery(int accountId, string accountName, string? clanTag, string? clanName, DateTime lastActivity, DateTime accountCreated, ICollection<string> selectedUpgrades)
    {
        Dictionary<string, object> response = new();

        response["account_id"] = accountId;
        response["nickname"] = GetAccountNameWithClanTag(accountName, clanTag);
        if (clanName != null) response["name"] = clanName;

        response["last_activity"] = $"{lastActivity.ToShortDateString()} ({lastActivity.Humanize()})";
        response["create_date"] = $"{accountCreated.ToShortDateString()} ({accountCreated.Humanize()})";
        response["selected_upgrades"] = selectedUpgrades;

        return PHP.Serialize(response);
    }

    private static string GetKDA(float gamesPlayed, int kills, int deaths, int assists)
    {
        if (gamesPlayed == 0)
        {
            return "0/0/0";
        }
        float killsPerGame = kills / gamesPlayed;
        float deathsPerGame = deaths / gamesPlayed;
        float assistsPerGame = assists / gamesPlayed;
        return $"{killsPerGame:0.0}/{deathsPerGame:0.0}/{assistsPerGame:0.0}"; ;
    }

    private static void GetSharedStats(
        Dictionary<string, object> response,
        string accountName,
        string? clanName,
        int accountId,
        float currentModeGamesPlayed,
        int kills,
        int deaths,
        int assists,
        string serializedHeroUsage,
        ICollection<string> selectedUpgrades,
        int wardsPlaced,
        int secondsPlayed,
        int experienceGained,
        int timeEarningExp,
        int creepDenies,
        int creepKills,
        int neautralKills,
        int actions,
        int publicDisconnects,
        int rankedDisconnects,
        int casualDisconnects,
        int midWarsDisconnects,
        int publicMatchesPlayed,
        int rankedMatchesPlayed,
        int casualMatchesPlayed,
        int midWarsMatchesPlayed)
    {
        response["nickname"] = accountName;
        if (clanName != null) response["name"] = clanName;

        response["account_id"] = accountId;
        response["avgWardsUsed"] = wardsPlaced / currentModeGamesPlayed;
        response["k_d_a"] = GetKDA(currentModeGamesPlayed, kills, deaths, assists);
        response["total_games_played"] = publicMatchesPlayed + rankedMatchesPlayed + casualMatchesPlayed + midWarsMatchesPlayed;
        response["total_discos"] = publicDisconnects + rankedDisconnects + casualDisconnects + midWarsDisconnects;
        response["avgGameLength"] = secondsPlayed / currentModeGamesPlayed;
        response["avgXP_min"] = (experienceGained * 60) / (float)timeEarningExp;
        response["avgDenies"] = creepDenies / currentModeGamesPlayed;
        response["avgCreepKills"] = creepKills / currentModeGamesPlayed;
        response["avgNeutralKills"] = neautralKills / currentModeGamesPlayed;
        response["avgActions_min"] = (actions * 60) / (float)secondsPlayed;
        response["selected_upgrades"] = selectedUpgrades;

        response["acc_games_played"] = publicMatchesPlayed;
        response["acc_discos"] = publicDisconnects;

        response["rnk_games_played"] = rankedMatchesPlayed;
        response["rnk_discos"] = rankedDisconnects;

        response["cs_games_played"] = casualMatchesPlayed;
        response["cs_discos"] = casualDisconnects;

        response["mid_discos"] = midWarsDisconnects;
        response["mid_games_played"] = midWarsMatchesPlayed;

        if (serializedHeroUsage.Length != 0)
        {
            Dictionary<string, int>? heroUsage = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(serializedHeroUsage);
            List<KeyValuePair<string, int>>? mostPlayed = (from entry in heroUsage orderby entry.Value descending select entry).Take(5).ToList();

            switch (mostPlayed.Count)
            {
                case 5:
                    response["favHero5"] = HeroTextureByHeroName(mostPlayed[4].Key);
                    response["favHero5Time"] = (mostPlayed[4].Value * 100f) / currentModeGamesPlayed;
                    response["favHero5_2"] = mostPlayed[4].Key;
                    goto case 4;

                case 4:
                    response["favHero4"] = HeroTextureByHeroName(mostPlayed[3].Key);
                    response["favHero4Time"] = (mostPlayed[3].Value * 100f) / currentModeGamesPlayed;
                    response["favHero4_2"] = mostPlayed[3].Key;
                    goto case 3;

                case 3:
                    response["favHero3"] = HeroTextureByHeroName(mostPlayed[2].Key);
                    response["favHero3Time"] = (mostPlayed[2].Value * 100f) / currentModeGamesPlayed;
                    response["favHero3_2"] = mostPlayed[2].Key;
                    goto case 2;

                case 2:
                    response["favHero2"] = HeroTextureByHeroName(mostPlayed[1].Key);
                    response["favHero2Time"] = (mostPlayed[1].Value * 100f) / currentModeGamesPlayed;
                    response["favHero2_2"] = mostPlayed[1].Key;
                    goto case 1;

                case 1:
                    response["favHero1"] = HeroTextureByHeroName(mostPlayed[0].Key);
                    response["favHero1Time"] = (mostPlayed[0].Value * 100f) / currentModeGamesPlayed;
                    response["favHero1_2"] = mostPlayed[0].Key;
                    break;
            }
        }
    }


    public static string GetPlayerStatsNormalSeasonOrMidWars(
        int accountId,
        string accountName,
        string clanName,
        int wins,
        int losses,
        int disconnects,
        int kills,
        int deaths,
        int assists,
        int secondsPlayed,
        int experienceGained,
        int timeEarningExp,
        int creepDenies,
        int creepKills,
        int neautralKills,
        int actions,
        int wardsPlaced,
        int goldEarned,
        int ks3Count,
        int ks4Count,
        int ks5Count,
        int ks6Count,
        int ks7Count,
        int ks8Count,
        int ks9Count,
        int ks10Count,
        int ks15Count,
        int smackdownCount,
        int humiliationCount,
        int bloodlustCount,
        int doublekillCount,
        int triplekillCount,
        int quadkillCount,
        int annihilationCount,
        int numberOfPlacementMatchesPlayed,
        float rating,
        string serializedHeroUsage,
        ICollection<string> selectedUpgrades,
        int publicMatchesPlayed,
        int publicDisconnects,
        int rankedMatchesPlayed,
        int rankedDisconnects,
        int casualMatchesPlayed,
        int casualDisconnects,
        int midWarsMatchesPlayed,
        int midWarsDisconnects,
        DateTime accountCreated,
        DateTime lastActivity)
    {
        Dictionary<string, object> response = new();
        int currentModeGamesPlayed = wins + losses;
        int currentModeDisconnects = disconnects;
        GetSharedStats(
            response: response,
            accountName: accountName,
            clanName: clanName,
            accountId: accountId,
            currentModeGamesPlayed: currentModeGamesPlayed,
            kills: kills,
            deaths: deaths,
            assists: assists,
            serializedHeroUsage: serializedHeroUsage,
            selectedUpgrades: selectedUpgrades,
            wardsPlaced: wardsPlaced,
            secondsPlayed: secondsPlayed,
            experienceGained: experienceGained,
            timeEarningExp: timeEarningExp,
            creepDenies: creepDenies,
            creepKills: creepKills,
            neautralKills: neautralKills,
            actions: actions,
            publicDisconnects: publicDisconnects,
            rankedDisconnects: rankedDisconnects,
            casualDisconnects: casualDisconnects,
            midWarsDisconnects: midWarsDisconnects,
            publicMatchesPlayed: publicMatchesPlayed,
            rankedMatchesPlayed: rankedMatchesPlayed,
            casualMatchesPlayed: casualMatchesPlayed,
            midWarsMatchesPlayed: midWarsMatchesPlayed);

        // ranked-specific.
        response["cam_bloodlust"] = bloodlustCount;
        response["cam_doublekill"] = doublekillCount;
        response["cam_triplekill"] = triplekillCount;
        response["cam_quadkill"] = quadkillCount;
        response["cam_annihilation"] = annihilationCount;
        response["cam_ks3"] = ks3Count;
        response["cam_ks4"] = ks4Count;
        response["cam_ks5"] = ks5Count;
        response["cam_ks6"] = ks6Count;
        response["cam_ks7"] = ks7Count;
        response["cam_ks8"] = ks8Count;
        response["cam_ks9"] = ks9Count;
        response["cam_ks10"] = ks10Count;
        response["cam_ks15"] = ks15Count;
        response["cam_smackdown"] = smackdownCount;
        response["cam_humiliation"] = humiliationCount;
        response["cam_wins"] = wins;
        response["cam_losses"] = losses;
        response["cam_herokills"] = kills;
        response["cam_heroassists"] = assists;
        response["cam_deaths"] = deaths;
        response["cam_gold"] = goldEarned;
        response["cam_exp"] = experienceGained;
        response["cam_secs"] = secondsPlayed;

        response["curr_season_cam_games_played"] = currentModeGamesPlayed;
        response["curr_season_cam_discos"] = currentModeDisconnects;

        response["current_level"] = numberOfPlacementMatchesPlayed < PlayerSeasonStats.NumPlacementMatches ? 0 : ChampionsOfNewerthRanks.RankForMmr(rating);
        response["level_percent"] = ChampionsOfNewerthRanks.PercentUntilNextRank(rating);

        response["last_activity"] = $"{lastActivity.ToShortDateString()} ({lastActivity.Humanize()})";
        response["create_date"] = $"{accountCreated.ToShortDateString()} ({accountCreated.Humanize()})";
        response["highest_level_current"] = 0; // highest rank - we don't keep that information atm.
        response["season_id"] = 22;

        return PHP.Serialize(response);
    }

    private static string GetPlayerStatsCasualSeason(
        int accountId,
        string accountName,
        string clanName,
        int wins,
        int losses,
        int disconnects,
        int kills,
        int deaths,
        int assists,
        int secondsPlayed,
        int experienceGained,
        int timeEarningExp,
        int creepDenies,
        int creepKills,
        int neautralKills,
        int actions,
        int wardsPlaced,
        int goldEarned,
        int ks3Count,
        int ks4Count,
        int ks5Count,
        int ks6Count,
        int ks7Count,
        int ks8Count,
        int ks9Count,
        int ks10Count,
        int ks15Count,
        int smackdownCount,
        int humiliationCount,
        int bloodlustCount,
        int doublekillCount,
        int triplekillCount,
        int quadkillCount,
        int annihilationCount,
        int numberOfPlacementMatchesPlayed,
        float rating,
        string serializedHeroUsage,
        ICollection<string> selectedUpgrades,
        int publicMatchesPlayed,
        int publicDisconnects,
        int rankedMatchesPlayed,
        int rankedDisconnects,
        int casualMatchesPlayed,
        int casualDisconnects,
        int midWarsMatchesPlayed,
        int midWarsDisconnects,
        DateTime accountCreated,
        DateTime lastActivity)
    {
        Dictionary<string, object> response = new();
        int currentModeGamesPlayed = casualMatchesPlayed;
        int currentModeDisconnects = casualDisconnects;
        GetSharedStats(
           response: response,
           accountName: accountName,
           clanName: clanName,
           accountId: accountId,
           currentModeGamesPlayed: currentModeGamesPlayed,
           kills: kills,
           deaths: deaths,
           assists: assists,
           serializedHeroUsage: serializedHeroUsage,
           selectedUpgrades: selectedUpgrades,
           wardsPlaced: wardsPlaced,
           secondsPlayed: secondsPlayed,
           experienceGained: experienceGained,
           timeEarningExp: timeEarningExp,
           creepDenies: creepDenies,
           creepKills: creepKills,
           neautralKills: neautralKills,
           actions: actions,
           publicDisconnects: publicDisconnects,
           rankedDisconnects: rankedDisconnects,
           casualDisconnects: casualDisconnects,
           midWarsDisconnects: midWarsDisconnects,
           publicMatchesPlayed: publicMatchesPlayed,
           rankedMatchesPlayed: rankedMatchesPlayed,
           casualMatchesPlayed: casualMatchesPlayed,
           midWarsMatchesPlayed: midWarsMatchesPlayed);

        response["cam_cs_bloodlust"] = bloodlustCount;
        response["cam_cs_doublekill"] = doublekillCount;
        response["cam_cs_triplekill"] = triplekillCount;
        response["cam_cs_quadkill"] = quadkillCount;
        response["cam_cs_annihilation"] = annihilationCount;
        response["cam_cs_ks3"] = ks3Count;
        response["cam_cs_ks4"] = ks4Count;
        response["cam_cs_ks5"] = ks5Count;
        response["cam_cs_ks6"] = ks6Count;
        response["cam_cs_ks7"] = ks7Count;
        response["cam_cs_ks8"] = ks8Count;
        response["cam_cs_ks9"] = ks9Count;
        response["cam_cs_ks10"] = ks10Count;
        response["cam_cs_ks15"] = ks15Count;
        response["cam_cs_smackdown"] = smackdownCount;
        response["cam_cs_humiliation"] = humiliationCount;
        response["cam_cs_wins"] = wins;
        response["cam_cs_losses"] = losses;
        response["cam_cs_herokills"] = kills;
        response["cam_cs_heroassists"] = assists;
        response["cam_cs_deaths"] = deaths;
        response["cam_cs_gold"] = goldEarned;
        response["cam_cs_exp"] = experienceGained;
        response["cam_cs_secs"] = secondsPlayed;

        response["curr_season_cam_cs_games_played"] = currentModeGamesPlayed;
        response["curr_season_cam_cs_discos"] = currentModeDisconnects;

        response["current_level"] = numberOfPlacementMatchesPlayed < PlayerSeasonStats.NumPlacementMatches ? 0 : ChampionsOfNewerthRanks.RankForMmr(rating);
        response["level_percent"] = ChampionsOfNewerthRanks.PercentUntilNextRank(rating);

        response["last_activity"] = lastActivity.ToShortDateString() + " (" + lastActivity.Humanize() + ")";
        response["create_date"] = accountCreated.ToShortDateString() + " (" + accountCreated.Humanize() + ")";
        response["highest_level_current"] = 0; // highest rank - we don't keep that information atm.
        response["season_id"] = 22;

        return PHP.Serialize(response);
    }

    private static string GetPlayerStatsPublic(int accountId,
        string accountName,
        string? clanName,
        int wins,
        int losses,
        int kills,
        int deaths,
        int assists,
        int secondsPlayed,
        int experienceGained,
        int timeEarningExp,
        int creepDenies,
        int creepKills,
        int neautralKills,
        int actions,
        int wardsPlaced,
        int goldEarned,
        float rating,
        string serializedHeroUsage,
        ICollection<string> selectedUpgrades,
        int publicDisconnects,
        int rankedMatchesPlayed,
        int rankedDisconnects,
        int casualMatchesPlayed,
        int casualDisconnects,
        int midWarsMatchesPlayed,
        int midWarsDisconnects)
    {
        Dictionary<string, object> response = new();
        int publicMatchesPlayed = wins + losses;
        int currentModeGamesPlayed = publicMatchesPlayed;
        int currentModeDisconnects = publicDisconnects;
        GetSharedStats(
           response: response,
           accountName: accountName,
           clanName: clanName,
           accountId: accountId,
           currentModeGamesPlayed: currentModeGamesPlayed,
           kills: kills,
           deaths: deaths,
           assists: assists,
           serializedHeroUsage: serializedHeroUsage,
           selectedUpgrades: selectedUpgrades,
           wardsPlaced: wardsPlaced,
           secondsPlayed: secondsPlayed,
           experienceGained: experienceGained,
           timeEarningExp: timeEarningExp,
           creepDenies: creepDenies,
           creepKills: creepKills,
           neautralKills: neautralKills,
           actions: actions,
           publicDisconnects: publicDisconnects,
           rankedDisconnects: rankedDisconnects,
           casualDisconnects: casualDisconnects,
           midWarsDisconnects: midWarsDisconnects,
           publicMatchesPlayed: publicMatchesPlayed,
           rankedMatchesPlayed: rankedMatchesPlayed,
           casualMatchesPlayed: casualMatchesPlayed,
           midWarsMatchesPlayed: midWarsMatchesPlayed);

        // public games-specific.
        response["acc_deaths"] = deaths;
        response["acc_exp"] = experienceGained;
        response["acc_gold"] = goldEarned;
        response["acc_heroassists"] = assists;
        response["acc_herokills"] = kills;
        response["acc_losses"] = losses;
        response["acc_pub_skill"] = rating;
        response["acc_secs"] = secondsPlayed;
        response["acc_wins"] = wins;

        return PHP.Serialize(response);
    }

    private static string HeroTextureByHeroName(string heroName)
    {
        return heroName switch
        {
            Heroes.Accursed.Identifier => "accursed",
            Heroes.Adrenaline.Identifier => "adrenaline",
            Heroes.Aluna.Identifier => "aluna",
            Heroes.Andromeda.Identifier => "andromeda",
            Heroes.Apex.Identifier => "apex",
            Heroes.Arachna.Identifier => "arachna",
            Heroes.Armadon.Identifier => "armadon",
            Heroes.Artesia.Identifier => "artesia",
            Heroes.Artillery.Identifier => "artillery",
            Heroes.WretchedHag.Identifier => "babayaga",
            Heroes.Behemoth.Identifier => "behemoth",
            Heroes.Balphagore.Identifier => "bephelgor",
            Heroes.Berzerker.Identifier => "berzerker",
            Heroes.Blitz.Identifier => "blitz",
            Heroes.Bombardier.Identifier => "bomb",
            Heroes.Bubbles.Identifier => "bubbles",
            Heroes.Bushwack.Identifier => "bushwack",
            Heroes.Calamity.Identifier => "calamity",
            Heroes.Qi.Identifier => "chi",
            Heroes.TheChipper.Identifier => "chipper",
            Heroes.Chronos.Identifier => "chronos",
            Heroes.Circe.Identifier => "circe",
            Heroes.CorruptedDisciple.Identifier => "corrupted_disciple",
            Heroes.Cthulhuphant.Identifier => "cthulhuphant",
            Heroes.Dampeer.Identifier => "dampeer",
            Heroes.Deadlift.Identifier => "deadlift",
            Heroes.Deadwood.Identifier => "deadwood",
            Heroes.Defiler.Identifier => "defiler",
            Heroes.Devourer.Identifier => "devourer",
            Heroes.PlagueRider.Identifier => "diseasedrider",
            Heroes.DoctorRepulsor.Identifier => "doctor_repulsor",
            Heroes.LordSalforis.Identifier => "dreadknight",
            Heroes.DrunkenMaster.Identifier => "drunkenmaster",
            Heroes.Blacksmith.Identifier => "dwarf_magi",
            Heroes.Slither.Identifier => "ebulus",
            Heroes.Electrician.Identifier => "electrician",
            Heroes.Ellonia.Identifier => "ellonia",
            Heroes.EmeraldWarden.Identifier => "emerald_warden",
            Heroes.Empath.Identifier => "empath",
            Heroes.Engineer.Identifier => "engineer",
            Heroes.Fayde.Identifier => "fade",
            Heroes.Nymphora.Identifier => "fairy",
            Heroes.Draconis.Identifier => "flamedragon",
            Heroes.FlintBeastwood.Identifier => "flint_beastwood",
            Heroes.Flux.Identifier => "flux",
            Heroes.ForsakenArcher.Identifier => "forsaken_archer",
            Heroes.Glacius.Identifier => "frosty",
            Heroes.Gauntlet.Identifier => "gauntlet",
            Heroes.Gemini.Identifier => "gemini",
            Heroes.Geomancer.Identifier => "geomancer",
            Heroes.TheGladiator.Identifier => "gladiator",
            Heroes.Goldenveil.Identifier => "goldenveil",
            Heroes.Grinex.Identifier => "grinex",
            Heroes.Gunblade.Identifier => "gunblade",
            Heroes.Hammerstorm.Identifier => "hammerstorm",
            Heroes.NightHound.Identifier => "hantumon",
            Heroes.Hellbringer.Identifier => "hellbringer",
            Heroes.SoulReaper.Identifier => "helldemon",
            Heroes.Swiftblade.Identifier => "hiro",
            Heroes.BloodHunter.Identifier => "hunter",
            Heroes.Myrmidon.Identifier => "hydromancer",
            Heroes.Ichor.Identifier => "ichor",
            Heroes.Magebane.Identifier => "javaras",
            Heroes.Jeraziah.Identifier => "jereziah",
            Heroes.Kane.Identifier => "kane",
            Heroes.Kinesis.Identifier => "kenisis",
            Heroes.KingKlout.Identifier => "king_klout",
            Heroes.Klanx.Identifier => "klanx",
            Heroes.Kraken.Identifier => "kraken",
            Heroes.MoonQueen.Identifier => "krixi",
            Heroes.Thunderbringer.Identifier => "kunas",
            Heroes.Legionnaire.Identifier => "legionnaire",
            Heroes.Lodestone.Identifier => "lodestone",
            Heroes.Magmus.Identifier => "magmar",
            "Hero_Magmus" => "magmar",
            Heroes.Maliken.Identifier => "maliken",
            Heroes.Martyr.Identifier => "martyr",
            Heroes.MasterOfArms.Identifier => "master_of_arms",
            Heroes.Midas.Identifier => "midas",
            Heroes.Xemplar.Identifier => "mimix",
            Heroes.Moira.Identifier => "moira",
            Heroes.Monarch.Identifier => "monarch",
            "Hero_MoneyKing" => "monkey_king",
            Heroes.MonkeyKing.Identifier => "monkey_king",
            Heroes.Moraxus.Identifier => "moraxus",
            Heroes.Pharaoh.Identifier => "mumra",
            Heroes.Nitro.Identifier => "nitro",
            Heroes.Nomad.Identifier => "nomad",
            Heroes.Oogie.Identifier => "oogie",
            Heroes.Ophelia.Identifier => "ophelia",
            Heroes.Pandamonium.Identifier => "panda",
            Heroes.Parallax.Identifier => "parallax",
            Heroes.Parasite.Identifier => "parasite",
            Heroes.Pearl.Identifier => "pearl",
            Heroes.Pestilence.Identifier => "pestilence",
            Heroes.Bramble.Identifier => "plant",
            Heroes.PollywogPriest.Identifier => "pollywogpriest",
            Heroes.Predator.Identifier => "predator",
            Heroes.Prisoner945.Identifier => "prisoner",
            Heroes.Prophet.Identifier => "prophet",
            Heroes.PuppetMaster.Identifier => "puppetmaster",
            Heroes.Pyromancer.Identifier => "pyromancer",
            Heroes.AmunRa.Identifier => "ra",
            Heroes.Rally.Identifier => "rally",
            Heroes.Rampage.Identifier => "rampage",
            Heroes.Ravenor.Identifier => "ravenor",
            Heroes.Revenant.Identifier => "revenant",
            Heroes.Rhapsody.Identifier => "rhapsody",
            Heroes.Riftwalker.Identifier => "riftmage",
            Heroes.Riptide.Identifier => "riptide",
            Heroes.Pebbles.Identifier => "rocky",
            Heroes.Salomon.Identifier => "salomon",
            Heroes.SandWraith.Identifier => "sand_wraith",
            "Hero_Saphire" => "saphire",
            Heroes.Sapphire.Identifier => "sapphire",
            Heroes.TheMadman.Identifier => "scar",
            Heroes.Scout.Identifier => "scout",
            Heroes.Shadowblade.Identifier => "shadowblade",
            Heroes.DementedShaman.Identifier => "shaman",
            Heroes.Shellshock.Identifier => "shellshock",
            Heroes.Silhouette.Identifier => "silhouette",
            Heroes.SirBenzington.Identifier => "sir_benzington",
            Heroes.Skrap.Identifier => "skrap",
            Heroes.Solstice.Identifier => "solstice",
            Heroes.Soulstealer.Identifier => "soulstealer",
            Heroes.Succubus.Identifier => "succubis",
            Heroes.Gravekeeper.Identifier => "taint",
            Heroes.Tarot.Identifier => "tarot",
            Heroes.Tempest.Identifier => "tempest",
            Heroes.KeeperOfTheForest.Identifier => "treant",
            Heroes.Tremble.Identifier => "tremble",
            Heroes.Tundra.Identifier => "tundra",
            Heroes.Valkyrie.Identifier => "valkyrie",
            Heroes.TheDarkLady.Identifier => "vanya",
            Heroes.Vindicator.Identifier => "vindicator",
            "Hero_Vindicatory" => "vindicator",
            Heroes.VoodooJester.Identifier => "voodoo",
            Heroes.Warchief.Identifier => "warchief",
            Heroes.WitchSlayer.Identifier => "witch_slayer",
            Heroes.WarBeast.Identifier => "wolfman",
            Heroes.Torturer.Identifier => "xalynx",
            Heroes.Wildsoul.Identifier => "yogi",
            Heroes.Zephyr.Identifier => "zephyr",
            "wl_Warlock" => "prophet", // NOTE: `wl_Warlock` is used for Prophet on the custom map "prophets".
            _ => heroName,
        };
    }

    private static string GetAccountNameWithClanTag(string accountName, string? clanTag)
    {
        if (clanTag == null) return accountName;
        return $"[{clanTag}]{accountName}";
    }
}