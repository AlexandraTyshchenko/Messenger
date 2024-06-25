using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Exceptions;
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

        public CreatePrivateConversationWithUserCommandHandler(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<ResultDto> Handle(CreatePrivateConversationWithUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _conversationRepository.CreatePrivateConversationWithUserAsync(request.CreatorUserId, request.UserId);
                return ResultDto.SuccessResult(HttpStatusCode.Created);
            }
            catch (CustomException ex)
            {
                return ResultDto.FailureResult(ex.StatusCode, ex.Message);
            }          
        }
    }
}
