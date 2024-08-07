using Messenger.Api.Hubs;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.Business.Services;

public class HubService : IHubService
{
    private readonly IHubContext<ChatHub> _chatHub;

    public HubService(IHubContext<ChatHub> chatHub)
    {
        _chatHub = chatHub;
    }

    public async Task JoinGroupAsync(IEnumerable<UserConnection> userConnections, Guid groupId)
    {
        foreach (UserConnection userConnection in userConnections)
        {
            await _chatHub.Groups.AddToGroupAsync(userConnection.ConnectionId, groupId.ToString().ToUpper());
        }
    }

    public async Task NotifyGroupAsync(Guid groupId, MessageWithSenderDto message, string method)
    {
        await _chatHub.Clients.Group(groupId.ToString().ToUpper()).SendAsync(method, message);
    }

    public async Task NotifyUsersConnectionsAsync(IEnumerable<UserConnection> userConnections, NotificationDto notification, 
        string method)
    {
        foreach (UserConnection userConnection in userConnections)
        {
            await _chatHub.Clients.Client(userConnection.ConnectionId).SendAsync(method, notification);
        }
    }

    public async Task DisconnectFromGroupAsync(IEnumerable<UserConnection> userConnections, Guid groupId)
    {
        foreach (UserConnection userConnection in userConnections)
        {
            await _chatHub.Groups.RemoveFromGroupAsync(userConnection.ConnectionId, groupId.ToString().ToUpper());
        }
    }

}
