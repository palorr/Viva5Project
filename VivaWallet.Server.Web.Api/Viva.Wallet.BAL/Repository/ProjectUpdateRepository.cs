using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL
{
    public class ProjectUpdateRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectUpdateRepository()
        {
            uow = new UnitOfWork();
        }

        public IList<ProjectUpdateModel> GetAllProjectUpdates(int projectId)
        {
            return uow.ProjectUpdateRepository.All()?
                    .Where(e => e.ProjectId == projectId)
                    .Select(e => new ProjectUpdateModel()
                    {
                        Id = e.Id,
                        ProjectId = e.ProjectId,
                        AttachmentSetId = e.AttachmentSetId,
                        Title = e.Title,
                        Description = e.Description,
                        WhenDateTime = e.WhenDateTime
                    }).OrderByDescending(e => e.WhenDateTime).ToList();
        }

        public StatusCodes InsertProjectUpdate(ProjectUpdateModel source, ClaimsIdentity identity, int projectId)
        {
            try
            {
                //get the project's creator username
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return StatusCodes.NOT_FOUND;
                }

                else
                {
                    //check if current user is the projectId's creator
                    //else return NOT ALLOWED
                    var userIdClaim = identity.Claims
                                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (_project.User.Username != userIdClaim.Value)
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

        public StatusCodes EditProjectUpdate(ProjectUpdateModel source, ClaimsIdentity identity, int projectId, int updateId)
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
                    var userIdClaim = identity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (_projectUpdate.Project.User.Username != userIdClaim.Value)
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
                    var userIdClaim = identity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (_projectUpdate.Project.User.Username != userIdClaim.Value)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    uow.ProjectUpdateRepository.Delete(_projectUpdate  );
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
