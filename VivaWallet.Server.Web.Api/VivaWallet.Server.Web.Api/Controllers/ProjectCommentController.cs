using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Viva.Wallet.BAL;

namespace VivaWallet.Server.Web.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/project")]
    public class ProjectCommentController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/comments")]
        public HttpResponseMessage Get(int projectId)
        {
            //var identity = User.Identity as ClaimsIdentity;

            //return identity.Name;

            using (var s = new ProjectCommentRepository())
            {

                var v = s.GetAllProjectComments(projectId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
    }
}
