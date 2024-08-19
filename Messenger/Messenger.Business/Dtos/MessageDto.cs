using Microsoft.AspNetCore.Http;

namespace Messenger.Business.Dtos;

public class MessageDto
{
    public string Text { get; set; }
    public bool IsJoinMessage { get; set; }
    public IFormFile Image { get; set; }
}
