using ShowsTracker.Application.Service.Auth.Dtos;
using System.Security.Claims;

namespace ShowsTracker.Application.Service.Auth
{
    public interface IAuthService
    {
        ServiceResponse<TokenInfo> GenerateToken(List<Claim> claims);
    }
}
