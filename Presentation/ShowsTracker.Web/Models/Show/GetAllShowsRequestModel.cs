namespace ShowsTracker.Web.Models.Show
{
    public class GetAllShowsRequestModel : PagedRequestModel
    {
        public string Search { get; set; }
    }
}
