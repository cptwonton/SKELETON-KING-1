namespace KINESIS;

public class ConnectedManager : IConnectedSubject
{
    private readonly ChatServerConnection<ConnectedManager> _chatServerConnection;

    public ConnectedManager(Socket socket, IProtocolRequestFactory<ConnectedManager> requestFactory, IDbContextFactory<BountyContext> dbContextFactory)
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
