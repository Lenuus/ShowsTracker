using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Voting.Dtos
{
    public class VoteForShowRequestDto
    {
        public Guid VotingSeasonId { get; set; }

        public Guid ShowId { get; set; }
    }
}
