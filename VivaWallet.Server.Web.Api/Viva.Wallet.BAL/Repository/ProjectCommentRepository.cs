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
    public class ProjectCommentRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectCommentRepository()
        {
            uow = new UnitOfWork();
        }
        
        public IList<ProjectCommentModel> GetAllProjectComments(int projectId)
        {
            return uow.ProjectCommentreRepository.All()?
                    .Where(e => e.ProjectId == projectId)
                    .Select(e => new ProjectCommentModel()
            {
                Id = e.Id,
                ProjectId = e.ProjectId,
                UserId = e.UserId,
                Name = e.User.Name,
                AttachmentSetId = e.AttachmentSetId,
                WhenDateTime = e.WhenDateTime,
                Description = e.Description
            }).ToList();
        }

        public void InsertComment(ProjectCommentModel source, int projectId)
        {

            try
            {
                var _projectComment = new ProjectComment()
                {
                    ProjectId = projectId,
                    UserId = source.UserId,
                    AttachmentSetId = source.AttachmentSetId,
                    WhenDateTime = DateTime.Now,
                    Description = source.Description
                };
                
                uow.ProjectCommentreRepository.Insert(_projectComment, true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int UpdateComment(ProjectCommentModel source, ClaimsIdentity identity,int projectId, int commentId)
        {
            try
            {
                var _projectComment = uow.ProjectCommentreRepository.FindById(commentId);

                if(_projectComment == null)
                {
                    return (int)StatusCodes.NOT_FOUND;
                }
                else
                {
                    // comment found. does the user that wishes to update it really is the comment creator? check this here
                    var userIdClaim = identity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                    
                    if (_projectComment.UserId.ToString() != userIdClaim.Value)
                    {
                        return (int)StatusCodes.NOT_AUTHORIZED;
                    }
                    
                    _projectComment.WhenDateTime = DateTime.Now;
                    _projectComment.Description = source.Description;
                    
                    uow.ProjectCommentreRepository.Update(_projectComment, true);
                }

                return (int)StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int DeleteComment(ClaimsIdentity identity, int projectId, int commentId)
        {
            try
            {
                var _projectComment = uow.ProjectCommentreRepository.FindById(commentId);

                //comment not found
                if (_projectComment == null)
                {
                    return (int)StatusCodes.NOT_FOUND;
                }
                else
                {
                    // comment found. does the user that wishes to delete it really is the comment creator? check this here
                    var userIdClaim = identity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (_projectComment.UserId.ToString() != userIdClaim.Value)
                    {
                        return (int)StatusCodes.NOT_AUTHORIZED;
                    }

                    uow.ProjectCommentreRepository.Delete(_projectComment);
                }

                return (int)StatusCodes.OK;
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
