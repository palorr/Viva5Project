using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Viva.Wallet.BAL;
using Viva.Wallet.BAL.Models;
using Viva.Wallet.BAL.Repository;

namespace VivaWallet.Server.Web.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/info")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("test")]
        public HttpResponseMessage Get()
        {
            var identity = User.Identity as ClaimsIdentity;

            //return identity.Name;

            using(var s = new ProjectRepository())
            {

                var v = s.GetAll();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("projectCategories")]
        public HttpResponseMessage GetProjectCategories()
        {
            using (var s = new ProjectCategoryRepository())
            {

                var v = s.GetAll();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
    }
    
}
