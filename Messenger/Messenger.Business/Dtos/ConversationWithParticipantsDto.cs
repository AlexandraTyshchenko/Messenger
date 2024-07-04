namespace Messenger.Business.Dtos;

public class ConversationWithParticipantsDto
{
    public Guid Id { get; set; }
    public GroupDto Group { get; set; }
    public IEnumerable<UserBasicInfoDto> Participants { get; set; } = Enumerable.Empty<UserBasicInfoDto>();
    public int ParticipantsCount {  get; set; }
}
