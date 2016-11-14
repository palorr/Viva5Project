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
    [RoutePrefix("api")]
    public class ProjectController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("projects")]
        public HttpResponseMessage Get()
        {
            using (var s = new ProjectRepository())
            {
                var v = s.GetAll();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("project/{projectId}")]
        public HttpResponseMessage GetProjectById(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new ProjectRepository())
            {
                var v = s.GetProjectById(projectId);

                if(!v.Any<ProjectModel>())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
        
    }
}
