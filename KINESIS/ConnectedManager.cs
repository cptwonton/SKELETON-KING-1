namespace KINESIS;

public class ConnectedManager : IConnectedSubject
{
    private readonly ChatServerConnection<ConnectedManager> _chatServerConnection;
    private static Manager.ManagerState _unknownManagerState = new(0, "", "", "", "", "", 0, false);
    private Manager.ManagerState _managerState = _unknownManagerState;

    public Manager.ManagerState ManagerState => _managerState;

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

    public void SendResponse(ProtocolResponse response)
    {
        _chatServerConnection.EnqueueResponse(response);
    }

    public void UpdateManagerState(Manager.ManagerState managerState)
    {
        _managerState = managerState;
    }
}
