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
                var newProject = new ProjectModel()
                {
                    Title = "Test Project For Me",
                    Description = "Descripiton Lalalallala",
                    FundingEndDate = DateTime.Now.AddDays(20),
                    FundingGoal = 190000,
                    ProjectCategoryId = 5,
                    Status = "CRE",
                    OwnerId = 1
                };

                s.Insert(newProject);

                var v = s.GetAll();

                

                

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
    }
}
