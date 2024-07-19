using Application.Exceptions;
using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Todos.UpdateTodoStatus;

public record UpdateTodoStatusCommand : IRequest<Response<Unit>>
{
    public int Id { get; set; }
    public short Status { get; set; }
}
public class UpdateTodoStatusCommandHandler : IRequestHandler<UpdateTodoStatusCommand, Response<Unit>>
{
    private readonly ITodoRepository _todoRepository;

    public UpdateTodoStatusCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Response<Unit>> Handle(UpdateTodoStatusCommand request, CancellationToken cancellationToken)
    {
        var exitingTodo = await _todoRepository.GetByIdAsync(request.Id);
        if (exitingTodo is null)
        {
            throw new ApiException($"Product Not Found.");
        }

        exitingTodo.Status = request.Status;

        await _todoRepository.UpdateAsync(exitingTodo);

        return Response<Unit>.MessageResponse($"Update {exitingTodo.Id} successfully", true);
    }
}
