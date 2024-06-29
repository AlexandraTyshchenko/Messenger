using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using System.Net;

namespace Messenger.Business.Commands
{
    public class DeleteParticipantFromConversationCommand : IRequest<ResultDto>
    {
        public Guid UserToDeleteId { get; set; }
        public Guid ConversationId { get; set; }
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
            List<ParticipantInConversation> participants = (await _participantRepository
                .GetParticipantsByConversationIdAsync(request.ConversationId))
                .ToList();

            int participantsCount = participants.Count();
            if (participantsCount <= 1)
            {
                return ResultDto.FailureResult(HttpStatusCode.BadRequest, "Cannot delete participant because" +
                    " it would leave the conversation with zero participants.");
            }

            ParticipantInConversation participantInConversation = await _participantRepository
                .DeleteParticipantFromConversationAsync(request.UserToDeleteId,request.ConversationId);

            if (participantInConversation == null)
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound, $"User with id {request.UserToDeleteId} wasn`t found " +
                    $"in conversation with id {request.ConversationId}");
            }

            if (participantInConversation.Role == Role.Admin)
            {
                ParticipantInConversation newAdminParticipant = (await _participantRepository
                    .GetParticipantsByConversationIdAsync(participantInConversation.Conversation.Id))
                    .OrderByDescending(x => x.JoinedAt)
                    .FirstOrDefault()!;
                await _participantRepository.UpdateParticipantRoleAsync(newAdminParticipant.Id, Role.Admin);
            }

            return ResultDto.SuccessResult(HttpStatusCode.OK);
        }
    }
}

