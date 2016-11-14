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
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

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
            
            if (!ModelState.IsValid || projectId <= 0 || commentId < 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectCommentRepository())
            {

                var httpStatusCode = HttpStatusCode.Created;
                
                //insert comment
                if(commentId.Equals(0))
                {
                    s.InsertComment(projectComment, projectId);
                }

                //update existing comment
                else
                {
                    int hasUpdated = s.UpdateComment(projectComment, identity, projectId, commentId);

                    switch(hasUpdated)
                    {
                        //comment not found
                        case 0:
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        //not authorized to update this comment
                        case 1:
                            httpStatusCode = HttpStatusCode.MethodNotAllowed;
                            break;
                        //comment updated ok
                        case 2:
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                    }
                }
                
                return Request.CreateResponse(httpStatusCode);
            }
        }
        
        [HttpDelete]
        [Route("{projectId}/comment/{commentId}")]
        public HttpResponseMessage DeleteComment(int projectId, int commentId)
        {
            if (projectId <= 0 || commentId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectCommentRepository())
            {

                var httpStatusCode = HttpStatusCode.NoContent;
                
                int hasDeleted = s.DeleteComment(identity, projectId, commentId);

                switch (hasDeleted)
                {
                    //comment not found
                    case 0:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;
                    //not authorized to delete this comment
                    case 1:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;
                    //comment deleted ok
                    case 2:
                        httpStatusCode = HttpStatusCode.NoContent;
                        break;
                }               

                return Request.CreateResponse(httpStatusCode);
            }
        }


    }
}
