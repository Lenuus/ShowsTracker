using ShowsTracker.Common.Dtos;
using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Show.Dtos
{
    public class GetAllShowsDto : PagedRequestDto
    {
        public string Search { get; set; }

        public List<Guid> Genres { get; set; } = new List<Guid>();

		public List<Status> Statuses { get; set; } = new List<Status>();

		public List<Category> Categories { get; set; } = new List<Category>();

        public double? StartRating { get; set; }

        public double? EndRating { get; set; }
    }
}
