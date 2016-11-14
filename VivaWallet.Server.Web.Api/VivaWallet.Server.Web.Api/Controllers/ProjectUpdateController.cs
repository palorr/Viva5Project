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
    public class ProjectUpdateController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/updates")]
        public HttpResponseMessage Get(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new ProjectUpdateRepository())
            {
                var v = s.GetAllProjectUpdates(projectId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [HttpPost]
        [Route("{projectId}/update/{updateId?}")]
        public HttpResponseMessage InsertOrEditProjectUpdate(ProjectUpdateModel projectUpdate, int projectId, int updateId = 0)
        {

            if (!ModelState.IsValid || projectId <= 0 || updateId < 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectUpdateRepository())
            {

                var httpStatusCode = HttpStatusCode.Created;

                //insert project update
                if (updateId.Equals(0))
                {
                    int hasInserted = s.InsertProjectUpdate(projectUpdate, identity, projectId);

                    switch (hasInserted)
                    {
                        //project creator not found
                        case 0:
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        //not authorized to insert a project update to this project - you are not the project creator
                        case 1:
                            httpStatusCode = HttpStatusCode.MethodNotAllowed;
                            break;
                        //project update inserted ok
                        case 2:
                            httpStatusCode = HttpStatusCode.Created;
                            break;
                    }
                }

                //update existing project update
                else
                {
                    int hasUpdated = s.EditProjectUpdate(projectUpdate, identity, projectId, updateId);

                    switch (hasUpdated)
                    {
                        //project update not found
                        case 0:
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        //not authorized to update this project update
                        case 1:
                            httpStatusCode = HttpStatusCode.MethodNotAllowed;
                            break;
                        //project update edited ok
                        case 2:
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                    }
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        [HttpDelete]
        [Route("{projectId}/comment/{updateId}")]
        public HttpResponseMessage DeleteComment(int projectId, int updateId)
        {
            if (projectId <= 0 || updateId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectUpdateRepository())
            {

                var httpStatusCode = HttpStatusCode.NoContent;

                int hasDeleted = s.DeleteProjectUpdate(identity, updateId);

                switch (hasDeleted)
                {
                    //project update not found
                    case 0:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;
                    //not authorized to delete this project update
                    case 1:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;
                    //project update deleted ok
                    case 2:
                        httpStatusCode = HttpStatusCode.NoContent;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }
    }
}
