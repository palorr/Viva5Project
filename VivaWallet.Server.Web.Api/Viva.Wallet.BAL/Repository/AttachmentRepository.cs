using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL.Repository
{
   public class AttachmentRepository : IDisposable
    {
        protected IUnitOfWork uow;

        public AttachmentRepository(IUnitOfWork _uow)
        {
            if (_uow == null)
                uow = new UnitOfWork();
            else
                uow = _uow;
        }

        public async Task saveAttachment(string username, long projectId, AttachmentModel newAttachemt)
        {
            try
            {
                var userId = uow.UserRepository.SearchFor(e => e.Username == username).FirstOrDefault();

                var projectOfuser = uow.ProjectRepository.SearchFor(e => e.Id == projectId && e.UserId == userId.Id).FirstOrDefault();

                if (projectOfuser == null)
                {
                    throw new Exception("Project do not belong to user");
                }

                var attachmentSetId = projectOfuser.AttachmentSetId;

                var _newAttachment = new Attachment();
                _newAttachment.AttachementSetId = attachmentSetId.Value;
                _newAttachment.FilePath = newAttachemt.FilePath;
                _newAttachment.HtmlCode = newAttachemt.HtmlCode;
                _newAttachment.Name = newAttachemt.Name;
                _newAttachment.OrderNo = newAttachemt.OrderNo;
                _newAttachment.CreatedDateTime = DateTime.Now;
                uow.AttachmentRepository.Insert(_newAttachment, false);

                await uow.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public IList<AttachmentModel> GetProjectAttachmets(long projectId)
        {
            try
            {
                var s = System.Web.HttpContext.Current.Request.Url;
                var publicurl = s.OriginalString.Replace(s.AbsolutePath, "/");
                

                var userProject = uow.ProjectRepository.SearchFor(e => e.Id == projectId ).FirstOrDefault();

                if (userProject == null)
                    throw new InvalidOperationException();

                var attachmentList = uow.AttachmentRepository.SearchFor(e => e.AttachementSetId == userProject.AttachmentSetId)
                    .Select(e => new AttachmentModel()
                    {
                        Id = e.Id,
                        CreatedAt = e.CreatedDateTime,
                        FilePath = e.FilePath != null ? e.FilePath.Replace("D:\\home\\site\\wwwroot\\" , publicurl) : null,
                        HtmlCode = e.HtmlCode,
                        Name = e.Name,
                        OrderNo = e.OrderNo

                    }).OrderBy(g => g.OrderNo).ToList();

                return attachmentList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteAttachment(string username , long attachmentId)
        {
            try
            {
                var user = uow.UserRepository.SearchFor(e => e.Username == username).SingleOrDefault();

                if(user == null)
                {
                    throw new UnauthorizedAccessException("User does not exists");
                }

                var attachmentToDelete = uow.AttachmentRepository.FindById(attachmentId);

                if(attachmentToDelete != null)
                {
                    var userOwner = uow.ProjectRepository
                        .SearchFor(e => e.AttachmentSetId == attachmentToDelete.AttachementSetId)
                        .Select(e=>e.User)
                        .FirstOrDefault();
                    if (userOwner != null &&  user.Id == userOwner.Id)
                    {
                        await uow.AttachmentRepository.DeleteAsync(attachmentToDelete ,true);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Dispose()
        {
           
        }
    }
    
    
}
