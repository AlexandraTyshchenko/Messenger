namespace Messenger.Infrastructure.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public User User { get; set; }
    public DateTime ExpiryDate { get; set; }
}
