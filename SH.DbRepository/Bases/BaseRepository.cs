using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SH.DbRepository
{
    public abstract class BaseRepository<Key, T> : IRepository<Key, T> where T : class, new()
    {
        #region 字段与构造函数

        private readonly DbContext _dbContext;

        public BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Queryable 查询

        public virtual IQueryable<T> Entities => _dbContext.Set<T>();

        public bool Tracking { get; set; } = true;

        #endregion

        #region IEnumerable 查询

        public virtual async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await Entities.AsNoTracking().Where(predicate).CountAsync();
        }

        public async Task<bool> Has(Expression<Func<T, bool>> predicate)
        {
            return (await Count(predicate)) > 0;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var query = Tracking ? Entities : Entities.AsNoTracking(); 
            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var query = Tracking ? Entities : Entities.AsNoTracking();
            return await query.Where(predicate)
                .ToListAsync(cancellationToken);

        }

        public virtual async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, int topCount, Expression<Func<T, object>> orderByExpression, bool descending = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(orderByExpression);
            var query = Tracking ? Entities : Entities.AsNoTracking();  
            query = query.Where(predicate);
            if (descending)
            {
                query = query.OrderByDescending(orderByExpression);
            }
            else
            {
                query = query.OrderBy(orderByExpression);
            }

            return
              await query
                .Take(topCount)
                .ToListAsync(cancellationToken);

        }

        public virtual Task<IPage<T>> QueryAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, int pageIndex, int pageSize, bool descending = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(orderByExpression);
            var query = Tracking ? Entities : Entities.AsNoTracking();
            return query.ToPageAsync(predicate, orderByExpression, descending, pageIndex, pageSize);

        }

        public virtual async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, int topCount, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(orderBy);
            var query = Tracking ? Entities : Entities.AsNoTracking();
             query = orderBy(query.Where(predicate));
            return await query
                .Take(topCount)
                .ToListAsync(cancellationToken);
        }

        public virtual Task<IPage<T>> QueryAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(orderBy);
            var query = Tracking ? Entities : Entities.AsNoTracking();  

            return query.ToPageAsync(predicate, orderBy, pageIndex, pageSize);
        }

        #endregion

        #region 主键查询

        public virtual async Task<T?> FindAsync(Key keyValues, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(keyValues);
            return await _dbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        #endregion

        #region 新增

        public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Add(entity);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().AddRange(entities);
            return Task.CompletedTask;
        }

        #endregion

        #region 更新

        public virtual Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }

        #endregion

        #region 删除

        public virtual Task RemoveAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }
        #endregion
    }

    public class UnitOfWorkEfCore<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _appDbContext;
        public UnitOfWorkEfCore(TContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task CommitAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
        public Task RollbackAsync()
        {
            // EF Core does not have built-in support for transactions in the same way as traditional databases.
            // You would need to implement your own transaction management if needed.
            return Task.CompletedTask;
        }
    }
}
