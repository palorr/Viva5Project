﻿using System;
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
    [RoutePrefix("api/projects")]
    public class ProjectController : ApiController
    {

        /*
         *  
         * PROJECT ROUTES
         *
         */

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetAllProjects()
        {
            using (var s = new ProjectRepository())
            {
                var v = s.GetAll();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}")]
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

        /*
         *  
         * PROJECT UPDATES ROUTES
         *
         */

        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/updates")]
        public HttpResponseMessage GetProjectUpdates(int projectId)
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
        [Route("{projectId}/updates/{updateId?}")]
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
                    ProjectUpdateRepository.StatusCodes hasInserted = s.InsertProjectUpdate(projectUpdate, identity, projectId);

                    switch (hasInserted)
                    {
                        //project creator not found
                        case ProjectUpdateRepository.StatusCodes.NOT_FOUND:
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;

                        //not authorized to insert a project update to this project - you are not the project creator
                        case ProjectUpdateRepository.StatusCodes.NOT_AUTHORIZED:
                            httpStatusCode = HttpStatusCode.MethodNotAllowed;
                            break;

                        //project update inserted ok
                        case ProjectUpdateRepository.StatusCodes.OK:
                            httpStatusCode = HttpStatusCode.Created;
                            break;
                    }
                }

                //update existing project update
                else
                {
                    ProjectUpdateRepository.StatusCodes hasUpdated = s.EditProjectUpdate(projectUpdate, identity, projectId, updateId);

                    switch (hasUpdated)
                    {
                        //project update not found
                        case ProjectUpdateRepository.StatusCodes.NOT_FOUND:
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;

                        //not authorized to update this project update
                        case ProjectUpdateRepository.StatusCodes.NOT_AUTHORIZED:
                            httpStatusCode = HttpStatusCode.MethodNotAllowed;
                            break;

                        //project update edited ok
                        case ProjectUpdateRepository.StatusCodes.OK:
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                    }
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        [HttpDelete]
        [Route("{projectId}/updates/{updateId}")]
        public HttpResponseMessage DeleteProjectUpdate(int projectId, int updateId)
        {
            if (projectId <= 0 || updateId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectUpdateRepository())
            {

                var httpStatusCode = HttpStatusCode.NoContent;

                ProjectUpdateRepository.StatusCodes hasDeleted = s.DeleteProjectUpdate(identity, updateId);

                switch (hasDeleted)
                {
                    //project update not found
                    case ProjectUpdateRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to delete this project update
                    case ProjectUpdateRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //project update deleted ok
                    case ProjectUpdateRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.NoContent;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        /*
         *  
         * PROJECT COMMENTS ROUTES
         *
         */

        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/comments")]
        public HttpResponseMessage GetProjectComments(int projectId)
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
        [Route("{projectId}/comments/{commentId?}")]
        public HttpResponseMessage InsertOrUpdateComment(ProjectCommentModel projectComment, int projectId, int commentId = 0)
        {

            if (!ModelState.IsValid || projectId <= 0 || commentId < 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectCommentRepository())
            {

                var httpStatusCode = HttpStatusCode.Created;

                //insert comment
                if (commentId.Equals(0))
                {
                    s.InsertComment(projectComment, projectId);
                }

                //update existing comment
                else
                {
                    ProjectCommentRepository.StatusCodes hasUpdated = s.UpdateComment(projectComment, identity, projectId, commentId);

                    switch (hasUpdated)
                    {
                        //comment not found
                        case ProjectCommentRepository.StatusCodes.NOT_FOUND:
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;

                        //not authorized to update this comment
                        case ProjectCommentRepository.StatusCodes.NOT_AUTHORIZED:
                            httpStatusCode = HttpStatusCode.MethodNotAllowed;
                            break;

                        //comment updated ok
                        case ProjectCommentRepository.StatusCodes.OK:
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                    }
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        [HttpDelete]
        [Route("{projectId}/comments/{commentId}")]
        public HttpResponseMessage DeleteComment(int projectId, int commentId)
        {
            if (projectId <= 0 || commentId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectCommentRepository())
            {

                var httpStatusCode = HttpStatusCode.NoContent;

                ProjectCommentRepository.StatusCodes hasDeleted = s.DeleteComment(identity, commentId);

                switch (hasDeleted)
                {
                    //comment not found
                    case ProjectCommentRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to delete this comment
                    case ProjectCommentRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //comment deleted ok
                    case ProjectCommentRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.NoContent;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

    }
}