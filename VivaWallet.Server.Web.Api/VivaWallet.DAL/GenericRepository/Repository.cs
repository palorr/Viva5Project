using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbSet dbSet;
        private VivaWalletEntities context;

        public Repository(VivaWalletEntities _context = null)
        {
            context = _context ?? new VivaWalletEntities();
            dbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> All()
        {
            try
            {
                return context.Set<TEntity>().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TEntity>> AllAsync()
        {
            try
            {
                return await  context.Set<TEntity>().ToListAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }

        public void Delete(TEntity entity, bool save = true)
        {
            try
            {
                context.Set<TEntity>().Remove(entity);
                
                if (save)
                    context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task DeleteAsync(TEntity entity, bool save = true)
        {
            context.Set<TEntity>().Remove(entity);

            if (save)
              await  context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public TEntity FindById(long id)
        {
            try
            {
              return context.Set<TEntity>().Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TEntity> FindByIdAsync(long id)
        {
            try
            {
                return await context.Set<TEntity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Insert(TEntity entity, bool save = true)
        {
            try
            {
                context.Set<TEntity>().Add(entity);

                if (save)
                    context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task InsertAsync(TEntity entity, bool save = true)
        {
            try
            {
                context.Set<TEntity>().Add(entity);

                if (save)
                   await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public IEnumerable<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return context.Set<TEntity>().Where(predicate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<IEnumerable<TEntity>> SearchForAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await context.Set<TEntity>().Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(TEntity entity, bool save = true)
        {
            dbSet.Attach(entity);

            context.Entry<TEntity>(entity).State = EntityState.Modified;

            if (save)
                context.SaveChanges();
        }
    }
}
