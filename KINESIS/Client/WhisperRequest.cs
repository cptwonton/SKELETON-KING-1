namespace KINESIS.Client;

public class WhisperRequest : ProtocolRequest<ConnectedClient>
{
    private readonly string _nickname;
    private readonly string _message;

    public WhisperRequest(string nickname, string message)
    {
        _nickname = nickname;
        _message = message;
    }

    public static WhisperRequest Decode(byte[] data, int offset, out int updatedOffset)
    {
        string nickname = ReadString(data, offset, out offset);
        string message = ReadString(data, offset, out offset);

        updatedOffset = offset;
        return new WhisperRequest(nickname, message);
    }

    public override void HandleRequest(IDbContextFactory<BountyContext> dbContextFactory, ConnectedClient whisperFrom)
    {
        foreach (var entry in ChatServer.ConnectedClientsByAccountId)
        {
            ConnectedClient whisperTo = entry.Value;
            var whisperToInformation = whisperTo.ClientInformation;
            if (_nickname.Equals(whisperToInformation.AccountName, StringComparison.OrdinalIgnoreCase) || _nickname.Equals(whisperToInformation.DisplayedName, StringComparison.OrdinalIgnoreCase))
            {
                WhisperTo(whisperFrom, whisperTo, whisperToInformation);
                return;
            }
        }

        // User is offline.
        whisperFrom.SendResponse(new WhisperFailedResponse());
    }

    private void WhisperTo(ConnectedClient whisperFrom, ConnectedClient whisperTo, ClientInformation whisperToInformation)
    {
        switch (whisperToInformation.ChatMode)
        {
            case ChatMode.Available:
                whisperTo.SendResponse(new WhisperResponse(whisperFrom.ClientInformation.DisplayedName, _message));
                break;

            case ChatMode.Afk:
                // When AFK, deliver the message but let sender know the recipient is AFK.
                whisperTo.SendResponse(new WhisperResponse(whisperFrom.ClientInformation.DisplayedName, _message));
                whisperFrom.SendResponse(new ChatModeAutoResponse(whisperToInformation.ChatMode, whisperToInformation.DisplayedName, whisperToInformation.ChatModeDescription));
                break;

            case ChatMode.Dnd:
                // When DND, don't deliver the message and let sender know the recipient is DND.
                whisperFrom.SendResponse(new ChatModeAutoResponse(whisperToInformation.ChatMode, whisperToInformation.DisplayedName, whisperToInformation.ChatModeDescription));
                break;

            case ChatMode.Invisible:
                // Treat invisible as offline.
                whisperFrom.SendResponse(new WhisperFailedResponse());
                break;
        }
    }
}
