using System.Security.Claims;

using Application.Interfaces.Services;

namespace WebApi.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid") ?? "unknown";
        }

        public string UserId { get; }
    }
}
