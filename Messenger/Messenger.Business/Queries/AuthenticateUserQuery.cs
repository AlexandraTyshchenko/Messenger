using MediatR;
using Messenger.Infrastructure.Dtos;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Business.Queries
{
    public class AuthenticateUserQuery : IRequest<IActionResult>
    {
        public UserLoginDto UserLogin { get; set; }
    }

    public class AuthenticateUserQueryHandler : IRequestHandler<AuthenticateUserQuery, IActionResult>
    {
        private readonly IUserAuthenticationRepository _userAuthenticationRepository;

        public AuthenticateUserQueryHandler(IUserAuthenticationRepository userAuthenticationRepository)
        {
            _userAuthenticationRepository = userAuthenticationRepository;
        }

        public async Task<IActionResult> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
        {
            bool isAuthenticated = await _userAuthenticationRepository.ValidateUserAsync(request.UserLogin);

            if (!isAuthenticated)
            {
                return new UnauthorizedResult();
            }

            var token = await _userAuthenticationRepository.CreateTokenAsync();
            var refreshToken = _userAuthenticationRepository.CreateRefreshToken();
            return new OkObjectResult(new { Token = token, RefreshToken = refreshToken });
        }
    }
}
