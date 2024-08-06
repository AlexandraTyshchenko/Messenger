using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Pagination;

namespace Messenger.Infrastructure.Interfaces;

public interface IMessageRepository
{
    public Task<IPagedEntities<Message>> GetMessagesByConversationIdAsync(Guid conversationId, int page, int pageSize);
    public Task<Message> AddMessageToConversationAsync(Message message);
    public Task<Message> GetMessageByIdAsync(Guid messageId);
    public Task<Message> DeleteMessageAsync(Guid messageId);
}
