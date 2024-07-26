using Application.Exceptions;
using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.DeleteTodo;

public record DeleteTodoCommand : IRequest<Response<int>>
{
    public int Id { get; set; }
}
public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Response<int>>
{
    private readonly ITodoRepository _todoRepository;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Response<int>> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var exitingTodo = await _todoRepository.GetByIdAsync(request.Id);
        if (exitingTodo is null)
        {
            throw new ApiException($"Product Not Found.");
        }

        exitingTodo.DeFlag = true;

        var addTodo = await _todoRepository.AddAsync(exitingTodo);

        return Response<int>.Success(addTodo.Id);
    }
}
