using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class LastBackedProjectsModel
    {
        public long fundingId { get; set; }

        public long projectId { get; set; }

        public long userId { get; set; }

        public decimal? AmountPaid { get; set; }

        public DateTime WhenDateTime { get; set; }

        public string userName { get; set; }

        public string projectTitle { get; set; }
    }
}
