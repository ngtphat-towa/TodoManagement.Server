using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.UpdateUser.Permission;

public class UpdateUserPermissionCommand : IRequest<Response<Unit>>
{
    public string Id { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}
public class UpdateUserPermissionCommandHandler : IRequestHandler<UpdateUserPermissionCommand, Response<Unit>>
{
    private readonly IUserService _userService;

    public UpdateUserPermissionCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Response<Unit>> Handle(UpdateUserPermissionCommand command, CancellationToken cancellationToken)
    {
        // Retrieve the user from the repository
        var user = await _userService.GetByIdAsync(command.Id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {command.Id} not found.");
        }

        // Update user permissions
        user.Permissions = string.Join(",", command.Permissions);

        // Save changes
        await _userService.UpdateAsync(user);

        return Response<Unit>.Success(Unit.Value);
    }
}
