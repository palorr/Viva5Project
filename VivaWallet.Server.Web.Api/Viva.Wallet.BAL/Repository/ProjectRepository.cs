using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL.Repository
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
        
        // OK
        public IList<ProjectModel> GetAll()
        {
           return uow.ProjectRepository
                     .All()?
                   //  .Where(e => (e.Status != "CRE" && e.Status != "FAI"))
                     .Select(e => new ProjectModel()
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

                       }
                     ).OrderByDescending(e => e.CreatedDate).ToList();
        }

        // OK
        public ProjectModel GetProjectById(long projectId, ClaimsIdentity identity = null)
        {
            Expression<Func<Project, bool>> predicate;

            if(identity == null)
            {
                predicate = e => e.Id == projectId && e.Status != "CRE" && e.Status != "FAI";
            } 
            else
            {
                long currentUserId;

                try
                {
                    currentUserId = uow.UserRepository
                                     .SearchFor(e => e.Username == identity.Name)
                                     .Select(e => e.Id)
                                     .SingleOrDefault();
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("User lookup for current logged in User Id failed", ex);
                }

                predicate = e => e.Id == projectId && e.UserId == currentUserId;
            }

            try
            {
                return uow.ProjectRepository
                          .SearchFor(predicate)
                          .Select(e => new ProjectModel()
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

                          }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Project lookup for project Id failed", ex);
            }
        }

        // OK
        public IList<ProjectModel> GetByCategoryId(long catId)
        {
            return uow.ProjectRepository
                      .SearchFor(e => e.ProjectCategoryId == catId)
                      .Select(e => new ProjectModel()
                      {
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
                      }).OrderByDescending(e => e.CreatedDate).ToList();

        }

        // OK
        public long Insert(ProjectModel source, ClaimsIdentity identity)
        {
            long requestorUserId;

            try
            {
                requestorUserId = uow.UserRepository
                                     .SearchFor(e => e.Username == identity.Name)
                                     .Select(e => e.Id)
                                     .SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup for requestor Id for project creation failed", ex);
            }

            try
            {
                var _pro = new Project()
                {
                    Title = source.Title,
                    Description = source.Description,
                    FundingEndDate = source.FundingEndDate,
                    FundingGoal = source.FundingGoal,
                    UserId = requestorUserId,
                    ProjectCategoryId = source.ProjectCategoryId,
                    CreatedDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Status = "CRE"
                };

                uow.ProjectRepository.Insert(_pro , true);

                return _pro.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
