using System;
using System.Collections.Generic;
using System.Text;

namespace SH.DbRepository
{
    public interface IUnitOfWork
    {
        // CommitAsync() 方法用于提交事务，确保所有操作要么全部成功，要么全部失败
        Task CommitAsync();
        // RollbackAsync() 方法用于回滚事务，撤销之前的操作
        Task RollbackAsync();

    }
}
