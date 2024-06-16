using AutoMapper;
using MediatR;
using Messenger.Infrastructure.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Business.Commands
{
    public class RegisterUserCommand : IRequest<IdentityResult>
    {
        public UserRegistrationDto UserRegistration { get; set; }
        public string Role {  get; set; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
    {
        private readonly IUserAuthenticationRepository _userAuthenticationRepository;
        private readonly IMapper _mapper;
        public RegisterUserCommandHandler(IUserAuthenticationRepository userAuthenticationRepository,
            IMapper mapper)
        {
            _userAuthenticationRepository = userAuthenticationRepository;
            _mapper = mapper;
        }

        public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            User user = _mapper.Map<User>(request.UserRegistration);

            return await _userAuthenticationRepository.RegisterUserAsync(user,request.UserRegistration.Password,request.Role);
        }
    }
}
