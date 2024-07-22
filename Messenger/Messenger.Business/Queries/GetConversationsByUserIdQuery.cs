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

public class GetConversationsByUserIdQuery : IRequest<ResultDto<IPagedEntities<ConversationWithParticipantsDto>>>
{
    public Guid UserId { get; set; }
    public PaginationParams PaginationParams { get; set; }

}

public class GetConversationsByUserIdQueryValidator : AbstractValidator<GetConversationsByUserIdQuery>
{
    public GetConversationsByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
             .NotEqual(Guid.Empty)
             .WithMessage("UserId cannot be an empty GUID.");

        RuleFor(x => x.PaginationParams)
             .NotNull()
             .SetValidator(new PaginationParamsValidator());
    }
}

public class GetConversationsByUserIdQueryHandler : IRequestHandler<GetConversationsByUserIdQuery, ResultDto<IPagedEntities<ConversationWithParticipantsDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetConversationsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<IPagedEntities<ConversationWithParticipantsDto>>> Handle(GetConversationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        IPagedEntities<Conversation> conversations = await _unitOfWork.Conversations
            .GetConversationsByUserIdAsync(request.UserId, request.PaginationParams.Page, 
            request.PaginationParams.PageSize);

        var mappedConversations = _mapper.MapPagedEntities<Conversation, 
            ConversationWithParticipantsDto>(conversations);


        return ResultDto.SuccessResult(mappedConversations, HttpStatusCode.OK);
    }
}
