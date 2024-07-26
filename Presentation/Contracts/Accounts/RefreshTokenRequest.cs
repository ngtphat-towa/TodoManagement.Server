namespace Contracts.Accounts
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class RevokeTokenRequest : RefreshTokenRequest { }
}
