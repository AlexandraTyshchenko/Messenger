using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class CreatePrivateConversationWithUserCommand : IRequest<ResultDto<ConversationDto>>
{
    public Guid CreatorUserId { get; set; }
    public Guid UserId { get; set; } 
}

public class CreatePrivateConversationWithUserCommandValidator : AbstractValidator<CreatePrivateConversationWithUserCommand>
{
    public CreatePrivateConversationWithUserCommandValidator()
    {
        RuleFor(x => x.CreatorUserId)
            .NotEqual(Guid.Empty)
            .WithMessage("CreatorUserId cannot be an empty GUID.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId cannot be an empty GUID.");
    }
}

public class CreatePrivateConversationWithUserCommandHandler : IRequestHandler<CreatePrivateConversationWithUserCommand,
    ResultDto<ConversationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHubService _hubService;

    public CreatePrivateConversationWithUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,IHubService hubService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hubService = hubService;
    }

    public async Task<ResultDto<ConversationDto>> Handle(CreatePrivateConversationWithUserCommand request,
        CancellationToken cancellationToken)
    {
        Conversation existingConversation = await _unitOfWork.Conversations
            .GetPrivateConversationWithUserAsync(request.CreatorUserId, request.UserId);

        if (existingConversation != null)
        {
            var mappedExistingConversation = _mapper.Map<ConversationDto>(existingConversation);

            return ResultDto.FailureResult(HttpStatusCode.Conflict,
                "Conversation with this user already exists.", mappedExistingConversation);
        }

        User creatorUser = await _unitOfWork.Users.GetUserByIdAsync(request.CreatorUserId);

        if (creatorUser == null)
        {
            return ResultDto.FailureResult<ConversationDto>(HttpStatusCode.NotFound, "Creator user was not found.");
        }

        User user = await _unitOfWork.Users.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            return ResultDto.FailureResult<ConversationDto>(HttpStatusCode.NotFound, "User was not found.");
        }

        Conversation conversation = await _unitOfWork.Conversations.CreateConversationWithUserAsync(creatorUser, user);

        await _unitOfWork.SaveChangesAsync();

        IEnumerable<UserConnection> userCreatorConnections = await _unitOfWork.Connections
            .GetUserConnectionsAsync(request.CreatorUserId);

        IEnumerable<UserConnection> userConnections = await _unitOfWork.Connections
            .GetUserConnectionsAsync(request.UserId);

        await _hubService.JoinGroupAsync(userCreatorConnections, conversation.Id);
        await _hubService.JoinGroupAsync(userConnections, conversation.Id);

        NotificationDto joinNotificationDto = new NotificationDto { Text = $"You are joined to conversation with {creatorUser.UserName}" };

        await _hubService.NotifyUsersConnectionsAsync(userConnections, joinNotificationDto, "JoinNotification");
        var mappedConversation = _mapper.Map<ConversationDto>(conversation);

        return ResultDto.SuccessResult(mappedConversation, HttpStatusCode.Created);
    }
}
