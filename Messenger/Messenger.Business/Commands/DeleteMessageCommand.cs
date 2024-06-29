using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class DeleteMessageCommand : IRequest<ResultDto>
    {
        public Guid MessageId { get; set; }
    }

    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, ResultDto>
    {
        private readonly IMessageRepository _messageRepository;

        public DeleteMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<ResultDto> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            Message message = await _messageRepository.DeleteMessageAsync(request.MessageId);

            if (message == null)
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound, $"Message with id {request.MessageId} wasn`t found");
            }

            return ResultDto.SuccessResult(HttpStatusCode.OK);
        }
    }
}
