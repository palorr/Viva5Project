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
        private IUnitOfWork uow;
        public UsersController()
        {
            uow = new UnitOfWork();
        }
        /*
         *  
         * USER ROUTES
         *
         */

        //GET ALL REGISTERED USERS - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetAllUsers()
        {
            using (var s = new UserRepository(uow))
            {
                var v = s.GetAllUsers();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET LAST 10 REGISTERED USERS - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("lastTen")]
        public HttpResponseMessage GetLastTenRegisteredUsers()
        {
            using (var s = new UserRepository(uow))
            {
                var v = s.GetLastTenRegisteredUsers();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET SPECIFIC USER BY ID - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{userId}")]
        public HttpResponseMessage GetUser(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new UserRepository(uow))
            {
                var v = s.GetUser(userId);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET SPECIFIC USER BY USERNAME - OK
        [HttpPost]
        [Route("findByUsername")]
        public HttpResponseMessage GetUserByUsername(UserModel user)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest); ;

            using (var s = new UserRepository(uow))
            {
                var v = s.GetUserByUsername(user.Username);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //UPDATE USER MAIN INFO - OK
        [HttpPut]
        [Route("")]
        public HttpResponseMessage UpdateUserMainInfo(UserModel user)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository(uow))
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

        //DEACTIVATE USER ACCOUNT - OK
        [HttpDelete]
        [Route("{userId}")]
        public HttpResponseMessage DeactivateUserAccount(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository(uow))
            {

                var httpStatusCode = HttpStatusCode.NoContent;

                UserRepository.StatusCodes hasDeleted = s.DeactivateUserAccount(identity, userId);

                switch (hasDeleted)
                {
                    //user not found
                    case UserRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to delete this user - requestor not this user
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

        //GET CURRENT LOGGED IN USER CREATED PROJECTS - OK
        [HttpGet]
        [Route("myCreatedProjects")]
        public HttpResponseMessage GetCurrentLoggedInUserCreatedProjects()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository(uow))
            {
                var v = s.GetCurrentLoggedInUserCreatedProjects(identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET SPECIFIC USER'S CREATED PROJECTS - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{userId}/userCreatedProjects")]
        public HttpResponseMessage GetUserCreatedProjects(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            
            using (var s = new UserRepository(uow))
            {
                var v = s.GetUserCreatedProjects(userId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
        
        //GET SPECIFIC USER'S BACKED PROJECTS - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{userId}/userBackedProjects")]
        public HttpResponseMessage GetUserFundedProjects(int userId)
        {
            if (userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new UserRepository(uow))
            {
                var v = s.GetUserFundedProjects(userId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET ALL CURRENT LOGGED IN USER FUNDED COMPLETED PROJECTS - OK - Show all in project list view
        [HttpGet]
        [Route("getUserFundedCompletedProjects/showAll")]
        public HttpResponseMessage GetUserFundedCompletedProjectsShowAll()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository(uow))
            {
                var v = s.GetUserFundedCompletedProjects(identity, true);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET LAST RECENT 5 LOGGED IN USER FUNDED PROJECTS - OK - Show first 5
        [HttpGet]
        [Route("getUserFundedCompletedProjects")]
        public HttpResponseMessage GetUserFundedCompletedProjectsShowTop5()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository(uow))
            {
                var v = s.GetUserFundedCompletedProjects(identity, false);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET ALL USERS BY NAME - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("getAllUsersByName/{searchTerm}")]
        public HttpResponseMessage GetAllUsersByName(string searchTerm)
        {
            using (var s = new UserRepository(uow))
            {
                
                var v = s.GetByName(searchTerm);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET ALL CURRENT LOGGED IN USER'S FUNDED PROJECTS LATEST UPDATES - OK
        [HttpGet]
        [Route("getAllMyFundedProjectsLatestUpdates")]
        public HttpResponseMessage GetCurrentUserFundedProjectsUpdates()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository(uow))
            {
                var v = s.GetCurrentUserFundedProjectsLatestUpdates(identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET ADMIN PANEL INFORMATION FOR ADMIN USERS ONLY - OK
        [HttpGet]
        [Route("getAdminPanelInfo")]
        public HttpResponseMessage GetAdminPanelInfo()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository(uow))
            {
                var v = s.GetAdminPanelInfo(identity);

                if(v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, "I am sorry, you do not have permission to view this page. You are not a CrowdVoice admin.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }
    }
}
