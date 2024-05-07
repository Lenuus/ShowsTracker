using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class ShowLink : IBaseEntity, ISoftDeletable
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        [Required]
        [MaxLength(2048)]
        public string Link { get; set; }

        [Required]
        public Guid ShowId { get; set; }

        public Show Show { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public bool IsDefaultLink { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
