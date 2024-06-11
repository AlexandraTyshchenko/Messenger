using Messenger.Infrastructure.Enums;

namespace Messenger.Infrastructure.Entities
{
    public class Conversation:BaseEntity
    {
        public int? GroupId {  get; set; }//todo set null and update migration
        public Group Group { get; set; }//todo remove grupId
        public ICollection<ParticipentInConversation> ParticipantsInGroup { get; set; } = new List<ParticipentInConversation>();

    }
}
