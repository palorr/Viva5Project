using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class ProjectExternalShareModel
    {
        public long? Id { get; set; }

        public long ProjectId { get; set; }

        public long UserId { get; set; }

        public string Target { get; set; }

        public string Source { get; set; }

        public DateTime WhenDateTime { get; set; }
    }
}
