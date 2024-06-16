using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Queries
{
    public class GetParticipantsByConversationId :IRequest<IEnumerable<UserBasicInfoDto>>
    {
        public Guid ConversationId { get; set; }
    }
    public class GetParticipantsByConversationIdHandler : IRequestHandler<GetParticipantsByConversationId, IEnumerable<UserBasicInfoDto>>
    {
        private readonly IMapper _mapper;
        private readonly IParticipantRepository _participantRepository;
        public GetParticipantsByConversationIdHandler(IParticipantRepository participantRepository, IMapper mapper)
        {
            _mapper = mapper;
            _participantRepository = participantRepository;
        }

        public async Task<IEnumerable<UserBasicInfoDto>> Handle(GetParticipantsByConversationId request, CancellationToken cancellationToken)
        {
            IEnumerable<User> participants = await _participantRepository.GetParticipantsByConversationIdAsync(request.ConversationId);

            return _mapper.Map<IEnumerable<UserBasicInfoDto>>(participants);
        }
    }
}
