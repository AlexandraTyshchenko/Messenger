namespace Messenger.Business.Dtos
{
    public class SearchUsersParams
    {
        public string UserName { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
