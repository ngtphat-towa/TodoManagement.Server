using Application.Interfaces.Common;

using Domain.Entities;

using MapsterMapper;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.GetByFilter;

public class GetByFilterQuery : IRequest<Response<IEnumerable<Todo>>>
{
    public DataFilter? Filter { get; set; }
}

public class GetByFilterQueryHandler : IRequestHandler<GetByFilterQuery, Response<IEnumerable<Todo>>>
{
    private readonly IGenericRepositoryAsync<Todo> _todoRepository;
    private readonly IMapper _mapper;

    public GetByFilterQueryHandler(IGenericRepositoryAsync<Todo> todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<Response<IEnumerable<Todo>>> Handle(GetByFilterQuery request, CancellationToken cancellationToken)
    {
        var result = await _todoRepository.GetByFilterAsync(request.Filter);
        return Response<IEnumerable<Todo>>.Success(result.Data ?? new List<Todo>());
    }
}
