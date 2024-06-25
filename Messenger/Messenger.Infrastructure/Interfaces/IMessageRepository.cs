using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IMessageRepository
    {
        public Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId);
        public Task<Message> AddMessageToConversationAsync(string messageText,
            string attachmentUrl, Guid conversationId, Guid senderId);
        public Task<Message> GetMessageByIdAsync(Guid messageId);
        public Task DeleteMessageByIdAsync(Guid messageId);
    }
}
