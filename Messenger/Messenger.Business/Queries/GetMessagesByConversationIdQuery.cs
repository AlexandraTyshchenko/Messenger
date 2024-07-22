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

public class GetMessagesByConversationIdQuery : IRequest<ResultDto<IPagedEntities<MessageWithSenderDto>>>
{
    public Guid ConversationId { get; set; }
    public PaginationParams PaginationParams { get; set; }
}

public class GetMessagesByConversationIdQueryValidator : AbstractValidator<GetMessagesByConversationIdQuery>
{
    public GetMessagesByConversationIdQueryValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEqual(Guid.Empty)
            .WithMessage("ConversationId cannot be an empty GUID.");

        RuleFor(x => x.PaginationParams)
             .NotNull()
             .SetValidator(new PaginationParamsValidator());
    }
}

public class GetMessagesByConversationIdQueryHandler : IRequestHandler<GetMessagesByConversationIdQuery, ResultDto<IPagedEntities<MessageWithSenderDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesByConversationIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<IPagedEntities<MessageWithSenderDto>>> Handle(GetMessagesByConversationIdQuery request, CancellationToken cancellationToken)
    {
        IPagedEntities<Message> pagedMessages = await _unitOfWork.Messages
            .GetMessagesByConversationIdAsync(request.ConversationId,
            request.PaginationParams.Page, request.PaginationParams.PageSize);

        var mappedUsers = _mapper.MapPagedEntities<Message, MessageWithSenderDto>(pagedMessages);
        return ResultDto.SuccessResult(mappedUsers, HttpStatusCode.OK);
    }
}
