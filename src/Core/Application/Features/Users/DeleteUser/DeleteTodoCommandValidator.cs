using Application.Features.Users.DeleteUser;

using FluentValidation;

namespace Application.Features.Users.UpdateUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(todo => todo.Id)
           .NotEmpty().WithMessage("{PropertyName} is required.")
           .NotNull().WithMessage("{PropertyName} is required.");
    }
}
