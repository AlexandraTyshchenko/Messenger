namespace Messenger.Business.Options;

public class JwtSettings
{
    public string Secret { get; set; }
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public double AccessTokenExpiresIn { get; set; }
    public double RefreshTokenExpiresIn { get; set; }
}
