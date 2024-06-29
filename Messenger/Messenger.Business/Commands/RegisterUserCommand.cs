using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
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
}
