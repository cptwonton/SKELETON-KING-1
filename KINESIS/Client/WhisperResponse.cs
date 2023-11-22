namespace KINESIS.Client;

public class WhisperResponse : ProtocolResponse
{
    private readonly string _nickname;
    private readonly string _message;

    public WhisperResponse(string nickname, string message)
    {
        _nickname = nickname;
        _message = message;
    }

    public override CommandBuffer Encode()
    {
        CommandBuffer response = new();
        response.WriteInt16(ChatServerResponse.WhisperedToPlayer);
        response.WriteString(_nickname);
        response.WriteString(_message);
        return response;
    }
}
