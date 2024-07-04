using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Interfaces;
using Messenger.Infrastructure.Repositories;
using Messenger.Infrastructure.Repositories.Repositories;

namespace Messenger.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _applicationContext;

    private IConversationRepository _conversationRepository;
    private IMessageRepository _messageRepository;
    private IParticipantRepository _participantRepository;
    private IUserRepository _userRepository;

    public UnitOfWork(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public IConversationRepository Conversations
    {
        get
        {
            if (_conversationRepository == null)
            {
                _conversationRepository = new ConversationRepository(_applicationContext);
            }
            return _conversationRepository;
        }
    }

    public IMessageRepository Messages
    {
        get
        {
            if (_messageRepository == null)
            {
                _messageRepository = new MessageRepository(_applicationContext);
            }
            return _messageRepository;
        }
    }

    public IParticipantRepository Participants
    {
        get
        {
            if (_participantRepository == null)
            {
                _participantRepository = new ParticipantRepository(_applicationContext);
            }
            return _participantRepository;
        }
    }

    public IUserRepository Users
    {
        get
        {
            if (_userRepository == null)
            {
                _userRepository = new UserRepository(_applicationContext);
            }
            return _userRepository;
        }
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
