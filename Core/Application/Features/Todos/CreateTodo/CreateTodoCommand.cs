using Application.Interfaces.Repositories;

using MapsterMapper;

using MediatR;

using Shared.Wrappers;

using Domain.Entity;

namespace Application.Features.Todos.CreateTodo;

public record CreateTodoCommand : IRequest<Response<int>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Opening = 1, Progressing = 2, Testing = 3, Done = 4, Rejected = 5,
    /// </summary>
    public short Status { get; set; } 
}
public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Response<int>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;


    public CreateTodoCommandHandler(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<Response<int>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = _mapper.Map<Todo>(request);
        var addTodo = await _todoRepository.AddAsync(todo);
        return Response<int>.Success(addTodo.Id, $"Create {nameof(Todo)} successfully");
    }
}
