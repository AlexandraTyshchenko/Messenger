using Messenger.Infrastructure.Enums;

namespace Messenger.Infrastructure.Entities
{
    public class Conversation:BaseEntity
    {
        public int GroupId {  get; set; }
        public int PrivateConversationId { get; set; }
        public Group Group { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<ParticipentInConversation> ParticipantsInGroup { get; set; } = new List<ParticipentInConversation>();

    }
}
