using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Queries
{
    public class GetUsersByUserNameQuery : IRequest<ResultDto<IEnumerable<UserBasicInfoDto>>>
    {
        public string UserName { get; set; }
    }

    public class GetUsersByUserNameQueryHandler : IRequestHandler<GetUsersByUserNameQuery, ResultDto<IEnumerable<UserBasicInfoDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersByUserNameQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResultDto<IEnumerable<UserBasicInfoDto>>> Handle(GetUsersByUserNameQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await _userRepository.GetUsersAsync(request.UserName);
            var mappedUsers = _mapper.Map<IEnumerable<UserBasicInfoDto>>(users);

            return ResultDto.SuccessResult(mappedUsers, HttpStatusCode.OK);
        }
    }

}
