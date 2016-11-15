using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class ProjectUpdateModel
    {
        public long? Id { get; set; }

        public long ProjectId { get; set; }
        
        public long? AttachmentSetId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime WhenDateTime { get; set; }
    }
}
