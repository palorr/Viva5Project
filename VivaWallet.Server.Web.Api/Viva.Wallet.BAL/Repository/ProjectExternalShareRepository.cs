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
    public class ProjectExternalShareRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectExternalShareRepository()
        {
            uow = new UnitOfWork();
        }

        public bool CreateExternalShare(ProjectExternalShareModel source, ClaimsIdentity identity, int projectId)
        {
            Project _project = uow.ProjectRepository.FindById((long)projectId);
            if (_project == null) return false;

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
                throw new InvalidOperationException("User lookup for requestor Id failed in CreateExternalShare", ex);
            }

            try
            {
                // STEP 1: Create new external share and save it to database table of external shares
                var _projectExternalShare = new ProjectExternalShare()
                {
                    ProjectId = source.ProjectId,
                    UserId = requestorUserId,
                    Target = source.Target,
                    Source = source.Source,
                    WhenDateTime = DateTime.Now
                };

                uow.ProjectExternalShareRepository.Insert(_projectExternalShare, true);

                //STEP 2 - Update the project statistic for external share
                using (var ps = new ProjectStatRepository())
                {
                    bool hasUpdatedProjectStat = ps.IncrementProjectStatSharesNo(projectId);

                    //project to update stat not found
                    if (!hasUpdatedProjectStat) return false;
                }

                return true;
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
