﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public bool UpdateComment(ProjectCommentModel source, int projectId, int commentId)
        {
            try
            {
                var _projectComment = uow.ProjectCommentreRepository.FindById(commentId);

                if(_projectComment != null)
                {
                    _projectComment.WhenDateTime = DateTime.Now;
                    _projectComment.Description = source.Description;
                }
                else
                {
                    return false;
                }

                uow.ProjectCommentreRepository.Update(_projectComment, true);

                return true;
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
    }
}
