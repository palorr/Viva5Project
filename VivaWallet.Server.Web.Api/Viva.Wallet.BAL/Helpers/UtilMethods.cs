using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva.Wallet.BAL.Helpers
{
    public static class UtilMethods
    {
        public static string GetHostUrl()
        {
            var requestURL = System.Web.HttpContext.Current.Request.Url;
            return requestURL.OriginalString.Replace(requestURL.AbsolutePath, "/");
        }
    }
}
