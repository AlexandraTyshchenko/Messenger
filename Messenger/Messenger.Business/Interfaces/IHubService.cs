using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;

namespace Messenger.Business.Interfaces;

public interface IHubService
{
    Task NotifyGroupAsync(Guid groupId, MessageWithSenderDto message, string method);
    Task NotifyUsersConnectionsAsync(IEnumerable<UserConnection> userConnections, NotificationDto notification, string method);
    Task JoinGroupAsync(IEnumerable<UserConnection> userConnections, Guid groupId);
    Task DisconnectFromGroupAsync(IEnumerable<UserConnection> userConnections, Guid groupId);
}
