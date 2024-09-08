using Messenger.Shared.Dtos;

namespace Messenger.Image.Api.Interfaces;

public interface IImageService
{
    Task<ImageResultDto> SaveImageAsync(IFormFile image, Guid conversationId);
    void DeleteImage(Guid conversationId, string imageFileName);
    Task<ImageResultDto> AddConversationImage(IFormFile image, Guid conversationId);
}
