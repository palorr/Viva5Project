using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class GlobalProjectStatsModel
    {
        public decimal NoOfTotalMoneyPledged { get; set; }
        public int NoOfTotalExternalShares { get; set; }
        public int NoOfTotalProjectComments { get; set; }
        public long NoOfTotalBackings { get; set; }
    }
}
