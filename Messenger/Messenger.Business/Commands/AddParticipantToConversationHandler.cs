using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class AddParticipantToConversationCommand : IRequest<ResultDto<IEnumerable<ParticipantDto>>>
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
            .NotNull().WithMessage("At least one selected user is required")
            .NotEmpty().WithMessage("At least one selected user is required");

        RuleForEach(x => x.UserIds)
            .NotEqual(Guid.Empty).WithMessage("At least one selected user is required");
    }
}

public class AddParticipantToConversationCommandHandler :
    IRequestHandler<AddParticipantToConversationCommand, ResultDto<IEnumerable<ParticipantDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHubService _hubService;

    public AddParticipantToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IHubService hubService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hubService = hubService;
    }

    public async Task<ResultDto<IEnumerable<ParticipantDto>>> Handle(AddParticipantToConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Conversations.GetGroupConversationByIdAsync(request.ConversationId) ;

        if (conversation == null)
        {
            return ResultDto.FailureResult<IEnumerable<ParticipantDto>>
                (HttpStatusCode.NotFound, "Group conversation was not found.");
        }

        var (users, missingUserIds) = await GetUsersAsync(request.UserIds);

        if (missingUserIds.Any())
        {
            return ResultDto.FailureResult<IEnumerable<ParticipantDto>>
                (HttpStatusCode.NotFound, $"Users with ids {string.Join(", ", missingUserIds)} were not found.");
        }

        var existingParticipants = await GetExistingParticipantsAsync(request.ConversationId, users);

        if (existingParticipants.Any())
        {
            return ResultDto.FailureResult<IEnumerable<ParticipantDto>>
                (HttpStatusCode.Conflict,
                $"Users {string.Join(", ", existingParticipants.Select(x => x.User.UserName))}" +
                $" already exist in groupConversation.");
        }

        var participants = await _unitOfWork.Participants.AddParticipantsToConversationAsync(users, conversation); 

        var userNames = string.Join(", ", participants.Select(x => x.User.UserName));
        var userConnections = await _unitOfWork.Connections.GetUsersConnectionsAsync(request.UserIds); ;

        var messageText = userNames;
        messageText += participants.Count() == 1 ?
            $"  was added to сonversation '{conversation.Group.Title}'" : $" " +
            $" were added to сonversation '{conversation.Group.Title}'";

        var message = new Message
        {
            Conversation = conversation,
            IsJoinMessage = true,
            Sender = null,
            Text = messageText,
            SentAt = DateTime.Now,
        };

        Message joinMessage = await _unitOfWork.Messages.AddMessageToConversationAsync(message); 

        var mappedJoinMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);
        await _unitOfWork.SaveChangesAsync();

        await _hubService.JoinGroupAsync(userConnections, conversation.Id);
        await _hubService.NotifyGroupAsync(conversation.Id, mappedJoinMessage, "ReceiveNotification");

        NotificationDto joinMessageDto = new NotificationDto { Text = $"You are joined to conversation {conversation.Group.Title}" };
        await _hubService.NotifyUsersConnectionsAsync(userConnections, joinMessageDto, "JoinNotification");

        var mappedParticipants = _mapper.Map<IEnumerable<ParticipantDto>>(participants);
        return ResultDto.SuccessResult(mappedParticipants, HttpStatusCode.OK);
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
}