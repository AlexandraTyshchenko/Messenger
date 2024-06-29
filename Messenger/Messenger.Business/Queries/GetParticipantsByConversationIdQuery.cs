using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Queries
{
    public class GetParticipantsByConversationIdQuery :IRequest<ResultDto<IEnumerable<UserBasicInfoDto>>>
    {
        public Guid ConversationId { get; set; }
    }
    public class GetParticipantsByConversationIdHandler : IRequestHandler<GetParticipantsByConversationIdQuery,
        ResultDto<IEnumerable<UserBasicInfoDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IParticipantRepository _participantRepository;
        public GetParticipantsByConversationIdHandler(IParticipantRepository participantRepository, IMapper mapper)
        {
            _mapper = mapper;
            _participantRepository = participantRepository;
        }

        public async Task<ResultDto<IEnumerable<UserBasicInfoDto>>> Handle(GetParticipantsByConversationIdQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<User> usersInConversation = (await _participantRepository
                .GetParticipantsByConversationIdAsync(request.ConversationId)).Select(x=>x.User).ToList();
            var mappedParticipants = _mapper.Map<IEnumerable<UserBasicInfoDto>>(usersInConversation);

            return ResultDto<IEnumerable<UserBasicInfoDto>>.SuccessResult(mappedParticipants, HttpStatusCode.OK);
        }
    }
}
