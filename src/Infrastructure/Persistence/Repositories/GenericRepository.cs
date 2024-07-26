using Application.Interfaces.Common;

using Microsoft.EntityFrameworkCore;

using Persistence.Context;

using Shared.Wrappers;

namespace Persistence.Repositories
{

    public class GenericRepository<T> : IGenericRepositoryAsync<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext
                 .Set<T>()
                 .ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<RecordPagination> GetCountTotalPagedResponseAsync(int pageNumber, int pageSize)
        {
            var totalRecords = await _dbContext.Set<T>().CountAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var paginationMetadata = new RecordPagination
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };

            return paginationMetadata;
        }


        public async Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
           .Set<T>()
           .Skip((pageNumber - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
