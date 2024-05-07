using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class ShowUser : IBaseEntity, ISoftDeletable
    {
        public Guid Id { get; set; }

        public Show Show { get; set; }

        [Required]
        public Guid ShowId { get; set; }

        public User User { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int CurrentEpisode { get; set; }

        public DateTime UpdateDate { get; set; }

        public WatchStatus Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}
