namespace Messenger.Business.Dtos;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImgUrl { get; set; } = string.Empty;
}
