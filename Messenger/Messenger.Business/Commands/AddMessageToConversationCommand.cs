using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class AddMessageToConversationCommand : IRequest<ResultDto<MessageWithSenderDto>>
    {
        public Guid SenderId { get; set; }
        public Guid ConversationId { get; set; }
        public MessageDto MessageDto { get; set; }
    }

    public class AddMessageToConversationCommandHandler : IRequestHandler<AddMessageToConversationCommand, ResultDto<MessageWithSenderDto>>
    {
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public AddMessageToConversationCommandHandler(IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public async Task<ResultDto<MessageWithSenderDto>> Handle(AddMessageToConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Message message = await _messageRepository.AddMessageToConversationAsync(request.MessageDto.MessageText,
                    request.MessageDto.AttachmentUrl, 
                    request.ConversationId, request.SenderId);

                var mappedMessage = _mapper.Map<MessageWithSenderDto>(message);

                return ResultDto<MessageWithSenderDto>.SuccessResult(mappedMessage, HttpStatusCode.Created);
            }
            catch (CustomException ex)
            {
                return ResultDto<MessageWithSenderDto>.FailureResult<MessageWithSenderDto>(ex.StatusCode, ex.Message);
            }
        }

    }
}
