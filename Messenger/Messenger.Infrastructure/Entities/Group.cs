namespace Messenger.Infrastructure.Entities
{
    public class Group:BaseEntity
    {
        public Conversation Conversation { get; set; }//todo updat migration
        public string Title {  get; set; }
        public string ImgUrl {  get; set; }
    }
}
