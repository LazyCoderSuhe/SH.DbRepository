using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SH.DbRepository
{
    /// <summary>
    /// EF 的数据库仓储接口，提供了基本的增删改查方法，具体实现由具体的仓储类来完成。
    /// </summary>
    public interface IRepository<Key, T> where T : class, new()
    {
        #region Queryable 查询

        /// <summary>
        /// 获取当前仓储实体的查询入口（以 <see cref="IQueryable{T}"/> 形式返
        /// </summary>
        public IQueryable<T> Entities { get; }
        /// <summary>
        /// EF 是否查询跟踪
        /// </summary>
        public bool Tracking { get; set; }
        #endregion

        #region IEnumerable 查询
        /// <summary>
        /// 获取满足条件的实体数量。
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> Count(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 判断是否存在满足条件的实体。
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> Has(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 获取当前仓储实体的全部数据（以 <see cref="IEnumerable{T}"/> 形式返回）。
        /// 该方法对初级开发人员更友好：避免直接暴露 <see cref="IQueryable{T}"/> 到上层后被误用，
        /// 减少在业务层拼接复杂查询导致的性能与维护风险。
        /// </summary>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>实体集合。</returns>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// 按条件查询当前仓储实体（以 <see cref="IEnumerable{T}"/> 形式返回）。
        /// 该方法对初级开发人员更友好：通过受控入口完成筛选，降低错误使用查询管道的可能性。
        /// </summary>
        /// <param name="predicate">查询条件表达式。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>满足条件的实体集合。</returns>
        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 对当前仓储实体执行排序查询（以 <see cref="IEnumerable{T}"/> 形式返回）。
        /// 该方法对初级开发人员更友好：通过明确参数控制升序/降序，避免在业务层误用复杂查询表达式。
        /// </summary>
        /// <typeparam name="TKey">排序键类型。</typeparam>
        /// <param name="predicate">查询条件表达式。</param>
        /// <param name="topCount">返回的前 N 条记录。</param>
        /// <param name="orderByExpression">排序字段表达式（例如：<c>x => x.CreatedTime</c>）。</param>
        /// <param name="descending"><c>true</c> 为降序；<c>false</c> 为升序。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>排序后的实体集合。</returns>
        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, int topCount, Expression<Func<T, object>> orderByExpression, bool descending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 对当前仓储实体执行多条件排序查询（以 <see cref="IEnumerable{T}"/> 形式返回）。
        /// 通过传入排序管道可组合 <c>OrderBy/ThenBy</c> 或 <c>OrderByDescending/ThenByDescending</c>。
        /// </summary>
        /// <param name="predicate">查询条件表达式。</param>
        /// <param name="topCount">返回的前 N 条记录。</param>
        /// <param name="orderBy">排序管道（例如：<c>q => q.OrderBy(x => x.CreatedTime).ThenBy(x => x.Id)</c>）。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>排序后的实体集合。</returns>
        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, int topCount, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, CancellationToken cancellationToken = default);

        /// <summary>
        /// 对当前仓储实体执行分页查询（以 <see cref="IEnumerable{T}"/> 形式返回）。
        /// 该方法对初级开发人员更友好：统一查询、排序与分页参数，减少重复手写 Skip/Take 时出现的边界错误。
        /// </summary>
        /// <param name="predicate">查询条件表达式。</param>
        /// <param name="orderByExpression">排序字段表达式（例如：<c>x => x.CreatedTime</c>）。</param>
        /// <param name="pageIndex">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页数量。</param>
        /// <param name="descending"><c>true</c> 为降序；<c>false</c> 为升序。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>满足条件并分页后的实体集合。</returns>
        Task<IPage<T>> QueryAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderByExpression, int pageIndex, int pageSize, bool descending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 对当前仓储实体执行分页查询（以 <see cref="IEnumerable{T}"/> 形式返回），支持多条件排序。
        /// 通过传入排序管道可组合 <c>OrderBy/ThenBy</c> 或 <c>OrderByDescending/ThenByDescending</c>。
        /// </summary>
        /// <param name="predicate">查询条件表达式。</param>
        /// <param name="orderBy">排序管道（例如：<c>q => q.OrderBy(x => x.CreatedTime).ThenBy(x => x.Id)</c>）。</param>
        /// <param name="pageIndex">页码（从 1 开始）。</param>
        /// <param name="pageSize">每页数量。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>满足条件并分页后的实体集合。</returns>
        Task<IPage<T>> QueryAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int pageIndex, int pageSize, CancellationToken cancellationToken = default);

        #endregion

        #region 主键查询

        /// <summary>
        /// 根据主键查找单个实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="keyValues">主键值（支持联合主键）。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>找到的实体；未找到时返回 <c>null</c>。</returns>
        Task<T?> FindAsync(Key keyValues, CancellationToken cancellationToken = default);

        #endregion

        #region 新增

        /// <summary>
        /// 异步新增单个实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="entity">要新增的实体。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>表示异步操作的任务。</returns>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步批量新增实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="entities">要新增的实体集合。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>表示异步操作的任务。</returns>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        #endregion

        #region 更新

        /// <summary>
        /// 更新单个实体。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="entity">要更新的实体。</param>
        Task UpdateAsync(T entity);
        /// <summary>
        /// 更新指定实体类型的多个实体。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>

        Task UpdateRangeAsync(IEnumerable<T> entities);
        #endregion

        #region 删除

        /// <summary>
        /// 删除单个实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">要删除的实体。</param>
        Task RemoveAsync(T entity);

        /// <summary>
        /// 删除指定实体类型的多个实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entities">要删除的实体集合。</param>
        void RemoveRange(IEnumerable<T> entities);

        #endregion
    }
}
