using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationContext _applicationContext;

    public RefreshTokenRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _applicationContext.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
       return await _applicationContext.RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
    }

    public  RefreshToken UpdateRefreshToken(RefreshToken refreshToken)
    {
        _applicationContext.RefreshTokens.Update(refreshToken);
        return refreshToken;
    }
}
