using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
   public class ProjectModel
    {

        public long? Id { get; set; }

        public long ProjectCategoryId { get; set; }

        public string ProjectCategoryDesc { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime FundingEndDate { get; set; }

        public decimal FundingGoal { get; set; }

        public string Status { get; set; }

        public string OwnerName { get; set; }

        public long? OwnerId { get; set; }


    }
}
