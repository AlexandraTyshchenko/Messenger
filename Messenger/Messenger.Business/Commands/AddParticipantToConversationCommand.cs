using MediatR;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Commands
{
    public class AddParticipantToConversationCommand : IRequest
    {
        public Guid[] UserIds { get; set; }
        public Guid ConversationId { get; set; }
    }

    public class AddParticipantToConversationCommandHandler : IRequestHandler<AddParticipantToConversationCommand>
    {
        private readonly IParticipantRepository _participantRepository;

        public AddParticipantToConversationCommandHandler(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public async Task<Unit> Handle(AddParticipantToConversationCommand request, CancellationToken cancellationToken)
        {
            await _participantRepository.AddParticipantsToConversationAsync(request.UserIds, request.ConversationId);

            return Unit.Value;
        }
    }
}
