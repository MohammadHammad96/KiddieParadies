using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KiddieParadies.Core.Models;

namespace KiddieParadies.Core.Services
{
    public interface IRepository<TEntity> where TEntity : Entity
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