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
    [RoutePrefix("api/projects")]
    public class ProjectsController : ApiController
    {

        /*
         *  
         * PROJECT ROUTES
         *
         */
        
        // OK
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

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("getAllProjectsByCategory/{categoryId}")]
        public HttpResponseMessage GetAllProjectsByCategory(int categoryId)
        {
            using (var s = new ProjectRepository())
            {
                s.CheckForFailedProjects();

                var v = s.GetByCategoryId((long)categoryId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getAllProjectsByName/{searchTerm}")]
        public HttpResponseMessage GetAllProjectsByName(string searchTerm)
        {
            using (var s = new ProjectRepository())
            {
                s.CheckForFailedProjects();

                var v = s.GetByName(searchTerm);

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
                s.CheckForFailedProjects();

                var v = s.GetAll();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //OK
        [AllowAnonymous]
        [HttpGet]
        [Route("trending")]
        public HttpResponseMessage GetTrendingProjects()
        {
            using (var s = new ProjectRepository())
            {
                s.CheckForFailedProjects();

                var v = s.GetTrendingProjects();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/allowAll")]
        public HttpResponseMessage GetProjectByIdForLoggedOutUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            
            using (var s = new ProjectRepository())
            {
                s.CheckForFailedProjects();

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
        [Route("{projectId}")]
        public HttpResponseMessage GetProjectByIdForLoggedInUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectRepository())
            {
                s.CheckForFailedProjects();

                var v = s.GetProjectById(projectId, identity);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/stats/allowAll")]
        public HttpResponseMessage GetProjectStatsByProjectIdForLoggedOutUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            
            using (var s = new ProjectStatRepository())
            {
                var v = s.GetProjectStats(projectId);

                if (v == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [HttpGet]
        [Route("{projectId}/stats")]
        public HttpResponseMessage GetProjectStatsByProjectIdForLoggedInUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectStatRepository())
            {
                var v = s.GetProjectStats(projectId, identity);

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
                //STEP 1 - Create new Attachment Set and assign it to the model coming from the client
                using (var attRepo = new AttachmentSetRepository())
                {
                    long attachmentSetId = attRepo.CreateAttachmentSet();
                    project.AttachmentSetId = attachmentSetId;
                }
                
                //STEP 2 - Create the Project and save to Projects table
                long newProjectId = s.Insert(project, identity);

                //STEP 3 - Create Project Stats Screen and Save to ProjectStats Table
                using (var sr = new ProjectStatRepository())
                {
                    bool statCreated = sr.CreateProjectStat((int)newProjectId);
                    if(!statCreated)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }

                //STEP 4 - Create new Project Funding Package for Donations
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

        // OK
        [HttpPut]
        [Route("{projectId}")]
        public HttpResponseMessage EditProject(ProjectModel project, int projectId)
        {
            if (!ModelState.IsValid || projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectRepository())
            {
                var httpStatusCode = HttpStatusCode.OK;

                ProjectRepository.StatusCodes hasUpdated = s.Update(project, identity, projectId);

                switch (hasUpdated)
                {
                    //project not found
                    case ProjectRepository.StatusCodes.NOT_FOUND:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;

                    //not authorized to update - you are not the project creator
                    case ProjectRepository.StatusCodes.NOT_AUTHORIZED:
                        httpStatusCode = HttpStatusCode.MethodNotAllowed;
                        break;

                    //project updated ok
                    case ProjectRepository.StatusCodes.OK:
                        httpStatusCode = HttpStatusCode.OK;
                        break;
                }

                return Request.CreateResponse(httpStatusCode);
            }
        }

        // OK
        [HttpGet]
        [Route("{projectId}/isCurrentUserProjectCreator")]
        public HttpResponseMessage IsRequestorUserProjectCreator(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var pr = new ProjectRepository())
            {
                var v = pr.IsCurrentUserAuthorized(projectId, "PROJECT", identity);
                
                return Request.CreateResponse(HttpStatusCode.OK, v);
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
        [Route("{projectId}/updates/allowAll")]
        public HttpResponseMessage GetProjectUpdatesForLoggedOutUsers(int projectId)
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
        [HttpGet]
        [Route("{projectId}/updates")]
        public HttpResponseMessage GetProjectUpdatesForLoggedInUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectUpdateRepository())
            {
                var v = s.GetAllProjectUpdates(projectId, identity);
                
                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [HttpGet]
        [Route("{projectId}/updates/{updateId}")]
        public HttpResponseMessage GetProjectUpdateByIdForLoggedInUsers(int projectId, int updateId)
        {
            if (projectId <= 0 || updateId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectUpdateRepository())
            {
                var v = s.GetProjectUpdateById(projectId, updateId, identity);

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
         
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/fundingPackages/allowAll")]
        public HttpResponseMessage GetProjectFundingPackagesForLoggedOutUsers(int projectId)
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
        [HttpGet]
        [Route("{projectId}/fundingPackages")]
        public HttpResponseMessage GetProjectFundingPackagesForLoggedInUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new FundingPackageRepository())
            {
                var v = s.GetAllProjectFundingPackages(projectId, identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [HttpGet]
        [Route("{projectId}/fundingPackages/{fundingPackageId}")]
        public HttpResponseMessage GetFundingPackageByIdForLoggedInUsers(int projectId, int fundingPackageId)
        {
            if (projectId <= 0 || fundingPackageId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new FundingPackageRepository())
            {
                var v = s.GetProjectFundingPackageById(projectId, fundingPackageId, identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [HttpGet]
        [Route("{projectId}/fundingPackages/{fundingPackageId}/forPaymentsView")]
        public HttpResponseMessage GetFundingPackageByIdForLoggedInUsersForPaymentView(int projectId, int fundingPackageId)
        {
            if (projectId <= 0 || fundingPackageId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            
            using (var s = new FundingPackageRepository())
            {
                var v = s.GetProjectFundingPackageByIdForPaymentView(projectId, fundingPackageId);

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

        /*
         * 
         * PROJECT EXTERNAL SHARES ROUTES
         * 
         */

        [HttpPost]
        [Route("{projectId}/externalShares")]
        public HttpResponseMessage CreateProjectExternalShare(ProjectExternalShareModel projectExternalShare, int projectId)
        {
            if (!ModelState.IsValid || projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            //STEP 1 - Update the project statistic for external share
            using (var ps = new ProjectStatRepository())
            {
                bool hasUpdatedProjectStat = ps.IncrementProjectStatSharesNo(projectId);

                //project to update stat not found
                if(!hasUpdatedProjectStat) return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            
            using (var s = new ProjectExternalShareRepository())
            {

                var httpStatusCode = HttpStatusCode.Created;

                // STEP 2: Create new external share and save it to database table of external shares
                bool hasInserted = s.CreateExternalShare(projectExternalShare, identity, projectId);

                switch (hasInserted)
                {
                    //project for creating external share not found
                    case false:
                        httpStatusCode = HttpStatusCode.NotFound;
                        break;
                        
                    //external share inserted ok
                    case true:
                        httpStatusCode = HttpStatusCode.Created;
                        break;
                }
                
                return Request.CreateResponse(httpStatusCode);
            }
        }

        /*
         * 
         * PROJECT FUNDINGS ROUTES 
         * 
         */

        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/getProjectFundings")]
        public HttpResponseMessage GetProjectFundings(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new UserFundingRepository())
            {
                var v = s.GetProjectFundings(projectId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        [HttpGet]
        [Route("getCurrentUserProjectFundings")]
        public HttpResponseMessage GetCurrentUserProjectFundings(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserFundingRepository())
            {
                var v = s.GetCurrentUserProjectFundings(identity, projectId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }


        [HttpPost]
        [Route("{projectId}/fundings")]
        public HttpResponseMessage CreateFundingForProject(UserFundingModel funding, int projectId)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserFundingRepository())
            {
                //STEP 1 - Create the Project Funding from UserFundingModel coming from the client
                long newFundingId = s.Insert(funding, projectId, identity);

                //STEP 2 - Update Project Stats Screen Amount + NoOfBackers
                using (var sr = new ProjectStatRepository())
                {
                    bool statAmountUpdated = sr.IncrementProjectStatMoneyPledged(projectId, funding.AmountPaid);
                    bool statNoOfBackersUpdated = sr.IncrementProjectStatBackersNo(projectId);

                    if (!statAmountUpdated || !statNoOfBackersUpdated)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
                
                return Request.CreateResponse(HttpStatusCode.Created, newFundingId);
            }
        }

    }
}
