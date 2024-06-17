using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

            var participant = new ParticipantInConversation { Conversation = conversation, JoinedAt = DateTime.UtcNow, User = userCreator, Role = Enums.Role.Admin };

            await _applicationContext.Conversations.AddAsync(conversation);

            await _applicationContext.ParticipantsInConversation.AddAsync(participant);

            await _applicationContext.SaveChangesAsync();
        }

        public async Task CreatePrivateConversationWithUserAsync(Guid creatorUserId, Guid userId)
        {
            var existingConversation = await _applicationContext.Conversations
                .FirstOrDefaultAsync(x => x.ParticipantsInConversation.Any(x => x.User.Id == creatorUserId) &&
                x.ParticipantsInConversation.Any(x => x.User.Id == userId));

            if (existingConversation != null)
            {

                throw new ArgumentException("conversation with this user already exists");
            }

            var creatorUser = await _applicationContext.Users.FindAsync(creatorUserId);

            if (creatorUser == null)
            {
                throw new ArgumentException("creatorUser not found");
            }

            var user = await _applicationContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("user not found");
            }

            var conversation = new Conversation();

            await _applicationContext.Conversations.AddAsync(conversation);

            var participantCreatorUser = new ParticipantInConversation
            {
                JoinedAt = DateTime.UtcNow,
                User = creatorUser,
                Conversation = conversation
            };

            var participantUser = new ParticipantInConversation
            {
                JoinedAt = DateTime.UtcNow,
                User = user,
                Conversation = conversation
            };

            await _applicationContext.ParticipantsInConversation.AddAsync(participantCreatorUser);

            await _applicationContext.ParticipantsInConversation.AddAsync(participantUser);

            await _applicationContext.SaveChangesAsync();
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
