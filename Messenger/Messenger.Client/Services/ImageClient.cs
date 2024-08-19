using Messanger.Image.Client.Options;
using Messenger.Business.Dtos;
using Messenger.Client.Interfaces;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Messanger.Image.Client.Services;

public class ImageClient : IImageClient
{
    private readonly HttpClient _httpClient;
    private const string resourse = "Images";
    private readonly string _serviceUrl;

    public ImageClient(HttpClient httpClient, IOptions<ImageServiceSettings>  options)
    {
        _httpClient = httpClient;
        _serviceUrl = options.Value.Url;
    }

    public async Task<ResultDto<ImageResultDto>> UploadImageAsync(IFormFile image, string authToken, Guid conversationId)
    {
        byte[] imageBytes;
        using (var memoryStream = new MemoryStream())
        {
            await image.CopyToAsync(memoryStream);
            imageBytes = memoryStream.ToArray();
        }

        var content = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(imageBytes);

        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(image.ContentType);
        content.Add(imageContent, "image", image.FileName);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        HttpResponseMessage response = await _httpClient.PostAsync($"{_serviceUrl}/api/Conversations" + $"/{conversationId}" + $"/{resourse}", content);
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
}
