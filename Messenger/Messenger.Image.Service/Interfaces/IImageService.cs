namespace Messenger.Image.Api.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile image, Guid conversationId);
    Task<bool> DeleteImageAsync(Guid conversationId, string imageFileName);
}
