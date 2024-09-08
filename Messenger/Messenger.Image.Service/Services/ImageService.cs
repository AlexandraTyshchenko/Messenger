using Messenger.Image.Api.Interfaces;
using Messenger.Shared.Dtos;

namespace Messenger.Image.Api.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<ImageResultDto> AddConversationImage(IFormFile image, Guid conversationId)
    {
        return await SaveImageToFolderAsync(image, conversationId, "conversationImage");
    }

    public void  DeleteImage(Guid conversationId, string imageFileName)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, $"conversations/{conversationId}/images/{imageFileName}");

        if (!File.Exists(uploadPath))
        {
            throw new Exception("Couldn`t delete file, as path doesn`t exist");
        }
        File.Delete(uploadPath);
    }

    public async Task<ImageResultDto> SaveImageAsync(IFormFile image, Guid conversationId)
    {
        return await SaveImageToFolderAsync(image, conversationId, "images");
    }

    private async Task<ImageResultDto> SaveImageToFolderAsync(IFormFile image, Guid conversationId, string folderName)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("No file uploaded.");
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        var uploadPath = Path.Combine(_environment.WebRootPath, "conversations", conversationId.ToString(), folderName);

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var relativePath = Path.Combine("conversations", conversationId.ToString(), folderName, fileName).Replace("\\", "/");

        return new ImageResultDto
        {
            RelativePath = relativePath,
            FileName = fileName,
        };
    }

}


