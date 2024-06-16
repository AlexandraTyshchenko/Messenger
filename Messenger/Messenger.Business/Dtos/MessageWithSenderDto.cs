namespace Messenger.Business.Dtos
{
    public class MessageWithSenderDto
    {
        public Guid Id { get; set; }
        public string SenderUserName { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }//todo allow null 
        public string SenderPhoneNumber { get; set; }
        public string ImgUrl { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string AttachmentUrl { get; set; }
    }
}
