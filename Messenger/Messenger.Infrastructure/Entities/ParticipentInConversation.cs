
using Messenger.Infrastructure.Enums;

namespace Messenger.Infrastructure.Entities
{
    public class ParticipentInConversation : BaseEntity
    {
        public User User { get; set; }
        public int UserId {  get; set; }//todo remove foreign keys
        public DateTime JoinedAt { get; set; }
        public Conversation Conversation { get; set; }
        public int ConversationId { get; set; }
        public Role Role { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
