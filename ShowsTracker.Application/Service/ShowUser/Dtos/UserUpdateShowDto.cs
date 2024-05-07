using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
    public class UserUpdateShowDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public int CurrentEpisode { get; set; }
    }
}
