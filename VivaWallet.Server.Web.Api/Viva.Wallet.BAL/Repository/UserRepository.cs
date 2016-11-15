using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL
{
    public class UserRepository : IDisposable
    {
        protected UnitOfWork uow;

        public UserRepository()
        {
            uow = new UnitOfWork();
        }

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

        public IList<UserModel> GetUser(int userId)
        {
            return uow.UserRepository.All()?
                    .Where(e => e.Id == userId)
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
                    }).ToList();
        }

        public UserModel GetUserByUsername(string userName)
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

        public void CreateUser(UserModel source)
        {
            try
            {
                var _user = new User()
                {
                    Username = source.Username,
                    IsVerified = true,
                    VerificationToken = "",
                    //VerificationToken = generateVerificationToken(),
                    CreatedDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now,
                    Name = "",
                    ShortBio = "",
                    AvatarImage = ""
                };

                uow.UserRepository.Insert(_user, true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public StatusCodes UpdateUser(UserModel source, ClaimsIdentity identity, int userId)
        {
            try
            {
                var _user = uow.UserRepository.FindById(userId);
                
                if (_user == null)
                {
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // user found. does the user that wishes to update it really is the user him/her self? check this here
                    var userIdClaim = identity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (_user.Username != userIdClaim.Value)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    //update user from UserModel
                    _user.UpdateDateTime = DateTime.Now;
                    _user.Name = source.Name;
                    _user.ShortBio = source.ShortBio;
                    _user.AvatarImage = source.AvatarImage;

                    uow.UserRepository.Update(_user, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

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
                    var userIdClaim = identity.Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                    if (_user.Username != userIdClaim.Value)
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
