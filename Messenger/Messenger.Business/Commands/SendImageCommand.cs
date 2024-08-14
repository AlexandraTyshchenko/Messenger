using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Options;
using Messenger.Client.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace Messenger.Business.Commands;

public class SendImageCommand : IRequest<ResultDto<ImageDto>>
{
    public IFormFile Image { get; set; }
    public Guid ConversationId { get; set; }
}


public class SendImageCommandHandler : IRequestHandler<SendImageCommand, ResultDto<ImageDto>>
{
    private readonly IImageClient _imageClient;
    private readonly List<string> _supportedFormats;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SendImageCommandHandler(IImageClient imageClient, IOptions<ImageServiceSettings> imageServiceSettings, IHttpContextAccessor httpContextAccessor)
    {
        _imageClient = imageClient;
        _supportedFormats = imageServiceSettings.Value.SupportedImageFormats;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResultDto<ImageDto>> Handle(SendImageCommand request, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(request.Image.FileName).ToLowerInvariant();

        var supportedFormatsString = string.Join(", ", _supportedFormats);

        if (!_supportedFormats.Contains(fileExtension))
        {
            return ResultDto.FailureResult<ImageDto>(HttpStatusCode.UnsupportedMediaType,
                $"Unsupported image format. Please upload a file of type: {supportedFormatsString}.");
        }

        string authToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var response = await _imageClient.UploadImageAsync(request.Image, authToken,request.ConversationId);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var imageDto = new ImageDto
            {
                ImgUrl = responseContent
            };

            return ResultDto.SuccessResult(imageDto);
        }

        return ResultDto.FailureResult<ImageDto>(HttpStatusCode.BadRequest,
                $"Error uploading image: {response.ReasonPhrase}");
    }
}

