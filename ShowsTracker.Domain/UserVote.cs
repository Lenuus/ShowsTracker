using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class UserVote : IBaseEntity, ISoftDeletable
    {
        public Guid Id { get; set; }

        [Required]
        public Guid VoteSeasonId { get; set; }

        public VoteSeason VoteSeason { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [Required]
        public Guid ShowId { get; set; }

        public Show Show { get; set; }

        [Required]
        public DateTime VoteDateTime { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
