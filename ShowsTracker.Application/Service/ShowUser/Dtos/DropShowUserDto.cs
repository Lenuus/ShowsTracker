using ShowsTracker.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.ShowUser.Dtos
{
    public class DropShowUserDto
    {
        public Guid ShowId { get; set; }
    }
}
