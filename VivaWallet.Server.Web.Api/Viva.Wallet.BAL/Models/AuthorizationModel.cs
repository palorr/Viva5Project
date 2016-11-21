using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class AuthorizationModel
    {
        public long RequestorId { get; set; }
        public long TargetId { get; set; }
        public string TargetType { get; set; }
        public bool isAllowed { get; set; }
    }
}
