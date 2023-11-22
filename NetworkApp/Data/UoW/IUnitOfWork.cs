using NetworkApp.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkApp.Data.UoW
{
    public interface IUnitOfWork: IDisposable
    {
        int SaveChanges(bool ensureAutoHistory = false);
        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
    }
}
