using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class DeleteConversationCommand : IRequest<ResultDto>
    {
        public Guid ConversationId { get; set; }
    }

    public class DeleteConversationHandler : IRequestHandler<DeleteConversationCommand, ResultDto>
    {
        private readonly IConversationRepository _conversationRepository;

        public DeleteConversationHandler(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<ResultDto> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _conversationRepository.DeleteConversationAsync(request.ConversationId);

                return ResultDto.SuccessResult(HttpStatusCode.OK);
            }
            catch (CustomException ex)
            {
                return ResultDto.FailureResult(ex.StatusCode, ex.Message);
            }
        }
    }
}
