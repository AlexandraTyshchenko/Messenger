namespace Messenger.Infrastructure.Pagination;
public interface IPagedEntities<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalSize { get; set; }
    public List<T> Entities { get; }
}
