using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IMessageRepository
    {
        public Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId);
        public Task<Message> AddMessageToConversationAsync(string text, Conversation conversation, User sender);
        public Task<Message> GetMessageByIdAsync(Guid messageId);
        public Task<Message> DeleteMessageAsync(Guid messageId);
    }
}
