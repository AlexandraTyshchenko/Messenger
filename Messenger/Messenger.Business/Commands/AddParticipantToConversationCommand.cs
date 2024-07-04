using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class AddParticipantToConversationCommand : IRequest<ResultDto<AffectedRowsDto>>
{
    public Guid[] UserIds { get; set; }
    public Guid ConversationId { get; set; }
}

public class AddParticipantToConversationCommandValidator : AbstractValidator<AddParticipantToConversationCommand>
{
    public AddParticipantToConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .Must(guid => guid != Guid.Empty);

        RuleForEach(x => x.UserIds)
            .Must(guid => guid != Guid.Empty);
    }
}

public class AddParticipantToConversationCommandHandler : 
    IRequestHandler<AddParticipantToConversationCommand, ResultDto<AffectedRowsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddParticipantToConversationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultDto<AffectedRowsDto>> Handle(AddParticipantToConversationCommand request,
        CancellationToken cancellationToken)
    {
        Conversation conversation = await _unitOfWork.Conversations
           .GetGroupConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound,
                "Group conversation was not found.");
        }

        IEnumerable<User> users = await _unitOfWork.Users.GetUsersByIdsAsync(request.UserIds);

        IEnumerable<Guid> missingUserIds = request.UserIds.Where(id => !users.Select(x => x.Id)
        .Contains(id)).ToList();

        if (missingUserIds.Any())
        {
            return ResultDto.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound,
                $"Users with ids {string.Join(", ", missingUserIds)} were not found.");
        }
       
        IEnumerable<ParticipantInConversation> existingParticipants = (await _unitOfWork.Participants
            .GetParticipantsByConversationIdAsync(request.ConversationId))
            .Where(x => users.Contains(x.User))
            .ToList();

        var participantsUserNames = string.Join(", ", existingParticipants.Select(x => x.User.UserName));

        if (existingParticipants.Any())
        {
            return ResultDto.FailureResult<AffectedRowsDto>(HttpStatusCode.Conflict,
                $"Users {participantsUserNames} already exist in conversation.");
        }

        if (conversation.Group == null)
        {
            return ResultDto.FailureResult<AffectedRowsDto>(HttpStatusCode.NotFound,
                $"Conversation with id {request.ConversationId} is not a group conversation.");
        }

        await _unitOfWork.Participants.AddParticipantsToConversationAsync(users, conversation);
        int affectedRows = await _unitOfWork.SaveChangesAsync();

        AffectedRowsDto result = new AffectedRowsDto()
        {
            AffectedRows = affectedRows,
        };

        return ResultDto.SuccessResult(result,HttpStatusCode.OK);
    }
}
