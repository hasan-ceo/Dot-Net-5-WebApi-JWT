using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tuteexy.Service
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public RepositoryAsync(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<T> GetAsync(long Id)
        {
            return await dbSet.FindAsync(Id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>> filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }


        

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }


            return await query.FirstOrDefaultAsync();
        }

        public async Task RemoveIDAsync(long Id)
        {
            T entity = await dbSet.FindAsync(Id);
            await RemoveEntityAsync(entity);
        }

        public async Task<bool> RemoveEntityAsync(T entity)
        {
            dbSet.Remove(entity);
            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
            return await Task.FromResult(true);
        }
    }
}
