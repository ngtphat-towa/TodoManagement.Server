using Application.Features.Todos.DeleteTodo;

using FluentValidation;

namespace Application.Features.Todos.UpdateTodo;

public class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(todo => todo.Id)
           .NotEmpty().WithMessage("{PropertyName} is required.")
           .NotNull().WithMessage("{PropertyName} is required.")
           .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
    }
}
