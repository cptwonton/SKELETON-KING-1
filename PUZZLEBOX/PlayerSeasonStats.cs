namespace PUZZLEBOX;

public class PlayerSeasonStats
{
    public const int NumPlacementMatches = 6;

    public void UpdateFrom(PlayerMatchResults playerMatchResults)
    {
        this.ConcedeVotes += playerMatchResults.concedevotes;
        this.Buybacks += playerMatchResults.buybacks;
        this.TimesDisconnected += playerMatchResults.discos;
        this.TimesKicked += playerMatchResults.kicked;
        this.HeroKills += playerMatchResults.herokills;
        this.HeroDamage += playerMatchResults.herodmg;
        this.HeroExp += playerMatchResults.heroexp;
        this.HeroKillsGold += playerMatchResults.herokillsgold;
        this.HeroAssists += playerMatchResults.heroassists;
        this.Deaths += playerMatchResults.deaths;
        this.GoldLost2Death += playerMatchResults.goldlost2death;
        this.SecsDead += playerMatchResults.secs_dead;
        this.TeamCreepKills += playerMatchResults.teamcreepkills;
        this.TeamCreepDmg += playerMatchResults.teamcreepdmg;
        this.TeamCreepExp += playerMatchResults.teamcreepexp;
        this.TeamCreepGold += playerMatchResults.teamcreepgold;
        this.NeutralCreepKills += playerMatchResults.neutralcreepkills;
        this.NeutralCreepDmg += playerMatchResults.neutralcreepdmg;
        this.NeutralCreepExp += playerMatchResults.neutralcreepexp;
        this.NeutralCreepGold += playerMatchResults.neutralcreepgold;
        this.BDmg += playerMatchResults.bdmg;
        this.BDmgExp += playerMatchResults.bdmgexp;
        this.Razed += playerMatchResults.razed;
        this.BGold += playerMatchResults.bgold;
        this.Denies += playerMatchResults.denies;
        this.ExpDenies += playerMatchResults.exp_denied;
        this.Gold += playerMatchResults.gold;
        this.GoldSpent += playerMatchResults.gold_spent;
        this.Exp += playerMatchResults.exp;
        this.Actions += playerMatchResults.actions;
        this.Secs += playerMatchResults.secs;
        this.Consumables += playerMatchResults.consumables;
        this.Wards += playerMatchResults.wards;
        // this.Level
        // this.LevelExp
        // this.MinExp
        // this.MaxExp
        this.TimeEarningExp += playerMatchResults.time_earning_exp;
        this.Bloodlust += playerMatchResults.bloodlust;
        this.DoubleKill += playerMatchResults.doublekill;
        this.TrippleKill += playerMatchResults.triplekill;
        this.QuadKill += playerMatchResults.quadkill;
        this.Annihilation += playerMatchResults.annihilation;
        this.Ks3 += playerMatchResults.ks3;
        this.Ks4 += playerMatchResults.ks4;
        this.Ks5 += playerMatchResults.ks5;
        this.Ks6 += playerMatchResults.ks6;
        this.Ks7 += playerMatchResults.ks7;
        this.Ks8 += playerMatchResults.ks8;
        this.Ks9 += playerMatchResults.ks9;
        this.Ks10 += playerMatchResults.ks10;
        this.Ks15 += playerMatchResults.ks15;
        this.Smackdown += playerMatchResults.smackdown;
        this.Humiliation += playerMatchResults.humiliation;
        this.Nemesis += playerMatchResults.nemesis;
        this.Retribution += playerMatchResults.retribution;
        // this.WinStreak
    }

    [Key]
    public int AccountId { get; set; }
    public float Rating { get; set; } = 1500;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Concedes { get; set; }
    public int ConcedeVotes { get; set; }
    public int Buybacks { get; set; }
    public int TimesDisconnected { get; set; }
    public int TimesKicked { get; set; }
    public int HeroKills { get; set; }
    public int HeroDamage { get; set; }
    public int HeroExp { get; set; }
    public int HeroKillsGold { get; set; }
    public int HeroAssists { get; set; }
    public int Deaths { get; set; }
    public int GoldLost2Death { get; set; }
    public int SecsDead { get; set; }
    public int TeamCreepKills { get; set; }
    public int TeamCreepDmg { get; set; }
    public int TeamCreepExp { get; set; }
    public int TeamCreepGold { get; set; }
    public int NeutralCreepKills { get; set; }
    public int NeutralCreepDmg { get; set; }
    public int NeutralCreepExp { get; set; }
    public int NeutralCreepGold { get; set; }
    public int BDmg { get; set; }
    public int BDmgExp { get; set; }
    public int Razed { get; set; }
    public int BGold { get; set; }
    public int Denies { get; set; }
    public int ExpDenies { get; set; }
    public int Gold { get; set; }
    public int GoldSpent { get; set; }
    public int Exp { get; set; }
    public int Actions { get; set; }
    public int Secs { get; set; }
    public int Consumables { get; set; }
    public int Wards { get; set; }
    public int EmPlayed { get; set; }
    public int Level { get; set; }
    public int LevelExp { get; set; }
    public int MinExp { get; set; }
    public int MaxExp { get; set; }
    public int TimeEarningExp { get; set; }
    public int Bloodlust { get; set; }
    public int DoubleKill { get; set; }
    public int TrippleKill { get; set; }
    public int QuadKill { get; set; }
    public int Annihilation { get; set; }
    public int Ks3 { get; set; }
    public int Ks4 { get; set; }
    public int Ks5 { get; set; }
    public int Ks6 { get; set; }
    public int Ks7 { get; set; }
    public int Ks8 { get; set; }
    public int Ks9 { get; set; }
    public int Ks10 { get; set; }
    public int Ks15 { get; set; }
    public int Smackdown { get; set; }
    public int Humiliation { get; set; }
    public int Nemesis { get; set; }
    public int Retribution { get; set; }
    public int WinStreak { get; set; }

    [Required]
    public string SerializedMatchIds { get; set; } = "";

    [Required]
    // This only needs to be in Ranked non-Casual?
    public PlayerAwardSummary PlayerAwardSummary { get; set; } = new();

    [Required]
    public string SerializedHeroUsage { get; set; } = "";

    [Required]
    [StringLength(NumPlacementMatches)]
    public string PlacementMatchesDetails { get; set; } = "";
}

//[PrimaryKey(nameof(AccountId))]
public class PlayerSeasonStatsRanked : PlayerSeasonStats
{
}

//[PrimaryKey(nameof(AccountId))]
public class PlayerSeasonStatsRankedCasual : PlayerSeasonStats
{
}

//[PrimaryKey(nameof(AccountId))]
public class PlayerSeasonStatsPublic : PlayerSeasonStats
{
}

//[PrimaryKey(nameof(AccountId))]
public class PlayerSeasonStatsMidWars : PlayerSeasonStats
{
}
