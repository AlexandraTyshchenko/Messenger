namespace Messenger.Business.Dtos;

public class SpamRequestDto
{
    public double Lambda { get; set; }
    public int DurationSeconds { get; set; }
    public string Text {  get; set; }
}