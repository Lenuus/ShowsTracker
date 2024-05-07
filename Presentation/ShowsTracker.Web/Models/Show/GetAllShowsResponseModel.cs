using ShowsTracker.Common.Enum;

namespace ShowsTracker.Web.Models.Show
{
    public class GetAllShowsResponseModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Category Category { get; set; }

        public Status Status { get; set; }

        public int TotalEpisode { get; set; }
    }
}
