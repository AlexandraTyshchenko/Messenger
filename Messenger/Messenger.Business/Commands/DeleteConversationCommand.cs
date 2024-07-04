using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class DeleteConversationCommand : IRequest<ResultDto<AffectedRowsDto>>
{
    public Guid ConversationId { get; set; }
}

public class DeleteConversationCommandValidator : AbstractValidator<DeleteConversationCommand>
{
    public DeleteConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
          .Must(guid => guid != Guid.Empty);
    }
}

public class DeleteConversationHandler : IRequestHandler<DeleteConversationCommand, ResultDto<AffectedRowsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteConversationHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto<AffectedRowsDto>> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        Conversation conversation = await _unitOfWork.Conversations.DeleteConversationAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound, $"Conversation with id {request.ConversationId} wasn't found.");
        }


        int affectedRows = await _unitOfWork.SaveChangesAsync();

        AffectedRowsDto result = new AffectedRowsDto
        {
            AffectedRows = affectedRows,
        };

        return ResultDto.SuccessResult(result, HttpStatusCode.OK);
    }
}
