using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly ApplicationContext _applicationContext;

    public ParticipantRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<IEnumerable<ParticipantInConversation>> AddParticipantsToConversationAsync(IEnumerable<User> users, Conversation conversation)
    {
        IEnumerable<ParticipantInConversation> participants = users.Select(x => new ParticipantInConversation
        {
            User = x,
            JoinedAt = DateTime.UtcNow,
            Conversation = conversation,
            Role = Role.Participant,
        });

        await _applicationContext.AddRangeAsync(participants);
        return participants;
    }

    public async Task<ParticipantInConversation> DeleteParticipantFromGroupConversationAsync(Guid userId, Guid conversationId)
    {
        ParticipantInConversation participantInConversation = await GetParticipantFromGroupConversationAsync(userId, conversationId);

        if (participantInConversation == null)
        {
            return null;
        }

        ParticipantInConversation deletedParticipantInConversation = _applicationContext.ParticipantsInConversation
            .Remove(participantInConversation).Entity;

        return deletedParticipantInConversation;
    }


    public async Task<IEnumerable<ParticipantInConversation>> GetParticipantsByConversationIdAsync(Guid conversationId)
    {
        return await _applicationContext.ParticipantsInConversation
            .Where(x => x.Conversation.Id == conversationId)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<ParticipantInConversation> GetParticipantFromConversationAsync(Guid userId, Guid conversationId)
    {
        ParticipantInConversation participant = await _applicationContext.ParticipantsInConversation
            .Include(x => x.Conversation)
                .ThenInclude(x => x.Group)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.User.Id == userId && x.Conversation.Id == conversationId);

        return participant;
    }

    public async Task<ParticipantInConversation> GetParticipantFromGroupConversationAsync(Guid userId, Guid conversationId)
    {
        ParticipantInConversation participant = await _applicationContext.ParticipantsInConversation
            .Include(x => x.Conversation)
                .ThenInclude(x => x.Group)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.User.Id == userId && x.Conversation.Id == conversationId && x.Conversation.Group != null);

        return participant;
    }
}