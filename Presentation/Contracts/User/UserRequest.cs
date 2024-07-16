namespace Contracts.User
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        /// <summary>
        ///   SuperAdmin = 1, Admin = 2, Moderator = 3, Basic = 4
        /// </summary>
        public short Role { get; set; }
        public string? Permissions { get; set; }
    }
    public class UpdateUserInfoRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
 
    }
}
