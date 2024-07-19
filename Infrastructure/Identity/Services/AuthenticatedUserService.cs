using System.Security.Claims;

using Application.Interfaces.Services;

using Domain.Entities;

using Microsoft.AspNetCore.Http;

namespace Identity.Services;

public class AuthenticatedUserService : IAuthenticatedUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccountService _accountService;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor, IAccountService accountService)
    {
        _httpContextAccessor = httpContextAccessor;
        _accountService = accountService;
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

    public async Task<IEnumerable<string>> Roles(string? userId)
    {
        var result = await _accountService.GetUserRolesAsync(userId ?? this.UserId!);
        return result.Data!;

    }

    public IEnumerable<Claim> Claims
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Claims ?? [];
        }
    }

    public async Task<IEnumerable<string>> Permissions(string? userId)
    {
        var result = await _accountService.GetUserPermissionsAsync(userId ?? this.UserId!);
        return result.Data!;

    }


}
