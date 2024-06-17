using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Queries
{
    public class GetMessagesByConversationIdQuery : IRequest<IEnumerable<MessageWithSenderDto>>
    {
        public Guid ConversationId { get; set; }
    }

    public class GetMessagesByConversationIdQueryHandler : IRequestHandler<GetMessagesByConversationIdQuery, IEnumerable<MessageWithSenderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public GetMessagesByConversationIdQueryHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<IEnumerable<MessageWithSenderDto>> Handle(GetMessagesByConversationIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Message> messages = await _messageRepository.GetMessagesByConversationIdAsync(request.ConversationId);

            return _mapper.Map<IEnumerable<MessageWithSenderDto>>(messages);
        }
    }
}
