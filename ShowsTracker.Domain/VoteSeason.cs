using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class VoteSeason : IBaseEntity, ISoftDeletable
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        
        [Required]
        public bool IsFinished { get; set; }

        public ICollection<VoteShow> Shows { get; set; }

        public ICollection<UserVote> UserVotes { get; set; }
    }
}
