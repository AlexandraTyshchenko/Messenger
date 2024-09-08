using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Messenger.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationContext _applicationContext;

    public MessageRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    public async Task<Message> AddMessageToConversationAsync(Message message)
    {
        if (message.Sender != null)
        {
            var trackedUser = _applicationContext.Users.Local
               .FirstOrDefault(u => u.Id == message.Sender.Id);

            if (trackedUser != null)
            {
                message.Sender = trackedUser;
            }
            else
            {
                _applicationContext.Attach(message.Sender);
            }
        }      

        await _applicationContext.Messages.AddAsync(message);
        return message;
    }



    public async Task<Message> DeleteMessageAsync(Guid messageId)
    {
        Message message = await GetMessageByIdAsync(messageId);

        if (message is null)
        {
            return null;
        }

        Message deletedMessage = _applicationContext.Messages.Remove(message).Entity;

        return deletedMessage;
    }

    public async Task<Message> GetMessageByIdAsync(Guid messageId)
    {
        Message message = await _applicationContext.Messages
            .Include(x => x.Conversation)
            .Include(x => x.Sender)
            .FirstOrDefaultAsync(x => x.Id == messageId);

        return message;
    }

    public async Task<IPagedEntities<Message>> GetMessagesByConversationIdAsync(Guid conversationId,
        int page, int pageSize)
    {
        IQueryable<Message> messages = _applicationContext.Messages
            .Include(x => x.Image)
            .Where(m => m.Conversation.Id == conversationId)
            .Include(x => x.Sender)
            .OrderByDescending(x => x.SentAt);

        IPagedEntities<Message> pagedMessages = await messages.WithPagingAsync(page, pageSize);

        return pagedMessages;
    }
}
