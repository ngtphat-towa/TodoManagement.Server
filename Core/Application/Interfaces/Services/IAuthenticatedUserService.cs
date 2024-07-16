using System.Security.Claims;

namespace Application.Interfaces.Services;

public interface IAuthenticatedUserService
{
    string UserId { get; } 
    string Username { get; }
    IEnumerable<string> Permissions { get; }
    IEnumerable<string> Roles { get; } 
    IEnumerable<Claim> Claims { get; }
}