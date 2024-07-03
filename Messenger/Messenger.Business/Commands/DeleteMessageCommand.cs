using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class DeleteMessageCommand : IRequest<ResultDto<AffectedRowsDto>>
{
    public Guid MessageId { get; set; }
}

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
          .Must(guid => guid != Guid.Empty);
    }
}

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, ResultDto<AffectedRowsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto<AffectedRowsDto>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        Message message = await _unitOfWork.Messages.DeleteMessageAsync(request.MessageId);

        if (message == null)
        {
            return ResultDto<AffectedRowsDto>.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound, $"Message with id {request.MessageId} wasn't found.");
        }

        int affectedRows = await _unitOfWork.SaveChangesAsync();

        AffectedRowsDto result = new AffectedRowsDto
        {
            AffectedRows = affectedRows,
        };

        return ResultDto<AffectedRowsDto>.SuccessResult(result, HttpStatusCode.OK);
    }
}
