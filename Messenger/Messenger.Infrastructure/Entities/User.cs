namespace Messenger.Infrastructure.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }//todo add identity
        public string LastName { get; set; }//todo add controllers for users, groups privateconversations messages
        public string PhoneNumber { get; set; }//todo add enpoints
        public string ImgUrl { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserContact> Contacts { get; set; } = new List<UserContact>();
        public ICollection<ParticipentInConversation> ParticipentInConversation { get; set; } = new List<ParticipentInConversation>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
