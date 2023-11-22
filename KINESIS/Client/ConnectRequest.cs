namespace KINESIS.Client;


public class ConnectRequestData
{
    public readonly string AccountName;
    public readonly string UserId;
    public readonly int ClanIdOrZero;
    public readonly string ClanTagOrEmpty;
    public readonly string SelectedChatSymbolCode;
    public readonly string SelectedChatNameColourCode;
    public readonly string SelectedAccountIconCode;

    public ConnectRequestData(string accountName, string userId, int clanIdOrZero, string clanTagOrEmpty, ICollection<string> selectedUpgradeCodes)
    {
        AccountName = accountName;
        UserId = userId;
        ClanIdOrZero = clanIdOrZero;
        ClanTagOrEmpty = clanTagOrEmpty;

        string? selectedChatSymbolCode = selectedUpgradeCodes.FirstOrDefault(upgrade => upgrade.StartsWith("cs."));
        SelectedChatSymbolCode = selectedChatSymbolCode != null ? selectedChatSymbolCode.Substring(3) : "";

        string? selectedChatNameColourCode = selectedUpgradeCodes.FirstOrDefault(upgrade => upgrade.StartsWith("cc."));
        SelectedChatNameColourCode = selectedChatNameColourCode != null ? selectedChatNameColourCode.Substring(3) : "white";

        string? selectedAccountIconCode = selectedUpgradeCodes.FirstOrDefault(upgrade => upgrade.StartsWith("ai."));
        SelectedAccountIconCode = selectedAccountIconCode != null ? selectedAccountIconCode.Substring(3) : "Default Icon";
    }
}

public class ConnectRequest : ProtocolRequest<ConnectedClient>
{
    // Some of these are currently unused but kept for future reference.
#pragma warning disable IDE0052
    private readonly int _accountId;
    private readonly string _sessionCookie;
    private readonly string _externalIp;
    private readonly string _sessionAuthHash;
    private readonly int _chatProtocolVersion;
    private readonly byte _operatingSystem;
    private readonly byte _osMajorVersion;
    private readonly byte _osMinorVersion;
    private readonly byte _osMicroVersion;
    private readonly string _osBuildCode;
    private readonly string _osArchitecture;
    private readonly byte _clientVersionMajor;
    private readonly byte _clientVersionMinor;
    private readonly byte _clientVersionMicro;
    private readonly byte _clientVersionHotfix;
    private readonly byte _lastKnownClientState;
    private readonly byte _clientChatModeState;
    private readonly string _clientRegion;
    private readonly string _clientLanguage;
#pragma warning restore IDE0052

    public ConnectRequest(int accountId, string sessionCookie, string externalIp, string sessionAuthHash, int chatProtocolVersion, byte operatingSystem, byte osMajorVersion, byte osMinorVersion, byte osMicroVersion, string osBuildCode, string osArchitecture, byte clientVersionMajor, byte clientVersionMinor, byte clientVersionMicro, byte clientVersionHotfix, byte lastKnownClientState, byte clientChatModeState, string clientRegion, string clientLanguage)
    {
        _accountId = accountId;
        _sessionCookie = sessionCookie;
        _externalIp = externalIp;
        _sessionAuthHash = sessionAuthHash;
        _chatProtocolVersion = chatProtocolVersion;
        _operatingSystem = operatingSystem;
        _osMajorVersion = osMajorVersion;
        _osMinorVersion = osMinorVersion;
        _osMicroVersion = osMicroVersion;
        _osBuildCode = osBuildCode;
        _osArchitecture = osArchitecture;
        _clientVersionMajor = clientVersionMajor;
        _clientVersionMinor = clientVersionMinor;
        _clientVersionMicro = clientVersionMicro;
        _clientVersionHotfix = clientVersionHotfix;
        _lastKnownClientState = lastKnownClientState;
        _clientChatModeState = clientChatModeState;
        _clientRegion = clientRegion;
        _clientLanguage = clientLanguage;
    }

    public static ConnectRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        ConnectRequest message = new(
            accountId: ReadInt(data, offset, out offset),
            sessionCookie: ReadString(data, offset, out offset),
            externalIp: ReadString(data, offset, out offset),
            sessionAuthHash: ReadString(data, offset, out offset),
            chatProtocolVersion: ReadInt(data, offset, out offset),
            operatingSystem: ReadByte(data, offset, out offset),
            osMajorVersion: ReadByte(data, offset, out offset),
            osMinorVersion: ReadByte(data, offset, out offset),
            osMicroVersion: ReadByte(data, offset, out offset),
            osBuildCode: ReadString(data, offset, out offset),
            osArchitecture: ReadString(data, offset, out offset),
            clientVersionMajor: ReadByte(data, offset, out offset),
            clientVersionMinor: ReadByte(data, offset, out offset),
            clientVersionMicro: ReadByte(data, offset, out offset),
            clientVersionHotfix: ReadByte(data, offset, out offset),
            lastKnownClientState: ReadByte(data, offset, out offset),
            clientChatModeState: ReadByte(data, offset, out offset),
            clientRegion: ReadString(data, offset, out offset),
            clientLanguage: ReadString(data, offset, out offset)
        );

        updatedOffset = offset;
        return message;
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient connectedClient)
    {
        int accountId = _accountId;
        string sessionCookie = _sessionCookie;

        using var bountyContext = dbContextFactory.CreateDbContext();

        ConnectRequestData? data = bountyContext.Accounts
            .Where(account => account.AccountId == accountId && account.Cookie == sessionCookie)
            .Select(account => new ConnectRequestData(
                    account.Name,
                    account.User.Id,
                    account.Clan == null ? 0 : account.Clan.ClanId,
                    account.Clan == null ? "" : account.Clan.Tag,
                    account.SelectedUpgradeCodes))
            .FirstOrDefault();
        if (data == null)
        {
            // Invalid session cookie, reject this connection.
            connectedClient.SendResponse(new ConnectionRejectedResponse(ConnectionRejectedReason.AuthFailed));
            connectedClient.Disconnect("Authentication Failed");
            return;
        }

        string upToDateClientVersion = "4.10.9"; // TODO: check current version properly.
        string clientVersion = $"{_clientVersionMajor}.{_clientVersionMinor}.{_clientVersionMicro}";
        if (clientVersion != upToDateClientVersion)
        {
            connectedClient.SendResponse(new ConnectionRejectedResponse(ConnectionRejectedReason.BadVersion));
            connectedClient.Disconnect("Client Version Does Not Match");
            return;
        }

        // Accept connection.
        connectedClient.SendResponse(new ConnectionAcceptedResponse());

        // Disconnect all subaccounts, if any are online.
        foreach (int otherAccountId in bountyContext.Accounts.Where(a => a.User.Id == data.UserId).Select(a => a.AccountId))
        {
            if (ChatServer.ConnectedClientsByAccountId.TryRemove(otherAccountId, out var otherConnectedClient))
            {
                // Drop older client.
                otherConnectedClient.SendResponse(new ConnectionRejectedResponse(ConnectionRejectedReason.AccountSharing));
                otherConnectedClient.Disconnect("Another Client Instance Has Replaced This Client Instance");
            }
        }

        // TODO: include clan name if applicable.
        string displayedName = data.AccountName;

        // TODO: pass correct flags.
        ChatClientFlags chatClientFlags = ChatClientFlags.IsPremium;
        ChatClientStatus chatClientStatus = ChatClientStatus.Connected;

        int ascensionLevel = 0;
        string upperCaseClanName = "";
        ClientInformation clientInformation = new ClientInformation(
                displayedName: displayedName,
                chatClientFlags: chatClientFlags,
                chatClientStatus: chatClientStatus,
                selectedChatSymbolCode: data.SelectedChatSymbolCode,
                selectedChatNameColourCode: data.SelectedChatNameColourCode,
                selectedAccountIconCode: data.SelectedAccountIconCode,
                ascensionLevel: ascensionLevel,
                upperCaseClanName: upperCaseClanName,
                serverAddress: "",
                gameName: "",
                matchId: 0,
                clanIdOrZero: data.ClanIdOrZero,
                clanTagOrEmpty: data.ClanTagOrEmpty,
                friendsClanmatesAccountIds: new int[0]);

        connectedClient.Initialize(_accountId, clientInformation);
    }
}
