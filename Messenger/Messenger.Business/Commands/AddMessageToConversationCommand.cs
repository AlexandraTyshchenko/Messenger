using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Commands
{
    public class AddMessageToConversationCommand : IRequest<MessageWithSenderDto>
    {
        public Guid SenderId { get; set; }
        public Guid ConversationId { get; set; }
        public MessageDto MessageDto { get; set; }
    }

    public class AddMessageToConversationCommandHandler : IRequestHandler<AddMessageToConversationCommand, MessageWithSenderDto>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public AddMessageToConversationCommandHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<MessageWithSenderDto> Handle(AddMessageToConversationCommand request, CancellationToken cancellationToken)
        {
            Message message = await _messageRepository.AddMessageToConversationAsync(request.MessageDto, request.ConversationId, request.SenderId);

            return _mapper.Map<MessageWithSenderDto>(message);
        }
    }
}
