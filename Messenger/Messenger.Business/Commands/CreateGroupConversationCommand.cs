using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Commands
{
    public class CreateGroupConversationCommand : IRequest
    {
        public GroupModelDto GroupModelDto { get; set; }
        public Guid CreatorUserId { get; set; }
    }

    public class CreateGroupConversationCommandHandler : IRequestHandler<CreateGroupConversationCommand>
    {
        private readonly IConversationRepository _conversationRepository;

        public CreateGroupConversationCommandHandler(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }
        public async Task<Unit> Handle(CreateGroupConversationCommand request, CancellationToken cancellationToken)
        {
            await _conversationRepository.CreateGroupConversationAsync
                (request.GroupModelDto.Title,
                request.GroupModelDto.ImgUrl,
                request.CreatorUserId);

            return Unit.Value;
        }
    }
}
