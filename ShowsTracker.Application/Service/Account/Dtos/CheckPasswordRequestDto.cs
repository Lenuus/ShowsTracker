using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Account.Dtos
{
    public class CheckPasswordRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
