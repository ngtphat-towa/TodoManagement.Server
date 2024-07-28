using Application.Interfaces.Repositories;

using Domain;

using FluentValidation;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Todos.UpdateTodo
{
    public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
    {
        private readonly ITodoRepository _todoRepository;

        public UpdateTodoCommandValidator(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;

            RuleFor(todo => todo.Id)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(todo => todo.Title)
                .NotNull()
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(256).WithMessage("{PropertyName} must not exceed 50 characters.");

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
