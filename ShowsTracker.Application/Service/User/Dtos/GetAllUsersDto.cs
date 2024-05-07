using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.User.Dtos
{
    public class GetAllUsersDto : PagedRequestDto
    {
        public string Search { get; set; }
    }
}
