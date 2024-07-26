using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.UpdateUser.Role
{
    public class UpdateUserRoleCommand : IRequest<Response<Unit>>
    {
        public string Id { get; set; } = string.Empty;
        /// <summary>
        ///   SuperAdmin = 1, Admin = 2, Moderator = 3, Basic = 4
        /// </summary>
        public short Role { get; set; }
    }
}
