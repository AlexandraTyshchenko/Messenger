using AutoMapper;
using MediatR;
using Messenger.Business.Commands;
using Messenger.Business.Queries;
using Messenger.Infrastructure.Dtos;
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
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistration,string role)
        {
            var command = new RegisterUserCommand { UserRegistration = userRegistration,Role = role };
            var result = await _mediator.Send(command);

            return !result.Succeeded ? new BadRequestObjectResult(result) : StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
        {
            var query = new AuthenticateUserQuery { UserLogin = user };
            var result = await _mediator.Send(query);

            return await _mediator.Send(query);
        }
    }
}
