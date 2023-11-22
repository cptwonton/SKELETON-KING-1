namespace KINESIS.Matchmaking;

public class RefreshMatchmakingStatsRequest : ProtocolRequest<ConnectedClient>
{
    public static RefreshMatchmakingStatsRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        updatedOffset = offset;
        return new RefreshMatchmakingStatsRequest();
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient connectedClient)
    {
        using var bountyContext = dbContextFactory.CreateDbContext();
        RefreshMatchmakingStatsRequestResponse? refreshMatchmakingStatsRequestResponse = bountyContext.Accounts
            .Where(account => account.AccountId == connectedClient.AccountId)
            .Select(account => new RefreshMatchmakingStatsRequestResponse(
                /* rating: */ account.PlayerSeasonStatsRanked.Rating,
                /* rank: */ ChampionsOfNewerthRanks.RankForMmr(account.PlayerSeasonStatsRanked.Rating),
                /* numberOfWins: */ account.PlayerSeasonStatsRanked.Wins,
                /* numberOfLosses: */ account.PlayerSeasonStatsRanked.Losses,
                /* winStreak: */ account.PlayerSeasonStatsRanked.WinStreak,
                /* numberOfMatchesPlayed: */ account.PlayerSeasonStatsRanked.Wins + account.PlayerSeasonStatsRanked.Losses,
                /* numberOfPlacementMatchesPlayed: */ account.PlayerSeasonStatsRanked.PlacementMatchesDetails.Length,
                /* placementMatchesDetails: */ account.PlayerSeasonStatsRanked.PlacementMatchesDetails,
                /* casualRating: */ account.PlayerSeasonStatsRankedCasual.Rating,
                /* casualRank: */ ChampionsOfNewerthRanks.RankForMmr(account.PlayerSeasonStatsRankedCasual.Rating),
                /* casualNumberOfWins: */ account.PlayerSeasonStatsRankedCasual.Wins,
                /* casualNumberOfLosses: */ account.PlayerSeasonStatsRankedCasual.Losses,
                /* casualWinStreak: */ account.PlayerSeasonStatsRankedCasual.WinStreak,
                /* casualNumberOfMatchesPlayed: */ account.PlayerSeasonStatsRankedCasual.Wins + account.PlayerSeasonStatsRankedCasual.Losses,
                /* casualNumberOfPlacementMatchesPlayed: */ account.PlayerSeasonStatsRankedCasual.PlacementMatchesDetails.Length,
                /* casualPlacementMatchesDetails: */ account.PlayerSeasonStatsRankedCasual.PlacementMatchesDetails,
                /* eligibleForMatchmaking: */ 1,
                /* seasonEnd: */ 1
            ))
            .FirstOrDefault();
        if (refreshMatchmakingStatsRequestResponse != null)
        {
            connectedClient.SendResponse(refreshMatchmakingStatsRequestResponse);
        }
        
        // The game doesn't always seem to request matchmaking settings.
        connectedClient.SendResponse(ChatServer.MatchmakingSettingsResponse);
    }
}
