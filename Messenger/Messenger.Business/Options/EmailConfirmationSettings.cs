namespace Messenger.Business.Options;

public class EmailConfirmationSettings
{
    public string Path { get; set; }
    public string Host { get; set; }
    public int? Port { get; set; }
    public string Scheme {  get; set; }
}
