using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Api.Hubs;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.Json;

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
    private readonly IHubContext<ChatHub> _messageHub;
    public AddMessageToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        IHubContext<ChatHub> messageHub)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _messageHub = messageHub;
    }
    public async Task<ResultDto<MessageWithSenderDto>> Handle(AddMessageToConversationCommand request, CancellationToken cancellationToken)
    {
        User sender = await _unitOfWork.Users.GetUserByIdAsync(request.SenderId);

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

        await _messageHub.Clients.Group(conversation.Id.ToString()).SendAsync("ReceiveNotification", mappedMessage);

        return ResultDto.SuccessResult(mappedMessage, HttpStatusCode.Created);
    }
}

