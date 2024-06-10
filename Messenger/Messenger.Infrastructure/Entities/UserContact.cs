namespace Messenger.Infrastructure.Entities
{
    public class UserContact:BaseEntity
    {
        public int? UserContactId {  get; set; }
        public User Contact { get; set; }
        public int? UserContactOwnerId {  get; set; }
        public User ContactOwner { get; set; }
    }
}
