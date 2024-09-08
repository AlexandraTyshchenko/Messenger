using Messenger.Business.Dtos;
using Messenger.Shared.Dtos;
using Microsoft.AspNetCore.Http;

namespace Messenger.Client.Interfaces;

public interface IImageClient
{
    Task<ResultDto<ImageResultDto>> UploadImageAsync(IFormFile image, Guid conversationId);
    Task<ResultDto<ImageResultDto>> DeleteImageAsync(string imageFileName, Guid conversationId);
    Task<ResultDto<ImageResultDto>> AddConversationImageAsync(IFormFile image, Guid conversationId);
}
