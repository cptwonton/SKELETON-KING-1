namespace ZORGATH;

public class EconomyConfiguration
{
    public readonly SignupRewards SignupRewards;
    public readonly EventRewards EventRewards;
    public readonly MatchRewards MatchRewards;

    public EconomyConfiguration(SignupRewards signupRewards, EventRewards eventRewards, MatchRewards matchRewards)
    {
        SignupRewards = signupRewards;
        EventRewards = eventRewards;
        MatchRewards = matchRewards;
    }
}

public struct SignupRewards
{
    public readonly int GoldCoins;
    public readonly int SilverCoins;
    public readonly int PlinkoTickets;

    public SignupRewards(int goldCoins, int silverCoins, int plinkoTickets)
    {
        GoldCoins = goldCoins;
        SilverCoins = silverCoins;
        PlinkoTickets = plinkoTickets;
    }
}

public struct EventRewards
{
    public readonly PostSignupBonus PostSignupBonus;

    public EventRewards(PostSignupBonus postSignupBonus)
    {
        PostSignupBonus = postSignupBonus;
    }
}

public struct PostSignupBonus
{
    public readonly int GoldCoins;
    public readonly int SilverCoins;
    public readonly int PlinkoTickets;
    public readonly int MatchesCount;

    public PostSignupBonus(int goldCoins, int silverCoins, int plinkoTickets, int matchesCount)
    {
        GoldCoins = goldCoins;
        SilverCoins = silverCoins;
        PlinkoTickets = plinkoTickets;
        MatchesCount = matchesCount;
    }
}

public struct MatchRewards
{
    public readonly GroupReward Solo;
    public readonly GroupReward TwoPersonGroup;
    public readonly GroupReward ThreePersonGroup;
    public readonly GroupReward FourPersonGroup;
    public readonly GroupReward FivePersonGroup;

    public MatchRewards(GroupReward solo, GroupReward twoPersonGroup, GroupReward threePersonGroup, GroupReward fourPersonGroup, GroupReward fivePersonGroup)
    {
        Solo = solo;
        TwoPersonGroup = twoPersonGroup;
        ThreePersonGroup = threePersonGroup;
        FourPersonGroup = fourPersonGroup;
        FivePersonGroup = fivePersonGroup;
    }
}

public struct GroupReward
{
    public readonly Reward Win;
    public readonly Reward Loss;

    public GroupReward(Reward win, Reward loss)
    {
        Win = win;
        Loss = loss;
    }
}

public struct Reward
{
    public readonly int GoldCoins;
    public readonly int SilverCoins;
    public readonly int PlinkoTickets;

    public Reward(int goldCoins, int silverCoins, int plinkoTickets)
    {
        GoldCoins = goldCoins;
        SilverCoins = silverCoins;
        PlinkoTickets = plinkoTickets;
    }
}
