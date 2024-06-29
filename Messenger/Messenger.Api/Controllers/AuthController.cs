using MediatR;
using Messenger.Api.Extensions;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistration)
        {
            ResultDto response = await _mediator.Send(new RegisterUserCommand
            {
                UserRegistration = userRegistration
            });

            return response.ToHttpResponse();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
        {
            ResultDto<RefreshTokenDto> response = await _mediator.Send(new AuthenticateUserQuery { UserLogin = user });

            return response.ToHttpResponse<RefreshTokenDto>();
        }
    }
}
