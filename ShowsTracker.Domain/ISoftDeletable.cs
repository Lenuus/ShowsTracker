﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public interface  ISoftDeletable
    {
        public bool IsDeleted { get; set; }
    }
}
