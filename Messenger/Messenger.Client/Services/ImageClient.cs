using Messanger.Image.Client.Dtos;
using Messenger.Client.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Messanger.Image.Client.Services;

public class ImageClient : IImageClient
{
    private readonly HttpClient _httpClient;
    private const string url = "https://localhost:7289/api/Conversations";
    private const string resourse = "Images";

    public ImageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> UploadImageAsync(IFormFile image, string authToken, Guid conversationId)
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
        return await _httpClient.PostAsync(url + $"/{conversationId}" + $"/{resourse}", content);
    }

    public async Task<HttpResponseMessage> DeleteImageAsync(string imageFileName, string authToken, Guid conversationId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var deleteUrl = url + $"/{conversationId}" + $"/{resourse}/{imageFileName}";

        return await _httpClient.DeleteAsync(deleteUrl);
    }
}
