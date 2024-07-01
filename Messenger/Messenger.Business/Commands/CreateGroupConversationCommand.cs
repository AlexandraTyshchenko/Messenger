using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class CreateGroupConversationCommand : IRequest<ResultDto<ConversationWithParticipantsDto>>
    {
        public GroupModelDto Group { get; set; }
        public Guid CreatorUserId { get; set; }
    }

    public class CreateGroupConversationCommandHandler : IRequestHandler<CreateGroupConversationCommand, ResultDto<ConversationWithParticipantsDto>>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        public CreateGroupConversationCommandHandler(IConversationRepository conversationRepository, IMapper mapper)
        {
            _conversationRepository = conversationRepository;
            _mapper = mapper;
        }

        public async Task<ResultDto<ConversationWithParticipantsDto>> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
        {
            Conversation conversation = await _conversationRepository.CreateGroupConversationAsync//todo return groupConversationDto
                (request.Group.Title,
                request.Group.ImgUrl,
                request.CreatorUserId);

            var mappedConversation = _mapper.Map<ConversationWithParticipantsDto>(conversation);

            return ResultDto<ConversationWithParticipantsDto>.SuccessResult(mappedConversation, HttpStatusCode.OK);
        }
    }
}
