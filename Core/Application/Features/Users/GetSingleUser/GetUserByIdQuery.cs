using Application.Exceptions;
using Application.Interfaces.Repositories;

using Domain.Entities;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.GetSingleUser;

public class GetUserByIdQuery : IRequest<Response<User>>
{
    public string Id { get; set; } = string.Empty;
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<User>>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Response<User>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        User? user = null;

        if (!string.IsNullOrEmpty(query.Id))
        {
            user = await _userService.GetByIdAsync(query.Id);
        }

        if (user == null)
        {
            throw new ApiException($"User not found.");
        }

        return Response<User>.Success(user);
    }
}
