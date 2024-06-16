namespace Messenger.Infrastructure.Entities
{
    public class UserContact:BaseEntity
    {
        public User Contact { get; set; }
        public User ContactOwner { get; set; }
    }
}
