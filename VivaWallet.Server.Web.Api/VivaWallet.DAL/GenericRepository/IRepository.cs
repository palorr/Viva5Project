using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL
{
    public interface IRepository<TEntity> :IDisposable
    {
        IEnumerable<TEntity> All();
        IEnumerable<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate);
        TEntity FindById(long id);
        void Delete(TEntity entity, bool save = true);
        void Insert(TEntity entity, bool save = true);
        void Update(TEntity entity, bool save = true);
        Task<IEnumerable<TEntity>> AllAsync();
        Task<IEnumerable<TEntity>> SearchForAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FindByIdAsync(long id);
        Task DeleteAsync(TEntity entity, bool save = true);
        Task InsertAsync(TEntity entity, bool save = true);
    }
}
