using AutoMapper;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Dtos;
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
                .ForMember(dest => dest.GroupDto, opt => opt.MapFrom(src => src.Group))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
                    src.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()
                ));

            CreateMap<UserContact, UserContactDto>()
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(x => x.Contact));

            CreateMap<UserRegistrationDto, User>();
            CreateMap<UserLoginDto, User>();

        }
    }
}
