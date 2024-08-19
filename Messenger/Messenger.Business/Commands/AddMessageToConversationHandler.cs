using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Options;
using Messenger.Client.Interfaces;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace Messenger.Business.Commands;

public class AddMessageToConversationCommand : IRequest<ResultDto<MessageWithSenderDto>>
{
    public Guid SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public MessageDto Message { get; set; }
    public IFormFile Image { get; set; }
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
    private readonly IImageClient _imageClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly List<string> _supportedFormats;

    public AddMessageToConversationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        IHubService hubService, IImageClient imageClient, IHttpContextAccessor httpContextAccessor, IOptions<ImageServiceSettings> imageServiceSettings)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _hubService = hubService;
        _imageClient = imageClient;
        _httpContextAccessor = httpContextAccessor;
        _supportedFormats = imageServiceSettings.Value.SupportedImageFormats;
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

        ResultDto<ImageResultDto> response;

        if (request.Image != null)
        {
            if (!IsValidFormat(request.Image))
            {
                return ResultDto.FailureResult<MessageWithSenderDto>(HttpStatusCode.UnsupportedMediaType,
                    $"Unsupported image format. Supported formats are: {string.Join(", ", _supportedFormats)}.");
            }

            var fileExtension = Path.GetExtension(request.Image.FileName).ToLowerInvariant();

            string authToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()
                .Replace("Bearer ", "");

            response = await _imageClient.UploadImageAsync(request.Image, authToken, request.ConversationId);

            if (!response.Success)
            {
                return ResultDto.FailureResult<MessageWithSenderDto>(HttpStatusCode.BadRequest, response.ErrorMessage);
            }
        }

        var message = new Message()
        {
            Conversation = conversation,
            IsJoinMessage = false,
            Sender = sender,
            Text = request.Message.Text,
            SentAt = DateTime.Now,
        };

        Message joinMessage = await _unitOfWork.Messages.AddMessageToConversationAsync(message);

        await _unitOfWork.SaveChangesAsync();

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        await _hubService.NotifyGroupAsync(conversation.Id, mappedMessage, "ReceiveNotification");

        return ResultDto.SuccessResult(mappedMessage, HttpStatusCode.Created);
    }
    private bool IsValidFormat(IFormFile image)
    {
        if (image == null) return false;
        var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
        return _supportedFormats.Contains(fileExtension);
    }
}

