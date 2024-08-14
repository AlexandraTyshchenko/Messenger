using Messenger.Image.Api.Interfaces;

namespace Messenger.Image.Api.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<bool> DeleteImageAsync(Guid conversationId, string imageFileName)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, $"conversations/{conversationId}/images/{imageFileName}");

        if (File.Exists(uploadPath))
        {
            File.Delete(uploadPath);
            return true;
        }

        return false;
    }

    public async Task<string> SaveImageAsync(IFormFile image, Guid conversationId)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("No file uploaded.");
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

        var uploadPath = Path.Combine(_environment.WebRootPath, "conversations",  conversationId.ToString(), "images");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var relativePath = Path.Combine("conversations",  conversationId.ToString(), "images", fileName).Replace("\\", "/");
        return relativePath;
    }

}


