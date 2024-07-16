using Application.Features.Todos.CreateTodo;
using Application.Features.Todos.DeleteTodo;
using Application.Features.Todos.GetAllTodos;
using Application.Features.Todos.GetSingleTodo;
using Application.Features.Todos.UpdateTodo;
using Application.Features.Todos.UpdateTodoStatus;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Contracts.Todo;
using Shared.Wrappers;

namespace WebApi.Controllers
{
    public class TodosController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TodosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
        {
            var command = request.Adapt<CreateTodoCommand>();
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteTodoRequest request)
        {
            var command = request.Adapt<DeleteTodoCommand>();
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter request)
        {
            var query = request.Adapt<GetAllTodosQuery>();
            var response = await _mediator.Send(query);
            var userResponses = response.Data.Adapt<IEnumerable<TodoResponse>>();

            return Ok(new PagedResponse<IEnumerable<TodoResponse>>(
                userResponses,
                response.PageNumber,
                response.PageSize,
                response.TotalPages,
                response.TotalRecords));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetTodoByIdQuery { Id = id };
            var response = await _mediator.Send(query);

            var todoResponse = response.Data.Adapt<TodoResponse>();

            return Ok(Response<TodoResponse>.Success(todoResponse));
        }

        [HttpPost("{title}")]
        public async Task<IActionResult> GetByTitle(string title)
        {

            var query = new GetSingleTitleQuery { Title = title };
            var response = await _mediator.Send(query);

            var todoResponse = response.Data.Adapt<TodoResponse>();

            return Ok(Response<TodoResponse>.Success(todoResponse));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoRequest request)
        {
            var command = request.Adapt<UpdateTodoCommand>();
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTodoStatusRequest request)
        {
            var command = request.Adapt<UpdateTodoStatusCommand>();
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

    }
}
