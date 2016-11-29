using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivaWallet.DAL
{
    public class EntityConfig : DbConfiguration
    {
        public EntityConfig()
        {
            AddInterceptor(new SoftDeleteInterceptor());
        }
    }
}
