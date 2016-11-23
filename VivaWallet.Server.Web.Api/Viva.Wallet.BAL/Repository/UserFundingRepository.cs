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
    public class UserFundingRepository : IDisposable
    {
        protected UnitOfWork uow;

        public UserFundingRepository()
        {
            uow = new UnitOfWork();
        }

        public IList<UserFundingModel> GetProjectFundings(int projectId)
        {
            return uow.UserFundingRepository
                        .SearchFor(e => e.FundingPackage.ProjectId == projectId)
                        .Select(e => new UserFundingModel()
                        {
                            Id = e.Id,
                            FundingPackageId = e.FundingPackageId,
                            UserId = e.UserId,
                            Username = e.User.Name,
                            WhenDateTime = e.WhenDateTime,
                            AmountPaid = e.AmountPaid,
                            TransactionId = e.TransactionId
                        }).OrderByDescending(e => e.WhenDateTime).ToList();
            
        }

        public IList<UserFundingModelWithProjectId> GetCurrentUserProjectFundings(ClaimsIdentity identity, int projectId)
        {
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
                throw new InvalidOperationException("User lookup for requestor Id failed in GetCurrentUserProjectFundings", ex);
            }

            return uow.UserFundingRepository
                      .SearchFor(e => e.UserId == requestorUserId)
                      .Select(e => new UserFundingModelWithProjectId()
                      {
                          Id = e.Id,
                          FundingPackageId = e.FundingPackageId,
                          UserId = e.UserId,
                          Username = e.User.Name,
                          WhenDateTime = e.WhenDateTime,
                          AmountPaid = e.AmountPaid,
                          TransactionId = e.TransactionId,
                          ProjectId = e.FundingPackage.ProjectId
                      }).OrderByDescending(e => e.WhenDateTime).ToList();


        }

        public long Insert(UserFundingModel source, int projectId, ClaimsIdentity identity)
        {
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
                throw new InvalidOperationException("User lookup for requestor Id for user funding creation failed", ex);
            }

            try
            {
                var _userFunding = new UserFunding()
                {
                    FundingPackageId = source.FundingPackageId,
                    UserId = requestorUserId,
                    WhenDateTime = DateTime.Now,
                    AmountPaid = source.AmountPaid,
                    TransactionId = source.TransactionId
                };

                uow.UserFundingRepository.Insert(_userFunding, true);

                return _userFunding.Id;
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

    }
}
