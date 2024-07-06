﻿using Application.Exceptions;
using Application.Interfaces.Repositories;

using Domain.Entity;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.GetSingleTodo;

public class GetSingleByIdQuery : IRequest<Response<Todo>>
{
    public int Id { get; set; }
}

public class GetTodoByIdQueryHandler : IRequestHandler<GetSingleByIdQuery, Response<Todo>>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoByIdQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Response<Todo>> Handle(GetSingleByIdQuery query, CancellationToken cancellationToken)
    {
        Todo? todo = null;

        // First, try to retrieve the Todo by Id
        if (query.Id != 0)
        {
            todo = await _todoRepository.GetByIdAsync(query.Id);
        }

        if (todo == null)
        {
            throw new ApiException($"Todo not found.");
        }

        return new Response<Todo>(todo);
    }
}