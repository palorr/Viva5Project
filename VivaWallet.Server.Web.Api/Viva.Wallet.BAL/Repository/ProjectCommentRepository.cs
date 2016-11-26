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

        private bool IsRequestorProjectCommentCreator(long commentId, long requestorId)
        {
            //represent logged out users with 0
            if (requestorId == 0)
                return false;

            //find project comment and check if the current user is the creator or not
            var _comment = uow.ProjectCommentreRepository.FindById(commentId);

            if (_comment.UserId == requestorId)
                return true;

            return false;
        }

        // OK
        public ProjectCommentModelToView GetProjectCommentById(int projectId, int commentId, ClaimsIdentity identity)
        {
            bool isRequestorProjectCommentCreator = false;

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
                throw new InvalidOperationException("User lookup for requestor Id for get all project comments failed", ex);
            }

            isRequestorProjectCommentCreator = this.IsRequestorProjectCommentCreator((long)commentId, requestorUserId);

            try
            {
                return uow.ProjectCommentreRepository
                          .SearchFor(e => (e.ProjectId == projectId && e.Id == commentId))
                          .Select(e => new ProjectCommentModelToView()
                          {
                              Id = e.Id,
                              ProjectId = e.ProjectId,
                              Name = e.User.Name,
                              UserId = e.UserId,
                              AttachmentSetId = e.AttachmentSetId,
                              Description = e.Description,
                              WhenDateTime = e.WhenDateTime,
                              ProjectTitle = e.Project.Title,
                              IsRequestorProjectCommentCreator = isRequestorProjectCommentCreator
                          }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Project comment id lookup failed", ex);
            }
        }

        // OK
        public IList<ProjectCommentModelToView> GetAllProjectComments(long projectId, ClaimsIdentity identity = null)
        {
            //represent logged out users with 0
            long requestorUserId = 0;

            if (identity != null)
            {
                try
                {
                    requestorUserId = uow.UserRepository
                                         .SearchFor(e => e.Username == identity.Name)
                                         .Select(e => e.Id)
                                         .SingleOrDefault();
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("User lookup for requestor Id for get all project comments failed", ex);
                }
                
            }

            return uow.ProjectCommentreRepository
                        .SearchFor(e => e.ProjectId == projectId)
                        .Select(e => new ProjectCommentModelToView()
                        {
                            Id = e.Id,
                            ProjectId = e.ProjectId,
                            Name = e.User.Name,
                            UserId = e.UserId,
                            AttachmentSetId = e.AttachmentSetId,
                            Description = e.Description,
                            WhenDateTime = e.WhenDateTime,
                            ProjectTitle = e.Project.Title,
                            IsRequestorProjectCommentCreator = this.IsRequestorProjectCommentCreator(e.Id, requestorUserId)
                        }).OrderByDescending(e => e.WhenDateTime).ToList();

        }

        // OK
        public IList<ProjectCommentModelToView> GetAllCurrentUserCreatedProjectComments(ClaimsIdentity identity)
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
                throw new InvalidOperationException("User lookup for requestor Id for get all project comments failed", ex);
            }

            //get current user created projects comments
            using (var ctx = new VivaWalletEntities())
            {

                IOrderedQueryable<ProjectCommentModelToView> tmp = 
                    ctx
                    .ProjectComments
                    .Join(ctx.Projects, pc => pc.ProjectId, pr => pr.Id, (pc, pr) => new { pc, pr })
                    .Join(ctx.Users, pcpr => pcpr.pc.UserId, us => us.Id, (pcpr, us) => new { pcpr.pc, pcpr.pr, us })
                    .Where(pcprus => (pcprus.pr.UserId == requestorUserId))
                    .Select(pcprus => new ProjectCommentModelToView()
                    {
                        Id = pcprus.pc.Id,
                        ProjectId = pcprus.pc.ProjectId,
                        Name = pcprus.us.Name,
                        UserId = pcprus.pc.UserId,
                        AttachmentSetId = pcprus.pc.AttachmentSetId,
                        Description = pcprus.pc.Description,
                        WhenDateTime = pcprus.pc.WhenDateTime,
                        ProjectTitle = pcprus.pr.Title,
                        IsRequestorProjectCommentCreator = false //all false because I want this for a list view in home page - every user can edit their comments in the project profile page in comments tab
                    }).OrderByDescending(e => e.WhenDateTime);

                IEnumerable<ProjectCommentModelToView> currentUserCreatedProjectComments = tmp.AsEnumerable();

                return currentUserCreatedProjectComments.ToList();
            }

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
