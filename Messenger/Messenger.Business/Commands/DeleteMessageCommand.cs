using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Exceptions;
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
            try
            {
                await _messageRepository.DeleteMessageByIdAsync(request.MessageId);
                return ResultDto.SuccessResult(HttpStatusCode.OK);
            }
            catch (CustomException ex)
            {
                return ResultDto.FailureResult(ex.StatusCode, ex.Message);
            }
        }
    }
}
