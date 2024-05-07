using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
    public class UpdateLinkRequestDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Link { get; set; }
    }
}
