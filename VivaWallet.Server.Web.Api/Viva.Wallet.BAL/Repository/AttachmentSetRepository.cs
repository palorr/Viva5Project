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
    public class AttachmentSetRepository : IDisposable
    {
        protected IUnitOfWork uow;

        public AttachmentSetRepository(IUnitOfWork _uow)
        {
            if (_uow == null)
                uow = new UnitOfWork();
            else
                uow = _uow;
        }

        public long CreateAttachmentSet()
        {
            try
            {
                var _attachmentSet = new AttachmentSet()
                {
                    CreatedDateTime = DateTime.Now,
                    IsUsed = true
                };

                uow.AttachmentSetRepository.Insert(_attachmentSet, true);
                
                return _attachmentSet.Id;
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
