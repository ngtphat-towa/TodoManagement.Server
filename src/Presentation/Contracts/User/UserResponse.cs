namespace Contracts.User;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    /// <summary>
    ///   SuperAdmin = 1, Admin = 2, Moderator = 3, Basic = 4
    /// </summary>
    public short Role { get; set; }
    public List<string>? Permissions { get; set; }
}
