using Application.Interfaces.Repositories;

using Domain;

using FluentValidation;

namespace Application.Features.Todos.CreateTodo
{
    public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
    {
        public CreateTodoCommandValidator()
        {
            RuleFor(todo => todo.Title)
                .NotNull()
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MaximumLength(256)
                .WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(todo => todo.Description)
                .NotNull()
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(256).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(todo => todo.Status)
                .NotNull().NotEmpty().WithMessage("{PropertyName} must be specified")
                .Must(x => Enum.IsDefined(typeof(TodoStatusEnum), (int)x))
                .WithMessage("{PropertyName} is not valid.");
        }
    }
}
