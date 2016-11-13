using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class ProjectCommentModel
    {
        public long? Id { get; set; }

        public long ProjectId { get; set; }

        public string Name { get; set; }

        public long UserId { get; set; }

        public long? AttachmentSetId { get; set; }

        public DateTime WhenDateTime { get; set; }

        public string Description { get; set; }
    }
}
