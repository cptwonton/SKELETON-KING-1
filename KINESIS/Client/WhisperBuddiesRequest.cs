namespace KINESIS.Client;

/// <summary>
///     Whisper Buddies is triggered when using `/b m` or `/f m` to message all of
///     your friends.
/// </summary>
public class WhisperBuddiesRequest : ProtocolRequest<ConnectedClient>
{
    // The username when encoding the message. For `<username Whispered To Buddies:> message`.
    private readonly string _message;

    public WhisperBuddiesRequest(string message)
    {
        _message = message;
    }

    public static WhisperBuddiesRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        string message = ReadString(data, offset, out offset);

        updatedOffset = offset;
        return new WhisperBuddiesRequest(message);
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient connectedClient)
    {
        ClientInformation clientInformation = connectedClient.ClientInformation;
        WhisperBuddiesResponse response = new WhisperBuddiesResponse(clientInformation.DisplayedName, _message);
        foreach (int friendAccountId in clientInformation.FriendAccountIds)
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
