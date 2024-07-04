using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

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

    public AddParticipantToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<ConversationWithParticipantsDto>> Handle(AddParticipantToConversationCommand request,
        CancellationToken cancellationToken)
    {
        Conversation conversation = await _unitOfWork.Conversations
           .GetGroupConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>(HttpStatusCode.NotFound,
                "Group conversation was not found.");
        }

        IEnumerable<User> users = await _unitOfWork.Users.GetUsersByIdsAsync(request.UserIds);

        IEnumerable<Guid> missingUserIds = request.UserIds.Where(id => !users.Select(x => x.Id)
            .Contains(id)).ToList();

        if (missingUserIds.Any())
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>(HttpStatusCode.NotFound,
                $"Users with ids {string.Join(", ", missingUserIds)} were not found.");
        }

        IEnumerable<ParticipantInConversation> existingParticipants = (await _unitOfWork.Participants
            .GetParticipantsByConversationIdAsync(request.ConversationId))
            .Where(x => users.Contains(x.User))
            .ToList();

        var participantsUserNames = string.Join(", ", existingParticipants.Select(x => x.User.UserName));

        if (existingParticipants.Any())
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>(HttpStatusCode.Conflict,
                $"Users {participantsUserNames} already exist in conversation.");
        }

        if (conversation.Group == null)
        {
            return ResultDto.FailureResult<ConversationWithParticipantsDto>(HttpStatusCode.NotFound,
                $"Conversation with id {request.ConversationId} is not a group conversation.");
        }

        IEnumerable<ParticipantInConversation> participants = await _unitOfWork.Participants
            .AddParticipantsToConversationAsync(users, conversation);

        var mappedParticipants = _mapper.Map<IEnumerable<UserBasicInfoDto>>(participants);

        await _unitOfWork.SaveChangesAsync();

        var mappedConversationWithParticipants = _mapper.Map<ConversationWithParticipantsDto>(conversation);

        return ResultDto.SuccessResult(mappedConversationWithParticipants, HttpStatusCode.OK);
    }
}
