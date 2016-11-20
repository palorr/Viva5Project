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
        public ProjectModelToView GetProjectById(long projectId, ClaimsIdentity identity = null)
        {
            bool isRequestorProjectCreator = false;

            if(identity != null)
            {
                isRequestorProjectCreator = IsProjectCreator((int)projectId, identity); 
            }

            try
            {
                return uow.ProjectRepository
                          .SearchFor(e => e.Id == projectId)
                          .Select(e => new ProjectModelToView()
                          {
                              Id = e.Id,
                              AttachmentSetId = e.AttachmentSetId,
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
                              UpdatedDate = e.UpdateDate,
                              IsRequestorProjectCreator = isRequestorProjectCreator
                          }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Project lookup for project Id failed", ex);
            }
        }

        // OK
        public bool IsProjectCreator(int projectId, ClaimsIdentity identity)
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
                throw new InvalidOperationException("User lookup for requestor Id failed", ex);
            }

            try
            {
                Project project = uow.ProjectRepository
                                     .SearchFor(e => e.Id == projectId)
                                     .SingleOrDefault();

                if (project.UserId == requestorUserId) return true;

                return false;

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
                            AttachmentSetId = e.AttachmentSetId,
                            Title = e.Title,
                            CreatedDate = e.CreatedDate,
                            Description =e.Description,
                            FundingEndDate =e.FundingEndDate,
                            FundingGoal = e.FundingGoal,
                            OwnerId = e.User.Id,
                            OwnerName = e.User.Name,
                            ProjectCategoryDesc = e.ProjectCategory.Name,
                            ProjectCategoryId = e.ProjectCategoryId,
                            Status = e.Status,
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
                    AttachmentSetId = source.AttachmentSetId,
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

        // OK
        public StatusCodes Update(ProjectModel source, ClaimsIdentity identity, int projectId)
        {
            try
            {
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // project found. does the user that wishes to update it really is the project creator? check this here
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
                        throw new InvalidOperationException("User lookup for requestor Id for project update creation failed", ex);
                    }

                    if (_project.UserId != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    _project.ProjectCategoryId = source.ProjectCategoryId;
                    //_project.AttachmentSetId = source.AttachmentSetId;
                    _project.Title = source.Title;
                    _project.Description = source.Description;
                    _project.UpdateDate = DateTime.Now;

                    // TO BE CONSIDERED IF THEY CAN CHANGE ONCE DEFINED IN CREATION PHASE - KICKSTARTER DOES NOT ALLOW THESE TO CHANGE
                    //_project.FundingEndDate = source.FundingEndDate;
                    //_project.FundingGoal = source.FundingGoal;

                    uow.ProjectRepository.Update(_project, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public enum StatusCodes
        {
            NOT_FOUND = 0,
            NOT_AUTHORIZED = 1,
            OK = 2
        };

    }
}
