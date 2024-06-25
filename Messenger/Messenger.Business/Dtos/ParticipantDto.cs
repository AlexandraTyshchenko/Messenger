namespace Messenger.Business.Dtos
{
    public class ParticipantDto
    {
        public Guid Id { get; set; }
        public UserBasicInfoDto UserBasicInfo { get; set; }//todo add role and conversation
    }
}
