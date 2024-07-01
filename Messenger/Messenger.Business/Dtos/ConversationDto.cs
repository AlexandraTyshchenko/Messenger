namespace Messenger.Business.Dtos
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public GroupDto Group { get; set; }
        public MessageWithSenderDto LastMessage {  get; set; }
    }
}
