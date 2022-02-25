using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace KiddieParadies.Extensions
{
    public static class QueryableExtensions
    {
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
}