using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Dtos;
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

        public async Task<Message> AddMessageToConversationAsync(MessageDto messageDto, Guid conversationId, Guid senderId)
        {
            var sender = await _applicationContext.Users.FirstOrDefaultAsync(x => x.Id == senderId);
            //todo add exception middleware
            var conversation = await _applicationContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);

            var message = new Message
            {
                MessageText = messageDto.MessageText,
                AttachmentUrl = messageDto.AttachmentUrl,
                SentAt = DateTime.UtcNow,
                Sender = sender,
                Conversation = conversation
            };

            await _applicationContext.Messages.AddAsync(message);

            await _applicationContext.SaveChangesAsync();
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
