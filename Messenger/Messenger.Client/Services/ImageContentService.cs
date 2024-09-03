using Messanger.Image.Client.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace Messanger.Image.Client.Services;

public class ImageContentService : IImageContentService
{
    public async Task<MultipartFormDataContent> CreateMultipartContentAsync(IFormFile image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

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

        return content;
    }
}
