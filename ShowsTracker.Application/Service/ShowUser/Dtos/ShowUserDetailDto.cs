using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
    public class ShowUserDetailDto
    {
        public Guid Id { get; set; }
        public Guid ShowId { get; set; }
        public Guid UserId { get; set; }
        public int CurrentEpisode { get; set; }
        public DateTime UpdateDate { get; set; }
        public int TotalEpisode { get; set; }
    }
}
