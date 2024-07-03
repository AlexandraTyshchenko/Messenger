using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Messenger.Business.Commands;

public class RegisterUserCommand : IRequest<ResultDto>
{
    public UserRegistrationDto UserRegistration { get; set; }
}

public class RegisterUserCommandHandlerValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandHandlerValidator()
    {
        RuleFor(x => x.UserRegistration.FirstName)
            .NotEmpty().WithMessage("FirstName is required");

        RuleFor(x => x.UserRegistration.LastName)
            .NotEmpty().WithMessage("LastName is required");

        RuleFor(x => x.UserRegistration.UserName)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.UserRegistration.Password)
            .NotEmpty().WithMessage("Password is required");

        RuleFor(x => x.UserRegistration.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.UserRegistration.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ResultDto>
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public RegisterUserCommandHandler(IMapper mapper, UserManager<User> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<ResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        User user = _mapper.Map<User>(request.UserRegistration);

        IdentityResult result = await _userManager.CreateAsync(user, request.UserRegistration.Password);

        if (result.Succeeded)
        {
            return ResultDto.SuccessResult(HttpStatusCode.OK);
        }

        return ResultDto.FailureResult(HttpStatusCode.BadRequest,
            string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
    }
}
