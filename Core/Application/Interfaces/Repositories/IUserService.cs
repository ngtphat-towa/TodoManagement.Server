using Domain.Entities;

using Shared.Wrappers;

namespace Application.Interfaces.Repositories;

public interface IUserService
{
    Task<User> AddAsync(User entity);
    Task DeleteAsync(User entity);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<RecordPagination> GetCountTotalPagedResponseAsync(int pageNumber, int pageSize);
    Task<IReadOnlyList<User>> GetPagedResponseAsync(int pageNumber, int pageSize);
    Task UpdateAsync(User entity);
}
