using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class CreatePrivateConversationWithUserCommand : IRequest<ResultDto>
    {
        public Guid CreatorUserId { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreatePrivateConversationWithUserCommandHandler : IRequestHandler<CreatePrivateConversationWithUserCommand, ResultDto>
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;

        public CreatePrivateConversationWithUserCommandHandler(IConversationRepository conversationRepository, IUserRepository userRepository)
        {
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
        }

        public async Task<ResultDto> Handle(CreatePrivateConversationWithUserCommand request, CancellationToken cancellationToken)
        {
            Conversation existingConversation = await _conversationRepository
                .GetPrivateConversationWithUserAsync(request.CreatorUserId, request.UserId);

            if (existingConversation != null)
            {
                return ResultDto.FailureResult(HttpStatusCode.Conflict, "Conversation with this user already exists.");
            }

            User creatorUser = await _userRepository.GetUserByIdAsync(request.CreatorUserId);

            User user = await _userRepository.GetUserByIdAsync(request.UserId);

            if (user == null)
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound, "User was not found.");
            }

            await _conversationRepository.CreateConversationWithUserAsync(creatorUser, user);

            return ResultDto.SuccessResult(HttpStatusCode.Created);
        }
    }
}

