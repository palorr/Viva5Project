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

        public static long GetCurrentUserId(UnitOfWork uow, string username)
        {
            try
            {
                return uow
                        .UserRepository
                        .SearchFor(e => e.Username == username)
                        .Select(e => e.Id)
                        .SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup for current logged in User Id failed", ex);
            }
        }
    }
}
