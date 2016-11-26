using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class ProjectCommentModelToView : ProjectCommentModel
    {
        public bool IsRequestorProjectCommentCreator { get; set; }
        public string ProjectTitle { get; set; }
    }
}