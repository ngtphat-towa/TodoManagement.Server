﻿namespace Contracts.Accounts
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset Expires { get; set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
        public DateTimeOffset Created { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public DateTimeOffset? Revoked { get; set; }
        public string RevokedByIp { get; set; } = string.Empty;
        public string ReplacedByToken { get; set; } = string.Empty;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
