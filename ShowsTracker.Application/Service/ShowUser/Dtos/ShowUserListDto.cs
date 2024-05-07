using ShowsTracker.Application.Service.Show.Dtos;
using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
    public class ShowUserListDto
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

		public int CurrentEpisode { get; set; }

		public DateTime LastUpdateDate { get; set; }

        public WatchStatus TrackStatus { get; set; }

        public Guid ShowId { get; set; }

        public List<ShowUserListGenreDto> Genres { get; set; }
    }
}
