using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class AddMessageToConversationCommand : IRequest<ResultDto<MessageWithSenderDto>>
    {
        public Guid SenderId { get; set; }
        public Guid ConversationId { get; set; }
        public MessageDto Message { get; set; }
    }

    public class AddMessageToConversationCommandHandler : IRequestHandler<AddMessageToConversationCommand, ResultDto<MessageWithSenderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConversationRepository _conversationRepository;

        public AddMessageToConversationCommandHandler(IMessageRepository messageRepository, IMapper mapper,
            IUserRepository userRepository, IConversationRepository conversationRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
        }

        public async Task<ResultDto<MessageWithSenderDto>> Handle(AddMessageToConversationCommand request, CancellationToken cancellationToken)
        {
            User sender = await _userRepository.GetUserByIdAsync(request.SenderId);

            Conversation conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);

            if (conversation == null)
            {
                return ResultDto<MessageWithSenderDto>.FailureResult<MessageWithSenderDto>(HttpStatusCode.NotFound,
                    "No conversation was found.");
            }

            Message message = await _messageRepository
                 .AddMessageToConversationAsync(request.Message.Text, conversation, sender);

            var mappedMessage = _mapper.Map<MessageWithSenderDto>(message);

            return ResultDto<MessageWithSenderDto>.SuccessResult(mappedMessage, HttpStatusCode.Created);
        }
    }
}

