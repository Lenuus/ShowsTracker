using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Account.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }
    }
}
