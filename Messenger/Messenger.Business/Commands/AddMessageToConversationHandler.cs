using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Options;
using Messenger.Business.Queues;
using Messenger.Business.Validators;
using Messenger.Client.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Shared.Dtos;
using Microsoft.Extensions.Options;
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


public class AddMessageToConversationCommandHandler : IRequestHandler<AddMessageToConversationCommand, ResultDto<MessageWithSenderDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly MessageQueue _messageQueue;
    private readonly IImageClient _imageClient;
    public AddMessageToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        MessageQueue messageQueue, IImageClient imageClient)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _messageQueue = messageQueue;
        _imageClient = imageClient;
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

        Image image = null;

        if (request.Message.Image != null)
        {

            var fileExtension = Path.GetExtension(request.Message.Image.FileName).ToLowerInvariant();

            ResultDto<ImageResultDto> response = await _imageClient.UploadImageAsync(request.Message.Image, request.ConversationId);

            if (!response.Success)
            {
                return ResultDto.FailureResult<MessageWithSenderDto>(response.HttpStatusCode, response.ErrorMessage);
            }

            image = new Image
            {
                ImageUrl = response.Payload.RelativePath,
                FileName = response.Payload.FileName,
            };
        }

        var message = new Message()
        {
            Image = image,
            Conversation = conversation,
            IsJoinMessage = false,
            Sender = sender,
            Text = request.Message.Text,
            SentAt = DateTime.Now,
        };

        Message joinMessage = await _unitOfWork.Messages.AddMessageToConversationAsync(message);

        await _unitOfWork.SaveChangesAsync();

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        await _messageQueue.EnqueueAsync(new ChatNotification
        {
            ConversationId = conversation.Id,
            Message = mappedMessage
        });

        return ResultDto.SuccessResult(mappedMessage, HttpStatusCode.Created);
    }
}

