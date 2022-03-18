using KiddieParadies.Core.Services;
using KiddieParadies.Extensions;
using KiddieParadies.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KiddieParadies.Infrastructure.Services
{
    public class ApplicationRepository<TEntity> : IApplicationRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;
        protected DbSet<TEntity> DbSet;

        public ApplicationRepository(ApplicationDbContext context)
        {
            _dbContext = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filters = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;

            query = query.ApplyFiltering(filters)
                .IncludeEntities(includeProperties)
                .ApplyOrdering(orderBy);

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var result = await DbSet.FindAsync(id);
            IQueryable<TEntity> query = result as IQueryable<TEntity>;

            query = query.IncludeEntities(includeProperties);


            DbSet = (DbSet<TEntity>)query;
            return await DbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }
    }
}