using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Account.Dtos
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password and password confirmation should be same")]
        public string PasswordConfirmation { get; set; }
    }
}
