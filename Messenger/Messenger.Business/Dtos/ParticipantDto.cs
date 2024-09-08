using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;

namespace Messenger.Business.Dtos;

public class ParticipantDto
{
    public UserBasicInfoDto UserInfo { get; set; }
    public Guid ConversationId { get; set; }
    public Role? Role { get; set; }
}
