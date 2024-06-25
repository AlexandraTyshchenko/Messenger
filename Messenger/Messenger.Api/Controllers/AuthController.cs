using Azure;
using MediatR;
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
            var command = new RegisterUserCommand { UserRegistration = userRegistration };
            var response = await _mediator.Send(command);

            return response.Success ? Created() : StatusCode((int)response.HttpStatusCode, response.ErrorMessage);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
        {
            var response = await _mediator.Send(new AuthenticateUserQuery { UserLogin = user });

            return response.Success ? Ok(response.Payload) : StatusCode((int)response.HttpStatusCode, response.ErrorMessage); 
        }
    }
}
