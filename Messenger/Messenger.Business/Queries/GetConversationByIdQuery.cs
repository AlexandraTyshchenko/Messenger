using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Queries
{
    public class GetConversationByIdQuery : IRequest<ResultDto<ConversationWithParticipantsDto>>
    {
        public Guid ConversationId { get; set; }
    }

    public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, ResultDto<ConversationWithParticipantsDto>>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMapper _mapper;

        public GetConversationByIdQueryHandler(IConversationRepository conversationRepository, IMapper mapper)
        {
            _conversationRepository = conversationRepository;
            _mapper = mapper;
        }

        public async Task<ResultDto<ConversationWithParticipantsDto>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
        {
            Conversation conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);

            if (conversation == null)
            {
                return ResultDto<ConversationWithParticipantsDto>.FailureResult<ConversationWithParticipantsDto>(HttpStatusCode.NotFound,
                    $"Conversation with id {request.ConversationId} wasn`t found.");
            }

            var mappedConversation = _mapper.Map<ConversationWithParticipantsDto>(conversation);

            return ResultDto<ConversationWithParticipantsDto>.SuccessResult(mappedConversation);
        }
    }
}
