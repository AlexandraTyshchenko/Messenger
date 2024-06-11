using MediatR;
using Messenger.Infrastructure.Context;
using Messenger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Business
{
    public class GetConversationByUserId
    {

            public class Query : IRequest<List<Conversation>>
            {
                public int UserId { get; set; }
            }

            public class QueryHandler : IRequestHandler<Query, List<Conversation>>
            {
                private readonly ApplicationContext _applicationContext;

                public QueryHandler(ApplicationContext applicationContext)
                {
                    _applicationContext = applicationContext;
                }

                public async Task<List<Conversation>> Handle(Query request, CancellationToken cancellationToken)
                {
                    var conversations = await _applicationContext.ParticipatsInConveration
                        .Include(x => x.Conversation)
                          .ThenInclude(x=>x.Group)
                        .Where(x => x.UserId == request.UserId)
                        .Select(x => x.Conversation)
                        .ToListAsync(cancellationToken);

                    return conversations;
                }
            }
        
    }

}
