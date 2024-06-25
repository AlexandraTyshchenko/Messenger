namespace Messenger.Infrastructure.Entities
{
    public class Message : BaseEntity
    {
        public string MessageText {  get; set; }//todo rename to text
        public User Sender { get; set; }
        public Conversation Conversation { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime UpdatedAt { get; set; }//todo remove update field
        public bool IsSeen { get; set; }//todo isseen подумати
        public string AttachmentUrl {  get; set; }//todo remove
    }
}
