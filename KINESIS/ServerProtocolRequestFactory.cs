namespace KINESIS;

public class ServerProtocolRequestFactory : IProtocolRequestFactory<ConnectedServer>
{
    public ProtocolRequest<ConnectedServer>? DecodeProtocolRequest(byte[] buffer, int offset, out int updatedOffset)
    {
        updatedOffset = offset;

        int messageId = BitConverter.ToInt16(buffer, offset);

        // Advance offset by 2 bytes that we just read. Note that we don't want to advance updatedOffset
        // unless the message is recognized.
        offset += 2;

        return messageId switch
        {
            // Unknown message.
            _ => null,
        };
    }
}
