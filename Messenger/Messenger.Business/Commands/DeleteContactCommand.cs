using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Commands
{
    public class DeleteContactCommand : IRequest
    {
        public Guid UserContactId { get; set; }
    }

    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand>
    {
        private readonly IContactsRepository _contactsRepository;

        public DeleteContactCommandHandler(IContactsRepository contactsRepository)
        {
            _contactsRepository = contactsRepository;
        }

        public async Task<Unit> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            await _contactsRepository.DeleteContactAsync(request.UserContactId);

            return Unit.Value;//todo спитати
        }
    }
}

