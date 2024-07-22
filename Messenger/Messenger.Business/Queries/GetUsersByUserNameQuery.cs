using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Business.Profiles;
using Messenger.Business.Validators;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Pagination;
using System.Net;

namespace Messenger.Business.Queries;

public class GetUsersByUserNameQuery : IRequest<ResultDto<IPagedEntities<UserBasicInfoDto>>>
{
    public string UserName { get; set; }
    public PaginationParams PaginationParams { get; set; }
}

public class GetUsersByUserNameQueryValidator : AbstractValidator<GetUsersByUserNameQuery>
{
    public GetUsersByUserNameQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotNull()
            .NotEmpty().WithMessage("UserName is required");

        RuleFor(x => x.PaginationParams)
           .NotNull()
           .SetValidator(new PaginationParamsValidator());
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
        IPagedEntities<User> users = await _unitOfWork.Users
            .GetUsersAsync(request.UserName, request.PaginationParams.Page, request.PaginationParams.PageSize);
        var mappedUsers = _mapper.MapPagedEntities<User, UserBasicInfoDto>(users);

        return ResultDto.SuccessResult(mappedUsers, HttpStatusCode.OK);
    }
}
