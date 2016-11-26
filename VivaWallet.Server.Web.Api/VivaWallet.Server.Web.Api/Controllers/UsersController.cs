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
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        /*
         *  
         * USER ROUTES
         *
         */

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetAllUsers()
        {
            using (var s = new UserRepository())
            {
                var v = s.GetAllUsers();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{userId}")]
        public HttpResponseMessage GetUser(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new UserRepository())
            {
                var v = s.GetUser(userId);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [HttpPost]
        [Route("findByUsername")]
        public HttpResponseMessage GetUserByUsername(UserModel user)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest); ;

            using (var s = new UserRepository())
            {
                var v = s.GetUserByUsername(user.Username);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [HttpPut]
        [Route("")]
        public HttpResponseMessage UpdateUserMainInfo(UserModel user)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository())
            {
                var httpStatusCode = HttpStatusCode.OK;

                bool hasUpdated = s.UpdateUser(user, identity);

                switch (hasUpdated)
                {
                    //user not found
                    case false:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;
                        
                    //user updated ok
                    case true:
                        httpStatusCode = HttpStatusCode.OK;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        // OK
        [HttpDelete]
        [Route("{userId}")]
        public HttpResponseMessage DeactivateUserAccount(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository())
            {

                var httpStatusCode = HttpStatusCode.NoContent;

                UserRepository.StatusCodes hasDeleted = s.DeactivateUserAccount(identity, userId);

                switch (hasDeleted)
                {
                    //user not found
                    case UserRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to delete this user
                    case UserRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //user deleted ok
                    case UserRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.NoContent;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        /*
         *
         * USER PROJECTS ROUTES
         * 
         */

        // OK
        [HttpGet]
        [Route("myCreatedProjects")]
        public HttpResponseMessage GetCurrentLoggedInUserCreatedProjects()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository())
            {
                var v = s.GetCurrentLoggedInUserCreatedProjects(identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{userId}/userCreatedProjects")]
        public HttpResponseMessage GetUserCreatedProjects(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            
            using (var s = new UserRepository())
            {
                var v = s.GetUserCreatedProjects(userId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
        
        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{userId}/userBackedProjects")]
        public HttpResponseMessage GetUserFundedProjects(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new UserRepository())
            {
                var v = s.GetUserFundedProjects(userId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK - Show all in project list view
        [HttpGet]
        [Route("getUserFundedCompletedProjects/showAll")]
        public HttpResponseMessage GetUserFundedCompletedProjectsShowAll()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository())
            {
                var v = s.GetUserFundedCompletedProjects(identity, true);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK - Show first 5
        [HttpGet]
        [Route("getUserFundedCompletedProjects")]
        public HttpResponseMessage GetUserFundedCompletedProjectsShowTop5()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository())
            {
                var v = s.GetUserFundedCompletedProjects(identity, false);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getAllUsersByName/{searchTerm}")]
        public HttpResponseMessage GetAllUsersByName(string searchTerm)
        {
            using (var s = new UserRepository())
            {
                
                var v = s.GetByName(searchTerm);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [HttpGet]
        [Route("getAllMyFundedProjectsLatestUpdates")]
        public HttpResponseMessage GetCurrentUserFundedProjectsUpdates()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository())
            {
                var v = s.GetCurrentUserFundedProjectsLatestUpdates(identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
    }
}
