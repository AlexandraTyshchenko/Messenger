using Messenger.Infrastructure.Interfaces;

namespace Messenger.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    IConversationRepository Conversations { get; }
    IMessageRepository Messages { get; }
    IParticipantRepository Participants { get; }
    IUserRepository Users { get; }
    IConnectionRepository Connections { get; }

    Task<int> SaveChangesAsync();
}
