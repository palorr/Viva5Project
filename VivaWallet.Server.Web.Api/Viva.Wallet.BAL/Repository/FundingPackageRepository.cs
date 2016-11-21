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
    public class FundingPackageRepository : IDisposable
    {
        protected UnitOfWork uow;

        public FundingPackageRepository()
        {
            uow = new UnitOfWork();
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        // OK
        public IList<FundingPackageModelToView> GetAllProjectFundingPackages(long projectId, ClaimsIdentity identity = null)
        {
            bool isRequestorProjectCreator = false;

            if (identity != null)
            {
                ProjectRepository _prRepo = new ProjectRepository();
                isRequestorProjectCreator = _prRepo.IsProjectCreator((int)projectId, identity);
            }

            return uow.FundingPackageRepository
                      .SearchFor(e => e.ProjectId == projectId)
                      .Select(e => new FundingPackageModelToView()
                      {
                          Id = e.Id,
                          ProjectId = e.ProjectId,
                          AttachmentSetId = e.AttachmentSetId,
                          Title = e.Title,
                          PledgeAmount = e.PledgeAmount,
                          Description = e.Description,
                          WhenDateTime = e.WhenDateTime,
                          EstimatedDeliveryDate = e.EstimatedDeliveryDate,
                          IsRequestorProjectCreator = isRequestorProjectCreator
                      }).OrderByDescending(e => e.WhenDateTime).ToList();
        }

        // OK
        public FundingPackageModelToView GetProjectFundingPackageById(int projectId, int fundingPackageId, ClaimsIdentity identity)
        {
            bool isRequestorProjectCreator = false;

            ProjectRepository _prRepo = new ProjectRepository();
            isRequestorProjectCreator = _prRepo.IsProjectCreator(projectId, identity);

            try
            {
                return uow.FundingPackageRepository
                          .SearchFor(e => (e.ProjectId == projectId && e.Id == fundingPackageId))
                          .Select(e => new FundingPackageModelToView()
                          {
                              Id = e.Id,
                              ProjectId = e.ProjectId,
                              AttachmentSetId = e.AttachmentSetId,
                              Title = e.Title,
                              PledgeAmount = e.PledgeAmount,
                              Description = e.Description,
                              WhenDateTime = e.WhenDateTime,
                              EstimatedDeliveryDate = e.EstimatedDeliveryDate,
                              IsRequestorProjectCreator = isRequestorProjectCreator
                          }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Funding Package Id lookup for project failed", ex);
            }
        }

        // OK
        public StatusCodes CreateFundingPackage(FundingPackageModel source, ClaimsIdentity identity, int projectId, bool isForDonations)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return StatusCodes.NOT_FOUND;
                }

                else
                {
                    //check if current user is the projectId's creator
                    //else return NOT ALLOWED
                    long requestorUserId = GetRequestorIdFromIdentity(identity);
                    
                    if (_project.User.Id != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    var _projectFundingPackage = new FundingPackage()
                    {
                        ProjectId = projectId,
                        AttachmentSetId = source.AttachmentSetId,
                        Title = source.Title,
                        PledgeAmount = (isForDonations ? null : source.PledgeAmount),
                        Description = source.Description,
                        WhenDateTime = DateTime.Now,
                        EstimatedDeliveryDate = DateTime.Now
                        //EstimatedDeliveryDate = (isForDonations? DateTime.Now : source.EstimatedDeliveryDate)
                    };

                    uow.FundingPackageRepository.Insert(_projectFundingPackage, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public StatusCodes EditFundingPackage(FundingPackageModel source, ClaimsIdentity identity, int projectId, int fundingPackageId)
        {
            try
            {
                var _project = uow.ProjectRepository.FindById(projectId);
                var _fundingPackage = uow.FundingPackageRepository.FindById(fundingPackageId);

                if (_project == null || _fundingPackage == null)
                {
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // funding package found. does the user that wishes to update it really is the project creator? check this here
                    long requestorUserId = GetRequestorIdFromIdentity(identity);

                    if (_fundingPackage.Project.UserId != requestorUserId)
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    _fundingPackage.AttachmentSetId = source.AttachmentSetId;
                    _fundingPackage.WhenDateTime = DateTime.Now;
                    _fundingPackage.Title = source.Title;
                    _fundingPackage.Description = source.Description;
                    _fundingPackage.EstimatedDeliveryDate = source.EstimatedDeliveryDate;

                    //if the funding package is not the donations package then you may also update the PledgeAmount
                    if(_fundingPackage.PledgeAmount != null && _fundingPackage.PledgeAmount.HasValue)
                    {
                        _fundingPackage.PledgeAmount = source.PledgeAmount;
                    }

                    uow.FundingPackageRepository.Update(_fundingPackage, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        public StatusCodes DeleteFundingPackage(ClaimsIdentity identity, int projectId, int fundingPackageId)
        {
            try
            {
                var _project = uow.ProjectRepository.FindById(projectId);
                var _fundingPackage = uow.FundingPackageRepository.FindById(fundingPackageId);

                if (_project == null || _fundingPackage == null)
                {
                    return StatusCodes.NOT_FOUND;
                }
                else
                {
                    // funding package found. does the user that wishes to delete it really is the project creator? check this here
                    long requestorUserId = GetRequestorIdFromIdentity(identity);

                    //if not the project creater OR if the funding package is the donations package NOT_AUTHORIZED to delete it in either cases
                    if ((_fundingPackage.Project.UserId != requestorUserId) || (_fundingPackage.PledgeAmount == null))
                    {
                        return StatusCodes.NOT_AUTHORIZED;
                    }

                    uow.FundingPackageRepository.Delete(_fundingPackage, true);
                }

                return StatusCodes.OK;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // OK
        private long GetRequestorIdFromIdentity(ClaimsIdentity identity)
        {
            try
            {
                long requestorUserId = uow.UserRepository
                                          .SearchFor(e => e.Username == identity.Name)
                                          .Select(e => e.Id)
                                          .SingleOrDefault();

                return requestorUserId;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("User lookup for requestor Id for project funding package module failed", ex);
            }
        }

        public enum StatusCodes
        {
            NOT_FOUND = 0,
            NOT_AUTHORIZED = 1,
            OK = 2
        };
    }
}
