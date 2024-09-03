using Microsoft.AspNetCore.Http;

namespace Messenger.Business.Dtos;

public class GroupModelDto
{
    public string Title { get; set; } = string.Empty;
    public IFormFile Image { get; set; }
}
