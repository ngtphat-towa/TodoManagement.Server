using Application.Interfaces.Common;
using Domain.Entity;

namespace Application.Interfaces.Repositories;

public interface ITodoRepository : IGenericRepositoryAsync<Todo>
{
    Task<Todo?> GetByTitleAsync(string title);

}
