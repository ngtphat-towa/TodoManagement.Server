using Application.Features.Users.CreateUser;
using Application.Features.Users.DeleteUser;
using Application.Features.Users.GetAllUsers;
using Application.Features.Users.GetSingleUser;
using Application.Features.Users.UpdateUser.Info;

using Contracts.User;

using Mapster;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Shared.Wrappers;

namespace WebApi.Controllers;

public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = request.Adapt<CreateUserCommand>();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserInfoRequest request)
    {
        var command = request.Adapt<UpdateUserInfoCommand>();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var command = new DeleteUserCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var query = new GetUserByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        var response = result.Data.Adapt<UserResponse>();

        return Ok(Response<UserResponse>.Success(response)); ;
    }
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var query = new GetUserByEmailQuery { Email = email };
        var result = await _mediator.Send(query);
        var response = result.Data.Adapt<UserResponse>();

        return Ok(Response<UserResponse>.Success(response));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationFilter pagination)
    {
        var query = pagination.Adapt<GetAllUsersQuery>();
        var result = await _mediator.Send(query);
        var responses = result.Data.Adapt<IEnumerable<UserResponse>>();

        return Ok(new PagedResponse<IEnumerable<UserResponse>>(
            responses,
            result.PageNumber,
            result.PageSize,
            result.TotalPages,
            result.TotalRecords));
    }
}
