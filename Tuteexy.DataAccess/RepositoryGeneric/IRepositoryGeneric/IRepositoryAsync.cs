using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tuteexy.Service
{
    public interface IRepositoryAsync<T> where T : class
    {
        Task<T> GetAsync(long Id);

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
            );

        Task<T> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );

        Task AddAsync(T entity);
        Task RemoveIDAsync(long Id);
        Task<bool> RemoveEntityAsync(T entity);
        Task<bool> RemoveRangeAsync(IEnumerable<T> entity);


    }
}
