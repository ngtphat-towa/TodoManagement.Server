using Application.Interfaces.Repositories;

using Domain;

using FluentValidation;

namespace Application.Features.Todos.CreateTodo;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    private readonly ITodoRepository _todoRepository;

    public CreateTodoCommandValidator(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;

        RuleFor(todo => todo.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.")
            .MustAsync(IsUniqueTask).WithMessage("{PropertyName} already exists.");

        RuleFor(todo => todo.Description)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(todo => todo.Status)
             .NotEmpty().WithMessage("{PropertyName} is required.")
             .NotNull()
             .Must(x => Enum.IsDefined(typeof(TodoStatusEnum), x))
             .WithMessage("{PropertyName} is not valid.");

    }

    private async Task<bool> IsUniqueTask(string title, CancellationToken cancellationToken)
    {
        return await _todoRepository.GetByTitleAsync(title)!=null;
    }
}