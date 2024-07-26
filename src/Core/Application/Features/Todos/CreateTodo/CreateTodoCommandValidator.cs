using Application.Interfaces.Repositories;

using Domain;

using FluentValidation;

namespace Application.Features.Todos.CreateTodo
{
    public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
    {
        private readonly ITodoRepository _todoRepository;

        public CreateTodoCommandValidator(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;

            RuleFor(todo => todo.Title)
                .NotNull()
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MaximumLength(50)
                .WithMessage("{PropertyName} must not exceed 50 characters.")
                .MustAsync(IsUniqueTask)
                .WithMessage("{PropertyName} already exists.");

            RuleFor(todo => todo.Description)
                .NotNull()
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(todo => todo.Status)
                .NotNull().NotEmpty().WithMessage("{PropertyName} must be specified")
                .Must(x => Enum.IsDefined(typeof(TodoStatusEnum), (int)x))
                .WithMessage("{PropertyName} is not valid.");
        }

        private async Task<bool> IsUniqueTask(string title, CancellationToken cancellationToken)
        {
            var data = await _todoRepository.GetByTitleAsync(title);
            return data == null;
        }
    }
}
