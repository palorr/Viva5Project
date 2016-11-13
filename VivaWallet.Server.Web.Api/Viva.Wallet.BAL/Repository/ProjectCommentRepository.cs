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
    public class ProjectCommentRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectCommentRepository()
        {
            uow = new UnitOfWork();
        }

        public string get()
        {

            var tp = uow.ProjectCommentreRepository.All();

            return null;
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

        public void Dispose()
        {
            uow.Dispose();
        }
    }
}
