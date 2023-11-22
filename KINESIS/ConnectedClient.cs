namespace KINESIS;

public class ClientInformation
{
    public readonly string DisplayedName;
    public readonly Client.ChatClientFlags ChatClientFlags;
    public readonly Client.ChatClientStatus ChatClientStatus;
    public readonly string SelectedChatSymbolCode;
    public readonly string SelectedChatNameColourCode;
    public readonly string SelectedAccountIconCode;
    public readonly int AscensionLevel;
    public readonly string UpperCaseClanName;
    public readonly string ServerAddress;
    public readonly string GameName;
    public readonly int MatchId;
    public readonly int ClanIdOrZero;
    public readonly string ClanTagOrEmpty;
    public readonly int[] FriendsClanmatesAccountIds;

    public ClientInformation(string displayedName, Client.ChatClientFlags chatClientFlags, Client.ChatClientStatus chatClientStatus, string selectedChatSymbolCode, string selectedChatNameColourCode, string selectedAccountIconCode, int ascensionLevel, string upperCaseClanName, string serverAddress, string gameName, int matchId, int clanIdOrZero, string clanTagOrEmpty, int[] friendsClanmatesAccountIds)
    {
        DisplayedName = displayedName;
        ChatClientFlags = chatClientFlags;
        ChatClientStatus = chatClientStatus;
        SelectedChatSymbolCode = selectedChatSymbolCode;
        SelectedChatNameColourCode = selectedChatNameColourCode;
        SelectedAccountIconCode = selectedAccountIconCode;
        AscensionLevel = ascensionLevel;
        UpperCaseClanName = upperCaseClanName;
        ServerAddress = serverAddress;
        GameName = gameName;
        MatchId = matchId;
        ClanIdOrZero = clanIdOrZero;
        ClanTagOrEmpty = clanTagOrEmpty;
        FriendsClanmatesAccountIds = friendsClanmatesAccountIds;
    }
}

public class ConnectedClient : IConnectedSubject
{
    private readonly ChatServerConnection<ConnectedClient> _chatServerConnection;
    private int _accountId;
    private ClientInformation _clientInformation = null!;
    private readonly List<Client.ChatChannel> _chatChannels = new();
    private Matchmaking.MatchmakingGroup? _matchmakingGroup = null;

    public int AccountId => _accountId;
    public ClientInformation ClientInformation => _clientInformation;
    public Matchmaking.MatchmakingGroup? MatchmakingGroup => _matchmakingGroup;

    public ConnectedClient(Socket socket, IProtocolRequestFactory<ConnectedClient> requestFactory, IDbContextFactory<BountyContext> dbContextFactory)
    {
        _chatServerConnection = new(this, socket, requestFactory, dbContextFactory);
    }

    public void Disconnect(string disconnectReason)
    {
        ChatServer.ConnectedClientsByAccountId.TryRemove(new KeyValuePair<int,ConnectedClient>(_accountId, this));

        Client.ChatChannel[] chatChannels;
        lock (this)
        {
            chatChannels = _chatChannels.ToArray();
            _chatChannels.Clear();
        }

        foreach (Client.ChatChannel chatChannel in chatChannels)
        {
            chatChannel.Remove(this, notifyClient: false);
        }

        _chatServerConnection.Stop();
    }

    public void Start()
    {
        _chatServerConnection.Start();
    }

    public bool ReconnectToLastGame()
    {
        // Not implemented.
        return false;
    }

    public void SendResponse(ProtocolResponse response)
    {
        _chatServerConnection.EnqueueResponse(response);
    }

    public void Initialize(int accountId, ClientInformation clientInformation)
    {
        _accountId = accountId;
        _clientInformation = clientInformation;

        // Register new connection, drop old connection if still registered.
        ChatServer.ConnectedClientsByAccountId.AddOrUpdate(accountId, this, (accountId, oldClient) =>
        {
            oldClient.Disconnect("Replaced with a new connection.");
            return this;
        });
    }

    /// <summary>
    ///     Notifies the ConnectedClient that the client has joined a specified chat channel.
    /// </summary>
    /// <param name="chatChannel">The chat channel that the client has joined.</param>
    /// <param name="response">A notification to send back to the client.</param>
    public void NotifyAddedToChatChannel(Client.ChatChannel chatChannel, ProtocolResponse response)
    {
        lock (this)
        {
            _chatChannels.Add(chatChannel);
        }
        _chatServerConnection.EnqueueResponse(response);
    }

    /// <summary>
    ///     Notifies the ConnectedClient that the client has been either willingly or forcefully removed from a chat channel.
    ///     When the client willingly leaves the channel, we do not necessarily need to notify them about it, in which case 
    ///     the <paramref name="response"/> can be null.
    /// </summary>
    /// <param name="chatChannel">The chat channel that the client has left.</param>
    /// <param name="response">An optional notification to send back to the client.</param>

    // 
    // response para
    public void NotifyRemovedFromChatChannel(Client.ChatChannel chatChannel, ProtocolResponse? response)
    {
        lock (this)
        {
            _chatChannels.Remove(chatChannel);
        }
        if (response != null)
        {
            _chatServerConnection.EnqueueResponse(response);
        }
    }

    public void NotifyJoiningGame(string serverAddress)
    {
        ClientInformation newClientInformation;
        while (true)
        {
            ClientInformation oldClientInformation = _clientInformation;
            newClientInformation = new ClientInformation(
                displayedName: oldClientInformation.DisplayedName,
                chatClientFlags: oldClientInformation.ChatClientFlags,
                chatClientStatus: Client.ChatClientStatus.JoiningGame,
                selectedChatSymbolCode: oldClientInformation.SelectedChatSymbolCode,
                selectedChatNameColourCode: oldClientInformation.SelectedChatNameColourCode,
                selectedAccountIconCode: oldClientInformation.SelectedAccountIconCode,
                ascensionLevel: oldClientInformation.AscensionLevel,
                upperCaseClanName: oldClientInformation.UpperCaseClanName,
                serverAddress: serverAddress,
                gameName: "",
                matchId: 0,
                clanIdOrZero: oldClientInformation.ClanIdOrZero,
                clanTagOrEmpty: oldClientInformation.ClanTagOrEmpty,
                friendsClanmatesAccountIds: oldClientInformation.FriendsClanmatesAccountIds);
            if (Interlocked.CompareExchange(ref _clientInformation, newClientInformation, oldClientInformation) == oldClientInformation)
            {
                break;
            }
        }

        HashSet<int> accountIds = new HashSet<int>(newClientInformation.FriendsClanmatesAccountIds);
        foreach (Client.ChatChannel chatChannel in _chatChannels)
        {
            chatChannel.CollectAccountIds(accountIds);
        }

        Client.ClientStatusUpdatedResponse clientStatusUpdatedResponse = new Client.ClientStatusUpdatedResponse(
            _accountId,
            newClientInformation.ChatClientStatus,
            newClientInformation.ChatClientFlags,
            newClientInformation.ServerAddress,
            newClientInformation.GameName,
            newClientInformation.MatchId,
            newClientInformation.ClanIdOrZero,
            newClientInformation.ClanTagOrEmpty,
            newClientInformation.SelectedChatSymbolCode,
            newClientInformation.SelectedChatNameColourCode,
            newClientInformation.SelectedAccountIconCode,
            newClientInformation.AscensionLevel);

        accountIds.Remove(_accountId);
        foreach (int accountId in accountIds)
        {
            if (ChatServer.ConnectedClientsByAccountId.TryGetValue(accountId, out var friend))
            {
                friend.SendResponse(clientStatusUpdatedResponse);
            }
        }
    }

    public void NotifyJoinedGame(string gameName, int matchId)
    {
        ClientInformation newClientInformation;
        while (true)
        {
            ClientInformation oldClientInformation = _clientInformation;
            newClientInformation = new ClientInformation(
                displayedName: oldClientInformation.DisplayedName,
                chatClientFlags: oldClientInformation.ChatClientFlags,
                chatClientStatus: Client.ChatClientStatus.InGame,
                selectedChatSymbolCode: oldClientInformation.SelectedChatSymbolCode,
                selectedChatNameColourCode: oldClientInformation.SelectedChatNameColourCode,
                selectedAccountIconCode: oldClientInformation.SelectedAccountIconCode,
                ascensionLevel: oldClientInformation.AscensionLevel,
                upperCaseClanName: oldClientInformation.UpperCaseClanName,
                serverAddress: oldClientInformation.ServerAddress,
                gameName: gameName,
                matchId: matchId,
                clanIdOrZero: oldClientInformation.ClanIdOrZero,
                clanTagOrEmpty: oldClientInformation.ClanTagOrEmpty,
                friendsClanmatesAccountIds: oldClientInformation.FriendsClanmatesAccountIds);
            if (Interlocked.CompareExchange(ref _clientInformation, newClientInformation, oldClientInformation) == oldClientInformation)
            {
                break;
            }
        }

        HashSet<int> accountIds = new HashSet<int>(newClientInformation.FriendsClanmatesAccountIds);
        foreach (Client.ChatChannel chatChannel in _chatChannels)
        {
            chatChannel.CollectAccountIds(accountIds);
        }

        Client.ClientStatusUpdatedResponse clientStatusUpdatedResponse = new Client.ClientStatusUpdatedResponse(
            _accountId,
            newClientInformation.ChatClientStatus,
            newClientInformation.ChatClientFlags,
            newClientInformation.ServerAddress,
            newClientInformation.GameName,
            newClientInformation.MatchId,
            newClientInformation.ClanIdOrZero,
            newClientInformation.ClanTagOrEmpty,
            newClientInformation.SelectedChatSymbolCode,
            newClientInformation.SelectedChatNameColourCode,
            newClientInformation.SelectedAccountIconCode,
            newClientInformation.AscensionLevel);

        accountIds.Remove(_accountId);
        foreach (int accountId in accountIds)
        {
            if (ChatServer.ConnectedClientsByAccountId.TryGetValue(accountId, out var friend))
            {
                friend.SendResponse(clientStatusUpdatedResponse);
            }
        }
    }

    public void NotifyLeftGame()
    {
        ClientInformation newClientInformation;
        while (true)
        {
            ClientInformation oldClientInformation = _clientInformation;
            newClientInformation = new ClientInformation(
                displayedName: oldClientInformation.DisplayedName,
                chatClientFlags: oldClientInformation.ChatClientFlags,
                chatClientStatus: Client.ChatClientStatus.Connected,
                selectedChatSymbolCode: oldClientInformation.SelectedChatSymbolCode,
                selectedChatNameColourCode: oldClientInformation.SelectedChatNameColourCode,
                selectedAccountIconCode: oldClientInformation.SelectedAccountIconCode,
                ascensionLevel: oldClientInformation.AscensionLevel,
                upperCaseClanName: oldClientInformation.UpperCaseClanName,
                serverAddress: oldClientInformation.ServerAddress,
                gameName: oldClientInformation.GameName,
                matchId: oldClientInformation.MatchId,
                clanIdOrZero: oldClientInformation.ClanIdOrZero,
                clanTagOrEmpty: oldClientInformation.ClanTagOrEmpty,
                friendsClanmatesAccountIds: oldClientInformation.FriendsClanmatesAccountIds);
            if (Interlocked.CompareExchange(ref _clientInformation, newClientInformation, oldClientInformation) == oldClientInformation)
            {
                break;
            }
        }

        HashSet<int> accountIds = new HashSet<int>(newClientInformation.FriendsClanmatesAccountIds);
        foreach (Client.ChatChannel chatChannel in _chatChannels)
        {
            chatChannel.CollectAccountIds(accountIds);
        }

        Client.ClientStatusUpdatedResponse clientStatusUpdatedResponse = new Client.ClientStatusUpdatedResponse(
            _accountId,
            newClientInformation.ChatClientStatus,
            newClientInformation.ChatClientFlags,
            newClientInformation.ServerAddress,
            newClientInformation.GameName,
            newClientInformation.MatchId,
            newClientInformation.ClanIdOrZero,
            newClientInformation.ClanTagOrEmpty,
            newClientInformation.SelectedChatSymbolCode,
            newClientInformation.SelectedChatNameColourCode,
            newClientInformation.SelectedAccountIconCode,
            newClientInformation.AscensionLevel);

        accountIds.Remove(_accountId);
        foreach (int accountId in accountIds)
        {
            if (ChatServer.ConnectedClientsByAccountId.TryGetValue(accountId, out var friend))
            {
                friend.SendResponse(clientStatusUpdatedResponse);
            }
        }
    }

    public Matchmaking.MatchmakingGroup? ReplaceMatchmakingGroup(Matchmaking.MatchmakingGroup? newGroup)
    {
        return Interlocked.Exchange(ref _matchmakingGroup, newGroup);
    }
}
