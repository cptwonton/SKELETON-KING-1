namespace KINESIS;

public class ClientProtocolRequestFactory : IProtocolRequestFactory<ConnectedClient>
{
    public ProtocolRequest<ConnectedClient>? DecodeProtocolRequest(byte[] buffer, int offset, out int updatedOffset)
    {
        updatedOffset = offset;

        int messageId = BitConverter.ToInt16(buffer, offset);

        // Advance offset by 2 bytes that we just read. Note that we don't want to advance updatedOffset
        // unless the message is recognized.
        offset += 2;

        return messageId switch
        {
            ChatServerRequest.JoinChatChannel => Client.JoinChannelRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.SendChatChannelMessage => Client.SendChatChannelMessageRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.LeaveChatChannel => Client.LeaveChatChannelRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.TrackPlayerAction => Client.TrackPlayerActionRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.ConnectClient => Client.ConnectRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.CreateMatchmakingGroup => Matchmaking.CreateMatchmakingGroupRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.LeaveMatchmakingGroup => Matchmaking.LeaveMatchmakingGroupRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.LeaveMatchmakingQueue => Matchmaking.LeaveMatchmakingQueueRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.LoadingStatusChanged => Matchmaking.LoadingStatusChangedRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.EnterMatchmakingQueue => Matchmaking.EnterMatchmakingQueueRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.RefreshMatchmakingSettings => Matchmaking.RefreshMatchmakingSettingsRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.RefreshMatchmakingStats => Matchmaking.RefreshMatchmakingStatsRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.NotifyJoiningGame => Client.NotifyJoiningGameRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.NotifyJoinedGame => Client.NotifyJoinedGameRequest.Decode(buffer, offset, out updatedOffset),
            ChatServerRequest.NotifyLeftGame => Client.NotifyLeftGameRequest.Decode(buffer, offset, out updatedOffset),

            // Unknown message.
            _ => null,
        };
    }
}
