namespace Messenger.Infrastructure.Entities;

public class Message : BaseEntity
{
    public string Text {  get; set; }
    public User Sender { get; set; }
    public Conversation Conversation { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsSeen { get; set; }
    public bool IsJoinMessage { get; set; } = false;
}
