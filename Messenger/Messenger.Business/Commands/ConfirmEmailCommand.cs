using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text;

namespace Messenger.Business.Commands;

public class ConfirmEmailCommand : IRequest<ResultDto>
{
    public string Token { get; set; }
    public string Email { get; set; }
}

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotNull()
            .NotEmpty()
            .WithMessage("Token cannot be empty.");

        RuleFor(x => x.Email)
          .NotNull()
          .NotEmpty()
          .WithMessage("Email cannot be empty.");
    }
}
public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ResultDto>
{
    private readonly UserManager<User> _userManager;

    public ConfirmEmailCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ResultDto> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        byte[] tokenDecodedBytes = WebEncoders.Base64UrlDecode(request.Token);
        var tokenDecoded = Encoding.UTF8.GetString(tokenDecodedBytes);

        var result = await _userManager.ConfirmEmailAsync
            (await _userManager.FindByEmailAsync(request.Email), tokenDecoded);

        if (result.Succeeded)
        {
            return ResultDto.SuccessResult(HttpStatusCode.OK);
        }

        return ResultDto.FailureResult(HttpStatusCode.BadRequest,
                       string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
    }
}
