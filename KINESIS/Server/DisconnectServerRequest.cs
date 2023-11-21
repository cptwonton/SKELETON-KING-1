namespace KINESIS.Server;

public class DisconnectServerRequest : ProtocolRequest<ConnectedServer>
{
    public static DisconnectServerRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        updatedOffset = offset;
        return new DisconnectServerRequest();
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedServer connectedServer)
    {
        connectedServer.Disconnect("Game server requested to be disconnected.");
    }
}
