using ShowsTracker.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Voting.Dtos
{
    public class GetAllVotingSeasonsRequestDto : PagedRequestDto
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Name { get; set; }

        public bool? IsFinished { get; set; }
    }
}
