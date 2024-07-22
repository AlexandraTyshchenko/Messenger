using MediatR;
using Messenger.Api.Extensions;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Api.Controllers;

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
        ResultDto<TokenDto> response = await _mediator.Send(new AuthenticateUserCommand { UserLogin = user });

        return response.ToHttpResponse();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        ResultDto<TokenDto> response = await _mediator.Send(new RefreshTokenCommand
        {
            RefreshToken = refreshTokenDto.RefreshToken
        });

        return response.ToHttpResponse();
    }

    [HttpPost("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        ResultDto response = await _mediator.Send(new ConfirmEmailCommand
        {
            Token = token,
            Email = email
        });

        return response.ToHttpResponse();
    }
}
