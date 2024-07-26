using System.ComponentModel.DataAnnotations;

namespace Contracts.Accounts;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
