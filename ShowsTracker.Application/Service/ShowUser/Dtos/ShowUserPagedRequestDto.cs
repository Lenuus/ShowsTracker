using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Enum;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
	public class ShowUserPagedRequestDto : PagedRequestDto
	{
		public string Search { get; set; }

		public List<Guid> Genres { get; set; } = new List<Guid>();

		public List<Status> Statuses { get; set; } = new List<Status>();

		public List<Category> Categories { get; set; } = new List<Category>();

		public double? StartRating { get; set; }

		public double? EndRating { get; set; }

        public List<WatchStatus> TrackStatuses { get; set; } = new List<WatchStatus>();
    }
}
