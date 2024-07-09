namespace Messenger.Business.Dtos
{
    public class SearchUsersParams
    {
        public string UserName { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
