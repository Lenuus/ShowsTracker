using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Voting.Dtos
{
    public class CreateVotingSeasonRequestDto
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<CreateVotingSeasonRequestShowDto> Shows { get; set; }
    }
}
