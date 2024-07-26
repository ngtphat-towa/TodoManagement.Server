using Domain.Entities;

using Shared.Wrappers;

namespace Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines methods for managing user accounts.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <param name="entity">The user to be added.</param>
        /// <returns>The added user with its ID.</returns>
        Task<User> AddAsync(User entity);

        /// <summary>
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="entity">The user to be deleted.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        /// <exception cref="ApplicationException">Thrown when the deletion fails.</exception>
        Task DeleteAsync(User entity);

        /// <summary>
        /// Retrieves all users from the system.
        /// </summary>
        /// <returns>A read-only list of all users.</returns>
        Task<IReadOnlyList<User>> GetAllAsync();

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetByIdAsync(string id);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Retrieves the total number of users and calculates pagination details.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>Pagination details including total pages and total records.</returns>
        Task<RecordPagination> GetCountTotalPagedResponseAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>A read-only list of users for the specified page.</returns>
        Task<IReadOnlyList<User>> GetPagedResponseAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Updates an existing user's information.
        /// </summary>
        /// <param name="entity">The user with updated information.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        /// <exception cref="ApplicationException">Thrown when the update fails.</exception>
        Task UpdateAsync(User entity);
    }
}
