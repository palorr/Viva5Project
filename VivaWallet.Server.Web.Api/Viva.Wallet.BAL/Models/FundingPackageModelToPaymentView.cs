using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class FundingPackageModelToPaymentView : FundingPackageModel
    {
        public string ProjectTitle { get; set; }
        public string ProjectDescription { get; set; }
    }
}
