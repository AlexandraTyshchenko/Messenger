using Microsoft.AspNetCore.Identity;

namespace Messenger.Infrastructure.Entities;

public class UserConnection : BaseEntity
{
    public User User { get; set; }
    public string ConnectionId {  get; set; }
}
