using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using System.Net;

namespace Messenger.Business.Commands;

public class DeleteParticipantFromConversationCommand : IRequest<ResultDto>
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
}

public class DeleteParticipantFromConversationCommandValidator : AbstractValidator<DeleteParticipantFromConversationCommand>
{
    public DeleteParticipantFromConversationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId cannot be an empty GUID.");

        RuleFor(x => x.ConversationId)
            .NotEqual(Guid.Empty)
            .WithMessage("ConversationId cannot be an empty GUID.");
    }
}

public class DeleteParticipantFromConversationCommandHandler
    : IRequestHandler<DeleteParticipantFromConversationCommand, ResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubService _hubService;
    private readonly IMapper _mapper;


    public DeleteParticipantFromConversationCommandHandler(IUnitOfWork unitOfWork, IHubService hubService,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _hubService = hubService;
        _mapper = mapper;
    }

    public async Task<ResultDto> Handle(DeleteParticipantFromConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult(HttpStatusCode.NotFound,
                $"Conversation with id {request.ConversationId} wasn't found.");
        }

        var participants = await _unitOfWork.Participants.GetParticipantsByConversationIdAsync(request.ConversationId);

        if (participants.Count() <= 1)
        {
            return ResultDto.FailureResult(HttpStatusCode.BadRequest,
                "Cannot delete participant because it would leave the conversation with zero participants.");
        }

        var participantInConversation = await _unitOfWork.Participants
            .DeleteParticipantFromGroupConversationAsync(request.UserId, request.ConversationId);

        if (participantInConversation == null)
        {
            return ResultDto.FailureResult(HttpStatusCode.NotFound,
                $"User with id {request.UserId} wasn't found in group conversation with id {request.ConversationId}.");
        }

        if (participantInConversation.Role == Role.Admin)
        {
            var newAdminParticipant = (await _unitOfWork.Participants
                .GetParticipantsByConversationIdAsync(participantInConversation.Conversation.Id))
                .OrderByDescending(x => x.JoinedAt)
                .FirstOrDefault();

            if (newAdminParticipant != null)
            {
                await _unitOfWork.Participants.UpdateParticipantRoleAsync(newAdminParticipant.Id, Role.Admin);
            }
        }

        var messageText = $"{participantInConversation.User.UserName} was deleted from сonversation '{conversation.Group.Title}'";

        Message message = await _unitOfWork.Messages.AddMessageToConversationAsync(messageText, conversation, null, true);

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(message);

        await _unitOfWork.SaveChangesAsync();

        IEnumerable<UserConnection> connections = await _unitOfWork.Connections.GetUserConnectionsAsync(request.UserId);

        await _hubService.NotifyGroupAsync(conversation.Id, mappedMessage, "ReceiveNotification");
        MessageDto leaveConversationMessageDto = new MessageDto { Text = $"You are deleted from conversation {conversation.Group.Title}" };

        await _hubService.NotifyUsersConnectionsAsync(connections, leaveConversationMessageDto, "LeaveConversationNotification");

        return ResultDto.SuccessResult(HttpStatusCode.OK);
    }
}
