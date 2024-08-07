namespace Messenger.Business.Dtos;

public class MessageWithSenderDto
{
    public Guid Id { get; set; }
    public UserBasicInfoDto Sender { get; set; }
    public Guid ConversationId { get; set; }
    public string Text { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsJoinMessage { get; set; } = false;

}
