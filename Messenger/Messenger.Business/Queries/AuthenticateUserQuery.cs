using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Queries
{
    public class AuthenticateUserQuery : IRequest<ResultDto<RefreshTokenDto>>
    {
        public UserLoginDto UserLogin { get; set; }
    }

    public class AuthenticateUserQueryHandler : IRequestHandler<AuthenticateUserQuery, ResultDto<RefreshTokenDto>>
    {
        private readonly IUserAuthenticationRepository _userAuthenticationRepository;

        public AuthenticateUserQueryHandler(IUserAuthenticationRepository userAuthenticationRepository)
        {
            _userAuthenticationRepository = userAuthenticationRepository;
        }

        public async Task<ResultDto<RefreshTokenDto>> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
        {
            bool isAuthenticated = await _userAuthenticationRepository.ValidateUserAsync(request.UserLogin.UserName,
                request.UserLogin.Password);

            if (!isAuthenticated)
            {
                return ResultDto<RefreshTokenDto>.FailureResult<RefreshTokenDto>(
                    HttpStatusCode.Unauthorized,
                    "Invalid credentials.");
            }

            var token = await _userAuthenticationRepository.CreateTokenAsync();
            var refreshToken = _userAuthenticationRepository.CreateRefreshToken();

            return ResultDto<RefreshTokenDto>.SuccessResult(new RefreshTokenDto
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }
    }
}
