using Messenger.Business.Interfaces;
using Messenger.Business.Options;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Messenger.Business.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager,
        IUnitOfWork unitOfWork)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> CreateTokenAsync(User user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
        issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.AccessExpirationTimeInMinutes),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("RelativePath", user.ImgUrl) 
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

    public async Task StoreRefreshTokenAsync(User user, string refreshToken)
    {
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            User = user,
            ExpiryDate = DateTime.Now.AddMinutes(_jwtSettings.RefreshExpirationTimeInMinutes)
        };

        await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();
    }
}
