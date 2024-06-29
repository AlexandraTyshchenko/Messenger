using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Messenger.Business.Queries
{
    public class AuthenticateUserQuery : IRequest<ResultDto<RefreshTokenDto>>
    {
        public UserLoginDto UserLogin { get; set; }
    }

    public class AuthenticateUserQueryHandler : IRequestHandler<AuthenticateUserQuery, ResultDto<RefreshTokenDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _applicationContext;

        public AuthenticateUserQueryHandler(UserManager<User> userManager,
            IConfiguration configuration,
            ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _configuration = configuration;
            _applicationContext = applicationContext;
        }

        public async Task<ResultDto<RefreshTokenDto>> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
        {
            User user = await ValidateUserAsync(request.UserLogin.UserName,
                request.UserLogin.Password);

            if (user == null)
            {
                return ResultDto<RefreshTokenDto>.FailureResult<RefreshTokenDto>(
                    HttpStatusCode.Unauthorized,
                    "Invalid credentials.");
            }

            string token = await CreateTokenAsync(user);
            string refreshToken = CreateRefreshToken();

            return ResultDto<RefreshTokenDto>.SuccessResult(new RefreshTokenDto
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }

        public async Task<User> ValidateUserAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var result = user != null && await _userManager.CheckPasswordAsync(user, password);
            return user;
        }

        public async Task<string> CreateTokenAsync(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string CreateRefreshToken()
        {
            var signingCredentials = GetSigningCredentials();
            var tokenOptions = GenerateRefreshTokenOptions(signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private JwtSecurityToken GenerateRefreshTokenOptions(SigningCredentials signingCredentials)
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: null,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["refreshTokenExpiresIn"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["accessTokenExpiresIn"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = _configuration.GetSection("jwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Secret"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
