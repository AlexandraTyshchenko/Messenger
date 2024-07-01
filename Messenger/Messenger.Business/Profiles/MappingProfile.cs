using AutoMapper;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;

namespace Messenger.Business.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserBasicInfoDto>();

            CreateMap<Group, GroupDto>();

            CreateMap<Message, MessageWithSenderDto>()
                .ForMember(dest => dest.SenderFirstName, opt => opt.MapFrom(src => src.Sender.FirstName))
                .ForMember(dest => dest.SenderLastName, opt => opt.MapFrom(src => src.Sender.LastName))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.Sender.ImgUrl))
                .ForMember(dest => dest.SenderUserName, opt => opt.MapFrom(src => src.Sender.UserName));

            CreateMap<Conversation, ConversationDto>()
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
                    src.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()
                ));

            CreateMap<Conversation, ConversationWithParticipantsDto>()
                .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.ParticipantsInConversation.Select(x => x.User).ToList()))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ParticipantsInConversation.Count()));

            CreateMap<UserRegistrationDto, User>();
            CreateMap<UserLoginDto, User>();
        }
    }
}
