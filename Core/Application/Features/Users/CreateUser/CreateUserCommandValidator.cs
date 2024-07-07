using FluentValidation;
using Domain.Enums;
using Application.Interfaces.Services;

namespace Application.Features.Users.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public CreateUserCommandValidator(IAuthenticatedUserService authenticatedUserService)
        {
            _authenticatedUserService = authenticatedUserService;

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.Role).Must(role => CanCreateUserWithRole(role))
                .WithMessage("You do not have permission to create a user with this role.");
        }

        private bool CanCreateUserWithRole(short roleToCreate)
        {
            var authenticatedRole = _authenticatedUserService.Roles.FirstOrDefault();

            if (authenticatedRole == null)
            {
                return false;
            }

            if (!Enum.TryParse(typeof(Roles), authenticatedRole, out object? parsedAuthenticatedRole))
            {
                return false;
            }

            Roles authenticatedUserRole = (Roles)parsedAuthenticatedRole;

            // Check permissions based on role hierarchy
            switch (authenticatedUserRole)
            {
                case Roles.SuperAdmin:
                    return true;
                case Roles.Admin:
                    return roleToCreate < (short)Roles.Admin;
                case Roles.Moderator:
                    return roleToCreate < (short)Roles.Moderator;
                case Roles.Basic:
                    return false;
                default:
                    return false;
            }
        }

    }
}
