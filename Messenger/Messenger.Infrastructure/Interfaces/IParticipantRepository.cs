using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<ParticipantInConversation>> GetParticipantsByConversationIdAsync(Guid conversationId);
        Task<ParticipantInConversation> GetParticipantFromConversationAsync(Guid userId, Guid conversationId);
        Task AddParticipantsToConversationAsync(Guid[] userIds, Guid conversationId);
        Task DeleteParticipantFromConversationAsync(Guid participantInConversationId);
        Task<ParticipantInConversation> GetParticipantByIdAsync(Guid participantInConversationId);
    }
}
