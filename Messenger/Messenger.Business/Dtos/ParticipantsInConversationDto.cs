namespace Messenger.Business.Dtos;

public class ParticipantsInConversationDto
{
    public Guid ConversationId { get; set; }
    public IEnumerable<ParticipantDto> Participants { get; set; }
}
