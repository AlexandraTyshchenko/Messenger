using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Exceptions;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class DeleteParticipantFromConversationCommand : IRequest<ResultDto>
    {
        public Guid ParticipantInConversationId { get; set; }
    }

    public class DeleteParticipantFromConversationCommandHandler
        : IRequestHandler<DeleteParticipantFromConversationCommand, ResultDto>
    {
        private readonly IParticipantRepository _participantRepository;

        public DeleteParticipantFromConversationCommandHandler(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }
        public async Task<ResultDto> Handle(DeleteParticipantFromConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _participantRepository.DeleteParticipantFromConversationAsync(request.ParticipantInConversationId);

                return ResultDto.SuccessResult(HttpStatusCode.OK);
            }
            catch (CustomException ex)
            {
                return ResultDto.FailureResult(ex.StatusCode, ex.Message);
            }
        }
    }
}
