using Domain;

using FluentValidation;

namespace Application.Features.Todos.UpdateTodoStatus;

public class UpdateTodoStatusCommandValidator : AbstractValidator<UpdateTodoStatusCommand>
{
    public UpdateTodoStatusCommandValidator()
    {
        RuleFor(todo => todo.Id)
         .NotEmpty().WithMessage("{PropertyName} is required.")
         .NotNull().WithMessage("{PropertyName} is required.")
         .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(todo => todo.Status)
         .NotEmpty().WithMessage("{PropertyName} is required.")
         .NotNull()
         .Must(x => Enum.IsDefined(typeof(TodoStatusEnum), x))
         .WithMessage("{PropertyName} is not valid.");
    }
}
