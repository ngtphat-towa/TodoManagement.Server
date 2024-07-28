﻿using Application.Interfaces.Services;

using Domain.Enums;

using FluentValidation;

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
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.Role)
                .Must(IsEnumDefined)
                .WithMessage("Invalid role specified.")
                .MustAsync(CanCreateUserWithRole)
                .WithMessage("You do not have permission to create a user with this role.");
        }

        private bool IsEnumDefined(short roleNumber)
        {
            return Enum.IsDefined(typeof(Roles), (int)roleNumber);
        }

        private async Task<bool> CanCreateUserWithRole(short roles, CancellationToken cancellationToken)
        {
            var currentUserRoles = await _authenticatedUserService.Roles();

            if (currentUserRoles == null || !currentUserRoles.Any())
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(Roles), (int)roles))
            {
                return false;

            }

            var roleToCheck = (Roles)roles;

            // Check if the current user has the necessary permissions based on their role
            foreach (var currentUserRole in currentUserRoles)
            {
                if (!Enum.TryParse(typeof(Roles), currentUserRole, out object? parsedCurrentUserRole))
                {
                    continue;
                }

                Roles authenticatedUserRole = (Roles)parsedCurrentUserRole;

                // SuperAdmin can create any user
                if (authenticatedUserRole == Roles.SuperAdmin)
                {
                    return true;
                }
                // Non-Super Admin can only create user below them
                if (roleToCheck.IsLowerOrBasic(authenticatedUserRole))
                {
                    return true;
                }
            }

            return false;
        }

    }
}