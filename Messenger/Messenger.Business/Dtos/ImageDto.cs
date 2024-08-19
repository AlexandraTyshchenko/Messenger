using Microsoft.AspNetCore.Http;

namespace Messenger.Business.Dtos;

public class ImageDto
{
    public IFormFile File { get; set; }
    public string Text { get; set; }
}
