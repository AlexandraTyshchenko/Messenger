using Microsoft.AspNetCore.Identity;

namespace Messenger.Infrastructure.Entities
{
    public class RefreshToken : IdentityUserToken<Guid>
    {
        public DateTime Expires { get; set; }
        public virtual User User { get; set; }

    }
}
