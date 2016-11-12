using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivaWallet.DAL
{
    interface IUnitOfWork
    {

        void SaveChanges(bool save = true);
        Task  SaveChangesAsync(bool save = true);

    }
}
