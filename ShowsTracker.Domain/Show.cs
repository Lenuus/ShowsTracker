using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class Show : IBaseEntity, ISoftDeletable
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public int TotalEpisode { get; set; }

        [Required]
        public Status Status { get; set; }

        public string Description { get; set; }

        [Required]
        public bool ApproveStatus { get; set; }

        public string CoverImageUrl { get; set; }

        public string BannerImageUrl { get; set; }

        public double? Rating { get; set; }

        public int? Rank { get; set; }

        public int? Popularity { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public StartSeason StartSeason { get; set; }

        public int? ReleaseGap { get; set; }

        public ReleaseType? ReleaseType { get; set; }

        public int? MyAnimeListId { get; set; }

        public ICollection<ShowLink> Links { get; set; }

        public ICollection<ShowUser> Users { get; set; }

        public ICollection<ShowGenre> Genres { get; set; }

        public ICollection<VoteShow> Votings { get; set; }

        public ICollection<UserVote> UserVotings { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public Guid InsertedUserId { get; set; }

        public User InsertedUser { get; set; }
    }
}
