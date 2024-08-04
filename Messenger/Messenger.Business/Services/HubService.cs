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
    
    public async Task NotifyGroupAsync(Guid groupId, MessageWithSenderDto message, string method)
    {
        await _chatHub.Clients.Group(groupId.ToString().ToUpper()).SendAsync(method, message);
    }

    public async Task NotifyUsersConnectionsAsync(IEnumerable<UserConnection> userConnections, MessageDto message, 
        string method)
    {
        foreach (UserConnection userConnection in userConnections)
        {
            await _chatHub.Clients.Client(userConnection.ConnectionId).SendAsync(method, message);
        }
    }
}
