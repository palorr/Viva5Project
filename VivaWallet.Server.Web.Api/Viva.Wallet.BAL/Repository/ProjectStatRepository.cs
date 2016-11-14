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
    public class ProjectStatRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectStatRepository()
        {
            uow = new UnitOfWork();
        }

        public IList<ProjectStatModel> GetProjectStats(int projectId)
        {
            return uow.ProjectStatRepository.All()?
                    .Where(e => e.ProjectId == projectId)
                    .Select(e => new ProjectStatModel()
                    {
                        Id = e.Id,
                        ProjectId = e.ProjectId,
                        BackersNo = e.BackersNo,
                        MoneyPledged = e.MoneyPledged,
                        SharesNo = e.SharesNo,
                        CommentsNo = e.CommentsNo  
                    }).ToList();
        }

        public void CreateProjectStat(int projectId)
        {
            try
            {
                var _projectStat = new ProjectStat()
                {
                    ProjectId = projectId,
                    BackersNo = 0,
                    MoneyPledged = 0,
                    SharesNo = 0,
                    CommentsNo = 0
                };

                uow.ProjectStatRepository.Insert(_projectStat, true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateProjectStat(ProjectStatModel source, int projectId)
        {
            try
            {
                var _projectStat = uow.ProjectStatRepository.All().Where(e => e.ProjectId == projectId).SingleOrDefault();

                _projectStat.BackersNo = source.BackersNo;
                _projectStat.MoneyPledged = source.MoneyPledged;
                _projectStat.SharesNo = source.SharesNo;
                _projectStat.CommentsNo = source.CommentsNo;

                uow.ProjectStatRepository.Update(_projectStat, true);
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteProjectStat(int projectId)
        {
            try
            {
                var _projectStat = uow.ProjectStatRepository.All().Where(e => e.ProjectId == projectId).SingleOrDefault();

                uow.ProjectStatRepository.Delete(_projectStat);
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
