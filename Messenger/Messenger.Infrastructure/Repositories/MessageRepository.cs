using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
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
            string attachmentUrl, Conversation conversation, User sender)
        {    
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

        public async Task DeleteMessageAsync(Message message)
        {          
           Message message1 =  _applicationContext.Messages.Remove(message).Entity;
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<Message> GetMessageByIdAsync(Guid messageId)
        {
            Message message = await _applicationContext.Messages
                .Include(x => x.Conversation)
                .Include(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == messageId);

            return message;
        }

        public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(Guid conversationId)
        {
            IEnumerable<Message> messages = await _applicationContext.Messages
                                        .Where(m => m.Conversation.Id == conversationId)
                                        .Include(x => x.Sender)
                                        .OrderBy(x => x.SentAt)
                                        .ToListAsync();

            return messages;
        }
    }
}
