using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class AdminPanelViewModel
    {
        public int NoOfTotalUsers { get; set; }
        public int NoOfTotalProjectUpdates { get; set; }
        public IList<ProjectCountByCategoryModel> NoOfProjectsPerCategory { get; set; }
        public IList<ProjectCountByStatusModel> NoOfProjectsPerStatus { get; set; }
        public GlobalProjectStatsModel GlobalProjectStats { get; set; }
    }
}
