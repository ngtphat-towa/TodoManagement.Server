using Application.Exceptions;
using Application.Interfaces.Repositories;

using Domain.Entities;
using Domain.Enums;

using Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Shared.Wrappers;

namespace Infrastructure.Persistence.Repositories
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> AddAsync(User entity)
        {
            var newUser = new ApplicationUser
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                UserName = entity.UserName,
                Email = entity.Email,
                EmailConfirmed = true
            };

            UserPermissionHelper.InitializePermissions(newUser, (Roles)entity.Role);

            var result = await _userManager.CreateAsync(newUser, entity.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, ((Roles)entity.Role).ToString());

                entity.Id = newUser.Id;

                return entity;
            }
            else
            {
                throw new ApiException($"Failed to create user '{entity.Email}'. Error: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        public async Task DeleteAsync(User entity)
        {
            var user = await _userManager.FindByIdAsync(entity.Id.ToString());
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Failed to delete user with ID '{entity.Id}': {result.Errors.FirstOrDefault()?.Description}");
                }
            }
            else
            {
                throw new KeyNotFoundException($"User with ID '{entity.Id}' not found.");
            }
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => MapToUserEntity(u)).ToList();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null ? MapToUserEntity(user) : null;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user != null ? MapToUserEntity(user) : null;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null ? MapToUserEntity(user) : null;
        }

        public async Task<RecordPagination> GetCountTotalPagedResponseAsync(int pageNumber, int pageSize)
        {
            var totalRecords = await _userManager.Users.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new RecordPagination
            {
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };
        }

        public async Task<IReadOnlyList<User>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            var users = await _userManager.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(u => MapToUserEntity(u)).ToList();
        }

        public async Task UpdateAsync(User entity)
        {
            var user = await _userManager.FindByIdAsync(entity.Id.ToString());
            if (user != null)
            {
                user.UserName = entity.UserName;
                user.Email = entity.Email;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Failed to update user with ID '{entity.Id}': {result.Errors.FirstOrDefault()?.Description}");
                }
            }
            else
            {
                throw new KeyNotFoundException($"User with ID '{entity.Id}' not found.");
            }
        }

        private User MapToUserEntity(ApplicationUser applicationUser)
        {
            Roles userRole = applicationUser switch
            {
                _ when _userManager.IsInRoleAsync(applicationUser, Roles.SuperAdmin.ToString()).Result => Roles.SuperAdmin,
                _ when _userManager.IsInRoleAsync(applicationUser, Roles.Admin.ToString()).Result => Roles.Admin,
                _ when _userManager.IsInRoleAsync(applicationUser, Roles.Moderator.ToString()).Result => Roles.Moderator,
                _ => Roles.Basic 
            };

            return new User
            {
                Id = applicationUser.Id,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                UserName = applicationUser.UserName!,
                Email = applicationUser.Email!,
                Role = (short)userRole, 
                Permissions = applicationUser.Permissions
            };
        }

    }
}
