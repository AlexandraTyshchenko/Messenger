using Messenger.Infrastructure.Entities;

namespace Messenger.Infrastructure.Interfaces
{
    public interface IContactsRepository
    {
        Task<IEnumerable<UserContact>> GetContactsByOwnerIdAsync(Guid ownerId); 
        Task<UserContact> AddUserToContacts(Guid ownerId, Guid contactId);
        Task DeleteContactAsync(Guid userContactId);
    }
}
