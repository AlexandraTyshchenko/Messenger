using Azure;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Enums;
using Messenger.Infrastructure.Interfaces;
using Messenger.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ApplicationContext _applicationContext;

    public ConversationRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Conversation> CreateGroupConversationAsync(string title,  Guid creatorId)
    {
        var group = new Group
        {
            Title = title,
        };

        await _applicationContext.Groups.AddAsync(group);

        var conversation = new Conversation { Group = group };

        User userCreator = await _applicationContext.Users.FindAsync(creatorId);

        var participant = new ParticipantInConversation
        {
            Conversation = conversation,
            JoinedAt = DateTime.UtcNow,
            User = userCreator,
            Role = Role.Admin
        };
        conversation.ParticipantsInConversation.Add(participant);

        await _applicationContext.Conversations.AddAsync(conversation);

        await _applicationContext.ParticipantsInConversation.AddAsync(participant);

        return conversation;
    }

    public async Task<Conversation> CreateConversationWithUserAsync(User creatorUser, User user)
    {
        var conversation = new Conversation();

        await _applicationContext.Conversations.AddAsync(conversation);

        var participantCreatorUser = new ParticipantInConversation
        {
            JoinedAt = DateTime.UtcNow,
            User = creatorUser,
            Conversation = conversation,
            Role = Role.Admin
        };

        var participantUser = new ParticipantInConversation
        {
            JoinedAt = DateTime.UtcNow,
            User = user,
            Conversation = conversation,
            Role = Role.Admin
        };

        await _applicationContext.ParticipantsInConversation.AddAsync(participantCreatorUser);
        await _applicationContext.ParticipantsInConversation.AddAsync(participantUser);

        return conversation;
    }

    public async Task<Conversation> DeleteConversationAsync(Guid conversationId)
    {
        Conversation conversation = await GetConversationByIdAsync(conversationId);

        if (conversation == null)
            return null;

        Conversation deletedConversation = _applicationContext.Remove(conversation).Entity;

        return deletedConversation;
    }

    public async Task<Conversation> GetConversationByIdAsync(Guid conversationId)
    {
        Conversation conversation = await _applicationContext.Conversations
            .Include(x => x.Group)
            .Include(x=>x.Messages)
            .Include(x => x.ParticipantsInConversation)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == conversationId);

        return conversation;
    }

    public async Task<IPagedEntities<Conversation>> GetConversationsByUserIdAsync(Guid userId, int page, int pageSize)
    {
        IQueryable<Conversation> conversations = _applicationContext.Conversations
             .Where(x => x.ParticipantsInConversation.Any(x => x.User.Id == userId))
             .Include(x => x.Group)
             .Include(x => x.Messages)
                 .ThenInclude(x => x.Sender)
             .Include(x => x.ParticipantsInConversation)
                .ThenInclude(x => x.User);

        IPagedEntities<Conversation> pagedConversations = await conversations.WithPagingAsync(page, pageSize);

        return pagedConversations;
    }

    public async Task<Conversation> GetPrivateConversationWithUserAsync(Guid creatorUserId, Guid userId)
    {
        return await _applicationContext.Conversations
                       .FirstOrDefaultAsync(x => x.ParticipantsInConversation.Any(x => x.User.Id == creatorUserId) &&
                       x.ParticipantsInConversation.Any(x => x.User.Id == userId) && x.Group == null);
    }

    public async Task<Conversation> GetGroupConversationByIdAsync(Guid conversationId)
    {
        Conversation conversation = await _applicationContext.Conversations
            .Include(x => x.Group)
            .Include(x => x.ParticipantsInConversation)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == conversationId && x.Group != null);

        return conversation;
    }
}
