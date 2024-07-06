namespace Contracts.Accounts;

public record AuthenticationRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
