namespace ShowsTracker.Web.Models
{
    public class PagedResponseModel<T> where T : class
    {
        public int CurrentPage { get; set; }

        public int TotalPage { get; set; }

        public int PageSize { get; set; }

        public List<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
