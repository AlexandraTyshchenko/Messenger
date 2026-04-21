using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Options;
using Messenger.Business.Queues;
using Messenger.Business.Validators;
using Microsoft.Extensions.Options;
using System.Net;

namespace Messenger.Business.Commands;

public class AddMessageToConversationCommand : IRequest<ResultDto<MessageDto>>
{
    public Guid SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public MessageDto Message { get; set; }
}

public class AddMessageToConversationCommandValidator : AbstractValidator<AddMessageToConversationCommand>
{
    public AddMessageToConversationCommandValidator(IOptions<ImageFormatsSettings> imageServiceSettings)
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
            .NotEmpty()
            .NotNull()
            .When(x => x.Message.Image == null)
            .WithMessage("Text is required.");

        When(x => x.Message.Image != null, () =>
        {
            RuleFor(x => x.Message.Image).SetValidator(new ImageValidator(imageServiceSettings));
        });
    }
}


public class AddMessageToConversationCommandHandler : IRequestHandler<AddMessageToConversationCommand, ResultDto<MessageDto>>
{
    private readonly MessageQueue _messageQueue;
    public AddMessageToConversationCommandHandler(MessageQueue messageQueue)
    {
        _messageQueue = messageQueue;
    }

    public async Task<ResultDto<MessageDto>> Handle(AddMessageToConversationCommand request, CancellationToken cancellationToken)
    {
        await _messageQueue.EnqueueAsync(new ChatNotification
        {
            SenderId = request.SenderId,
            ConversationId = request.ConversationId,
            Message = request.Message
        }, cancellationToken);

        return ResultDto.SuccessResult(request.Message, HttpStatusCode.Accepted);
    }
}

