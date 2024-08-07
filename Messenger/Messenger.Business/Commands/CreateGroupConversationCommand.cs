using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class CreateGroupConversationCommand : IRequest<ResultDto<ConversationDto>>
{
    public GroupModelDto Group { get; set; }
    public Guid CreatorUserId { get; set; }
}

public class CreateGroupConversationCommandValidator : AbstractValidator<CreateGroupConversationCommand>
{
    public CreateGroupConversationCommandValidator()
    {
        RuleFor(x => x.Group)
            .NotNull()
            .WithMessage("Group cannot be null.");

        RuleFor(x => x.Group.Title)
            .NotNull()
            .WithMessage("Group title cannot be null.")
            .NotEmpty()
            .WithMessage("Group title cannot be empty.");     
    }
}

public class CreateGroupConversationCommandHandler : IRequestHandler<CreateGroupConversationCommand, 
    ResultDto<ConversationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateGroupConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<ConversationDto>> Handle(CreateGroupConversationCommand request, 
        CancellationToken cancellationToken)
    {
        Conversation conversation = await _unitOfWork.Conversations.CreateGroupConversationAsync(
            request.Group.Title,
            request.Group.ImgUrl,
            request.CreatorUserId);

        await _unitOfWork.SaveChangesAsync();

        var mappedConversation = _mapper.Map<ConversationDto>(conversation);

        return ResultDto.SuccessResult(mappedConversation, HttpStatusCode.OK);
    }
}
