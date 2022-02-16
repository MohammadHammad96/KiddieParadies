using KiddieParadies.Core.Services;
using KiddieParadies.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KiddieParadies.Services
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(ApplicationDbContext context)
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

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
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

    public static class QueryableExtensions
    {
        //public static string GetPropertyName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        //{
        //    MemberExpression memberExpression = (MemberExpression)property.Body;
        //    return memberExpression.Member.Name;
        //}

        public static IQueryable<TEntity> ApplyFiltering<TEntity>(this IQueryable<TEntity> query,
            Expression<Func<TEntity, bool>> filters)
        {
            if (filters == null)
                return query;

            return query.Where(filters);
        }

        public static IQueryable<TEntity> IncludeEntities<TEntity>(this IQueryable<TEntity> query,
            params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            if (includeProperties == null)
                return query;

            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return query;
        }

        public static IQueryable<TEntity> ApplyOrdering<TEntity>(this IQueryable<TEntity> query,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            if (orderBy == null)
                return query;

            return orderBy(query);
        }

        public static IQueryable<TEntity> ApplyPaging<TEntity>(this IQueryable<TEntity> query, int page, int pageSize)
        {
            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }

}