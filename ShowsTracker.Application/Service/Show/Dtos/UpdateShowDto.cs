using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Show.Dtos
{
    public class UpdateShowDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public Status Status { get; set; }

        public int TotalEpisode { get; set; }
    }
}
