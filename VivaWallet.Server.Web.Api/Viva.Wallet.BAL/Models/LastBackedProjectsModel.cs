using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class LastBackedProjectsModel
    {
        public long FundingId { get; set; }

        public long ProjectId { get; set; }

        public long UserId { get; set; }

        public decimal? AmountPaid { get; set; }

        public DateTime WhenDateTime { get; set; }

        public string UserName { get; set; }

        public string ProjectTitle { get; set; }
    }
}
