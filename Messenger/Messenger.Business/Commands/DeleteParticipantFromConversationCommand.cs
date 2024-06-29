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
            ParticipantInConversation participantInConversation = await _participantRepository
                .GetParticipantByIdAsync(request.ParticipantInConversationId);

            if (participantInConversation == null)
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound, $"Participant with id {participantInConversation.Id} wasn`t found");
            }

            List<ParticipantInConversation> participants = (await _participantRepository
                .GetParticipantsByConversationIdAsync(participantInConversation.Conversation.Id))
                .ToList();

            int participantsCount = participants.Count();
            if (participantsCount <= 1)
            {
                return ResultDto.FailureResult(HttpStatusCode.BadRequest, "Cannot delete the last participant from conversation.");
            }

            await _participantRepository.DeleteParticipantFromConversationAsync(participantInConversation);

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

