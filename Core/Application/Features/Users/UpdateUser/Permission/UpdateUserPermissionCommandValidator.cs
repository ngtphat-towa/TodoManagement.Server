using FluentValidation;
using Domain.Enums;
using Application.Interfaces.Services;

namespace Application.Features.Users.UpdateUser.Permission
{
    public class UpdateUserPermissionCommandValidator : AbstractValidator<UpdateUserPermissionCommand>
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public UpdateUserPermissionCommandValidator(IAuthenticatedUserService authenticatedUserService)
        {
            _authenticatedUserService = authenticatedUserService;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID must be provided.");

            RuleFor(x => x.Permissions)
                .NotNull().WithMessage("Permissions list must be provided.")
                .Must(PermissionsAreValid).WithMessage("One or more permissions are invalid.");

            RuleFor(x => x)
                .MustAsync(CanUpdateUserWithPermission)
                .WithMessage("You do not have permission to update user permissions.");
        }

        private bool PermissionsAreValid(List<string> permissionsToAdd)
        {
            // Validate each permission in permissionsToAdd
            foreach (var permission in permissionsToAdd)
            {
                // Split permission into Controller and Action parts
                var parts = permission.Split('_');
                if (parts.Length != 2)
                {
                    return false;
                }

                var controllerString = parts[0];
                var actionString = parts[1];

                if (!Enum.TryParse<ControllerPermission>(controllerString, out var controller))
                {
                    return false;
                }

                if (!Enum.TryParse<ActionPermission>(actionString, out var action))
                {
                    return false;
                }

            }

            return true;
        }

        private async Task<bool> CanUpdateUserWithPermission(UpdateUserPermissionCommand command, CancellationToken cancellationToken)
        {
            // Get authenticated user's roles and permissions
            var authenticatedRoles = await _authenticatedUserService.Roles();
            var authenticatedPermissions = await _authenticatedUserService.Permissions();

            if (authenticatedRoles == null || !authenticatedRoles.Any())
            {
                return false;
            }

            // Check if the user is SuperAdmin
            if (authenticatedRoles.Contains(Roles.SuperAdmin.ToString()))
            {
                return true;
            }

            // Convert command permissions to the format used in authenticated permissions
            var permissionsToAdd = command.Permissions;

            // Check if the authenticated user has all the required permissions
            return permissionsToAdd.All(p => authenticatedPermissions.Contains(p));
        }
    }
}
