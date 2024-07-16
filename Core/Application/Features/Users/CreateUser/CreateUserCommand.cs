using Application.Interfaces.Repositories;

using Domain.Entities;

using Mapster;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.CreateUser;

public class CreateUserCommand : IRequest<Response<string>>
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
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Response<string>>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Response<string>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var newUser = command.Adapt<User>();
        var addedUser = await _userService.AddAsync(newUser);
        return Response<string>.Success(addedUser.Id);

    }
}