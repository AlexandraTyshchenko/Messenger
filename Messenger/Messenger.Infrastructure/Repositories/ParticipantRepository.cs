using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationContext _applicationContext;

        public ParticipantRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task AddParticipantsToConversationAsync(Guid[] userIds, Guid conversationId)
        {
            var users = await _applicationContext.Users
                                        .Where(user => userIds.Contains(user.Id))
                                        .ToListAsync();

            var missingUserIds = userIds.Where(id => !users.Select(x => x.Id).Contains(id)).ToList();

            if (missingUserIds.Count != 0)
            {
                throw new NotFoundException($"users with ids {string.Join(", ", missingUserIds)} were not found ");
            }

            var conversation = await _applicationContext.Conversations.FindAsync(conversationId);

            if (conversation == null)
            {
                throw new NotFoundException("conversation not found.");
            }

            var existingParticipants = await _applicationContext.ParticipantsInConversation.
                Where(x => users.Contains(x.User) && x.Conversation.Id == conversation.Id).ToListAsync();

            var participantsUserNames = string.Join(", ", existingParticipants.Select(x => x.User.UserName));

            if (existingParticipants.Count != 0)
            {
                throw new ConflictException($"{participantsUserNames} already exist in conversation");
            }

            var participants = users.Select(x => new ParticipantInConversation
            {
                User = x,
                JoinedAt = DateTime.UtcNow,
                Conversation = conversation,
                Role = Enums.Role.BasicUser,
            });

            await _applicationContext.AddRangeAsync(participants);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteParticipantFromConversationAsync(Guid participantInConversationId)
        {
            var participantInConversation = await _applicationContext.ParticipantsInConversation
                 .Include(x => x.Conversation)
                 .FirstOrDefaultAsync(x => x.Id == participantInConversationId);

            if (participantInConversation == null)
            {
                throw new NotFoundException($"Participant with id {participantInConversationId} wasn`t found");
            }

            var participantsCount = await _applicationContext.ParticipantsInConversation
                .Where(x => x.Conversation.Id == participantInConversation.Conversation.Id)
                .CountAsync();

            if (participantsCount <= 1)
            {
                throw new BadRequestException("Cannot delete the last participant from conversation.");
            }

            _applicationContext.ParticipantsInConversation.Remove(participantInConversation);
            await _applicationContext.SaveChangesAsync();

            if (participantInConversation.Role == Role.Admin)
            {
                var newAdminParticipant = await _applicationContext.ParticipantsInConversation
                    .Where(x => x.Conversation.Id == participantInConversation.Conversation.Id)
                    .OrderByDescending(x => x.JoinedAt)
                    .FirstOrDefaultAsync();

                if (newAdminParticipant != null)
                {
                    newAdminParticipant.Role = Role.Admin;
                    await _applicationContext.SaveChangesAsync();
                }
            }
        }


        public async Task<IEnumerable<ParticipantInConversation>> GetParticipantsByConversationIdAsync(Guid conversationId)
        {
            return await _applicationContext.ParticipantsInConversation.Where(x => x.Conversation.Id == conversationId)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<ParticipantInConversation> GetParticipantFromConversationAsync(Guid userId, Guid conversationId)
        {
            ParticipantInConversation participant = await _applicationContext.ParticipantsInConversation
                .FirstOrDefaultAsync(x => x.User.Id == userId && x.Conversation.Id == conversationId);     

            return participant;
        }

        public async Task<ParticipantInConversation> GetParticipantByIdAsync(Guid participantInConversationId)
        {
            var participantInConversation = await _applicationContext.ParticipantsInConversation
                            .Include(x => x.Conversation)
                            .Include(x=>x.User)
                            .FirstOrDefaultAsync(x => x.Id == participantInConversationId);

            if (participantInConversation == null)
            {
                throw new NotFoundException($"Participant with id {participantInConversationId} wasn`t found");
            }

            return participantInConversation;
        }
    }
}