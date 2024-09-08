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
        List<Task> joinGroupTasks = new List<Task>();

        foreach (UserConnection userConnection in userConnections)
        {
            joinGroupTasks.Add(_chatHub.Groups.AddToGroupAsync(userConnection.ConnectionId, groupId.ToString().ToUpper()));
        }

        await Task.WhenAll(joinGroupTasks);
    }

    public async Task NotifyGroupAsync(Guid groupId, MessageWithSenderDto message, string method)
    {
        await _chatHub.Clients.Group(groupId.ToString().ToUpper()).SendAsync(method, message);
    }

    public async Task NotifyUsersConnectionsAsync(IEnumerable<UserConnection> userConnections, NotificationDto notification, string method)
    {
        List<Task> notifyTasks = new List<Task>();

        foreach (UserConnection userConnection in userConnections)
        {
            notifyTasks.Add(_chatHub.Clients.Client(userConnection.ConnectionId).SendAsync(method, notification));
        }

        await Task.WhenAll(notifyTasks);
    }

    public async Task DisconnectFromGroupAsync(IEnumerable<UserConnection> userConnections, Guid groupId)
    {
        List<Task> disconnectTasks = new List<Task>();

        foreach (UserConnection userConnection in userConnections)
        {
            disconnectTasks.Add(_chatHub.Groups.RemoveFromGroupAsync(userConnection.ConnectionId, groupId.ToString().ToUpper()));
        }

        await Task.WhenAll(disconnectTasks);
    }
}
