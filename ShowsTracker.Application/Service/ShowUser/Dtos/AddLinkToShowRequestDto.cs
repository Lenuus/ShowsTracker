using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
    public class AddLinkToShowRequestDto
    {
        [Required]
        public Guid ShowId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Link { get; set; }
    }
}
