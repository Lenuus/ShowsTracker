using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Common.Helpers
{
    public interface IClaimManager
    {
        IEnumerable<Claim> GetClaims();

        string GetEmail();

        Guid GetUserId();

        string GetRole();
    }
}
