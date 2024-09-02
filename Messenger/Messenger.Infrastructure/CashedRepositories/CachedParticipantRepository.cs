using Messenger.Infrastructure.Cache;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Messenger.Infrastructure.KeyBuilder;

namespace Messenger.Infrastructure.CachedRepositories;

public class CachedParticipantRepository : IParticipantRepository
{
    private readonly IParticipantRepository _innerRepository;
    private readonly ICacheService _cacheService;
    private readonly ICacheKeyBuilder _cacheKeyBuilder;
    public CachedParticipantRepository(IParticipantRepository innerRepository, ICacheService cacheService, ICacheKeyBuilderFactory cacheKeyBuilderFactory)
    {
        _innerRepository = innerRepository;
        _cacheService = cacheService;
        _cacheKeyBuilder = cacheKeyBuilderFactory.Create(typeof(ParticipantInConversation));
    }

    public async Task<IEnumerable<ParticipantInConversation>> AddParticipantsToConversationAsync(IEnumerable<User> users,
        Conversation conversation)
    {
        string participantListKey = _cacheKeyBuilder.AppendParameter(conversation.Id)
            .Build();

        IEnumerable<ParticipantInConversation> result = await _innerRepository
            .AddParticipantsToConversationAsync(users, conversation);

        await _cacheService.RemoveAsync(participantListKey);
        return result;
    }

    public async Task<ParticipantInConversation> DeleteParticipantFromGroupConversationAsync(Guid userId, Guid conversationId)
    {
        string participantListKey = _cacheKeyBuilder.AppendParameter(conversationId)
            .Build();

        string participantKey = _cacheKeyBuilder.AppendParameter(userId)
            .AppendParameter(conversationId)
            .Build();

        ParticipantInConversation result = await _innerRepository
            .DeleteParticipantFromGroupConversationAsync(userId, conversationId);

        await _cacheService.RemoveAsync(participantListKey);
        await _cacheService.RemoveAsync(participantKey);
        return result;
    }

    public async Task<ParticipantInConversation> GetParticipantFromConversationAsync(Guid userId, Guid conversationId)
    {
        string participantKey = _cacheKeyBuilder.AppendParameter(userId)
            .AppendParameter(conversationId)
            .Build();

        ParticipantInConversation cachedParticipant = await _cacheService.GetAsync<ParticipantInConversation>(participantKey);

        if (cachedParticipant != null)
        {
            return cachedParticipant;
        }

        ParticipantInConversation result = await _innerRepository.GetParticipantFromConversationAsync(userId, conversationId);

        await _cacheService.SetAsync(participantKey, result);

        return result;
    }

    public async Task<ParticipantInConversation> GetParticipantFromGroupConversationAsync(Guid userId, Guid conversationId)
    {
        string participantKey = _cacheKeyBuilder.AppendParameter(userId)
              .AppendParameter(conversationId)
              .AppendParameter("group")
              .Build();

        ParticipantInConversation cachedParticipant = await _cacheService.GetAsync<ParticipantInConversation>(participantKey);

        if (cachedParticipant != null)
        {
            return cachedParticipant;
        }

        ParticipantInConversation result = await _innerRepository.GetParticipantFromGroupConversationAsync(userId, conversationId);

        await _cacheService.SetAsync(participantKey, result);

        return result;
    }

    public async Task<IEnumerable<ParticipantInConversation>> GetParticipantsByConversationIdAsync(Guid conversationId)
    {
        string participantListKey = _cacheKeyBuilder.AppendParameter(conversationId)
            .Build();

        IEnumerable<ParticipantInConversation> cachedParticipants = await _cacheService.GetAsync<IEnumerable<ParticipantInConversation>>(participantListKey);

        if (cachedParticipants != null)
        {
            return cachedParticipants;
        }

        IEnumerable<ParticipantInConversation> result = await _innerRepository
            .GetParticipantsByConversationIdAsync(conversationId);

        await _cacheService.SetAsync(participantListKey, result);

        return result;
    }
}
