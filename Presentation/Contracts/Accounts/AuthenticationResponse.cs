﻿namespace Contracts.Accounts;

public class AuthenticationResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = default!;
    public bool IsVerified { get; set; }
    public string JWToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
