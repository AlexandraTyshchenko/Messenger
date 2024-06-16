namespace Messenger.Business.Dtos
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public GroupDto GroupDto { get; set; }
        public MessageWithSenderDto LastMessage {  get; set; }
    }
}
