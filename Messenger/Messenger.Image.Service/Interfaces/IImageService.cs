using Messenger.Shared.Dtos;

namespace Messenger.Image.Api.Interfaces;

public interface IImageService
{
    Task<ImageResultDto> SaveImageAsync(IFormFile image, Guid conversationId);
    Task DeleteImageAsync(Guid conversationId, string imageFileName);
}
