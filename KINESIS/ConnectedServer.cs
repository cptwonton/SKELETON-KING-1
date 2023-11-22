namespace KINESIS;

public class ConnectedServer : IConnectedSubject
{
    private static Server.ServerState _unknownServerState = new Server.ServerState(0, "", 0, "", "", 0, 0, Server.ServerStatus.Unknown, "", "", "", 0, 0, null!);
    private Server.ServerState _serverState = _unknownServerState;
    private int _accountId = 0;
    private int _lastHostedMatchId = 0;
    private bool _serverPortIsReachable = false;
    private readonly ChatServerConnection<ConnectedServer> _chatServerConnection;

    public Server.ServerState ServerState => _serverState;
    public int LastHostedMatchId => _lastHostedMatchId;

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

        // Regardless of the ServerStatus, make sure the reference is no longer in the idle set.
        RemoveFromIdleSet();

        ChatServer.ConnectedServers.TryRemove(this, out _);
    }

    public void SendResponse(ProtocolResponse response)
    {
        _chatServerConnection.EnqueueResponse(response);
    }

    public void Initialize(int accountId)
    {
        _accountId = accountId;
    }

    public void UpdateServerState(Server.ServerState newServerState)
    {
        while (true)
        {
            Server.ServerState oldServerState = _serverState;
            if (Interlocked.CompareExchange(ref _serverState, newServerState, oldServerState) == oldServerState)
            {
                if (oldServerState == _unknownServerState)
                {
                    // Schedule a ping.
                    Server.ServerStatusChecker.Instance.EnqueueRequest(newServerState.Address, newServerState.Port, OnPortCheckComplete);
                }
                else if (_serverPortIsReachable)
                {
                    bool wasIdle = oldServerState.Status == Server.ServerStatus.Idle;
                    bool isIdle = newServerState.Status == Server.ServerStatus.Idle;
                    if (wasIdle == isIdle)
                    {
                        // do nothing.
                    }
                    else if (wasIdle)
                    {
                        // Was idle but not anymore.
                        RemoveFromIdleSet();
                    }
                    else
                    {
                        // Was not idle but now is.
                        AddToIdleSet();
                    }
                }
                break;
            }
        }
    }
    private void OnPortCheckComplete(bool portIsReachable)
    {
        if (!portIsReachable)
        {
            Disconnect("Server port is unreachable");
            return;
        }

        if (_serverState.Status == Server.ServerStatus.Idle)
        {
            AddToIdleSet();
        }

        ChatServer.ConnectedServers.TryAdd(this, true);
        _serverPortIsReachable = true;
    }

    private void AddToIdleSet()
    {
        ConcurrentDictionary<ConnectedServer, bool> idleServersByRegion;
        while (true)
        {
            if (ChatServer.IdleServersByRegion.TryGetValue(_serverState.Location, out var tmp))
            {
                idleServersByRegion = tmp;
                break;
            }
            else
            {
                idleServersByRegion = new ConcurrentDictionary<ConnectedServer, bool>();
                if (ChatServer.IdleServersByRegion.TryAdd(_serverState.Location, idleServersByRegion))
                {
                    break;
                }
            }
        }
        idleServersByRegion.TryAdd(this, true);

        ConcurrentDictionary<ConnectedServer, bool> idleServersByManager;
        while (true)
        {
            if (ChatServer.IdleServersByManager.TryGetValue(_accountId, out var tmp))
            {
                idleServersByManager = tmp;
                break;
            }
            else
            {
                idleServersByManager = new ConcurrentDictionary<ConnectedServer, bool>();
                if (ChatServer.IdleServersByManager.TryAdd(_accountId, idleServersByManager))
                {
                    break;
                }
            }
        }
        idleServersByManager.TryAdd(this, true);
    }

    private void RemoveFromIdleSet()
    {
        if (ChatServer.IdleServersByRegion.TryGetValue(_serverState.Location, out var idleServersByRegion))
        {
            idleServersByRegion.TryRemove(this, out _);
        }

        if (ChatServer.IdleServersByManager.TryGetValue(_accountId, out var idleServersByManager))
        {
            idleServersByManager.TryRemove(this, out _);
        }
    }
}
