﻿using System;
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
    [RoutePrefix("api/projects")]
    public class ProjectsController : ApiController
    {

        /*
         *  
         * PROJECT ROUTES
         *
         */

        [AllowAnonymous]
        [HttpGet]
        [Route("projectCategories")]
        public HttpResponseMessage GetProjectCategories()
        {
            using (var s = new ProjectCategoryRepository())
            {
                var v = s.GetAll();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //OK
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

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}")]
        public HttpResponseMessage GetProjectByIdForAllUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new ProjectRepository())
            {
                var v = s.GetProjectById(projectId);

                if(v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [HttpGet]
        [Route("myProjects/{projectId}")]
        public HttpResponseMessage GetMyProjectsById(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectRepository())
            {
                var v = s.GetProjectById(projectId, identity);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [HttpPost]
        [Route("")]
        public HttpResponseMessage CreateProject(ProjectModel project)
        {
            //STEP 1 - Create the Project from Project Model coming from the client
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectRepository())
            {
                long newProjectId = s.Insert(project, identity);

                //STEP 2 - Create Project Stats Screen and Save to ProjectStats Table
                using (var sr = new ProjectStatRepository())
                {
                    bool statCreated = sr.CreateProjectStat((int)newProjectId);
                    if(!statCreated)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }

                //STEP 3 - Create new Project Funding Package for Donations
                using (var fpRepo = new FundingPackageRepository())
                {
                    FundingPackageModel newFundingPackageModel = new FundingPackageModel();
                    newFundingPackageModel.AttachmentSetId = null;
                    newFundingPackageModel.Title = "Donations Funding Package";
                    newFundingPackageModel.Description = "Feel free to donate whatever amount you wish!";

                    fpRepo.CreateFundingPackage(newFundingPackageModel, identity, (int)newProjectId, true);
                }

                return Request.CreateResponse(HttpStatusCode.Created, newProjectId);
            }
        }

        /*
         *  
         * PROJECT UPDATES ROUTES
         *
         */

        // OK
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

        // OK
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
                    ProjectUpdateRepository.StatusCodes hasUpdated = s.EditProjectUpdate(projectUpdate, identity, updateId);

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

        // OK
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

        // OK
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

        // OK
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
                    ProjectCommentRepository.StatusCodes hasInserted = s.InsertComment(projectComment, identity, projectId);

                    switch (hasInserted)
                    {
                        //project not found to insert the comment
                        case ProjectCommentRepository.StatusCodes.NOT_FOUND:
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;

                        //comment inserted ok
                        case ProjectCommentRepository.StatusCodes.OK:
                            httpStatusCode = HttpStatusCode.Created;
                            break;
                    }
                }

                //update existing comment
                else
                {
                    ProjectCommentRepository.StatusCodes hasUpdated = s.UpdateComment(projectComment, identity, commentId);

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

        // OK
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

        /*
         * 
         * PROJECT FUNDING PACKAGES ROUTES
         * 
         * */

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/fundingPackages")]
        public HttpResponseMessage GetProjectFundingPackages(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new FundingPackageRepository())
            {
                var v = s.GetAllProjectFundingPackages(projectId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/fundingPackages/{fundingPackageId}")]
        public HttpResponseMessage GetSpecificFundingPackageFromProject(int projectId, int fundingPackageId)
        {
            if (projectId <= 0 || fundingPackageId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new FundingPackageRepository())
            {
                var v = s.GetProjectFundingPackageById(fundingPackageId);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [HttpPost]
        [Route("{projectId}/fundingPackages")]
        public HttpResponseMessage CreateFundingPackage(FundingPackageModel fundingPackage, int projectId)
        {
            if (!ModelState.IsValid || projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new FundingPackageRepository())
            {

                var httpStatusCode = HttpStatusCode.Created;

                FundingPackageRepository.StatusCodes hasInserted = s.CreateFundingPackage(fundingPackage, identity, projectId, false);

                switch (hasInserted)
                {
                    //project for inserting the funding package not found
                    case FundingPackageRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to add this funding package to this specific project
                    case FundingPackageRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //funding package inserted ok
                    case FundingPackageRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.Created;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        // OK
        [HttpPut]
        [Route("{projectId}/fundingPackages/{fundingPackageId}")]
        public HttpResponseMessage UpdateFundingPackage(FundingPackageModel fundingPackage, int projectId, int fundingPackageId)
        {
            if (!ModelState.IsValid || projectId <= 0 || fundingPackageId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new FundingPackageRepository())
            {
                var httpStatusCode = HttpStatusCode.OK;

                FundingPackageRepository.StatusCodes hasUpdated = s.EditFundingPackage(fundingPackage, identity, projectId, fundingPackageId);

                switch (hasUpdated)
                {
                    //project or funding package not found
                    case FundingPackageRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to update this funding package. You are not the project creator that has this specific funding package
                    case FundingPackageRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //funding package updated ok
                    case FundingPackageRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.OK;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        // OK
        [HttpDelete]
        [Route("{projectId}/fundingPackages/{fundingPackageId}")]
        public HttpResponseMessage DeleteFundingPackage(int projectId, int fundingPackageId)
        {
            if (projectId <= 0 || fundingPackageId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new FundingPackageRepository())
            {

                var httpStatusCode = HttpStatusCode.NoContent;

                FundingPackageRepository.StatusCodes hasDeleted = s.DeleteFundingPackage(identity, projectId, fundingPackageId);

                switch (hasDeleted)
                {
                    //project not found or funding package to delete not found
                    case FundingPackageRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to delete this funding package - either you are not the project creator or this is a donations package automatically created by the system
                    case FundingPackageRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //funding package deleted ok
                    case FundingPackageRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.NoContent;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

    }
}
