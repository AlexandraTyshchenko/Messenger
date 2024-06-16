using Microsoft.AspNetCore.Identity;

namespace Messenger.Infrastructure.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string PhoneNumber { get; set; } 
        public string ImgUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<UserContact> Contacts { get; set; } = new List<UserContact>();
        public ICollection<ParticipantInConversation> ParticipantsInConversation { get; set; } = new List<ParticipantInConversation>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
