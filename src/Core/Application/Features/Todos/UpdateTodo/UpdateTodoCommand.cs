using Application.Exceptions;
using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.UpdateTodo;

public record UpdateTodoCommand : IRequest<Response<int>>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Opening = 1, Progressing = 2, Testing = 3, Done = 4, Rejected = 5,
    /// </summary>
    public short Status { get; set; }
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

        return Response<int>.Success(exitingTodo.Id, $"Update {exitingTodo.Id} sucessfully");
    }
}
