using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class DeleteConversationCommand : IRequest<ResultDto>
{
    public Guid ConversationId { get; set; }
}

public class DeleteConversationHandler : IRequestHandler<DeleteConversationCommand, ResultDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteConversationHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        Conversation conversation = await _unitOfWork.Conversations.DeleteConversationAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult(HttpStatusCode.NotFound, $"Conversation with id {request.ConversationId} wasn't found.");
        }

        await _unitOfWork.SaveChangesAsync();

        return ResultDto.SuccessResult(HttpStatusCode.OK);
    }
}
