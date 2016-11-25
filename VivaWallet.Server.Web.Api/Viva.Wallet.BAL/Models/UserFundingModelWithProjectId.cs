using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class UserFundingModelWithProjectId : UserFundingModel
    {
        public long ProjectId { get; set; }
    }
}
