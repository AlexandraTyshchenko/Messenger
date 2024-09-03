using Microsoft.AspNetCore.Http;

namespace Messanger.Image.Client.Interfaces;

public interface IImageContentService
{
    Task<MultipartFormDataContent> CreateMultipartContentAsync(IFormFile image);
}
