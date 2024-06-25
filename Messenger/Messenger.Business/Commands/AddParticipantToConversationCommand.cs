using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Exceptions;
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

        public AddParticipantToConversationCommandHandler(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public async Task<ResultDto> Handle(AddParticipantToConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _participantRepository.AddParticipantsToConversationAsync(request.UserIds, request.ConversationId);
                return ResultDto.SuccessResult(HttpStatusCode.Created);
            }
            catch (CustomException ex)
            {
                return ResultDto.FailureResult(ex.StatusCode, ex.Message);
            }
        }
    }
}
