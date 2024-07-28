using Application.Features.Todos.CreateTodo;
using Application.Features.Todos.DeleteTodo;
using Application.Features.Todos.GetAllTodos;
using Application.Features.Todos.GetSingleTodo;
using Application.Features.Todos.UpdateTodo;
using Application.Features.Todos.UpdateTodoStatus;
using Application.Features.Todos.GetByFilter;

using Contracts.Todo;

using Mapster;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Shared.Wrappers;

namespace WebApi.Controllers
{
    /// <summary>
    /// API endpoints for managing Todo items.
    /// </summary>
    [Authorize]
    public class TodosController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TodosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new Todo item.
        /// </summary>
        /// <param name="request">The Todo item to create.</param>
        /// <returns>The created Todo item.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
        {
            var command = request.Adapt<CreateTodoCommand>();
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Deletes a Todo item by ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item to delete.</param>
        /// <returns>A success message.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteTodoCommand { Id = id };
            var response = await _mediator.Send(command);
            if (response.Succeeded)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        /// <summary>
        /// Gets all Todo items with pagination.
        /// </summary>
        /// <param name="page">Page number (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <param name="filter">Filter criteria.</param>
        /// <param name="sort">Sort criteria.</param>
        /// <returns>A paged list of Todo items.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<TodoResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] PaginationFilter? paginationFilter,
            [FromQuery] DataFilter? filter = null,
            [FromQuery] DataSort? sort = null)
        {
            var query = new GetAllTodosQuery
            {
                Pagination = paginationFilter,
                Filter = filter,
                Sort = sort
            };
            var response = await _mediator.Send(query);
            var todoResponses = response.Data.Adapt<IEnumerable<TodoResponse>>();
            var pagedResponse = new PagedResponse<IEnumerable<TodoResponse>>(
                todoResponses,
                response.PageNumber,
                response.PageSize,
                response.TotalPages,
                response.TotalRecords);
            return Ok(pagedResponse);
        }

        /// <summary>
        /// Gets a Todo item by ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item.</param>
        /// <returns>The Todo item.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<TodoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetTodoByIdQuery { Id = id };
            var response = await _mediator.Send(query);
            if (response.Data == null)
            {
                return NotFound();
            }
            var todoResponse = response.Data.Adapt<TodoResponse>();
            return Ok(Response<TodoResponse>.Success(todoResponse));
        }

        /// <summary>
        /// Updates a Todo item by ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item to update.</param>
        /// <param name="request">The updated Todo item data.</param>
        /// <returns>The updated Todo item.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Response<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoRequest request)
        {
            var command = request.Adapt<UpdateTodoCommand>();
            command.Id = id;
            var response = await _mediator.Send(command);
            if (!response.Succeeded)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Updates the status of a Todo item by ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item.</param>
        /// <param name="request">The updated status.</param>
        /// <returns>The updated Todo item ID.</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(Response<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTodoStatusRequest request)
        {
            var command = request.Adapt<UpdateTodoStatusCommand>();
            command.Id = id;
            var response = await _mediator.Send(command);
            if (!response.Succeeded)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Gets Todo items by filter.
        /// </summary>
        /// <param name="filter">Filter criteria.</param>
        /// <returns>A list of Todo items that match the filter criteria.</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(Response<IEnumerable<TodoResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter([FromQuery] DataFilter filter)
        {
            var query = new GetByFilterQuery { Filter = filter };
            var response = await _mediator.Send(query);
            var todoResponses = response.Data.Adapt<IEnumerable<TodoResponse>>();
            return Ok(Response<IEnumerable<TodoResponse>>.Success(todoResponses));
        }
    }
}
