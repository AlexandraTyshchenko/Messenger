using Messenger.Infrastructure.Entities;

namespace Messenger.Business.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(User user);
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(User user, string refreshToken);
}
