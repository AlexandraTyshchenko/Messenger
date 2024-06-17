using MediatR;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Commands
{
    public class CreatePrivateConversationWithUserCommand :IRequest
    {
        public Guid CreatorUserId { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreatePrivateConversationWithUserCommandHandler : IRequestHandler<CreatePrivateConversationWithUserCommand>
    {
        private readonly IConversationRepository _conversationRepository;

        public CreatePrivateConversationWithUserCommandHandler(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<Unit> Handle(CreatePrivateConversationWithUserCommand request, CancellationToken cancellationToken)
        {
            await _conversationRepository.CreatePrivateConversationWithUserAsync(request.CreatorUserId, request.UserId);

            return Unit.Value;
        }
    }
}
