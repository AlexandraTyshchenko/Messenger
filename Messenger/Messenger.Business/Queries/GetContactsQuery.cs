using AutoMapper;
using MediatR;
using Messenger.Business.Dtos;
using Messenger.Infrastructure.Entities;
using Messenger.Infrastructure.Interfaces;

namespace Messenger.Business.Queries
{
    public class GetContactsQuery : IRequest<IEnumerable<UserContactDto>>
    {
        public Guid OwnerId { get; set; }
    }

    public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, IEnumerable<UserContactDto>>
    {
        private readonly IContactsRepository _contactsRepository;
        private readonly IMapper _mapper;
        public GetContactsQueryHandler(IContactsRepository contactsRepository,
            IMapper mapper)
        {
            _contactsRepository = contactsRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserContactDto>> Handle(GetContactsQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<UserContact> contacts = await _contactsRepository.GetContactsByOwnerIdAsync(request.OwnerId);

            return _mapper.Map<IEnumerable<UserContactDto>>(contacts);
        }
    }
}
