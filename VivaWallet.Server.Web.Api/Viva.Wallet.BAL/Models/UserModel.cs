using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Models
{
    public class UserModel
    {
        public long? Id { get; set; }

        public string Username { get; set; }

        public bool IsVerified { get; set; }

        public string VerificationToken { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }

        public string Name { get; set; }

        public string ShortBio { get; set; }

        public string AvatarImage { get; set; }
    }
}
