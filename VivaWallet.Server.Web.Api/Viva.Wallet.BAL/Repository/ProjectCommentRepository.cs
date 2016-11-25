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
    public class ProjectCommentRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectCommentRepository()
        {
            uow = new UnitOfWork();
        }
        
        // OK
        public IList<ProjectCommentModel> GetAllProjectComments(int projectId)
        {
            return uow.ProjectCommentreRepository
                      .SearchFor(e => e.ProjectId == projectId)
                      .Select(e => new ProjectCommentModel()
                      {
                          Id = e.Id,
                          ProjectId = e.ProjectId,
                          UserId = e.UserId,
                          Name = e.User.Name,
                          AttachmentSetId = e.AttachmentSetId,
                          WhenDateTime = e.WhenDateTime,
                          Description = e.Description
                      }).OrderByDescending(e => e.WhenDateTime).ToList();
        }

        // OK
        public StatusCodes InsertComment(ProjectCommentModel source, ClaimsIdentity identity, int projectId)
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
                        throw new InvalidOperationException("User lookup for requestor Id for project comment creation failed", ex);
                    }

                    var _projectComment = new ProjectComment()
                    {
                        ProjectId = projectId,
                        UserId = requestorUserId,
                        AttachmentSetId = source.AttachmentSetId,
                        WhenDateTime = DateTime.Now,
                        Description = source.Description
                    };

                    uow.ProjectCommentreRepository.Insert(_projectComment, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public StatusCodes UpdateComment(ProjectCommentModel source, ClaimsIdentity identity, int commentId)
        {
            try
            {
                var _projectComment = uow.ProjectCommentreRepository.FindById(commentId);

                if(_projectComment == null)
                {
                    //comment not found
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // comment found. does the user that wishes to update it really is the comment creator? check this here
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
                        throw new InvalidOperationException("User lookup for requestor Id for project comment edit failed", ex);
                    }

                    if (_projectComment.UserId != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }
                    
                    _projectComment.WhenDateTime = DateTime.Now;
                    _projectComment.Description = source.Description;
                    
                    uow.ProjectCommentreRepository.Update(_projectComment, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public StatusCodes DeleteComment(ClaimsIdentity identity, int commentId)
        {
            try
            {
                var _projectComment = uow.ProjectCommentreRepository.FindById(commentId);

                //comment not found
                if (_projectComment == null)
                {
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // comment found. does the user that wishes to delete it really is the comment creator? check this here
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
                        throw new InvalidOperationException("User lookup for requestor Id for project comment delete failed", ex);
                    }

                    if (_projectComment.UserId != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    uow.ProjectCommentreRepository.Delete(_projectComment);
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
