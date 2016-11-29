using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class ProjectUpdateModelToView : ProjectUpdateModel
    {
        public bool IsRequestorProjectCreator { get; set; }
    }
}
