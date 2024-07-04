namespace Messenger.Infrastructure.Pagination;

public interface IPagedEntities<out T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalSize { get; set; }
    public IEnumerable<T> Entities { get; }
}

