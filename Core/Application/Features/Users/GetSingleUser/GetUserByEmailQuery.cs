using Application.Interfaces.Repositories;

using Domain.Entities;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.GetSingleUser;

public record GetUserByEmailQuery : IRequest<Response<User>>
{
    public string Email { get; set; } = string.Empty;
}
public class GetUserByTitleQueryHandler : IRequestHandler<GetUserByEmailQuery, Response<User>>
{
    private readonly IUserService _userService;
    public GetUserByTitleQueryHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<Response<User>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByEmailAsync(query.Email);
        if (user == null) throw new KeyNotFoundException($"User with {query.Email} not found.");
        return Response<User>.Success(user);
    }
}