using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Viva.Wallet.BAL.Helpers;
using Viva.Wallet.BAL.Models;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL.Repository
{
    public class UserFundingRepository : IDisposable
    {
        protected UnitOfWork uow;

        //Orestis Meikopoulos VIVA
        //private const string merchantId = "e8da1677-f9ca-4126-a72c-2b574a291d22";
        //private const string apiKey = "e]CmmG";

        //Viva Team 5 
        private const string merchantId = "22413f73-97d5-4040-a57b-44c7979f0731";
        private const string apiKey = "=Npy2s";

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
            long requestorUserId = UtilMethods.GetCurrentUserId(uow, identity.Name);

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
            long requestorUserId = UtilMethods.GetCurrentUserId(uow, identity.Name);

            try
            {
                //STEP 1 - Create the Project Funding from UserFundingModel coming from the client
                var _userFunding = new UserFunding()
                {
                    FundingPackageId = source.FundingPackageId,
                    UserId = requestorUserId,
                    WhenDateTime = DateTime.Now,
                    AmountPaid = source.AmountPaid,
                    TransactionId = source.TransactionId
                };

                uow.UserFundingRepository.Insert(_userFunding, true);

                //STEP 2 - Update Project Stats Screen Amount + NoOfBackings
                using (var sr = new ProjectStatRepository())
                {
                    bool statAmountUpdated = sr.IncrementProjectStatMoneyPledged(projectId, source.AmountPaid);
                    bool statNoOfBackersUpdated = sr.IncrementProjectStatBackersNo(projectId);
                }

                return _userFunding.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<TransactionResult> ChargeAsync(string vivaWalletToken)
        {
            var cl = new RestClient("http://demo.vivapayments.com/api/")
            {
                Authenticator = new HttpBasicAuthenticator(merchantId, apiKey)
            };
            var request = new RestRequest("transactions", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddParameter("PaymentToken", vivaWalletToken);

            var response = await cl.ExecuteTaskAsync<TransactionResult>(request);

            return response.ResponseStatus == ResponseStatus.Completed &&
                response.StatusCode == System.Net.HttpStatusCode.OK ? response.Data : null;
        }

        public void Dispose()
        {
            uow.Dispose();
        }

    }
}
