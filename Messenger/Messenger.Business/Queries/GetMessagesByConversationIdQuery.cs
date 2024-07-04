using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Queries;

public class GetMessagesByConversationIdQuery : IRequest<ResultDto<IEnumerable<MessageWithSenderDto>>>
{
    public Guid ConversationId { get; set; }
}

public class GetMessagesByConversationIdQueryValidator : AbstractValidator<GetMessagesByConversationIdQuery>
{
    public GetMessagesByConversationIdQueryValidator()
    {
        RuleFor(x => x.ConversationId)
          .Must(guid => guid != Guid.Empty);
    }
}

public class GetMessagesByConversationIdQueryHandler : IRequestHandler<GetMessagesByConversationIdQuery, ResultDto<IEnumerable<MessageWithSenderDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesByConversationIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<IEnumerable<MessageWithSenderDto>>> Handle(GetMessagesByConversationIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Message> messages = await _unitOfWork.Messages.GetMessagesByConversationIdAsync(request.ConversationId);

        var mappedMessages = _mapper.Map<IEnumerable<MessageWithSenderDto>>(messages);

        return ResultDto.SuccessResult(mappedMessages, HttpStatusCode.OK);
    }
}
