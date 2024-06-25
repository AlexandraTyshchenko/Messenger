using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
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
        private readonly IUserAuthenticationRepository _userAuthenticationRepository;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IUserAuthenticationRepository userAuthenticationRepository,
            IMapper mapper)
        {
            _userAuthenticationRepository = userAuthenticationRepository;
            _mapper = mapper;
        }

        public async Task<ResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            User user = _mapper.Map<User>(request.UserRegistration);

            IdentityResult result = await _userAuthenticationRepository
                .RegisterUserAsync(user, request.UserRegistration.Password);

            if (result.Succeeded)
            {
                return ResultDto.SuccessResult(HttpStatusCode.OK);
            }

            return ResultDto.FailureResult(HttpStatusCode.BadRequest, 
                string.Join("\n", result.Errors.Select(x => x.Description)));
        }
    }
}
