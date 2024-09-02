using AutoMapper;
using FluentValidation;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using System.Net;

namespace Messenger.Business.Queries;

public class GetParticipantsByConversationIdQuery :IRequest<ResultDto<IEnumerable<ParticipantDto>>>
{
    public Guid ConversationId { get; set; }
}

public class GetParticipantsByConversationIdQueryValidator : AbstractValidator<GetParticipantsByConversationIdQuery>
{
    public GetParticipantsByConversationIdQueryValidator()
    {
        RuleFor(x => x.ConversationId)
          .NotEqual(Guid.Empty);
    }
}

public class GetParticipantsByConversationIdHandler : IRequestHandler<GetParticipantsByConversationIdQuery,
    ResultDto<IEnumerable<ParticipantDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetParticipantsByConversationIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResultDto<IEnumerable<ParticipantDto>>> Handle(GetParticipantsByConversationIdQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<ParticipantInConversation> usersInConversation = await _unitOfWork.Participants
            .GetParticipantsByConversationIdAsync(request.ConversationId);

        var mappedParticipants = _mapper.Map<IEnumerable<ParticipantDto>>(usersInConversation);

        return ResultDto.SuccessResult(mappedParticipants, HttpStatusCode.OK);
    }
}
