using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
    public class AddShowUserDto
    {
        public Guid ShowId { get; set; }
    }
}
