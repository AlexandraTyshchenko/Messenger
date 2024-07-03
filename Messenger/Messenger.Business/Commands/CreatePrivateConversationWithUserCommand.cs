using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class CreatePrivateConversationWithUserCommand : IRequest<ResultDto<AffectedRowsDto>>
{
    public Guid CreatorUserId { get; set; }
    public Guid UserId { get; set; }
}

public class CreatePrivateConversationWithUserCommandValidator : AbstractValidator<CreatePrivateConversationWithUserCommand>
{
    public CreatePrivateConversationWithUserCommandValidator()
    {
        RuleFor(x => x.CreatorUserId)
          .Must(guid => guid != Guid.Empty);

        RuleFor(x => x.UserId)
            .Must(guid => guid != Guid.Empty);
    }
}

public class CreatePrivateConversationWithUserCommandHandler : IRequestHandler<CreatePrivateConversationWithUserCommand, ResultDto<AffectedRowsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePrivateConversationWithUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto<AffectedRowsDto>> Handle(CreatePrivateConversationWithUserCommand request, CancellationToken cancellationToken)
    {
        Conversation existingConversation = await _unitOfWork.Conversations
            .GetPrivateConversationWithUserAsync(request.CreatorUserId, request.UserId);

        if (existingConversation != null)
        {
            return ResultDto.FailureResult<AffectedRowsDto>(HttpStatusCode.Conflict, "Conversation with this user already exists.");
        }

        User creatorUser = await _unitOfWork.Users.GetUserByIdAsync(request.CreatorUserId);

        User user = await _unitOfWork.Users.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return ResultDto.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound, "User was not found.");
        }

        await _unitOfWork.Conversations.CreateConversationWithUserAsync(creatorUser, user);
        int affectedRows = await _unitOfWork.SaveChangesAsync();

        AffectedRowsDto result = new AffectedRowsDto()
        {
            AffectedRows = affectedRows,
        };

        return ResultDto.SuccessResult(result,HttpStatusCode.Created);
    }
}
