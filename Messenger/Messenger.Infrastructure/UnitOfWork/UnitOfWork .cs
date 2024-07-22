using Messenger.Infrastructure;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _applicationContext;

    public IConversationRepository Conversations { get; }
    public IMessageRepository Messages { get; }
    public IParticipantRepository Participants { get; }
    public IUserRepository Users { get; }
    public IConnectionRepository Connections { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public UnitOfWork(ApplicationContext applicationContext, IConversationRepository conversationRepository,
        IMessageRepository messageRepository, IParticipantRepository participantRepository, 
        IUserRepository userRepository, IConnectionRepository connections,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _applicationContext = applicationContext;
        Conversations = conversationRepository;
        Messages = messageRepository;
        Participants = participantRepository;
        Users = userRepository;
        Connections = connections;
        RefreshTokens = refreshTokenRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _applicationContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _applicationContext.Dispose();
    }
}