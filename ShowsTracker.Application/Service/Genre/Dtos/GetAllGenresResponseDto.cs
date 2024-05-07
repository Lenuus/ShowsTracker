using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Genre.Dtos
{
    public class GetAllGenresResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
