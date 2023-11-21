namespace KINESIS.Manager;

public class DisconnectManagerRequest : ProtocolRequest<ConnectedManager>
{
    public static DisconnectManagerRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        updatedOffset = offset;
        return new();
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedManager connectedManager)
    {
        connectedManager.Disconnect("Server manager requested to be disconnected.");
    }
}
