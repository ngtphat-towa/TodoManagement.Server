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
using Azure.Core;

namespace WebApi.Controllers
{
    public class TodoController : BaseApiController
    {
        public TodoController(IMediator mediator) : base(mediator) { }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
        {
            var command = request.Adapt<CreateTodoCommand>();
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] DeleteTodoRequest request)
        {
            var command = request.Adapt<DeleteTodoCommand>();
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter request)
        {
            var query = request.Adapt<GetAllTodosQuery>();
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] GetTodoByIdRequest request)
        {
            var query = request.Adapt<GetSingleByIdQuery>();
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("get-by-title")]
        public async Task<IActionResult> GetByTitle([FromBody] GetTodoByTitleRequest request)
        {

            var query = request.Adapt<GetSingleTitleQuery>();
            var response = await Mediator.Send(query);
            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateTodoRequest request)
        {
            var command = request.Adapt<UpdateTodoCommand>();
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTodoStatusRequest request)
        {
            var command = request.Adapt<UpdateTodoStatusCommand>();
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}
