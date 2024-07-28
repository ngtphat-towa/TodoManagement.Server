﻿using Application.Interfaces.Repositories;

using Domain.Entities;

using MapsterMapper;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.GetAllTodos;

public class GetAllTodosQuery : IRequest<PagedResponse<IEnumerable<Todo>>>
{
    public PaginationFilter? Pagination { get; set; }
    public DataFilter? Filter { get; set; }
    public DataSort? Sort { get; set; }

}

public class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, PagedResponse<IEnumerable<Todo>>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;

    public GetAllTodosQueryHandler(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<IEnumerable<Todo>>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        // Apply pagination, filtering, and sorting
        var pagedTodos = await _todoRepository.GetPagedResponseAsync(
           request.Pagination,
            request.Filter,
            request.Sort);

        // Map the resulting todos to a view model if needed (though in this case it's directly returning Todo entities)
        var todoViewModel = _mapper.Map<IEnumerable<Todo>>(pagedTodos.Data ?? new List<Todo>());

        return new PagedResponse<IEnumerable<Todo>>(
            todoViewModel,
            pagedTodos.PageNumber,
            pagedTodos.PageSize,
            pagedTodos.TotalPages,
            pagedTodos.TotalRecords);
    }
}
