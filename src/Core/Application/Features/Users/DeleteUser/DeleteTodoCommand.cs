using Application.Exceptions;
using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.DeleteUser;

public record DeleteUserCommand : IRequest<Response<string>>
{
    public string Id { get; set; } = string.Empty;
}
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<string>>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Response<string>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var exitingUser = await _userService.GetByIdAsync(command.Id);
        if (exitingUser is null)
        {
            throw new ApiException($"Todo with ID '{command.Id}' not found.");
        }

        await _userService.DeleteAsync(exitingUser);

        return Response<string>.Success(exitingUser.Id);
    }
}
