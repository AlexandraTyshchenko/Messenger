using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Net;

namespace Messenger.Business.Queries
{
    public class GetParticipantsByConversationIdQuery :IRequest<ResultDto<IEnumerable<ParticipantDto>>>
    {
        public Guid ConversationId { get; set; }
    }
    public class GetParticipantsByConversationIdHandler : IRequestHandler<GetParticipantsByConversationIdQuery,
        ResultDto<IEnumerable<ParticipantDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IParticipantRepository _participantRepository;
        public GetParticipantsByConversationIdHandler(IParticipantRepository participantRepository, IMapper mapper)
        {
            _mapper = mapper;
            _participantRepository = participantRepository;
        }

        public async Task<ResultDto<IEnumerable<ParticipantDto>>> Handle(GetParticipantsByConversationIdQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<ParticipantInConversation> participants = await _participantRepository.GetParticipantsByConversationIdAsync(request.ConversationId);
            var mappedParticipants = _mapper.Map<IEnumerable<ParticipantDto>>(participants);

            return ResultDto<IEnumerable<ParticipantDto>>.SuccessResult(mappedParticipants, HttpStatusCode.OK);
        }
    }
}
