using Application.Interfaces.Repositories;

using Domain.Entities;

using MapsterMapper;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.GetAllTodos;

public class GetAllTodosQuery : PaginationFilter, IRequest<PagedResponse<IEnumerable<Todo>>>
{
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
        var validFilter = _mapper.Map<PaginationFilter>(request);
        var todo = await _todoRepository.GetPagedResponseAsync(validFilter.PageNumber, validFilter.PageSize);
        var record = await _todoRepository.GetCountTotalPagedResponseAsync(validFilter.PageNumber, validFilter.PageSize);
        var todoViewModel = _mapper.Map<IEnumerable<Todo>>(todo);
        return new PagedResponse<IEnumerable<Todo>>(todoViewModel, validFilter.PageNumber, validFilter.PageSize, record.TotalPages, record.TotalRecords);
    }
}