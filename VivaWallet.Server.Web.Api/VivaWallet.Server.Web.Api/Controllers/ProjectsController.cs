using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
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
        
        // GET ALL PROJECT CATEGORIES - OK
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

        // RETURN ALL PROJECTS BY CATEGORY - OK
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

        //RETURN ALL PROJECTS BY SEARCH TERM NAME - OK
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

        //RETURN ALL PROJECTS - OK
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

        //RETURN TOP 10 TRENDING PROJECTS - OK
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

        //RETURN LAST TEN BACKED PROJECTS - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("lastTenBackedProjects")]
        public HttpResponseMessage GetLastTenBackedProjects()
        {
            using (var s = new ProjectRepository())
            {
                s.CheckForFailedProjects();

                var v = s.GetLastTenBackedProjects();

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        // GET PROJECT BY ID FOR LOGGED OUT USERS - OK
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

        // GET PROJECT BY ID FOR LOGGED IN USERS - OK
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

        //GET PROJECT STATS BY ID FOR LOGGED OUT USERS - OK
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

        //GET PROJECT STATS BY ID FOR LOGGED IN USERS
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

        //CREATE NEW PROJECT - OK
        [HttpPost]
        [Route("")]
        public HttpResponseMessage CreateProject(ProjectModel project)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectRepository())
            {
                long newProjectId = s.Insert(project, identity);
                
                return Request.CreateResponse(HttpStatusCode.Created, newProjectId);
            }
        }

        //UPDATE PROJECT - OK
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

        //CHECK IF CURRENT LOGGED IN USER IS PROJECT CREATOR - OK
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

        //GET A PROJECT'S UPDATES FOR LOGGED OUT USERS - OK
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

        //GET A PROJECT'S UPDATES FOR LOGGED IN USERS - OK
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

        //GET SINGLE PROJECT UPDATE BY ID FOR A PROJECT FOR LOGGED IN USERS - OK
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

        //INSERT OR UPDATE PROJECT UPDATE - OK
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

                        //not authorized to update this project update - not the project creator
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

        //DELETE PROJECT UPDATE BY ID - OK
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

                    //not authorized to delete this project update - not project creator
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

        //GET A PROJECT'S COMMENTS FOR LOGGED OUT USERS - OK
        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/comments/allowAll")]
        public HttpResponseMessage GetProjectCommentsForLoggedOutUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            using (var s = new ProjectCommentRepository())
            {
                var v = s.GetAllProjectComments(projectId);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET A PROJECT'S COMMENTS FOR LOGGED IN USERS - OK
        [HttpGet]
        [Route("{projectId}/comments")]
        public HttpResponseMessage GetProjectCommentsForLoggedInUsers(int projectId)
        {
            if (projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectCommentRepository())
            {
                var v = s.GetAllProjectComments(projectId, identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET A PROJECT'S SPECIFIC COMMENT BY ID FOR LOGGED IN USERS - OK
        [HttpGet]
        [Route("{projectId}/comments/{commentId}")]
        public HttpResponseMessage GetProjectCommentByIdForLoggedInUsers(int projectId, int commentId)
        {
            if (projectId <= 0 || commentId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectCommentRepository())
            {
                var v = s.GetProjectCommentById(projectId, commentId, identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //GET ALL CURRENT LOGGED IN USER'S CREATED PROJECT COMMENTS - OK
        [HttpGet]
        [Route("getAllCurrentUserCreatedProjectComments")]
        public HttpResponseMessage GetAllCurrentUserCreatedProjectComments()
        {
            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectCommentRepository())
            {
                var v = s.GetAllCurrentUserCreatedProjectComments(identity);

                return Request.CreateResponse(HttpStatusCode.OK, v);
            }
        }

        //INSERT OR UPDATE COMMENT - OK
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

                        //not authorized to update this comment - requestor not the comment creator
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

        //DELETE SPECIFIC COMMENT - OK
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

                    //not authorized to delete this comment - requestor not comment creator
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
        
        //GET PROJECT FUNDING PACKAGES FOR LOGGED OUT USERS - OK 
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

        // GET PROJECT FUNDING PACKAGES FOR LOGGED IN USERS - OK
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

        //GET SPECIFIC PROJECT FUNDING PACKAGE FOR LOGGED IN USERS - OK
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

        //GET SPECIFIC PROJECT FUNDING PACKAGE FOR LOGGED IN USERS FOR PAYMENTS VIEW - OK
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

        //CREATE NEW PROJECT FUNDING PACKAGE - OK
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

                    //not authorized to add this funding package to this specific project - requestor not the project creator
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

        //UPDATE EXISTING PROJECT FUNDING PACKAGE - OK
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

        //DELETE SPECIFIC PROJECT FUNDING PACKAGE - OK
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

        //CREATE EXTERNAL SHARE FOR A PROJECT - OK
        [HttpPost]
        [Route("{projectId}/externalShares")]
        public HttpResponseMessage CreateProjectExternalShare(ProjectExternalShareModel projectExternalShare, int projectId)
        {
            if (!ModelState.IsValid || projectId <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new ProjectExternalShareRepository())
            {

                var httpStatusCode = HttpStatusCode.Created;

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

        //GET A PROJECTS FUNDINGS - OK
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

        //GET CURRENT LOGGED IN USER ALL PROJECT FUNDINGS - OK
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
        
        //CREATE PROJECT FUNDING - OK
        [HttpPost]
        [Route("{projectId}/fundings")]
        public HttpResponseMessage CreateFundingForProject(UserFundingModel funding, int projectId)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var identity = User.Identity as ClaimsIdentity;

            using (var s = new UserFundingRepository())
            {
                
                long newFundingId = s.Insert(funding, projectId, identity);
                
                return Request.CreateResponse(HttpStatusCode.Created, newFundingId);
            }
        }

        //CHECKOUT TO VIVA PAYMENTS - OK
        [HttpPost]
        [Route("fundingPackages/{fundingPackageId}/checkout")]
        public async Task<IHttpActionResult> CheckoutToViva(VivaWalletTokenModel vivaWalletModel, int fundingPackageId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            using (var s = new UserFundingRepository())
            {
                TransactionResult task = await s.ChargeAsync(vivaWalletModel.vivaWalletToken);

                if(task == null)
                {
                    return BadRequest();
                }

                return Ok(task);
            }
        }

        [HttpPost]
        [Route("{projectId}/image")]
        public async Task<HttpResponseMessage> SaveImage(long projectId, AttachmentModel source)
        {
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            var path = mappedPath + Helpers.HelperMethods.getImagePhotoName(null);

            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var image = source.FilePath;
                if (image != null)
                {
                    var imageData = Helpers.HelperMethods.FixBase64ForImage(image);
                    var bytes = Convert.FromBase64String(imageData);

                    using (Image imageToSave = Image.FromStream(new MemoryStream(bytes)))
                    {
                        imageToSave.Save(path, ImageFormat.Jpeg);
                    }

                    source.FilePath = path;
                }
                using (var repo = new AttachmentRepository())
                {
                    await repo.saveAttachment(identity.Name, projectId, source);
                }
                
            }
            catch (Exception ex)
            {
                File.Delete(path);

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{projectId}/attachmets")]
        public HttpResponseMessage GetAttachments(long projectId)
        {
            try
            {
                    
                using (var repo = new AttachmentRepository())
                {
                    var res = repo.GetProjectAttachmets( projectId);

                    return Request.CreateResponse(HttpStatusCode.OK, res);
                }
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("{attachmentId}/delete")]
        [HttpPost] 
        public async Task<HttpResponseMessage> DeleteAttachment(long attachmentId)
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;

                using (var repo = new AttachmentRepository())
                {
                    await repo.DeleteAttachment(identity.Name, attachmentId);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError , ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("lll")]
        public string ggg()
        {
            var s = System.Web.HttpContext.Current.Request.Url;
            return s.OriginalString.Replace(s.AbsolutePath ,"");
        }

    }
}
