using Domain.Enums;

using Identity.Models;

using Microsoft.AspNetCore.Identity;

namespace Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));

            if (!await roleManager.RoleExistsAsync(Roles.Admin.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));

            if (!await roleManager.RoleExistsAsync(Roles.Moderator.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));

            if (!await roleManager.RoleExistsAsync(Roles.Basic.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            await CreateOrUpdateUserAsync(userManager, "superadmin@example.com", "SuperAdmin123!", Roles.SuperAdmin);
            await CreateOrUpdateUserAsync(userManager, "admin@example.com", "Admin123!", Roles.Admin);
            await CreateOrUpdateUserAsync(userManager, "moderator@example.com", "Moderator123!", Roles.Moderator);
            await CreateOrUpdateUserAsync(userManager, "basic@example.com", "Basic123!", Roles.Basic);
        }

        private static async Task CreateOrUpdateUserAsync(UserManager<ApplicationUser> userManager, string email, string password, Roles role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true 
                };

                var result = await userManager.CreateAsync(newUser, password);
                if (result.Succeeded)
                {
                    UserPermissionHelper.InitializePermissions(newUser, role);
                    await userManager.AddToRoleAsync(newUser, role.ToString());
                }
                else
                {
                    throw new Exception($"Failed to create user '{email}'. Error: {result.Errors}");
                }
            }
            else
            {
                // Update user if needed
                await userManager.AddToRoleAsync(existingUser, role.ToString());
                UserPermissionHelper.InitializePermissions(existingUser, role);
            }
        }

       
    }
}
