using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class AddParticipantToConversationCommand : IRequest<ResultDto>
    {
        public Guid[] UserIds { get; set; }
        public Guid ConversationId { get; set; }
    }

    public class AddParticipantToConversationCommandHandler : IRequestHandler<AddParticipantToConversationCommand, ResultDto>
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConversationRepository _conversationRepository;

        public AddParticipantToConversationCommandHandler(IParticipantRepository participantRepository,
            IUserRepository userRepository, IConversationRepository conversationRepository)
        {
            _participantRepository = participantRepository;
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
        }

        public async Task<ResultDto> Handle(AddParticipantToConversationCommand request, CancellationToken cancellationToken)
        {

            IEnumerable<User> users = await _userRepository.GetUsersByIdsAsync(request.UserIds);

            IEnumerable<Guid> missingUserIds = request.UserIds.Where(id => !users.Select(x => x.Id).Contains(id)).ToList();

            if (missingUserIds.Any())
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound,
                    $"users with ids {string.Join(", ", missingUserIds)} were not found ");
            }

            Conversation conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);

            if (conversation == null)
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound, "conversation not found.");
            }

            IEnumerable<ParticipantInConversation> existingParticipants = (await _participantRepository
                .GetParticipantsByConversationIdAsync(request.ConversationId))
                .Where(x => users.Contains(x.User)).ToList();

            string participantsUserNames = string.Join(", ", existingParticipants.Select(x => x.User.UserName));

            if (existingParticipants.Any())
            {
                return ResultDto.FailureResult(HttpStatusCode.Conflict,$"{participantsUserNames} already exist in conversation");
            }

            await _participantRepository.AddParticipantsToConversationAsync(users, conversation);
            return ResultDto.SuccessResult(HttpStatusCode.OK);
        }
    }
}
