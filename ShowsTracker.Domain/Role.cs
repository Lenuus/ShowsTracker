﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class Role : IdentityRole<Guid>, IBaseEntity
    {
        public ICollection<UserRole> Users { get; set; }
    }
}
