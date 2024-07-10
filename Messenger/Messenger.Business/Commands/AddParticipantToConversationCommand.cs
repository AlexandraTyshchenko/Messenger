using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Api.Hubs;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.RegularExpressions;

namespace Messenger.Business.Commands;

public class AddParticipantToConversationCommand : IRequest<ResultDto<ConversationWithParticipantsDto>>
{
    public Guid[] UserIds { get; set; }
    public Guid ConversationId { get; set; }
}

public class AddParticipantToConversationCommandValidator : AbstractValidator<AddParticipantToConversationCommand>
{
    public AddParticipantToConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
           .NotEqual(Guid.Empty).WithMessage("ConversationId cannot be empty.");

        RuleFor(x => x.UserIds)
            .NotNull().WithMessage("UserIds cannot be null.")
            .NotEmpty().WithMessage("UserIds cannot be empty.");

        RuleForEach(x => x.UserIds)
            .NotEqual(Guid.Empty).WithMessage("UserId cannot be empty.");
    }
}

public class AddParticipantToConversationCommandHandler :
    IRequestHandler<AddParticipantToConversationCommand, ResultDto<ConversationWithParticipantsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHubContext<ChatHub> _chatHub;

    public AddParticipantToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<ChatHub> chatHub)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _chatHub = chatHub;
    }

    public async Task<ResultDto<ConversationWithParticipantsDto>> Handle(AddParticipantToConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await GetConversationAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>
                (HttpStatusCode.NotFound, "Group conversation was not found.");
        }

        var (users, missingUserIds) = await GetUsersAsync(request.UserIds);

        if (missingUserIds.Any())
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>
                (HttpStatusCode.NotFound, $"Users with ids {string.Join(", ", missingUserIds)} were not found.");
        }

        var existingParticipants = await GetExistingParticipantsAsync(request.ConversationId, users);

        if (existingParticipants.Any())
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>
                (HttpStatusCode.Conflict,
                $"Users {string.Join(", ", existingParticipants.Select(x => x.User.UserName))}" +
                $" already exist in conversation.");
        }

        var participants = await AddParticipantsToConversationAsync(users, conversation);
        await _unitOfWork.SaveChangesAsync();

        var userNames = string.Join(", ", participants.Select(x => x.User.UserName));
        var userConnections = await GetUserConnectionsAsync(request.UserIds);

        await NotifyGroupAsync(conversation.Id, $"participants {userNames}");
        await NotifyJoinedUsersConnection(userConnections, conversation);

        var mappedConversationWithParticipants = _mapper.Map<ConversationWithParticipantsDto>(conversation);
        return ResultDto.SuccessResult(mappedConversationWithParticipants, HttpStatusCode.OK);
    }

    private async Task<Conversation> GetConversationAsync(Guid conversationId)
    {
        return await _unitOfWork.Conversations.GetGroupConversationByIdAsync(conversationId);
    }

    private async Task<(IEnumerable<User> Users, IEnumerable<Guid> MissingUserIds)> GetUsersAsync(Guid[] userIds)
    {
        var users = await _unitOfWork.Users.GetUsersByIdsAsync(userIds);
        var missingUserIds = userIds.Where(id => !users.Select(x => x.Id).Contains(id)).ToList();
        return (users, missingUserIds);
    }

    private async Task<IEnumerable<ParticipantInConversation>> GetExistingParticipantsAsync(Guid conversationId, IEnumerable<User> users)
    {
        var participants = await _unitOfWork.Participants.GetParticipantsByConversationIdAsync(conversationId);
        return participants.Where(x => users.Contains(x.User)).ToList();
    }

    private async Task<IEnumerable<ParticipantInConversation>> AddParticipantsToConversationAsync(IEnumerable<User> users, Conversation conversation)
    {
        return await _unitOfWork.Participants.AddParticipantsToConversationAsync(users, conversation);
    }

    private async Task NotifyGroupAsync(Guid groupId, string message)
    {
        await _chatHub.Clients.Group(groupId.ToString()).SendAsync("ReceiveNotification", message);
    }

    private async Task<IEnumerable<UserConnection>> GetUserConnectionsAsync(Guid[] userIds)
    {
        return await _unitOfWork.Connections.GetUserConnectionsAsync(userIds);
    }

    private async Task NotifyJoinedUsersConnection(IEnumerable<UserConnection> userConnections, Conversation conversation)
    {
        foreach (UserConnection userConnection in userConnections)
        {
            await _chatHub.Clients.Client(userConnection.ConnectionId).SendAsync("JoinNotification", new ConversationNotificationDto
            {
                ConversationId = conversation.Id,
                Message = $"you are joined to conversation {conversation.Group.Title}"
            });
        }
    }
}
