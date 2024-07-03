using Microsoft.EntityFrameworkCore;

namespace Messenger.Infrastructure.Pagination;

public static class Pagination
{
    public async static Task<IPagedEntities<T>> WithPagingAsync<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var totalEntities = await query.CountAsync();

        IEnumerable<T> entities = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(); 

        return new PagedEntities<T>(page, pageSize, totalEntities, entities);
    }
}
