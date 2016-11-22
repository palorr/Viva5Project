using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class ProjectStatModelToView : ProjectStatModel
    {
        public bool IsRequestorProjectCreator { get; set; }
        public decimal FundingGoal { get; set; }
        public DateTime FundingEndDate { get; set; }
    }
}
