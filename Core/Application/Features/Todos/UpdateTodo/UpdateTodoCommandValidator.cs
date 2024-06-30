using Application.Interfaces.Repositories;

using FluentValidation;

namespace Application.Features.Todos.UpdateTodo;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    private readonly ITodoRepository _todoRepository;

    public UpdateTodoCommandValidator(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;

        RuleFor(todo => todo.Id)
           .NotEmpty().WithMessage("{PropertyName} is required.")
           .NotNull().WithMessage("{PropertyName} is required.")
           .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
    }
}
