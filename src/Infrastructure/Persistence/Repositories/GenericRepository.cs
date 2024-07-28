using System.Linq.Expressions;

using Application.Interfaces.Common;

using Microsoft.EntityFrameworkCore;

using Persistence.Context;

using Shared.Wrappers;

namespace Persistence.Repositories
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<Response<IEnumerable<T>>> GetByFilterAsync(DataFilter? dataFilter)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (dataFilter != null && !string.IsNullOrWhiteSpace(dataFilter.FilterName) && !string.IsNullOrWhiteSpace(dataFilter.FilterValue))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, dataFilter.FilterName);
                var value = ConvertToPropertyType(property.Type, dataFilter.FilterValue);
                var equals = Expression.Equal(property, Expression.Constant(value));
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
                query = query.Where(lambda);
            }

            var result = await query.ToListAsync();
            return Response<IEnumerable<T>>.Success(result);
        }

        public async Task<PagedResponse<IReadOnlyList<T>>> GetPagedResponseAsync(
           PaginationFilter? paginationFilter,
           DataFilter? dataFilter,
           DataSort? dataSort)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            // Apply filter logic
            if (dataFilter != null && !string.IsNullOrWhiteSpace(dataFilter.FilterName) && !string.IsNullOrWhiteSpace(dataFilter.FilterValue))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, dataFilter.FilterName);
                var value = ConvertToPropertyType(property.Type, dataFilter.FilterValue);
                var equals = Expression.Equal(property, Expression.Constant(value));
                var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
                query = query.Where(lambda);
            }

            // Apply sort logic
            if (dataSort != null && !string.IsNullOrWhiteSpace(dataSort.SortBy))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, dataSort.SortBy);
                var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);
                query = dataSort.IsDescending ? query.OrderByDescending(lambda) : query.OrderBy(lambda);
            }

            // Get the total count of records
            var totalRecords = await query.CountAsync();

            // Apply pagination logic
            var pagedQuery = query.Skip((paginationFilter!.PageNumber - 1) * paginationFilter.PageSize)
                                  .Take(paginationFilter.PageSize);

            var items = await pagedQuery.ToListAsync();

            // Calculate the total number of pages
            var totalPages = (int)Math.Ceiling(totalRecords / (double)paginationFilter.PageSize);

            return new PagedResponse<IReadOnlyList<T>>(
                items,
                paginationFilter.PageNumber,
                paginationFilter.PageSize,
                totalPages,
                totalRecords);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        private object ConvertToPropertyType(Type propertyType, string value)
        {
            if (propertyType == typeof(short) || propertyType == typeof(short?))
            {
                return short.Parse(value);
            }
            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                return int.Parse(value);
            }
            if (propertyType == typeof(long) || propertyType == typeof(long?))
            {
                return long.Parse(value);
            }
            if (propertyType == typeof(float) || propertyType == typeof(float?))
            {
                return float.Parse(value);
            }
            if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                return double.Parse(value);
            }
            if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                return decimal.Parse(value);
            }
            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return bool.Parse(value);
            }
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                return DateTime.Parse(value);
            }
            if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
            {
                return Guid.Parse(value);
            }

            // Default to string if no matching type found
            return value;
        }
    }
}
