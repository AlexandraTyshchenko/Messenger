using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
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
            Conversation conversation =  await _conversationRepository.DeleteConversationAsync(request.ConversationId);

            if (conversation == null)
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound, $"Conversation with id {request.ConversationId} wasn`t found.");
            }

            return ResultDto.SuccessResult(HttpStatusCode.OK);
        }
    }
}
