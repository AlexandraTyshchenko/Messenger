using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class CreatePrivateConversationWithUserCommand : IRequest<ResultDto<ConversationWithParticipantsDto>>
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

public class CreatePrivateConversationWithUserCommandHandler : IRequestHandler<CreatePrivateConversationWithUserCommand,
    ResultDto<ConversationWithParticipantsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePrivateConversationWithUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<ConversationWithParticipantsDto>> Handle(CreatePrivateConversationWithUserCommand request,
        CancellationToken cancellationToken)
    {
        Conversation existingConversation = await _unitOfWork.Conversations
            .GetPrivateConversationWithUserAsync(request.CreatorUserId, request.UserId);

        if (existingConversation != null)
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>(HttpStatusCode.Conflict,
                "Conversation with this user already exists.");
        }

        User creatorUser = await _unitOfWork.Users.GetUserByIdAsync(request.CreatorUserId);

        User user = await _unitOfWork.Users.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>(HttpStatusCode.NotFound, "User was not found.");
        }

        Conversation result = await _unitOfWork.Conversations.CreateConversationWithUserAsync(creatorUser, user);
        await _unitOfWork.SaveChangesAsync();

        var mappedConversation = _mapper.Map<ConversationWithParticipantsDto>(result);

        return ResultDto.SuccessResult(mappedConversation, HttpStatusCode.Created);
    }
}
