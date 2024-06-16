using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Queries
{
    public class GetConversationByUserId : IRequest<IEnumerable<ConversationDto>>
    {
        public Guid UserId { get; set; }

    }
    public class GetConversationByUserIdQueryHandler : IRequestHandler<GetConversationByUserId, IEnumerable<ConversationDto>>
    {
        private readonly IMapper _mapper;
        private readonly IConversationRepository _conversationRepository;
        public GetConversationByUserIdQueryHandler(ApplicationContext applicationContext, IMapper mapper, IConversationRepository conversationRepository)
        {
            _mapper = mapper;
            _conversationRepository = conversationRepository;
        }

        public async Task<IEnumerable<ConversationDto>> Handle(GetConversationByUserId request, CancellationToken cancellationToken)
        {
            IEnumerable<Conversation> conversations = await _conversationRepository.GetConversationsByUserIdAsync(request.UserId);//todo почитати як працює під капотом await 

            return _mapper.Map<IEnumerable<ConversationDto>>(conversations);
        }
    }
}
