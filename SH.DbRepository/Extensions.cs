using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SH.DbRepository
{
    public static class Extensions
    {
        public static async Task<IPage<T>> ToPageAsync<T>(this IOrderedQueryable<T> datas,  int pageIndex = 1, int pageSize = 20)
        {        
            var count = await datas.CountAsync();         
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 20;
            var data = await datas.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageModel<T>
            {
                Datas = data,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }

        public static async Task<IPage<T>> ToPageAsync<T>(this IQueryable<T> datas, Expression<Func<T, bool>>? predicate, Expression<Func<T, object>> orderByExpression, bool isDesc = false, int pageIndex = 1, int pageSize = 20)
        {
            var query = predicate != null ? datas.Where(predicate) : datas;
            var count = await query.CountAsync();
            if (isDesc)
            {
                query = query.OrderByDescending(orderByExpression);
            }
            else
            {
                query = query.OrderBy(orderByExpression);
            }
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 20;
            var data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageModel<T>
            {
                Datas = data,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }

      
        public static async Task<IPage<T>> ToPageAsync<T>(this IQueryable<T> datas, Expression<Func<T, bool>>? predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int pageIndex = 1, int pageSize = 20)
        {
            ArgumentNullException.ThrowIfNull(datas);
            ArgumentNullException.ThrowIfNull(orderBy);

            var query = predicate != null ? datas.Where(predicate) : datas;
            var count = await query.CountAsync();
            query = orderBy(query);

            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 20;

            var data = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PageModel<T>
            {
                Datas = data,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }
    }
}
