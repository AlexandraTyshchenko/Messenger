using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Business.Commands
{
    public class RefreshTokenCommand : IRequest<IActionResult>
    {
        public TokenModel TokenModel { get; set; }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IActionResult>
    {
        private readonly IUserAuthenticationRepository _userAuthenticationRepository;

        public RefreshTokenCommandHandler(IUserAuthenticationRepository userAuthenticationRepository)
        {
            _userAuthenticationRepository = userAuthenticationRepository;
        }

        public async Task<IActionResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            //todo спитати
            return null;
        }
    }

}
