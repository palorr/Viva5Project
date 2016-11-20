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
    public class ProjectUpdateRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectUpdateRepository()
        {
            uow = new UnitOfWork();
        }
        
        // OK
        public IList<ProjectUpdateModelToView> GetAllProjectUpdates(long projectId, ClaimsIdentity identity = null)
        {
            bool isRequestorProjectCreator = false;

            if (identity != null)
            {
                ProjectRepository _prRepo = new ProjectRepository();
                isRequestorProjectCreator = _prRepo.IsProjectCreator((int)projectId, identity);
            }

            try
            {
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
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Project lookup for project Id failed", ex);
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
            uow.Dispose();
        }

        public enum StatusCodes
        {
            NOT_FOUND = 0,
            NOT_AUTHORIZED = 1,
            OK = 2
        };
    }
}
