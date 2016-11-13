using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Viva.Wallet.BAL;
using Viva.Wallet.BAL.Models;

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

        [HttpPost]
        [Route("{projectId}/comment/{commentId?}")]
        public HttpResponseMessage InsertOrUpdateComment(ProjectCommentModel projectComment, int projectId, int commentId = 0)
        {
            
            var identity = User.Identity as ClaimsIdentity;

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new ProjectCommentRepository())
            {

                var httpStatusCode = HttpStatusCode.Created;
                
                if(commentId.Equals(0))
                {
                    s.InsertComment(projectComment, projectId);
                }
                else
                {
                    bool hasUpdated = s.UpdateComment(projectComment, projectId, commentId);

                    if (hasUpdated)
                    {
                        httpStatusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        httpStatusCode = HttpStatusCode.NotFound;
                    }
                }
                
                return Request.CreateResponse(httpStatusCode);
            }
        }


    }
}
