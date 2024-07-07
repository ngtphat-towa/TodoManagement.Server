using System.Security.Claims;

using Application.Interfaces.Services;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace Identity.Services;

public class AuthenticatedUserService : IAuthenticatedUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }

    public string Username
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? string.Empty;
        }
    }

    public IEnumerable<string> Roles
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
        }
    }

    public IEnumerable<Claim> Claims
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Claims ?? [];
        }
    }

    public IEnumerable<string> Permissions
    {
        get
        {
            var permissionList = Enumerable.Empty<string>();
            var permissionsClaim = _httpContextAccessor.HttpContext?.User.FindFirst("permissions")?.Value;
            if (permissionsClaim != null)
            {
                permissionList = JsonConvert.DeserializeObject<List<string>>(permissionsClaim) ?? [];
            }
            return permissionList;
        }
    }

}
