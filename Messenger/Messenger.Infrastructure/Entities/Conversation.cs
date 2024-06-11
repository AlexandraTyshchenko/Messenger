using Messenger.Infrastructure.Enums;

namespace Messenger.Infrastructure.Entities
{
    public class Conversation:BaseEntity
    {
        public int GroupId {  get; set; }
        public Group Group { get; set; }
        public ICollection<ParticipentInConversation> ParticipantsInGroup { get; set; } = new List<ParticipentInConversation>();

    }
}
