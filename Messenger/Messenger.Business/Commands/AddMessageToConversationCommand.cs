using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Commands;

public class AddMessageToConversationCommand : IRequest<ResultDto<MessageWithSenderDto>>
{
    public Guid SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public MessageDto Message { get; set; }
}

public class AddMessageToConversationCommandValidator : AbstractValidator<AddMessageToConversationCommand>
{
    public AddMessageToConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEqual(Guid.Empty)
            .WithMessage("ConversationId cannot be empty.");

        RuleFor(x => x.SenderId)
            .NotEqual(Guid.Empty)
            .WithMessage("SenderId cannot be empty.");

        RuleFor(x => x.Message)
            .NotNull()
            .WithMessage("Message cannot be null.");

        RuleFor(x => x.Message.Text)
            .NotNull()
            .WithMessage("Text cannot be null.")
            .NotEmpty()
            .WithMessage("Text cannot be empty.");
    }
}

public class AddMessageToConversationCommandHandler : IRequestHandler<AddMessageToConversationCommand, ResultDto<MessageWithSenderDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubService _hubService;
    public AddMessageToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        IHubService hubService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _hubService = hubService;
    }
    public async Task<ResultDto<MessageWithSenderDto>> Handle(AddMessageToConversationCommand request, CancellationToken cancellationToken)
    {
        User sender = await _unitOfWork.Users.GetUserByIdAsync(request.SenderId);

        if (sender == null)
        {
            return ResultDto.FailureResult<MessageWithSenderDto>(HttpStatusCode.NotFound,
                "No sender was found.");
        }

        Conversation conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<MessageWithSenderDto>(HttpStatusCode.NotFound,
                "No conversation was found.");
        }

        Message message = await _unitOfWork.Messages
             .AddMessageToConversationAsync(request.Message.Text, conversation, sender);

        await _unitOfWork.SaveChangesAsync();

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(message);

        await _hubService.NotifyGroupAsync(conversation.Id, mappedMessage, "ReceiveNotification");

        return ResultDto.SuccessResult(mappedMessage, HttpStatusCode.Created);
    }
}

