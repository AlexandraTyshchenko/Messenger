using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Messenger.Business.Commands;

public class AuthenticateUserCommand : IRequest<ResultDto<TokenDto>>
{
    public UserLoginDto UserLogin { get; set; }
}

public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator()
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

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, ResultDto<TokenDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthenticateUserCommandHandler(UserManager<User> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<ResultDto<TokenDto>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        User user = await _userManager.FindByNameAsync(request.UserLogin.UserName);

        bool isValidated = await ValidateUserAsync(user, request.UserLogin.Password);

        if (!isValidated)
        {
            return ResultDto.FailureResult<TokenDto>(HttpStatusCode.BadRequest, "Invalid credentials.");
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return ResultDto.FailureResult<TokenDto>(HttpStatusCode.BadRequest, "Email not confirmed.");
        }

        string token = await _tokenService.CreateTokenAsync(user);
        string refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.StoreRefreshTokenAsync(user, refreshToken);

        return ResultDto.SuccessResult(new TokenDto
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }

    private async Task<bool> ValidateUserAsync(User user, string password)
    {
        var result = user != null && await _userManager.CheckPasswordAsync(user, password);
        return result;
    }
}
