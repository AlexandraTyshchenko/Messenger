using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationContext _applicationContext;
        public MessageRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<Message> AddMessageToConversationAsync(string messageText,
            string attachmentUrl, Guid conversationId, Guid senderId)
        {
            User sender = await _applicationContext.Users.FirstOrDefaultAsync(x => x.Id == senderId);

            Conversation conversation = await _applicationContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);

            if (conversation == null)
            {
                throw new NotFoundException("No conversation was found");
            }

            var message = new Message
            {
                MessageText = messageText,
                AttachmentUrl = attachmentUrl,
                SentAt = DateTime.UtcNow,
                Sender = sender!,
                Conversation = conversation
            };

            await _applicationContext.Messages.AddAsync(message);

            await _applicationContext.SaveChangesAsync();
            return message;
        }

        public async Task DeleteMessageByIdAsync(Guid messageId)
        {
            Message message = await _applicationContext.Messages
                         .Include(x => x.Conversation)
                         .Include(x => x.Sender)
                         .FirstOrDefaultAsync(x => x.Id == messageId);

            if (message == null)
            {
                throw new NotFoundException($"Message with id {messageId} wasn`t found");
            }

            _applicationContext.Messages.Remove(message);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<Message> GetMessageByIdAsync(Guid messageId)
        {
            Message message = await _applicationContext.Messages
                .Include(x => x.Conversation)
                .Include(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == messageId);

            if (message == null)
            {
                throw new NotFoundException($"Message with id {messageId} wasn`t found");
            }

            return message;
        }

        public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId)
        {
            var messages = await _applicationContext.Messages
                                        .Where(m => m.Conversation.Id == conversationId)
                                        .Include(x => x.Sender)
                                        .OrderBy(x => x.SentAt)
                                        .ToListAsync();

            return messages;
        }

    }
}
