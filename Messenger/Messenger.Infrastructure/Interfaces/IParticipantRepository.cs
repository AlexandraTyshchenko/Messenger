using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<User>> GetParticipantsByConversationIdAsync(Guid conversationId);
        Task<bool> IsUserInConversationAsync(Guid userId, Guid conversationId);
        Task<ParticipantInConversation> GetUserFromConversationAsync(Guid userId, Guid conversationId);
        Task AddParticipantsToConversationAsync(Guid[] userIds, Guid conversationId);
    }
}
