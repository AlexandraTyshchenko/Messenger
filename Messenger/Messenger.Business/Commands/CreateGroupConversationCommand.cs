using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class CreateGroupConversationCommand : IRequest<ResultDto<ConversationWithParticipantsDto>>
{
    public GroupModelDto Group { get; set; }
    public Guid CreatorUserId { get; set; }
}

public class CreateGroupConversationCommandValidator : AbstractValidator<CreateGroupConversationCommand>
{
    public CreateGroupConversationCommandValidator()
    {
        RuleFor(x => x.Group.Title)
           .NotEmpty();

        RuleFor(x => x.Group.ImgUrl)
           .NotEmpty();
    }
}

public class CreateGroupConversationCommandHandler : IRequestHandler<CreateGroupConversationCommand, ResultDto<ConversationWithParticipantsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateGroupConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<ConversationWithParticipantsDto>> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
    {
        Conversation conversation = await _unitOfWork.Conversations.CreateGroupConversationAsync(
            request.Group.Title,
            request.Group.ImgUrl,
            request.CreatorUserId);

        await _unitOfWork.SaveChangesAsync();

        var mappedConversation = _mapper.Map<ConversationWithParticipantsDto>(conversation);

        return ResultDto<ConversationWithParticipantsDto>.SuccessResult(mappedConversation, HttpStatusCode.OK);
    }
}
