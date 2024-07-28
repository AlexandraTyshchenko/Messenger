using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Messenger.Api.Hubs;

[Authorize]

public class ChatHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChatHub> _logger;

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
           await Clients.Caller.SendAsync("Error", "User not found.");
           Context.Abort();
           return;
        }

        await _unitOfWork.Connections.AddConnectionAsync(user, connectionId);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task JoinGroups()
    {
        var userId = Context.UserIdentifier;
        IEnumerable<string> conversationsConnections = await _unitOfWork.Connections
            .GetUserConversationConnections(new Guid(userId));

        foreach (var conversationsConnection in conversationsConnections)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationsConnection);
            await Clients.Group(conversationsConnection).SendAsync("JoinGroups", $"successfully joined" +
                $" to {conversationsConnection}");
        }
    }
}
