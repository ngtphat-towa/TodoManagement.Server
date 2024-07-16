using Application.Exceptions;
using Application.Features.Todos.CreateTodo;
using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.UpdateTodo;

public record UpdateTodoCommand : CreateTodoCommand
{
    public int Id { get; set; }
}
public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, Response<int>>
{
    private readonly ITodoRepository _todoRepository;

    public UpdateTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Response<int>> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var exitingTodo = await _todoRepository.GetByIdAsync(request.Id);
        if (exitingTodo is null)
        {
            throw new ApiException($"Product Not Found.");
        }

        exitingTodo.Title = request.Title;
        exitingTodo.Description = request.Description;
        exitingTodo.Status = request.Status;

        await _todoRepository.UpdateAsync(exitingTodo);

        return Response<int>.Success(exitingTodo.Id);
    }
}
