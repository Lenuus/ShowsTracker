using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Domain
{
    public class UserLogin: IdentityUserLogin<Guid>
    {
    }
}
