using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class DeleteMessageCommand : IRequest<ResultDto>
{
    public Guid MessageId { get; set; }
}

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
           .NotEqual(Guid.Empty)
           .WithMessage("MessageId cannot be an empty GUID.");
    }
}

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, ResultDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        Message message = await _unitOfWork.Messages.DeleteMessageAsync(request.MessageId);

        if (message == null)
        {
            return ResultDto.FailureResult(HttpStatusCode.NotFound, $"Message with id {request.MessageId} wasn't found.");
        }

        await _unitOfWork.SaveChangesAsync();

        return ResultDto.SuccessResult( HttpStatusCode.OK);
    }
}
