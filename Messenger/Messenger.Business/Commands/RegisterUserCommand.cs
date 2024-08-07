using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
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
        RuleFor(x => x.UserRegistration)
           .NotNull()
           .WithMessage("UserRegistration cannot be null.");

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
            .Matches(@"\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})").WithMessage("Invalid phone number format");
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ResultDto>
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IUrlHelperService _urlHelperService;
    private readonly IEmailService _emailService;

    public RegisterUserCommandHandler(IMapper mapper, UserManager<User> userManager,
        IUrlHelperService urlHelperService, IEmailService emailService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _urlHelperService = urlHelperService;
        _emailService = emailService;
    }

    public async Task<ResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        User user = _mapper.Map<User>(request.UserRegistration);

        IdentityResult result = await _userManager.CreateAsync(user, request.UserRegistration.Password);
        if (result.Succeeded)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = _urlHelperService.GenerateEmailConfirmationLink(user.Email, token);
                await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by clicking <a href='{confirmationLink}'>" +
                    $"here</a>.");

                return ResultDto.SuccessResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);

                return ResultDto.FailureResult(HttpStatusCode.InternalServerError,
                    "Failed to send confirmation email.");
            }
        }
        return ResultDto.FailureResult(HttpStatusCode.BadRequest,
                  string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
    }
}