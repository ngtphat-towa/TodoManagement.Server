using Application.Features.Users.CreateUser;
using Application.Features.Users.DeleteUser;
using Application.Features.Users.GetAllUsers;
using Application.Features.Users.GetSingleUser;
using Application.Features.Users.UpdateUser;

using Contracts.User;

using Mapster;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Shared.Wrappers;

namespace WebApi.Controllers;

public class UsersController :BaseApiController
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

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var command = request.Adapt<UpdateUserCommand>();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
    {
        var command = request.Adapt<DeleteUserCommand>();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetUserById([FromQuery] GetUserByIdRequest request)
    {
        var query = request.Adapt<GetUserByIdQuery>();
        var result = await _mediator.Send(query);
        var response = result.Data.Adapt<UserResponse>();

        return Ok(Response<UserResponse>.Success(response)); ;
    }
    [HttpGet("get-by-email")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] GetUserByEmailRequest request)
    {
        var query = request.Adapt<GetUserByEmailQuery>();
        var result = await _mediator.Send(query);
        var response = result.Data.Adapt<UserResponse>();

        return Ok(Response<UserResponse>.Success(response));
    }

    [HttpGet("all")]
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
