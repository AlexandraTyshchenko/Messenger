using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Profiles;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Pagination;
using System.Net;

namespace Messenger.Business.Queries;

public class GetUsersByUserNameQuery : IRequest<ResultDto<IPagedEntities<UserBasicInfoDto>>>
{
    public string UserName { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetUsersByUserNameQueryValidator : AbstractValidator<GetUsersByUserNameQuery>
{
    public GetUsersByUserNameQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be a positive number") ;

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be a positive number")
            .LessThan(1000);
    }
}

public class GetUsersByUserNameQueryHandler : IRequestHandler<GetUsersByUserNameQuery, ResultDto<IPagedEntities<UserBasicInfoDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersByUserNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<IPagedEntities<UserBasicInfoDto>>> Handle(GetUsersByUserNameQuery request,
        CancellationToken cancellationToken)
    {
        IPagedEntities<User> users = await _unitOfWork.Users.GetUsersAsync(request.UserName,request.Page,request.PageSize);    
        var mappedUsers = _mapper.MapPagedEntities<User, UserBasicInfoDto>(users);

        return ResultDto.SuccessResult(mappedUsers, HttpStatusCode.OK);
    }
}
