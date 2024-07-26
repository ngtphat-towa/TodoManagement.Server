using Application.Interfaces.Services;

using FluentValidation;

namespace Application.Features.Users.UpdateUser.Info;

public class UpdateUserInfoCommandValidator : AbstractValidator<UpdateUserInfoCommand>
{
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public UpdateUserInfoCommandValidator(IAuthenticatedUserService authenticatedUserService)
    {
        _authenticatedUserService = authenticatedUserService;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID must be provided.");

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
    }

}
