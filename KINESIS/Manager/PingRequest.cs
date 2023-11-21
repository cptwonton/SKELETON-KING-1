namespace KINESIS.Manager;

public class PingRequest : ProtocolRequest<ConnectedManager>
{
    private static PingReceivedResponse _pingReceivedResponse = new PingReceivedResponse();

    public static PingRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        updatedOffset = offset;
        return new();
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedManager connectedManager)
    {
        connectedManager.SendResponse(_pingReceivedResponse);
    }
}
