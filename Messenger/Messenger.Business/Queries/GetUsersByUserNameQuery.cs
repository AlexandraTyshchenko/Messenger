using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Queries;

public class GetUsersByUserNameQuery : IRequest<ResultDto<IEnumerable<UserBasicInfoDto>>>
{
    public string UserName { get; set; }
}

public class GetUsersByUserNameQueryHandler : IRequestHandler<GetUsersByUserNameQuery, ResultDto<IEnumerable<UserBasicInfoDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersByUserNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<IEnumerable<UserBasicInfoDto>>> Handle(GetUsersByUserNameQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await _unitOfWork.Users.GetUsersAsync(request.UserName);
        var mappedUsers = _mapper.Map<IEnumerable<UserBasicInfoDto>>(users);

        return ResultDto<IEnumerable<UserBasicInfoDto>>.SuccessResult(mappedUsers, HttpStatusCode.OK);
    }
}
