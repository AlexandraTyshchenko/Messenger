using Messanger.Image.Client.Interfaces;
using Messanger.Image.Client.Options;
using Messenger.Business.Dtos;
using Messenger.Client.Interfaces;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Messanger.Image.Client.Services;

public class ImageClient : IImageClient
{
    private readonly HttpClient _httpClient;
    private const string Resourse = "Images";
    private readonly string _baseServiceUrl;
    private readonly IImageContentService _imageContentService;
    private const string ConversationRelativePath = "api/Conversations/";

    public ImageClient(HttpClient httpClient, IOptions<ImageServiceSettings> options, IImageContentService imageContentService)
    {
        _httpClient = httpClient;
        _baseServiceUrl = options.Value.Url;
        _imageContentService = imageContentService;
    }

    public async Task<ResultDto<ImageResultDto>> UploadImageAsync(IFormFile image, Guid conversationId)
    {
        MultipartFormDataContent content = await _imageContentService.CreateMultipartContentAsync(image);

        string url = $"{_baseServiceUrl}{ConversationRelativePath}" + $"{conversationId}" + $"/{Resourse}";
        HttpResponseMessage response = await _httpClient.PostAsync(url, content);

        return await ProcessResponseAsync(response);
    }

    public async Task<ResultDto<ImageResultDto>> DeleteImageAsync(string imageFileName, Guid conversationId)
    {
        var deleteUrl = $"{_baseServiceUrl}{ConversationRelativePath}" + $"{conversationId}" + $"/{Resourse}/{imageFileName}";

        HttpResponseMessage response = await _httpClient.DeleteAsync(deleteUrl);


        return await ProcessResponseAsync(response);
    }

    private async Task<ResultDto<ImageResultDto>> ProcessResponseAsync(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var data = JsonConvert.DeserializeObject<ImageResultDto>(responseContent);
            return ResultDto.SuccessResult(data);
        }
        else
        {
            return ResultDto.FailureResult<ImageResultDto>(response.StatusCode, responseContent);
        }
    }

    public async Task<ResultDto<ImageResultDto>> AddConversationImageAsync(IFormFile image, Guid conversationId)
    {
        MultipartFormDataContent content = await _imageContentService.CreateMultipartContentAsync(image);

        string url = $"{_baseServiceUrl}{ConversationRelativePath}" + $"{conversationId}" + $"/{Resourse}/conversationImage";
        HttpResponseMessage response = await _httpClient.PostAsync(url, content);

        return await ProcessResponseAsync(response);
    }
}
