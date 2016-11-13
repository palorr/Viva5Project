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
        protected UnitOfWork uow;

        public ProjectCategoryRepository()
        {
            uow = new UnitOfWork();
        }
        
        public IList<ProjectCategoriesModel> GetAll()
        {
            return uow.ProjectCategoryRepository.All()?.Select(e => new ProjectCategoriesModel()
            {
                Id = e.Id,
                Name = e.Name
            }).ToList();
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }
}
