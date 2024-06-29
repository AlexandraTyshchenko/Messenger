using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Messenger.Business.Interfaces
{
    public interface IUserAuthenticationService
    {
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<bool> ValidateUserAsync(string userName, string password);
        Task<string> CreateTokenAsync();
        string CreateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
