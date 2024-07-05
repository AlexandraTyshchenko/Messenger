using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Options;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Messenger.Business.Queries;

public class AuthenticateUserQuery : IRequest<ResultDto<AccessTokenDto>>
{
    public UserLoginDto UserLogin { get; set; }
}

public class AuthenticateUserQueryValidator : AbstractValidator<AuthenticateUserQuery>
{
    public AuthenticateUserQueryValidator()
    {
        RuleFor(x => x.UserLogin)
            .NotEmpty()
            .WithMessage("UserLogin cannot be empty.");

        RuleFor(x => x.UserLogin.UserName)
            .NotEmpty().WithMessage("UserName is required");

        RuleFor(x => x.UserLogin.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class AuthenticateUserQueryHandler : IRequestHandler<AuthenticateUserQuery, ResultDto<AccessTokenDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly ApplicationContext _applicationContext;

    public AuthenticateUserQueryHandler(UserManager<User> userManager,
        IOptions<JwtSettings> jwtSettings,
        ApplicationContext applicationContext)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _applicationContext = applicationContext;
    }

    public async Task<ResultDto<AccessTokenDto>> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        User user = await ValidateUserAsync(request.UserLogin.UserName, request.UserLogin.Password);

        if (user == null)
        {
            return ResultDto.FailureResult<AccessTokenDto>(
                HttpStatusCode.Unauthorized, "Invalid credentials.");
        }

        string token = await CreateTokenAsync(user);

        return ResultDto.SuccessResult(new AccessTokenDto
        {
            Token = token,
        });
    }

    private async Task<User> ValidateUserAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        var result = user != null && await _userManager.CheckPasswordAsync(user, password);
        return user;
    }

    private async Task<string> CreateTokenAsync(User user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiresIn),
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
