using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class AttachmentSetModel
    {
        public long? Id { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public bool IsUsed { get; set; }
     }
}
