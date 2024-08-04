using AutoMapper;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;

namespace Messenger.Business.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserBasicInfoDto>();

        CreateMap<Group, GroupDto>();

        CreateMap<Message, MessageWithSenderDto>()
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
            .ForMember(dest=>dest.ConversationId,opt=>opt.MapFrom(src=>src.Conversation.Id));

        CreateMap<Conversation, ConversationDto>()
             .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
             .ForMember(dest => dest.PrivateConversationParticipants, opt => opt
                .MapFrom(src => src.Group == null ? src.ParticipantsInConversation.Select(x => x.User).ToList() : null))
             .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.ParticipantsInConversation.Count()))
             .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
                     src.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()));


        CreateMap<ParticipantInConversation, UserBasicInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.User.ImgUrl));

        CreateMap<UserRegistrationDto, User>();
        CreateMap<UserLoginDto, User>();

        CreateMap<ParticipantInConversation, ParticipantsDto>()
            .ForMember(dest => dest.UserInfo, opt => opt.MapFrom(x => x.User))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(x => x.Role))
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(x => x.Conversation.Id));

        CreateMap<Conversation, ParticipantsInConversationDto>()
            .ForMember(dest=>dest.ConversationId,opt=>opt.MapFrom(x=>x.Id))
            .ForMember(dest=>dest.Participants,opt=>opt.MapFrom(x=>x.ParticipantsInConversation));

    }
}
