using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class User : IdentityUser<Guid>, IBaseEntity, ISoftDeletable
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public ICollection<ShowUser> Shows { get; set; }

        public ICollection<UserRole> Roles { get; set; }

        public ICollection<Show> CreatedShows { get; set; }

        public ICollection<ShowLink> Links { get; set; }

        public ICollection<UserVote> UserVotings { get; set; }

        public bool IsDeleted { get; set; }
    }
}
