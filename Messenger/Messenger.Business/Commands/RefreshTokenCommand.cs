using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Options;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;

namespace Messenger.Business.Commands;

public class RefreshTokenCommand : IRequest<ResultDto<TokenDto>>
{
    public string RefreshToken { get; set; }
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull()
            .NotEmpty()
            .WithMessage("Refresh token cannot be empty.");
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResultDto<TokenDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(UserManager<User> userManager,
        IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;

    }

    public async Task<ResultDto<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _unitOfWork.RefreshTokens.GetRefreshTokenAsync(request.RefreshToken);

        if (refreshToken == null || refreshToken.ExpiryDate <= DateTime.Now)
        {
            return ResultDto.FailureResult<TokenDto>(HttpStatusCode.BadRequest, "Invalid or expired refresh token.");
        }

        var user = await _userManager.FindByIdAsync(refreshToken.User.Id.ToString());

        if (user == null)
        {
            return ResultDto.FailureResult<TokenDto>(HttpStatusCode.BadRequest, "Invalid refresh token.");
        }

        string newToken = await _tokenService.CreateTokenAsync(user);
        string newRefreshToken = _tokenService.GenerateRefreshToken();

        // Update the refresh token in the database
        refreshToken.Token = newRefreshToken;
        refreshToken.ExpiryDate = DateTime.Now.AddMinutes(_jwtSettings.RefreshExpirationTimeInMinutes);
        _unitOfWork.RefreshTokens.UpdateRefreshToken(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return ResultDto.SuccessResult(new TokenDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken
        });
    }
}
