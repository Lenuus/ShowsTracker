using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Genre.Dtos
{
    public class GetAllGenresRequestDto : PagedRequestDto
    {
        public string Search { get; set; }
    }
}
