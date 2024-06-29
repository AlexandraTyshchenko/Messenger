using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<ParticipantInConversation>> GetParticipantsByConversationIdAsync(Guid conversationId);
        Task UpdateParticipantRoleAsync(Guid participantId, Role role);
        Task<ParticipantInConversation> GetParticipantFromConversationAsync(Guid userId, Guid conversationId);
        Task AddParticipantsToConversationAsync(IEnumerable<User> users, Conversation conversation);
        Task DeleteParticipantFromConversationAsync(ParticipantInConversation participantInConversation);
        Task<ParticipantInConversation> GetParticipantByIdAsync(Guid participantInConversationId);
    }
}
