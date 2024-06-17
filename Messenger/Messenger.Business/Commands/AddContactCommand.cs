using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Commands
{
    public class AddContactCommand : IRequest<UserContactDto>
    {
        public Guid OwnerId { get; set; }
        public Guid ContactId { get; set; }
    }

    public class AddContactCommandHandler : IRequestHandler<AddContactCommand, UserContactDto>
    {
        private readonly IContactsRepository _contactsRepository;
        private readonly IMapper _mapper;

        public AddContactCommandHandler(IContactsRepository contactsRepository,
            IMapper mapper)
        {
            _contactsRepository = contactsRepository;
            _mapper = mapper;
        }
        public async Task<UserContactDto> Handle(AddContactCommand request, CancellationToken cancellationToken)
        {
            UserContact contact = await _contactsRepository.AddUserToContacts(request.OwnerId, request.ContactId);

            return _mapper.Map<UserContactDto>(contact);
        }
    }
}
