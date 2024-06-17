using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Repositories
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly ApplicationContext _applicationContext;

        public ContactsRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<UserContact> AddUserToContacts(Guid ownerId, Guid contactId)//todo подумати, чи варто взагалі робити active user оскільки це перевірка скрізь, що не є раціональним підходом
        {
            var existingContact = await _applicationContext.UserContacts.FirstOrDefaultAsync(x => x.ContactOwner.Id == ownerId && x.Contact.Id == contactId);

            if (existingContact != null) {
                throw new ArgumentException("Contact already exists");
            }
            var contact = await _applicationContext.Users.FirstOrDefaultAsync(x => x.Id == contactId);
            if (contact == null)
            {
                throw new ArgumentException("Contact not found");
            }

            var contactOwner = await _applicationContext.Users.FirstOrDefaultAsync(x => x.Id == ownerId);
            if (contactOwner == null)
            {
                throw new ArgumentException("Contact owner not found");
            }

            var userContact = new UserContact
            {
                Contact = contact,
                ContactOwner = contactOwner,
            };

            _applicationContext.UserContacts.Add(userContact);

            await _applicationContext.SaveChangesAsync();

            return userContact;
        }

        public async Task DeleteContactAsync(Guid userContactId)
        {
            var userContact = await _applicationContext.UserContacts.FirstOrDefaultAsync(x => x.Id == userContactId);

            if (userContact == null)
            {
                throw new ArgumentException("Contact not found");
            }
            _applicationContext.Remove(userContact);

            await _applicationContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserContact>> GetContactsByOwnerIdAsync(Guid ownerId)
        {
            var contacts = await _applicationContext.UserContacts
                .Where(x => x.ContactOwner.Id == ownerId)
                .Include(x => x.ContactOwner)
                .Include(x => x.Contact)
                .ToListAsync();

            return contacts;
        }
    }
}
