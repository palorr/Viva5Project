using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class AttachmentModel
    {
        public long? Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string FilePath { get; set; }

        public string Name { get; set; }

        public string HtmlCode { get; set; }

        public int OrderNo { get; set; }

    }
}
