namespace KINESIS.Server;

public class PingRequest : ProtocolRequest<ConnectedServer>
{
    private static PingReceivedResponse _pingReceivedResponse = new PingReceivedResponse();

    public static PingRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        updatedOffset = offset;
        return new();
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedServer connectedServer)
    {
        connectedServer.SendResponse(_pingReceivedResponse);
    }
}
