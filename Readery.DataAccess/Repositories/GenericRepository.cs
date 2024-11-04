using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Readery.Core.Extensions;
using Readery.Core.Models;
using Readery.Core.Pagination;
using Readery.Core.Repositores;
using Readery.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Readery.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<T?> Get(Expression<Func<T, bool>>? filter,
             Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            IQueryable<T> query = dbContext.Set<T>();

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.FirstOrDefaultAsync(filter);

        }

        public async Task<PaginatedList<T>> GetAll(
      bool condition = false,
      Expression<Func<T, bool>>? predicate = null,
      string? columnName = null,
      string? navigationProperty = null,
      string? sortDirection = null,
      int? pageNumber = null,
      int? pageSize = null,
      params string[]? includes)
        {
            IQueryable<T> query = dbContext.Set<T>();

            // Include related entities if specified
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (condition && predicate != null)
            {
                query = query.WhereIf(condition, predicate);
            }

            if (columnName != null && sortDirection != null)
            {
                query = query.OrderByProperty(columnName!, sortDirection!, navigationProperty);
            }

            // Handle pagination
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber.Value > 0 && pageSize.Value > 0)
            {
                // Return paginated result
                return await PaginatedList<T>.CreateAsync(query.AsNoTracking(), pageNumber.Value, pageSize.Value);
            }
            else
            {
                // Return all results without pagination
                var allItems = await query.AsNoTracking().ToListAsync();
                return new PaginatedList<T>(allItems, allItems.Count, 1, allItems.Count);  // Returning full list with dummy pagination
            }
        }


        public async Task AddAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
        }

        public async Task BulkInsertAsync(IEnumerable<T> entities)
        {

            // BulkInsert method from EFCore.BulkExtensions
            await dbContext.BulkInsertAsync(entities.ToList());
        }
        public void DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public async Task BulkDeleteAsync<T>(IEnumerable<int> ids) where T : BaseEntity
        {
            if (ids == null || !ids.Any())
                return;

            await dbContext.Set<T>()
                .Where(entity => ids.Contains(entity.Id)) // Works because T has an Id property
                .ExecuteDeleteAsync();
        }


        public void UpdateAsync(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }
    }
}
