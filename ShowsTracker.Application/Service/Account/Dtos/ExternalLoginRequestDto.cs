using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Account.Dtos
{
    public class ExternalLoginRequestDto
    {
        public string Email { get; set; }

        public string Provider { get; set; }
    }
}
