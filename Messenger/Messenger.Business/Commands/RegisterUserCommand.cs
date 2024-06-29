using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Messenger.Business.Commands
{
    public class RegisterUserCommand : IRequest<ResultDto>
    {
        public UserRegistrationDto UserRegistration { get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ResultDto>
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IUserAuthenticationService userAuthenticationService,
            IMapper mapper)
        {
            _userAuthenticationService = userAuthenticationService;
            _mapper = mapper;
        }

        public async Task<ResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            User user = _mapper.Map<User>(request.UserRegistration);

            IdentityResult result = await _userAuthenticationService
                .RegisterUserAsync(user, request.UserRegistration.Password);

            if (result.Succeeded)
            {
                return ResultDto.SuccessResult(HttpStatusCode.OK);
            }

            return ResultDto.FailureResult(HttpStatusCode.BadRequest, 
                string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
        }
    }
}
