using Contracts.Accounts;

using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<RefreshToken>? RefreshTokens { get; set; }
        public string Permissions { get; set; } = "[]";
    }
}
