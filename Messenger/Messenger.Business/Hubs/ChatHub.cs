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

    public ChatHub(IUnitOfWork unitOfWork, ILogger<ChatHub> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async override Task OnConnectedAsync()
    {
        try
        {
            string userId = Context.UserIdentifier;
            string connectionId = Context.ConnectionId;
            User user = await _unitOfWork.Users.GetUserByIdAsync(Guid.Parse(userId));

            if (user == null)
            {
                await Clients.Caller.SendAsync("Error", "User not found.");
                _logger.LogWarning($"User {userId} not found.");

                Context.Abort();
                return;
            }

            await _unitOfWork.Connections.AddConnectionAsync(user, connectionId);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during OnConnectedAsync.");
            throw;
        }
    }

    public async override Task OnDisconnectedAsync(Exception exception)
    {
        await Disconnect(exception);
    }

    private async Task Disconnect(Exception exception)
    {
        try
        {
            var connectionId = Context.ConnectionId;
            UserConnection connection = await _unitOfWork.Connections.RemoveConnectionAsync(connectionId);

            if (connection == null)
            {
                _logger.LogError($"Connection {connectionId} could not be deleted.");
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during OnDisconnectedAsync.");
            throw;
        }
        finally
        {
            await base.OnDisconnectedAsync(exception);
        }
    }

    public async Task JoinGroups()
    {
        try
        {
            var userId = Context.UserIdentifier;
            IEnumerable<string> conversationsConnections = await _unitOfWork.Connections
                .GetUserConversationConnections(new Guid(userId));

            List<Task> joinGroups = new List<Task>();
            List<Task> sendMessages = new List<Task>();

            foreach (var conversationsConnection in conversationsConnections)
            {
                joinGroups.Add(Groups.AddToGroupAsync(Context.ConnectionId, conversationsConnection));
                sendMessages.Add(Clients.Group(conversationsConnection).SendAsync("JoinGroups", $"successfully joined" +
                    $" to {conversationsConnection}"));
            }

            await Task.WhenAll(joinGroups);
            await Task.WhenAll(sendMessages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during JoinGroups.");
            throw;
        }
    }

}

