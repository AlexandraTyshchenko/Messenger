namespace Messenger.Infrastructure.Entities;

public class Image : BaseEntity
{
    public string ImageUrl { get; set; }
    public string FileName {  get; set; }
    public Message Message { get; set; }
}
