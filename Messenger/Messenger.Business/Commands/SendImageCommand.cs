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

public class SendImageCommand : IRequest<ResultDto<ImageResultDto>>
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public ImageDto Image { get; set; }
}

public class SendImageCommandValidator : AbstractValidator<SendImageCommand>
{
    private readonly List<string> _supportedFormats;

    public SendImageCommandValidator(IOptions<ImageServiceSettings> imageServiceSettings)
    {
        _supportedFormats = imageServiceSettings.Value.SupportedImageFormats;

        RuleFor(x => x.Image)
           .NotNull().WithMessage("File is required.");

        RuleFor(x => x.Image.File)
            .NotNull().WithMessage("File is required.")
            .Must(IsValidFormat).WithMessage(x =>
                $"Unsupported image format. Supported formats are: {string.Join(", ", _supportedFormats)}.");

        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("Conversation ID is required.");
    }

    private bool IsValidFormat(IFormFile image)
    {
        if (image == null) return false;
        var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
        return _supportedFormats.Contains(fileExtension);
    }
}

public class SendImageCommandHandler : IRequestHandler<SendImageCommand, ResultDto<ImageResultDto>>
{
    private readonly IImageClient _imageClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubService _hubService;
    private readonly IMapper _mapper;

    public SendImageCommandHandler(IImageClient imageClient, IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork, IHubService hubService, IMapper mapper)
    {
        _imageClient = imageClient;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
        _hubService = hubService;
        _mapper = mapper;
    }

    public async Task<ResultDto<ImageResultDto>> Handle(SendImageCommand request, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(request.Image.File.FileName).ToLowerInvariant();

        string authToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        ResultDto<ImageResultDto> response = await _imageClient.UploadImageAsync(request.Image.File, authToken, request.ConversationId);

        Conversation conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<ImageResultDto>(HttpStatusCode.NotFound,
                "No conversation was found.");
        }

        User sender = await _unitOfWork.Users.GetUserByIdAsync(request.SenderId);

        if (sender == null)
        {
            return ResultDto.FailureResult<ImageResultDto>(HttpStatusCode.NotFound,
                "No sender was found.");
        }
        var message = new Message
        {
            ImageUrl = response.Payload.RelativePath,
            Conversation = conversation,
            IsJoinMessage = false,
            Sender = sender,
            Text = request.Image.Text,
            SentAt = DateTime.Now,
        };

        Message joinMessage = await _unitOfWork.Messages.AddMessageToConversationAsync(message);

        await _unitOfWork.SaveChangesAsync();

        var mappedMessage = _mapper.Map<MessageWithSenderDto>(joinMessage);

        await _hubService.NotifyGroupAsync(conversation.Id, mappedMessage, "ReceiveNotification");
        return response;
    }
}

