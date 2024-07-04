using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
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

    public DeleteParticipantFromConversationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.SaveChangesAsync();

        return ResultDto.SuccessResult(HttpStatusCode.OK);
    }
}
