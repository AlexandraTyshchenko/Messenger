using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using System.Net;

namespace Messenger.Business.Queries
{
    public class AuthenticateUserQuery : IRequest<ResultDto<RefreshTokenDto>>
    {
        public UserLoginDto UserLogin { get; set; }
    }

    public class AuthenticateUserQueryHandler : IRequestHandler<AuthenticateUserQuery, ResultDto<RefreshTokenDto>>
    {
        private readonly IUserAuthenticationService _userAuthenticationService;

        public AuthenticateUserQueryHandler(IUserAuthenticationService userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        public async Task<ResultDto<RefreshTokenDto>> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
        {
            bool isAuthenticated = await _userAuthenticationService.ValidateUserAsync(request.UserLogin.UserName,
                request.UserLogin.Password);

            if (!isAuthenticated)
            {
                return ResultDto<RefreshTokenDto>.FailureResult<RefreshTokenDto>(
                    HttpStatusCode.Unauthorized,
                    "Invalid credentials.");
            }

            string token = await _userAuthenticationService.CreateTokenAsync();
            string refreshToken = _userAuthenticationService.CreateRefreshToken();

            return ResultDto<RefreshTokenDto>.SuccessResult(new RefreshTokenDto
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }
    }
}
