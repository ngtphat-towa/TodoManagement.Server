using Application.Interfaces.Repositories;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.Context;

namespace Persistence.Repositories;

public class TodoRepository : GenericRepository<Todo>, ITodoRepository
{
    private readonly DbSet<Todo> _todos;

    public TodoRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _todos = dbContext.Set<Todo>();
    }

    public async Task<Todo?> GetByTitleAsync(string title)
    {
        return await _todos.FirstOrDefaultAsync(t => t.Title.Contains(title));
    }
}
