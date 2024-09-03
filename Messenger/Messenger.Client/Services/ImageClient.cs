using Messanger.Image.Client.Interfaces;
using Messanger.Image.Client.Options;
using Messenger.Business.Dtos;
using Messenger.Client.Interfaces;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;

namespace Messanger.Image.Client.Services;

public class ImageClient : IImageClient
{
    private readonly HttpClient _httpClient;
    private const string resourse = "Images";
    private readonly string _serviceUrl;
    private readonly IImageContentService _imageContentService;

    public ImageClient(HttpClient httpClient, IOptions<ImageServiceSettings>  options,IImageContentService imageContentService)
    {
        _httpClient = httpClient;
        _serviceUrl = options.Value.Url;
        _imageContentService = imageContentService;
    }

    public async Task<ResultDto<ImageResultDto>> UploadImageAsync(IFormFile image, string authToken, Guid conversationId)
    {
        MultipartFormDataContent content = await _imageContentService.CreateMultipartContentAsync(image);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        string url = $"{_serviceUrl}/api/Conversations" + $"/{conversationId}" + $"/{resourse}";
        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return await ProcessResponseAsync(response);
    }

    public async Task<ResultDto<ImageResultDto>> DeleteImageAsync(string imageFileName, string authToken, Guid conversationId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var deleteUrl = $"{_serviceUrl}/api/Conversations" + $"/{conversationId}" + $"/{resourse}/{imageFileName}";

        HttpResponseMessage response = await _httpClient.DeleteAsync(deleteUrl);

        var responseContent = await response.Content.ReadAsStringAsync();

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

    public async Task<ResultDto<ImageResultDto>> AddConversationImageAsync(IFormFile image, string authToken, Guid conversationId)
    {
        MultipartFormDataContent content = await _imageContentService.CreateMultipartContentAsync(image);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        string url = $"{_serviceUrl}/api/Conversations" + $"/{conversationId}" + $"/{resourse}/conversationImage";
        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        return await ProcessResponseAsync(response);
    }
}
