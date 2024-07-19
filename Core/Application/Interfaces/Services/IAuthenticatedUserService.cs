using System.Security.Claims;

namespace Application.Interfaces.Services;

public interface IAuthenticatedUserService
{
    string UserId { get; } 
    string Username { get; }
    Task<IEnumerable<string>> Permissions(string? userId = null);
    Task<IEnumerable<string>> Roles(string? userId = null);
    IEnumerable<Claim> Claims { get; }
}