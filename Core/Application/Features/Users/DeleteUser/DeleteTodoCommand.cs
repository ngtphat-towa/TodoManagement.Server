using Application.Exceptions;
using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.DeleteUser;

public record DeleteUserCommand : IRequest<Response<string>>
{
    public string Id { get; set; }
}
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Response<string>>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var exitingUser = await _userService.GetByIdAsync(request.Id);
        if (exitingUser is null)
        {
            throw new ApiException($"Product Not Found.");
        }

        await _userService.DeleteAsync(exitingUser);

        return Response<string>.Success(exitingUser.Id);
    }
}
