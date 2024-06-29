using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IMessageRepository
    {
        public Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId);
        public Task<Message> AddMessageToConversationAsync(string messageText,
            string attachmentUrl, Conversation conversation, User sender);
        public Task<Message> GetMessageByIdAsync(Guid messageId);
        public Task DeleteMessageAsync(Message message);
    }
}
