using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL
{
    public class ProjectRepository  :IDisposable
    {
        protected UnitOfWork uow;

        public ProjectRepository()
        {
            uow = new UnitOfWork();
        }

        public void Dispose()
        {
            uow.Dispose();
        }
        
        public IList<ProjectModel> GetAll()
        {
           return uow.ProjectRepository.All()?.Select(e => new ProjectModel()
            {
               Id = e.Id,
               Title = e.Title,
               CreatedDate = e.CreatedDate,
               Description = e.Description,
               FundingEndDate = e.FundingEndDate,
               FundingGoal = e.FundingGoal,
               OwnerId = e.User.Id,
               OwnerName = e.User.Name,
               ProjectCategoryDesc = e.ProjectCategory.Name,
               ProjectCategoryId = e.ProjectCategoryId,
               Status = e.Status,
               UpdatedDate = e.UpdateDate

           }).OrderByDescending(e => e.CreatedDate).ToList();
        }

        public IList<ProjectModel> GetProjectById(long projectId)
        {
            return uow.ProjectRepository.All().Where(e => e.Id == projectId)?.Select(e => new ProjectModel()
            {
                Id = e.Id,
                Title = e.Title,
                CreatedDate = e.CreatedDate,
                Description = e.Description,
                FundingEndDate = e.FundingEndDate,
                FundingGoal = e.FundingGoal,
                OwnerId = e.User.Id,
                OwnerName = e.User.Name,
                ProjectCategoryDesc = e.ProjectCategory.Name,
                ProjectCategoryId = e.ProjectCategoryId,
                Status = e.Status,
                UpdatedDate = e.UpdateDate

            }).ToList();
        }

        public IList<ProjectModel> GetByCategoryId(long catId)
        {
            
            return uow.ProjectRepository.SearchFor(e => e.ProjectCategoryId == catId)
                .Select(e=>new ProjectModel() {
                    Id = e.Id,
                    Title = e.Title,
                    CreatedDate = e.CreatedDate,
                    Description =e.Description,
                    FundingEndDate =e.FundingEndDate,
                    FundingGoal = e.FundingGoal,
                    OwnerId = e.User.Id,
                    OwnerName = e.User.Name,
                    ProjectCategoryDesc = e.ProjectCategory.Name,
                    ProjectCategoryId =e.ProjectCategoryId,
                    Status =e.Status,
                    UpdatedDate = e.UpdateDate

                }).OrderByDescending(e=>e.CreatedDate).ToList();

        }

        public void Insert(ProjectModel source)
        {

            try
            {
                var _pro = new Project()
                {
                    Title = source.Title,
                    Description = source.Description,
                    FundingEndDate = source.FundingEndDate,
                    FundingGoal = source.FundingGoal,
                    UserId = source.OwnerId.Value,
                    ProjectCategoryId = source.ProjectCategoryId,
                    CreatedDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = source.Status
                    
                };

                uow.ProjectRepository.Insert(_pro , true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
