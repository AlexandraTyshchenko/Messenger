using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IUserAuthenticationRepository
    {
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<bool> ValidateUserAsync(string userName, string password);
        Task<string> CreateTokenAsync();
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
