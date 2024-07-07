using Application.Interfaces.Repositories;

using Domain.Entities;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.CreateUser;

public class CreateUserCommand : IRequest<Response<string>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
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

    public async Task<Response<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Username,
            Password = request.Password,
            Email = request.Email,
            Role = request.Role,
            Permissions = request.Permissions
        };

        var addedUser = await _userService.AddAsync(newUser);
        return Response<string>.Success(addedUser.Id);

    }
}