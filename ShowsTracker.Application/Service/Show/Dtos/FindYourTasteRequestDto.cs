using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Show.Dtos
{
    public class FindYourTasteRequestDto
    {
        public Category Category { get; set; }

        public List<Guid> Genres { get; set; } = new List<Guid>();
    }
}
