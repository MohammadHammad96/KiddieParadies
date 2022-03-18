using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KiddieParadies.Core.Services
{
    public interface IApplicationRepository<TEntity> where TEntity : class
    {

        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filters = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);

        Task AddAsync(TEntity entity);

        void Delete(TEntity entity);
    }
}
