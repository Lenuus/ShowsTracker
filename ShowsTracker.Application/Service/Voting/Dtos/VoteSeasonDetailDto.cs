using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Voting.Dtos
{
    public class VoteSeasonDetailDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public long TotalVote { get; set; }

        public bool IsFinished { get; set; }

        public List<VoteSeasonDetailShowDto> Shows { get; set; }
    }
}
