using AutoMapper;
using Messenger.Infrastructure.Pagination;

namespace Messenger.Business.Profiles;

public static class AutoMapperExtensions
{
    public static IPagedEntities<TDestination> MapPagedEntities<TSource, TDestination>(
       this IMapper mapper, IPagedEntities<TSource> source)
    {
        var mappedItems = source.Entities.Select(item => mapper.Map<TDestination>(item)).ToList();

        return new PagedEntities<TDestination>(source.Page,source.PageSize, source.TotalSize, mappedItems);
    }
}
