using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Queries;

public class GetConversationsByUserIdQuery : IRequest<ResultDto<IEnumerable<ConversationDto>>>
{
    public Guid UserId { get; set; }

}

public class GetConversationsByUserIdQueryValidator : AbstractValidator<GetConversationsByUserIdQuery>
{
    public GetConversationsByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId)
             .NotEqual(Guid.Empty)
             .WithMessage("UserId cannot be an empty GUID.");
    }
}

public class GetConversationsByUserIdQueryHandler : IRequestHandler<GetConversationsByUserIdQuery, ResultDto<IEnumerable<ConversationDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetConversationsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<IEnumerable<ConversationDto>>> Handle(GetConversationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Conversation> conversations = await _unitOfWork.Conversations.GetConversationsByUserIdAsync(request.UserId);

        var mappedConversations = _mapper.Map<IEnumerable<ConversationDto>>(conversations);

        return ResultDto.SuccessResult(mappedConversations, HttpStatusCode.OK);
    }
}
