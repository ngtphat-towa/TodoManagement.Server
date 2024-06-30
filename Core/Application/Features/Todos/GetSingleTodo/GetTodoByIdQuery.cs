using Application.Exceptions;
using Application.Interfaces.Repositories;

using Domain.Entity;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.GetSingleTodo;

public record GetTodoByIdQuery : IRequest<Response<Todo>>
{
    public int Id { get; set; }
}
public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, Response<Todo>>
{
    private readonly ITodoRepository _todoRepository;
    public GetTodoByIdQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }
    public async Task<Response<Todo>> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(query.Id);
        if (todo == null) throw new ApiException($"Todo not found.");
        return new Response<Todo>(todo);
    }
}