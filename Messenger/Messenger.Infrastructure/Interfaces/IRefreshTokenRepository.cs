using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces;

public interface IRefreshTokenRepository
{
    public Task AddRefreshTokenAsync(RefreshToken refreshToken);
    public RefreshToken UpdateRefreshToken(RefreshToken refreshToken);
    public Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
}
