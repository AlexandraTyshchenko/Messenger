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

        public async Task AddParticipantsToConversationAsync(IEnumerable<User> users, Conversation conversation)
        {
            IEnumerable<ParticipantInConversation> participants = users.Select(x => new ParticipantInConversation
            {
                User = x,
                JoinedAt = DateTime.UtcNow,
                Conversation = conversation,
                Role = Role.BasicUser,
            });

            await _applicationContext.AddRangeAsync(participants);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteParticipantFromConversationAsync(ParticipantInConversation participantInConversation)
        {
            _applicationContext.ParticipantsInConversation.Remove(participantInConversation);
            await _applicationContext.SaveChangesAsync();   
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
            ParticipantInConversation participantInConversation = await _applicationContext.ParticipantsInConversation
                            .Include(x => x.Conversation)
                            .Include(x=>x.User)
                            .FirstOrDefaultAsync(x => x.Id == participantInConversationId);

            return participantInConversation;
        }

        public async Task UpdateParticipantRoleAsync(Guid participantId, Role role)
        {
            ParticipantInConversation participant= await _applicationContext.ParticipantsInConversation.FindAsync(participantId);
  
            participant.Role = role;
            await _applicationContext.SaveChangesAsync();    
        }
    }
}