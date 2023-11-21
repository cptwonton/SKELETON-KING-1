namespace KINESIS;

public class ManagerProtocolRequestFactory : IProtocolRequestFactory<ConnectedManager>
{
    public ProtocolRequest<ConnectedManager>? DecodeProtocolRequest(byte[] buffer, int offset, out int updatedOffset)
    {
        updatedOffset = offset;

        int messageId = BitConverter.ToInt16(buffer, offset);

        // Advance offset by 2 bytes that we just read. Note that we don't want to advance updatedOffset
        // unless the message is recognized.
        offset += 2;

        return messageId switch
        {
            ChatServerRequest.ConnectManager => Manager.ConnectManagerRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.DisconnectManager => Manager.DisconnectManagerRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.UpdateManagerStatus => Manager.UpdateManagerStatusRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.Ping => Manager.PingRequest.Decode(buffer, offset, out updatedOffset),

            // Unknown message.
            _ => null,
        };
    }
}
