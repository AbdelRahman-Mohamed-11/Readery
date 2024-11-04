using Microsoft.EntityFrameworkCore.Query;
using Readery.Core.Models;
using Readery.Core.Pagination;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Readery.Core.Repositores
{
    public interface IGenericRepository<T> where T : class
    {
        Task<PaginatedList<T>> GetAll(bool condition = false,
            Expression<Func<T, bool>>? predicate = null,
            string? columnName = null, string? navigationProperty = null, string? sortDirection = null,
            int? pageNumber = 1, int? pageSize = 5, params string[]? includes);

        Task<T?> Get(Expression<Func<T, bool>>? filter,
             Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);

        Task AddAsync(T entity);

        public Task BulkInsertAsync(IEnumerable<T> entities);
        void UpdateAsync(T entity);

        void DeleteAsync(T entity);

        public Task BulkDeleteAsync<T>(IEnumerable<int> ids) where T : BaseEntity;
    }
}
