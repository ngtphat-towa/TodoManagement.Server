using Shared.Wrappers;

namespace Application.Interfaces.Common;

public interface IGenericRepositoryAsync<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<Response<IEnumerable<T>>> GetByFilterAsync(DataFilter? dataFilter);
    Task<PagedResponse<IReadOnlyList<T>>> GetPagedResponseAsync(
        PaginationFilter paginationFilter,
        DataFilter? dataFilter = null,
         DataSort? dataSort = null);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}