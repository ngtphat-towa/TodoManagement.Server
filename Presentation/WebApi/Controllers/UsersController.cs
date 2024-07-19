using Application.Features.Users.CreateUser;
using Application.Features.Users.DeleteUser;
using Application.Features.Users.GetAllUsers;
using Application.Features.Users.GetSingleUser;
using Application.Features.Users.UpdateUser.Info;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Contracts.User;
using Shared.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    /// <summary>
    /// API endpoints for managing user operations.
    /// </summary>
    [Authorize]

    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The user data to create.</param>
        /// <returns>The created user.</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var command = request.Adapt<CreateUserCommand>();
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="request">The updated user information.</param>
        /// <returns>The updated user.</returns>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(Response<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserInfoRequest request)
        {
            var command = request.Adapt<UpdateUserInfoCommand>();
            command.Id = userId;
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A success message.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Response<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result.Data == null)
            {
                return NotFound();
            }
            var response = result.Data.Adapt<UserResponse>();
            return Ok(Response<UserResponse>.Success(response));
        }

        /// <summary>
        /// Gets a user by email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user.</returns>
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(Response<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var query = new GetUserByEmailQuery { Email = email };
            var result = await _mediator.Send(query);
            if (result.Data == null)
            {
                return NotFound();
            }
            var response = result.Data.Adapt<UserResponse>();
            return Ok(Response<UserResponse>.Success(response));
        }

        /// <summary>
        /// Gets all users with pagination.
        /// </summary>
        /// <param name="pagination">Pagination parameters.</param>
        /// <returns>A paged list of users.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationFilter pagination)
        {
            var query = pagination.Adapt<GetAllUsersQuery>();
            var result = await _mediator.Send(query);
            var responses = result.Data.Adapt<IEnumerable<UserResponse>>();

            var pagedResponse = new PagedResponse<IEnumerable<UserResponse>>(
                responses,
                result.PageNumber,
                result.PageSize,
                result.TotalPages,
                result.TotalRecords);

            return Ok(pagedResponse);
        }
    }
}
