
using Messenger.Infrastructure.Enums;

namespace Messenger.Infrastructure.Entities
{
    public class ParticipantInConversation : BaseEntity
    {
        public User User { get; set; }
        public DateTime JoinedAt { get; set; }
        public Conversation Conversation { get; set; }
        public Role? Role { get; set; }
    }
}
