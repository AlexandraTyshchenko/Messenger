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
        public Guid UserId { get; set; }
        public Guid ConversationId { get; set; }
    }

    public class DeleteParticipantFromConversationCommandHandler
        : IRequestHandler<DeleteParticipantFromConversationCommand, ResultDto>
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IConversationRepository _conversationRepository;

        public DeleteParticipantFromConversationCommandHandler(IParticipantRepository participantRepository, 
            IConversationRepository conversationRepository)
        {
            _participantRepository = participantRepository;
            _conversationRepository = conversationRepository;
        }
        public async Task<ResultDto> Handle(DeleteParticipantFromConversationCommand request, CancellationToken cancellationToken)
        {
            Conversation conversation = await _conversationRepository.GetConversationByIdAsync(request.ConversationId);

            List<ParticipantInConversation> participants = (await _participantRepository
                .GetParticipantsByConversationIdAsync(request.ConversationId))
                .ToList();

            if (participants.Count() <= 1)
            {
                return ResultDto.FailureResult(HttpStatusCode.BadRequest, "Cannot delete participant because" +
                    " it would leave the conversation with zero participants.");
            }

            ParticipantInConversation participantInConversation = await _participantRepository
                .DeleteParticipantFromGroupConversationAsync(request.UserId, request.ConversationId);

            if (participantInConversation == null)
            {
                return ResultDto.FailureResult(HttpStatusCode.NotFound, $"User with id {request.UserId}" +
                    $" wasn`t found in group conversation with id {request.ConversationId}.");
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

