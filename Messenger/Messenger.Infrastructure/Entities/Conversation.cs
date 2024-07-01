namespace Messenger.Infrastructure.Entities;

public class Conversation : BaseEntity
{
    public Group Group { get; set; }
    public ICollection<ParticipantInConversation> ParticipantsInConversation { get; set; } = new List<ParticipantInConversation>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
