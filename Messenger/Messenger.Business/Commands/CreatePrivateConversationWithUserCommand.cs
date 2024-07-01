using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class CreatePrivateConversationWithUserCommand : IRequest<ResultDto>
{
    public Guid CreatorUserId { get; set; }
    public Guid UserId { get; set; }
}

public class CreatePrivateConversationWithUserCommandHandler : IRequestHandler<CreatePrivateConversationWithUserCommand, ResultDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePrivateConversationWithUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto> Handle(CreatePrivateConversationWithUserCommand request, CancellationToken cancellationToken)
    {
        Conversation existingConversation = await _unitOfWork.Conversations
            .GetPrivateConversationWithUserAsync(request.CreatorUserId, request.UserId);

        if (existingConversation != null)
        {
            return ResultDto.FailureResult(HttpStatusCode.Conflict, "Conversation with this user already exists.");
        }

        User creatorUser = await _unitOfWork.Users.GetUserByIdAsync(request.CreatorUserId);

        User user = await _unitOfWork.Users.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return ResultDto.FailureResult(HttpStatusCode.NotFound, "User was not found.");
        }

        await _unitOfWork.Conversations.CreateConversationWithUserAsync(creatorUser, user);
        await _unitOfWork.SaveChangesAsync();

        return ResultDto.SuccessResult(HttpStatusCode.Created);
    }
}
