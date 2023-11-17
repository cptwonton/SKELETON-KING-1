namespace PUZZLEBOX;

public class CombinedPlayerAwardSummary
{
    public readonly int MVP;
    public readonly List<string> Top4Names;
    public readonly List<int> Top4Nums;

    public CombinedPlayerAwardSummary(int mvp, List<string> top4Names, List<int> top4Nums)
    {
        MVP = mvp;
        Top4Names = top4Names;
        Top4Nums = top4Nums;
    }

    public static CombinedPlayerAwardSummary AddUp(PlayerAwardSummary pas1, PlayerAwardSummary pas2, PlayerAwardSummary pas3, PlayerAwardSummary pas4)
    {
        PlayerAwardSummary playerAwardSummary = new PlayerAwardSummary()
        {
            TopAnnihilations = pas1.TopAnnihilations + pas2.TopAnnihilations + pas3.TopAnnihilations + pas4.TopAnnihilations,
            MostQuadKills = pas1.MostQuadKills + pas2.MostQuadKills + pas3.MostQuadKills + pas4.MostQuadKills,
            BestKillStreak = pas1.BestKillStreak + pas2.BestKillStreak + pas3.BestKillStreak + pas4.BestKillStreak,
            MostSmackdowns = pas1.MostSmackdowns + pas2.MostSmackdowns + pas3.MostSmackdowns + pas4.MostSmackdowns,
            MostKills = pas1.MostKills + pas2.MostKills + pas3.MostKills + pas4.MostKills,
            MostAssists = pas1.MostAssists + pas2.MostAssists + pas3.MostAssists + pas4.MostAssists,
            LeastDeaths = pas1.LeastDeaths + pas2.LeastDeaths + pas3.LeastDeaths + pas4.LeastDeaths,
            TopSiegeDamage = pas1.TopSiegeDamage + pas2.TopSiegeDamage + pas3.TopSiegeDamage + pas4.TopSiegeDamage,
            MostWardsKilled = pas1.MostWardsKilled + pas2.MostWardsKilled + pas3.MostWardsKilled + pas4.MostWardsKilled,
            TopHeroDamage = pas1.TopHeroDamage + pas2.TopHeroDamage + pas3.TopHeroDamage + pas4.TopHeroDamage,
            TopCreepScore = pas1.TopCreepScore + pas2.TopCreepScore + pas3.TopCreepScore + pas4.TopCreepScore,
            MVP = pas1.MVP + pas2.MVP + pas3.MVP + pas4.MVP,
        };

        List<KeyValuePair<int, string>> awardToCount = new()
        {
            new KeyValuePair<int, string>(playerAwardSummary.TopAnnihilations, "awd_mann"),
            new KeyValuePair<int, string>(playerAwardSummary.MostQuadKills, "awd_mqk"),
            new KeyValuePair<int, string>(playerAwardSummary.BestKillStreak, "awd_lgks"),
            new KeyValuePair<int, string>(playerAwardSummary.MostSmackdowns, "awd_msd"),
            new KeyValuePair<int, string>(playerAwardSummary.MostKills, "awd_mkill"),
            new KeyValuePair<int, string>(playerAwardSummary.MostAssists, "awd_masst"),
            new KeyValuePair<int, string>(playerAwardSummary.LeastDeaths, "awd_ledth"),
            new KeyValuePair<int, string>(playerAwardSummary.TopSiegeDamage, "awd_mbdmg"),
            new KeyValuePair<int, string>(playerAwardSummary.MostWardsKilled, "awd_mwk"),
            new KeyValuePair<int, string>(playerAwardSummary.TopHeroDamage, "awd_mhdd"),
            new KeyValuePair<int, string>(playerAwardSummary.TopCreepScore, "awd_hcs")
        };

        awardToCount.Sort((a, b) => -a.Key.CompareTo(b.Key));
        return new CombinedPlayerAwardSummary(
            mvp: playerAwardSummary.MVP,
            top4Names: awardToCount.Select(a => a.Value).Take(4).ToList(),
            top4Nums: awardToCount.Select(a => a.Key).Take(4).ToList()
        );
    }
}

[Owned]
public class PlayerAwardSummary
{
    public int TopAnnihilations { get; set; }
    public int MostQuadKills { get; set; }
    public int BestKillStreak { get; set; }
    public int MostSmackdowns { get; set; }
    public int MostKills { get; set; }
    public int MostAssists { get; set; }
    public int LeastDeaths { get; set; }
    public int TopSiegeDamage { get; set; }
    public int MostWardsKilled { get; set; }
    public int TopHeroDamage { get; set; }
    public int TopCreepScore { get; set; }
    public int MVP { get; set; }
}
