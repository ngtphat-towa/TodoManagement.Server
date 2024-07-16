namespace Domain.Enums;

/// <summary>
///   SuperAdmin = 1, Admin = 2, Moderator = 3, Basic = 4
/// </summary>
public enum Roles
{
    SuperAdmin = 1,
    Admin = 2,
    Moderator = 3,
    Basic = 4
}
public static class RoleHelper
{
    public static bool IsSuperAdmin(this Roles roles) => roles == Roles.SuperAdmin;
    /// <summary>
    /// Checks if the specified role is lower than the compared role or is the Basic role.
    /// </summary>
    /// <param name="role">The role to check.</param>
    /// <param name="compared">The role of the authenticated user or the role to compare against.</param>
    /// <returns>True if the role is lower than compared or is Basic; false otherwise.</returns>
    public static bool IsLowerOrBasic(this Roles role, Roles compared)
    {
        return role > compared || role == Roles.Basic;
    }
}