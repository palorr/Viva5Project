using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Helpers;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL.Repository
{
    public class ProjectUpdateRepository : IDisposable
    {
        protected IUnitOfWork uow;

        public ProjectUpdateRepository(IUnitOfWork _uow)
        {
            if (_uow == null)
                uow = new UnitOfWork();
            else
                uow = _uow;
        }
        
        // OK
        public IList<ProjectUpdateModelToView> GetAllProjectUpdates(long projectId, ClaimsIdentity identity = null)
        {
            bool isRequestorProjectCreator = false;

            if (identity != null)
            {
                ProjectRepository _prRepo = new ProjectRepository(uow);
                isRequestorProjectCreator = _prRepo.IsProjectCreator((int)projectId, identity);
            }
            
            return uow.ProjectUpdateRepository
                        .SearchFor(e => e.ProjectId == projectId)
                        .Select(e => new ProjectUpdateModelToView()
                        {
                            Id = e.Id,
                            ProjectId = e.ProjectId,
                            AttachmentSetId = e.AttachmentSetId,
                            Title = e.Title,
                            Description = e.Description,
                            WhenDateTime = e.WhenDateTime,
                            IsRequestorProjectCreator = isRequestorProjectCreator
                        }).OrderByDescending(e => e.WhenDateTime).ToList();
            
        }

        // OK
        public ProjectUpdateModelToView GetProjectUpdateById(int projectId, int updateId, ClaimsIdentity identity)
        {
            bool isRequestorProjectCreator = false;

            ProjectRepository _prRepo = new ProjectRepository(uow);
            isRequestorProjectCreator = _prRepo.IsProjectCreator(projectId, identity);
        
            try
            {
                return uow.ProjectUpdateRepository
                          .SearchFor(e => (e.ProjectId == projectId && e.Id == updateId))
                          .Select(e => new ProjectUpdateModelToView()
                          {
                              Id = e.Id,
                              ProjectId = e.ProjectId,
                              AttachmentSetId = e.AttachmentSetId,
                              Title = e.Title,
                              Description = e.Description,
                              WhenDateTime = e.WhenDateTime,
                              IsRequestorProjectCreator = isRequestorProjectCreator
                          }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Project update id lookup failed", ex);
            }
        }

        // OK
        public StatusCodes InsertProjectUpdate(ProjectUpdateModel source, ClaimsIdentity identity, int projectId)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return StatusCodes.NOT_FOUND;
                }

                else
                {
                    //check if current user is the projectId's creator
                    //else return NOT ALLOWED
                    long requestorUserId = UtilMethods.GetCurrentUserId(uow, identity.Name);

                    if (_project.User.Id != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    var _projectUpdate = new ProjectUpdate()
                    {
                        ProjectId = projectId,
                        AttachmentSetId = source.AttachmentSetId,
                        Title = source.Title,
                        Description = source.Description,
                        WhenDateTime = DateTime.Now,
                    };

                    uow.ProjectUpdateRepository.Insert(_projectUpdate, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public StatusCodes EditProjectUpdate(ProjectUpdateModel source, ClaimsIdentity identity, int updateId)
        {
            try
            {
                var _projectUpdate = uow.ProjectUpdateRepository.FindById(updateId);

                if (_projectUpdate == null)
                {
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // update found. does the user that wishes to update it really is the project creator? check this here
                    long requestorUserId = UtilMethods.GetCurrentUserId(uow, identity.Name);

                    if (_projectUpdate.Project.UserId != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    _projectUpdate.WhenDateTime = DateTime.Now;
                    _projectUpdate.Title = source.Title;
                    _projectUpdate.Description = source.Description;

                    uow.ProjectUpdateRepository.Update(_projectUpdate, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public StatusCodes DeleteProjectUpdate(ClaimsIdentity identity, int updateId)
        {
            try
            {
                var _projectUpdate = uow.ProjectUpdateRepository.FindById(updateId);

                //project update not found
                if (_projectUpdate == null)
                {
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // project update found. does the user that wishes to delete it really is the project creator? check this here
                    long requestorUserId = UtilMethods.GetCurrentUserId(uow, identity.Name);

                    if (_projectUpdate.Project.UserId != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    uow.ProjectUpdateRepository.Delete(_projectUpdate, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
           
        }

        public enum StatusCodes
        {
            NOT_FOUND = 0,
            NOT_AUTHORIZED = 1,
            OK = 2
        };
    }
}
