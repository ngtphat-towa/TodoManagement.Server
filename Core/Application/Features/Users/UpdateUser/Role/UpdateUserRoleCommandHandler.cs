﻿using Application.Interfaces.Services;

using Domain.Enums;

using FluentValidation;

namespace Application.Features.Users.UpdateUser.Role
{
    public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public UpdateUserRoleCommandValidator(IAuthenticatedUserService authenticatedUserService)
        {
            _authenticatedUserService = authenticatedUserService;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID must be provided.");

            RuleFor(x => x.Role)
            .Must(role => RoleIsValid(role))
            .WithMessage("Invalid role specified.")
            .MustAsync(CanUpdateUserRoleWithPermission)
            .WithMessage("You do not have permission to update user role.");

        }

        private bool RoleIsValid(short roleToUpdate)
        {
            return Enum.IsDefined(typeof(Roles), roleToUpdate);
        }

        private async Task<bool> CanUpdateUserRoleWithPermission(short role, CancellationToken cancellationToken)
        {
            // Get authenticated user's roles
            var currentUserRoles = await _authenticatedUserService.Roles();

            if (currentUserRoles == null || !currentUserRoles.Any())
            {
                return false;
            }

            // Check if the user is SuperAdmin
            if (currentUserRoles.Contains(Roles.SuperAdmin.ToString()))
            {
                return true;
            }

            // Convert roleToCreate to Roles enum type
            Roles roleToCheck = (Roles)role;

            // Check if the current user can update roles based on their own role
            foreach (var currentUserRole in currentUserRoles)
            {
                if (!Enum.TryParse(typeof(Roles), currentUserRole, out object? parsedCurrentUserRole))
                {
                    continue;
                }

                Roles authenticatedUserRole = (Roles)parsedCurrentUserRole;

                // SuperAdmin can update any role
                if (authenticatedUserRole.IsSuperAdmin())
                {
                    return true;
                }

                // Non-SuperAdmin users can only update roles lower or equal to their own role
                if (roleToCheck.IsLowerOrBasic(authenticatedUserRole))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
