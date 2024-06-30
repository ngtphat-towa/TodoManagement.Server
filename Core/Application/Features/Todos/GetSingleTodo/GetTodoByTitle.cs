using Application.Exceptions;
using Application.Interfaces.Repositories;

using Domain.Entity;
using MediatR;
using Shared.Wrappers;

namespace Application.Features.Todos.GetSingleTodo;

public record GetTodoByTitleQuery : IRequest<Response<Todo>>
{
    public string Title { get; set; } = string.Empty;
}
public class GetTodoByTitleQueryHandler : IRequestHandler<GetTodoByTitleQuery, Response<Todo>>
{
    private readonly ITodoRepository _todoRepository;
    public GetTodoByTitleQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }
    public async Task<Response<Todo>> Handle(GetTodoByTitleQuery query, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByTitleAsync(query.Title);
        if (todo == null) throw new ApiException($"Todo with {query.Title} not found.");
        return new Response<Todo>(todo);
    }
}