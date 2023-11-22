namespace KINESIS.Client;

public class NotifyLeftGameRequest : ProtocolRequest<ConnectedClient>
{
    public static NotifyLeftGameRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        updatedOffset = offset;
        return new NotifyLeftGameRequest();
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient connectedClient)
    {
        connectedClient.NotifyLeftGame();
    }
}

