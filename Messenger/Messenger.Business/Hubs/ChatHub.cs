using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace Messenger.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatHub(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async override Task OnConnectedAsync()
    {
        string userId = Context.UserIdentifier;
        string connectionId = Context.ConnectionId;
        User user = await _unitOfWork.Users.GetUserByIdAsync(Guid.Parse(userId));

        if (user == null)
        {
           // await Clients.Caller.SendAsync("Error", "User not found.");
           // Context.Abort();
            return;
        }

        await _unitOfWork.Connections.AddConnectionAsync(user, connectionId);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task JoinGroups()
    {
        var userId = Context.UserIdentifier;
        IEnumerable<Conversation> conversations = await _unitOfWork.Conversations
            .GetConversationsByUserIdAsync(new Guid(userId));

        foreach (var conversation in conversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
            await Clients.Group(conversation.Id.ToString()).SendAsync("ReceiveMessage", $"successfully joined" +
                $" to {conversation.Id}");
        }
    }

    //  [ConversationRoleFilter(Role.Participant)]
    public async Task JoinGroup(ConversationNotificationDto conversationMessageDto)
    {
        string userId = Context.UserIdentifier;

        ParticipantInConversation participantInConversation = await _unitOfWork.Participants
            .GetParticipantFromConversationAsync(new Guid(userId), conversationMessageDto.ConversationId);

        if (participantInConversation == null)
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationMessageDto.ConversationId.ToString());
    }
}
