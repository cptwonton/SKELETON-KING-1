namespace KINESIS;

public class ConnectedServer : IConnectedSubject
{
    private readonly ChatServerConnection<ConnectedServer> _chatServerConnection;

    public ConnectedServer(Socket socket, IProtocolRequestFactory<ConnectedServer> requestFactory, IDbContextFactory<BountyContext> dbContextFactory)
    {
        _chatServerConnection = new(this, socket, requestFactory, dbContextFactory);
    }

    public void Start()
    {
        _chatServerConnection.Start();
    }

    public void Disconnect(string disconnectReason)
    {
        _chatServerConnection.Stop();
    }
}
