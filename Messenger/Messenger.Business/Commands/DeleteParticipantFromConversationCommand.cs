using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Enums;
using System.Net;

namespace Messenger.Business.Commands;

public class DeleteParticipantFromConversationCommand : IRequest<ResultDto<AffectedRowsDto>>
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
}

public class DeleteParticipantFromConversationCommandValidator : AbstractValidator<DeleteParticipantFromConversationCommand>
{
    public DeleteParticipantFromConversationCommandValidator()
    {
        RuleFor(x => x.UserId)
          .Must(guid => guid != Guid.Empty);

        RuleFor(x => x.ConversationId)
         .Must(guid => guid != Guid.Empty);
    }
}

public class DeleteParticipantFromConversationCommandHandler
    : IRequestHandler<DeleteParticipantFromConversationCommand, ResultDto<AffectedRowsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteParticipantFromConversationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto<AffectedRowsDto>> Handle(DeleteParticipantFromConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto<AffectedRowsDto>.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound, 
                $"Conversation with id {request.ConversationId} wasn't found.");
        }

        var participants = await _unitOfWork.Participants.GetParticipantsByConversationIdAsync(request.ConversationId);

        if (participants.Count() <= 1)
        {
            return ResultDto<AffectedRowsDto>.FailureResult<AffectedRowsDto>(HttpStatusCode.BadRequest, 
                "Cannot delete participant because it would leave the conversation with zero participants.");
        }

        var participantInConversation = await _unitOfWork.Participants
            .DeleteParticipantFromGroupConversationAsync(request.UserId, request.ConversationId);

        if (participantInConversation == null)
        {
            return ResultDto<AffectedRowsDto>.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound,
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

        int affectedRows = await _unitOfWork.SaveChangesAsync();

        AffectedRowsDto result = new AffectedRowsDto
        {
            AffectedRows = affectedRows,
        };

        return ResultDto.SuccessResult(result, HttpStatusCode.OK);
    }
}
