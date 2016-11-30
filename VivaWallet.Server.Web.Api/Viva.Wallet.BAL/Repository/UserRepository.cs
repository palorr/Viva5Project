using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL.Repository
{
    public class UserRepository : IDisposable
    {
        protected UnitOfWork uow;

        public UserRepository()
        {
            uow = new UnitOfWork();
        }

        // OK
        public IList<UserModel> GetAllUsers()
        {
            return uow.UserRepository.All()?
                    .Select(e => new UserModel()
                    {
                        Id = e.Id,
                        Username = e.Username,
                        IsVerified = e.IsVerified,
                        VerificationToken = e.VerificationToken,
                        CreatedDateTime = e.CreatedDateTime,
                        UpdatedDateTime = e.UpdateDateTime,
                        Name = e.Name,
                        ShortBio = e.ShortBio,
                        AvatarImage = e.AvatarImage
                    }).OrderByDescending(e => e.UpdatedDateTime).ToList();
        }

        // OK
        public IList<UserModel> GetLastTenRegisteredUsers()
        {
            
            return uow.UserRepository.All()?
                    .Select(e => new UserModel()
                    {
                        Id = e.Id,
                        Username = e.Username,
                        IsVerified = e.IsVerified,
                        VerificationToken = e.VerificationToken,
                        CreatedDateTime = e.CreatedDateTime,
                        UpdatedDateTime = e.UpdateDateTime,
                        Name = e.Name,
                        ShortBio = e.ShortBio,
                        AvatarImage = e.AvatarImage
                    }).OrderByDescending(e => e.CreatedDateTime).Take(10).ToList();
        }

        // OK
        public UserModel GetUser(int userId)
        {
            try
            {
                return uow.UserRepository
                          .SearchFor(e => e.Id == userId)
                          .Select(e => new UserModel()
                          {
                              Id = e.Id,
                              Username = e.Username,
                              IsVerified = e.IsVerified,
                              VerificationToken = e.VerificationToken,
                              CreatedDateTime = e.CreatedDateTime,
                              UpdatedDateTime = e.UpdateDateTime,
                              Name = e.Name,
                              ShortBio = e.ShortBio,
                              AvatarImage = e.AvatarImage
                          }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup failed", ex);
            }
        }

        // OK
        public UserModel GetUserByUsername(string userName)
        {
            try
            {
                return uow.UserRepository
                          .SearchFor(e => e.Username == userName)
                          .Select(e => new UserModel()
                            {
                                Id = e.Id,
                                Username = e.Username,
                                IsVerified = e.IsVerified,
                                VerificationToken = e.VerificationToken,
                                CreatedDateTime = e.CreatedDateTime,
                                UpdatedDateTime = e.UpdateDateTime,
                                Name = e.Name,
                                ShortBio = e.ShortBio,
                                AvatarImage = e.AvatarImage
                            }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup failed", ex);
            }
        }

        // OK
        public void CreateUser(UserModel source)
        {
            try
            {
                var _user = new User()
                {
                    Username = source.Username,
                    IsVerified = true,
                    VerificationToken = "",
                    //VerificationToken = this.generateVerificationToken(),
                    CreatedDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now,
                    Name = source.Name,
                    //Name = "Anonymous User",
                    ShortBio = "",
                    AvatarImage = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSODALYDYo2dqN0DG_kPNi2X7EAy1K8SpRRZQWkNv9alC62IHggOw"
                };

                uow.UserRepository.Insert(_user, true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public bool UpdateUser(UserModel source, ClaimsIdentity identity)
        {
            try
            {
                User _user;
                    
                try
                {
                    _user = uow.UserRepository
                               .SearchFor(e => e.Username == identity.Name)
                               .SingleOrDefault();
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("User lookup for requestor Id failed", ex);
                }

                if (_user == null) return false; 
                
                //update user from UserModel
                _user.UpdateDateTime = DateTime.Now;
                _user.Name = source.Name;
                _user.ShortBio = source.ShortBio;
                _user.AvatarImage = source.AvatarImage;

                uow.UserRepository.Update(_user, true);
                
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<UserModel> GetByName(string searchTerm)
        {
            return uow.UserRepository
                      .SearchFor(e => e.Name.Contains(searchTerm))
                      .Select(e => new UserModel()
                      {
                          Id = e.Id,
                          Username = e.Username,
                          IsVerified = e.IsVerified,
                          CreatedDateTime = e.CreatedDateTime,
                          UpdatedDateTime = e.UpdateDateTime,
                          Name = e.Name,
                          ShortBio = e.ShortBio,
                          AvatarImage = e.AvatarImage,
                          IsAdmin = e.IsAdmin

                      }).OrderByDescending(e => e.CreatedDateTime).ToList();

        }

        // OK
        public StatusCodes DeactivateUserAccount(ClaimsIdentity identity, int userId)
        {
            try
            {
                var _user = uow.UserRepository.FindById(userId);

                //user not found
                if (_user == null)
                {
                    return StatusCodes.NOT_FOUND;
                }

                else
                {
                    // user found. is the user authorized? check this here
                    long requestorUserId;

                    try
                    {
                        requestorUserId = uow.UserRepository
                                             .SearchFor(e => e.Username == identity.Name)
                                             .Select(e => e.Id)
                                             .SingleOrDefault();
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException("User lookup for requestor Id failed", ex);
                    }

                    //var userIdClaim = identity.Claims
                    //.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (userId != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    uow.UserRepository.Delete(_user);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public IList<ProjectModel> GetUserCreatedProjects(int userId)
        {
            var _user = uow.UserRepository.FindById(userId);

            //user not found
            if (_user == null)
            {
                return new List<ProjectModel>() { };
            }

            else { 
                return uow.ProjectRepository
                          //.SearchFor(e => (e.UserId == userId && e.Status != "CRE" && e.Status != "FAI"))
                          .SearchFor(e => e.UserId == userId)
                          .Select(e => new ProjectModel()
                            {
                                Id = e.Id,
                                OwnerId = e.UserId,
                                OwnerName = e.User.Name,
                                ProjectCategoryId = e.ProjectCategoryId,
                                ProjectCategoryDesc = e.ProjectCategory.Name,
                                Title = e.Title,
                                Description = e.Description,
                                CreatedDate = e.CreatedDate,
                                UpdatedDate = e.UpdateDate,
                                FundingEndDate = e.FundingEndDate,
                                FundingGoal = e.FundingGoal,
                                Status = e.Status
                            }).OrderByDescending(e => e.UpdatedDate).ToList();
            }
        }

        // OK
        public IList<ProjectModel> GetCurrentLoggedInUserCreatedProjects(ClaimsIdentity identity)
        {
            long currentUserId;

            try
            {
                currentUserId = uow.UserRepository
                                 .SearchFor(e => e.Username == identity.Name)
                                 .Select(e => e.Id)
                                 .SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup for current logged in User Id failed", ex);
            }
            
            return uow.ProjectRepository
                    .SearchFor(e => e.UserId == currentUserId)
                    .Select(e => new ProjectModel()
                    {
                        Id = e.Id,
                        OwnerId = e.UserId,
                        OwnerName = e.User.Name,
                        ProjectCategoryId = e.ProjectCategoryId,
                        ProjectCategoryDesc = e.ProjectCategory.Name,
                        Title = e.Title,
                        Description = e.Description,
                        CreatedDate = e.CreatedDate,
                        UpdatedDate = e.UpdateDate,
                        FundingEndDate = e.FundingEndDate,
                        FundingGoal = e.FundingGoal,
                        Status = e.Status
                    }).OrderByDescending(e => e.UpdatedDate).ToList();

        }

        /*
         
        // OK
        public IList<ProjectModel> GetUserAllFundings(int userId)
        {
            var _user = uow.UserRepository.FindById(userId);

            //user not found
            if (_user == null)
            {
                return new List<ProjectModel>() { };
            }

            else
            {
                using (var ctx = new VivaWalletEntities())
                {
                    //Get user funded projects
                    return ctx.Database.SqlQuery<ProjectModel>(
                        @" 
                            SELECT 
                                p.Id Id,
	                            pc.Id ProjectCategoryId, 
	                            pc.Name ProjectCategoryDesc,
	                            p.AttachmentSetId AttachmentSetId,
	                            p.Title Title,
	                            p.Description Description,
	                            p.CreatedDate CreatedDate,
	                            p.UpdateDate UpdateDate,
	                            p.FundingEndDate FundingEndDate,
	                            p.FundingGoal FundingGoal,
	                            p.Status Status,
	                            p.UserId OwnerId,
	                            u.Name OwnerName
                            FROM 
	                            UserFundings uf
                            LEFT JOIN
	                            FundingPackages fp
                            ON 
	                            uf.FundingPackageId = fp.Id
                            LEFT JOIN 
	                            Projects p
                            ON 
	                            fp.ProjectId = p.Id
                            LEFT JOIN
                                ProjectCategories pc
                            ON 
                                p.ProjectCategoryId = pc.Id
                            LEFT JOIN
	                            Users u
                            ON
	                            p.UserId = u.Id
                            WHERE
	                            uf.UserId = {0}
                            ORDER BY
	                            uf.WhenDateTime DESC
                        ", userId
                    ).ToList<ProjectModel>();
                }
            }
        }     
             
        
        */

        // OK
        public IList<ProjectModel> GetUserFundedProjects(int userId)
        {
            var _user = uow.UserRepository.FindById(userId);

            //user not found
            if (_user == null)
            {
                return new List<ProjectModel>() { };
            }

            else
            {
                //get user funded projects that have status = "COM"
                using (var ctx = new VivaWalletEntities())
                {

                    //first return rows as IEnumerable - Reason? A user may have backed this project
                    //that completed multiple times 
                    //as a result we may end have many same rows
                    //create a function distinctBy to remove same entries from the IEnumerable

                    IOrderedQueryable<ProjectModel> userFundingsOrderedQueryable = ctx.UserFundings
                        .Join(ctx.FundingPackages, uf => uf.FundingPackageId, fp => fp.Id, (uf, fp) => new { uf, fp })
                        .Join(ctx.Projects, uffp => uffp.fp.ProjectId, pr => pr.Id, (uffp, pr) => new { uffp.fp, uffp.uf, pr })
                        .Where(uffppr => (uffppr.uf.UserId == userId))
                        .Select(uffppr => new ProjectModel()
                        {
                            Id = uffppr.pr.Id,
                            OwnerId = uffppr.pr.UserId,
                            OwnerName = uffppr.pr.User.Name,
                            ProjectCategoryId = uffppr.pr.ProjectCategoryId,
                            ProjectCategoryDesc = uffppr.pr.ProjectCategory.Name,
                            Title = uffppr.pr.Title,
                            Description = uffppr.pr.Description,
                            CreatedDate = uffppr.pr.CreatedDate,
                            UpdatedDate = uffppr.pr.UpdateDate,
                            FundingEndDate = uffppr.pr.FundingEndDate,
                            FundingGoal = uffppr.pr.FundingGoal,
                            Status = uffppr.pr.Status
                        }).OrderByDescending(e => e.UpdatedDate);

                    IEnumerable<ProjectModel> userFundings = userFundingsOrderedQueryable.AsEnumerable();
                    
                    //return the filtered set of rows as a IList for the view to render
                    return UserRepository.DistinctBy(userFundings, p => p.Id).ToList();
                }
            }
        }

        public IList<ProjectUpdateModel> GetCurrentUserFundedProjectsLatestUpdates(ClaimsIdentity identity)
        {
            long currentUserId;

            try
            {
                currentUserId = uow.UserRepository
                                 .SearchFor(e => e.Username == identity.Name)
                                 .Select(e => e.Id)
                                 .SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup for current logged in User Id failed in GetUserFundedProjectsCompletedNotifications", ex);
            }
            
            //get user funded projects updates
            using (var ctx = new VivaWalletEntities())
            {

                //first return rows as IEnumerable - Reason? A user may have backed this project
                //that completed multiple times 
                //as a result we may end have many same rows
                //create a function distinctBy to remove same entries from the IEnumerable

                IOrderedQueryable<ProjectUpdateModel> userFundingsOrderedQueryable = ctx.UserFundings
                    .Join(ctx.FundingPackages, uf => uf.FundingPackageId, fp => fp.Id, (uf, fp) => new { uf, fp })
                    .Join(ctx.Projects, uffp => uffp.fp.ProjectId, pr => pr.Id, (uffp, pr) => new { uffp.fp, uffp.uf, pr })
                    .Join(ctx.ProjectUpdates, uffppr => uffppr.pr.Id, pu => pu.ProjectId, (uffppr, pu) => new { uffppr.pr, uffppr.fp, uffppr.uf, pu })
                    .Where(uffpprpu => (uffpprpu.uf.UserId == currentUserId) && (uffpprpu.pr.Id == uffpprpu.pu.ProjectId))
                    .Select(uffpprpu => new ProjectUpdateModel()
                    {
                        Id = uffpprpu.pr.Id,
                        ProjectId = uffpprpu.pr.Id,
                        AttachmentSetId = uffpprpu.pr.AttachmentSetId,
                        Title = uffpprpu.pu.Title,
                        Description = uffpprpu.pu.Description,
                        WhenDateTime = uffpprpu.pu.WhenDateTime
                    }).OrderByDescending(e => e.WhenDateTime);

                IEnumerable<ProjectUpdateModel> userFundings = userFundingsOrderedQueryable.AsEnumerable();

                //return the filtered set of rows as a IList for the view to render
                return UserRepository.DistinctBy(userFundings, p => p.Id).ToList();
            }
            
        }
        
        public IList<ProjectModel> GetUserFundedCompletedProjects(ClaimsIdentity identity, bool showAll)
        {
            long currentUserId;

            try
            {
                currentUserId = uow.UserRepository
                                 .SearchFor(e => e.Username == identity.Name)
                                 .Select(e => e.Id)
                                 .SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup for current logged in User Id failed in GetUserFundedProjectsCompletedNotifications", ex);
            }
            
            //get user funded projects that have status = "COM"
            using (var ctx = new VivaWalletEntities())
            {

                //first return rows as IEnumerable - Reason? A user may have backed this project
                //that completed multiple times 
                //as a result we may end have many same rows
                //create a function distinctBy to remove same entries from the IEnumerable

                IOrderedQueryable<ProjectModel> userFundingsOrderedQueryable = ctx.UserFundings
                    .Join(ctx.FundingPackages, uf => uf.FundingPackageId, fp => fp.Id, (uf, fp) => new { uf, fp })
                    .Join(ctx.Projects, uffp => uffp.fp.ProjectId, pr => pr.Id, (uffp, pr) => new { uffp.fp, uffp.uf, pr })
                    .Where(uffppr => (uffppr.uf.UserId == currentUserId && uffppr.pr.Status == "COM"))
                    .Select(uffppr => new ProjectModel()
                    {
                        Id = uffppr.pr.Id,
                        OwnerId = uffppr.pr.UserId,
                        OwnerName = uffppr.pr.User.Name,
                        ProjectCategoryId = uffppr.pr.ProjectCategoryId,
                        ProjectCategoryDesc = uffppr.pr.ProjectCategory.Name,
                        Title = uffppr.pr.Title,
                        Description = uffppr.pr.Description,
                        CreatedDate = uffppr.pr.CreatedDate,
                        UpdatedDate = uffppr.pr.UpdateDate,
                        FundingEndDate = uffppr.pr.FundingEndDate,
                        FundingGoal = uffppr.pr.FundingGoal,
                        Status = uffppr.pr.Status
                    }).OrderByDescending(e => e.UpdatedDate);

                IEnumerable<ProjectModel> userFundings;

                //if showAll true show all completed projects
                if (showAll)
                {
                    userFundings = userFundingsOrderedQueryable.AsEnumerable();
                }

                //else show top most recent completed
                else
                {
                    userFundings = userFundingsOrderedQueryable.Take(5).AsEnumerable();
                }

                //return the filtered set of rows as a IList for the view to render
                return UserRepository.DistinctBy(userFundings, p => p.Id).ToList();
            }

        }
        
        public static IEnumerable<TSource> 
            DistinctBy<TSource, TKey>
            (IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public enum StatusCodes
        {
            NOT_FOUND = 0,
            NOT_AUTHORIZED = 1,
            OK = 2
        };
    }
}
