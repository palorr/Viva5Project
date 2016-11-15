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
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        /*
         *  
         * USER ROUTES
         *
         */

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

                if (!v.Any<UserModel>())
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

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

        [HttpPost]
        [Route("")]
        public HttpResponseMessage CreateUser(UserModel user)
        {
            if (!ModelState.IsValid || user.Id.HasValue)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new UserRepository())
            {
                try
                { 
                    s.CreateUser(user);
                }
                catch(Exception e)
                {
                    throw;
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        [HttpPut]
        [Route("{userId}")]
        public HttpResponseMessage UpdateUserMainInfo(UserModel user, int userId)
        {
            if (!ModelState.IsValid || userId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserRepository())
            {
                var httpStatusCode = HttpStatusCode.OK;

                UserRepository.StatusCodes hasUpdated = s.UpdateUser(user, identity, userId);

                switch (hasUpdated)
                {
                    //user not found
                    case UserRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to update this user
                    case UserRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //user updated ok
                    case UserRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.OK;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

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
    }
}
