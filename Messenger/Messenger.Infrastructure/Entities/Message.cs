namespace Messenger.Infrastructure.Entities
{
    public class Message : BaseEntity
    {
        public string MessageText {  get; set; }
        public ParticipentInConversation Sender { get; set; }
        public int ParticipentInConversationId { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsSeen { get; set; }
        public string AttachmentUrl {  get; set; }

    }
}
