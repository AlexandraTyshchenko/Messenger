using Messanger.Image.Client.Dtos;
using Messenger.Business.Dtos;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Http;

namespace Messenger.Client.Interfaces;

public interface IImageClient
{
    Task<ResultDto<ImageResultDto>> UploadImageAsync(IFormFile image,string authToken,Guid conversationId);
    Task<ResultDto<ImageResultDto>> DeleteImageAsync(string imageFileName, string authToken, Guid conversationId);
    Task<ResultDto<ImageResultDto>> AddConversationImageAsync(IFormFile image, string authToken, Guid conversationId);
}
