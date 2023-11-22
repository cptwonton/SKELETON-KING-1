namespace KINESIS.Client;

public class ClanWhisperRequest : ProtocolRequest<ConnectedClient>
{
    private readonly string _message;

    public ClanWhisperRequest(string message)
    {
        _message = message;
    }

    public static ClanWhisperRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        string message = ReadString(data, offset, out offset);
        updatedOffset = offset;
        return new ClanWhisperRequest(message);
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient connectedClient)
    {
        ClientInformation clientInformation = connectedClient.ClientInformation;
        ClanWhisperResponse response = new ClanWhisperResponse(connectedClient.AccountId, _message);

        if (clientInformation.ClanmateAccountIds.Length == 0)
        {
            // No clanmates to whisper to.
            connectedClient.SendResponse(new ClanWhisperFailedResponse());
            return;
        }

        foreach (int friendAccountId in clientInformation.ClanmateAccountIds)
        {
            if (ChatServer.ConnectedClientsByAccountId.TryGetValue(friendAccountId, out var client))
            {
                switch (client.ClientInformation.ChatMode)
                {
                    // For DND and Invisible users, do not send them the message.
                    case ChatMode.Dnd:
                    case ChatMode.Invisible:
                        continue;

                    case ChatMode.Afk:
                    case ChatMode.Available:
                        client.SendResponse(response);
                        continue;
                }
            }
        }
    }
}
