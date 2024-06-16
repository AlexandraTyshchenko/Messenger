using Messenger.Infrastructure.Dtos;
using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IMessageRepository
    {
        public Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId);
        public Task<Message> AddMessageToConversationAsync(MessageDto message, Guid conversationId, Guid userId);
    }
}
