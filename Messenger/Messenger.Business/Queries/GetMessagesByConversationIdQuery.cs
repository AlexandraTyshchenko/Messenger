using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Queries
{
    public class GetMessagesByConversationIdQuery : IRequest<ResultDto<IEnumerable<MessageWithSenderDto>>>
    {
        public Guid ConversationId { get; set; }
    }

    public class GetMessagesByConversationIdQueryHandler : IRequestHandler<GetMessagesByConversationIdQuery, ResultDto<IEnumerable<MessageWithSenderDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public GetMessagesByConversationIdQueryHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<ResultDto<IEnumerable<MessageWithSenderDto>>> Handle(GetMessagesByConversationIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Message> messages = await _messageRepository.GetMessagesByConversationIdAsync(request.ConversationId);

            var mappedMasseges = _mapper.Map<IEnumerable<MessageWithSenderDto>>(messages);

            return ResultDto<IEnumerable<MessageWithSenderDto>>.SuccessResult(mappedMasseges, HttpStatusCode.OK);
        }
    }
}
