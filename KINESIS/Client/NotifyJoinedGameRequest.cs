namespace KINESIS.Client;

public class NotifyJoinedGameRequest : ProtocolRequest<ConnectedClient>
{
    private readonly string _gameName;
    private readonly int _matchId;
    private readonly bool _joinChannel;

    public NotifyJoinedGameRequest(string gameName, int matchId, bool joinChannel)
    {
        _gameName = gameName;
        _matchId = matchId;
        _joinChannel = joinChannel;
    }

    public static NotifyJoinedGameRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        string gameName = ReadString(data, offset, out offset);
        int matchId = ReadInt(data, offset, out offset);
        bool joinChannel = ReadByte(data, offset, out offset) != 0;
        updatedOffset = offset;

        return new NotifyJoinedGameRequest(gameName, matchId, joinChannel);
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient connectedClient)
    {
        // int accountId = connectedClient.AccountId;

        int matchId = _matchId;
        // Note: there should generally be at most one but sometimes there is a bug that produces more than one server...
        foreach (var entry in ChatServer.ConnectedServers.Where(s => s.Key.LastHostedMatchId == matchId))
        {
            ConnectedServer connectedServer = entry.Key;
            /*
            // Are we in the roster?
            MatchInfo? matchInfo = connectedServer.MatchInfo;
            if (matchInfo == null || (!matchInfo.Legion.AccountIds.Contains(accountId) && !matchInfo.Hellbourne.AccountIds.Contains(accountId)))
            {
                MatchId = 0;
            }
            */
        }

        connectedClient.NotifyJoinedGame(_gameName, matchId);

        using BountyContext bountyContext = dbContextFactory.CreateDbContext();
        bountyContext.Accounts.Where(account => account.AccountId == connectedClient.AccountId)
                .ExecuteUpdate(s => s.SetProperty(b => b.LastPlayedMatchId, b => matchId));
    }
}
