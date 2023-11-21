namespace KINESIS.Manager;

public class ConnectManagerRequest : ProtocolRequest<ConnectedManager>
{
    private readonly int _accountId;
    private readonly string _sessionCookie;
    private readonly int _chatProtocolVersion;

    public ConnectManagerRequest(int accountId, string sessionCookie, int chatProtocolVersion)
    {
        _accountId = accountId;
        _sessionCookie = sessionCookie;
        _chatProtocolVersion = chatProtocolVersion;
    }

    public static ConnectManagerRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        ConnectManagerRequest message = new ConnectManagerRequest(
            accountId: ReadInt(data, offset, out offset),
            sessionCookie: ReadString(data, offset, out offset),
            chatProtocolVersion: ReadInt(data, offset, out offset)
        );

        updatedOffset = offset;
        return message;
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedManager connectedManager)
    {
        using var bountyContext = dbContextFactory.CreateDbContext();
        bool found = bountyContext.Accounts.Any(account => account.AccountId == _accountId && account.Cookie == _sessionCookie);
        if (!found)
        {
            connectedManager.Disconnect("Failed to resolve a manager with provided Id and SessionCookie.");
        }

        ChatServer.ConnectedManagersByAccountId.AddOrUpdate(
            _accountId,
            connectedManager,
            (accountId, previousConnectedManager) =>
            {
                previousConnectedManager.Disconnect("Another manager instance has replaced this manager instance.");
                return connectedManager;
            });

        connectedManager.SendResponse(new AcceptManagerConnectionResponse());

        // Let the GameServerManager know we want to enable replay uploads and stat resubmission.
        connectedManager.SendResponse(new UpdateManagerOptionsResponse(
            submitStatsEnabled: true,
            uploadReplaysEnabled: true,
            uploadToFTPOnDemandEnabled: false,
            uploadToHTTPOnDemandEnabled: true,
            resubmitStatsEnabled: true,
            statsResubmitMatchIDCutoff: 1)
        );
    }
}
