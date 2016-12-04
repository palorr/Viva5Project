using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL.Repository
{
    public class ProjectCategoryRepository : IDisposable
    {
        protected IUnitOfWork uow;

        public ProjectCategoryRepository(IUnitOfWork _uow)
        {
            if (_uow == null)
                uow = new UnitOfWork();
            else
                uow = _uow;
        }
        
        // OK
        public IList<ProjectCategoriesModel> GetAll()
        {
            return uow.ProjectCategoryRepository
                      .All()?
                      .Select(e => new ProjectCategoriesModel()
                      {
                          Id = e.Id,
                          Name = e.Name
                      }).ToList();
        }

        public void Dispose()
        {
            
        }
    }
}
