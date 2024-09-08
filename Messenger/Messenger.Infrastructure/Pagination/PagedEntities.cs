namespace Messenger.Infrastructure.Pagination;

public class PagedEntities<T> : IPagedEntities<T>
{
    public PagedEntities()
    {
        
    }
    public PagedEntities(int page, int pageSize, int totalSize, List<T> entities)
    {
        Page = page;
        PageSize = pageSize;
        TotalSize = totalSize;
        Entities = entities;
    }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalSize { get; set; }
    public List<T> Entities { get; set; }
}
