using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Queries
{
    public class GetConversationsByUserIdQuery : IRequest<ResultDto<IEnumerable<ConversationDto>>>
    {
        public Guid UserId { get; set; }

    }
    public class GetConversationsByUserIdQueryHandler : IRequestHandler<GetConversationsByUserIdQuery, ResultDto<IEnumerable<ConversationDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IConversationRepository _conversationRepository;
        public GetConversationsByUserIdQueryHandler(ApplicationContext applicationContext, IMapper mapper, IConversationRepository conversationRepository)
        {
            _mapper = mapper;
            _conversationRepository = conversationRepository;
        }

        public async Task<ResultDto<IEnumerable<ConversationDto>>> Handle(GetConversationsByUserIdQuery request, 
            CancellationToken cancellationToken)
        {
            IEnumerable<Conversation> conversations = await _conversationRepository
                .GetConversationsByUserIdAsync(request.UserId);//todo почитати як працює під капотом await 

            var mappedConversations = _mapper.Map<IEnumerable<ConversationDto>>(conversations);

            return ResultDto<IEnumerable<ConversationDto>>.SuccessResult(mappedConversations,HttpStatusCode.OK);
        }
    }
}
