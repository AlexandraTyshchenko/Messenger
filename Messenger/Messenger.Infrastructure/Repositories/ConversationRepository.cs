using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Messenger.Infrastructure.Repositories.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationContext _applicationContext;

        public ConversationRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task CreateGroupConversationAsync(string title, string imgUrl, Guid creatorId)
        {
            var group = new Group
            {
                Title = title,
                ImgUrl = imgUrl
            };

            await _applicationContext.Groups.AddAsync(group);

            var conversation = new Conversation { Group = group };

            var userCreator = await _applicationContext.Users.FindAsync(creatorId);

            var participant = new ParticipantInConversation
            { 
                Conversation = conversation, 
                JoinedAt = DateTime.UtcNow,
                User = userCreator,
                Role = Role.Admin 
            };

            await _applicationContext.Conversations.AddAsync(conversation);

            await _applicationContext.ParticipantsInConversation.AddAsync(participant);

            await _applicationContext.SaveChangesAsync();
        }

        public async Task CreatePrivateConversationWithUserAsync(Guid creatorUserId, Guid userId)
        {
            var existingConversation = await _applicationContext.Conversations
                .FirstOrDefaultAsync(x => x.ParticipantsInConversation.Any(x => x.User.Id == creatorUserId) &&
                x.ParticipantsInConversation.Any(x => x.User.Id == userId) && x.Group == null);

            if (existingConversation != null)
            {
                throw new ConflictException("conversation with this user already exists");
            }

            var creatorUser = await _applicationContext.Users.FindAsync(creatorUserId);

            var user = await _applicationContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            var conversation = new Conversation();

            await _applicationContext.Conversations.AddAsync(conversation);

            var participantCreatorUser = new ParticipantInConversation
            {
                JoinedAt = DateTime.UtcNow,
                User = creatorUser,
                Conversation = conversation,
                Role = Role.Admin
            };

            var participantUser = new ParticipantInConversation
            {
                JoinedAt = DateTime.UtcNow,
                User = user,
                Conversation = conversation,
                Role = Role.Admin
            };

            await _applicationContext.ParticipantsInConversation.AddAsync(participantCreatorUser);

            await _applicationContext.ParticipantsInConversation.AddAsync(participantUser);

            await _applicationContext.SaveChangesAsync();
        }

        public async Task<Conversation> DeleteConversationAsync(Guid conversationId)
        {
            var conversation = await _applicationContext.FindAsync<Conversation>(conversationId);

            if (conversation == null)
            {
                throw new NotFoundException($"Conversation with id {conversationId} wasn`t found");
            }

            _applicationContext.Remove(conversation);

            await _applicationContext.SaveChangesAsync();

            return conversation;
        }

        public async Task<Conversation> GetConversationByIdAsync(Guid conversationId)
        {
            var conversation = await _applicationContext.Conversations.Include(x=>x.Group).FirstOrDefaultAsync(x=>x.Id == conversationId);

            if (conversation == null)
            {
                throw new NotFoundException($"Conversation with id {conversationId} wasn`t found");
            }
            return conversation;
        }

        public async Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(Guid userId)
        {
            return await _applicationContext.Conversations
                 .Where(x => x.ParticipantsInConversation.Any(x => x.User.Id == userId))
                 .Include(x => x.Group)
                 .Include(x => x.Messages)
                     .ThenInclude(x => x.Sender)
                 .ToListAsync();
        }
    }
}
