namespace Messenger.Infrastructure.Entities
{
    public class Group:BaseEntity
    {
        public Conversation Conversation { get; set; }
        public string Title {  get; set; }
        public string ImgUrl {  get; set; }
    }
}
