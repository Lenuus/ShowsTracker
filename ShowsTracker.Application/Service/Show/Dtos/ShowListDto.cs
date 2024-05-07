using ShowsTracker.Common.Enum;
using ShowsTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Show.Dtos
{
    public class ShowListDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Category Category { get; set; }

        public Status Status { get; set; }

        public int TotalEpisode { get; set; }

        public string CoverImageUrl { get; set; }

        public double? Rating { get; set; }

        public int? ReleaseGap { get; set; }

        public ReleaseType? ReleaseType { get; set; }

        public IEnumerable<ShowListLinkDto> Links { get; set; }

		public bool IsFollowedByCurrentUser { get; set; }

		public int CurrentEpisode { get; set; }

        public List<ShowListGenreDto> Genres { get; set; }
    }
}
