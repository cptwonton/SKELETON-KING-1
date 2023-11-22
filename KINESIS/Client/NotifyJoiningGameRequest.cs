namespace KINESIS.Client;

public class NotifyJoiningGameRequest : ProtocolRequest<ConnectedClient>
{
    private readonly string _serverAddress;

    public NotifyJoiningGameRequest(string serverAddress)
    {
        _serverAddress = serverAddress;
    }

    public static NotifyJoiningGameRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        string serverAddress = ReadString(data, offset, out offset);
        updatedOffset = offset;

        return new NotifyJoiningGameRequest(serverAddress);
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient connectedClient)
    {
        // How about we join a game we haven't finished yet instead?
        if (connectedClient.ReconnectToLastGame())
        {
            return;
        }

        connectedClient.NotifyJoiningGame(_serverAddress);
    }
}

