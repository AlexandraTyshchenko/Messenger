using Messanger.Image.Client.Dtos;
using Microsoft.AspNetCore.Http;

namespace Messenger.Client.Interfaces;

public interface IImageClient
{
    Task<HttpResponseMessage> UploadImageAsync(IFormFile image,string authToken,Guid conversationId);
    Task<HttpResponseMessage> DeleteImageAsync(string imageFileName, string authToken, Guid conversationId);
}
