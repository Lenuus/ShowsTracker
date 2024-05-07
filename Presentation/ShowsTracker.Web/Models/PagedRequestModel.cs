namespace ShowsTracker.Web.Models
{
    public class PagedRequestModel
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = int.MaxValue;
    }
}
