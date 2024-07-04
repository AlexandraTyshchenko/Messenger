using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using System.Net;

namespace Messenger.Business.Queries;

public class GetConversationByIdQuery : IRequest<ResultDto<ConversationDto>>
{
    public Guid ConversationId { get; set; }
}

public class GetConversationByIdQueryValidator : AbstractValidator<GetConversationByIdQuery>
{
    public GetConversationByIdQueryValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEqual(Guid.Empty)
            .WithMessage("ConversationId cannot be an empty GUID."); ;
    }
}

public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, ResultDto<ConversationDto>>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetConversationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<ConversationDto>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(request.ConversationId);

        if (conversation == null)
        {
            return ResultDto.FailureResult<ConversationDto>(HttpStatusCode.NotFound,
                $"Conversation with id {request.ConversationId} wasn't found.");
        }

        var mappedConversation = _mapper.Map<ConversationDto>(conversation);

        return ResultDto.SuccessResult(mappedConversation);
    }
}
