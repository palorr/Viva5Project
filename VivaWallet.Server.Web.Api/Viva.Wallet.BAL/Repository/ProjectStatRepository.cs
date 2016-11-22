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
    public class ProjectStatRepository : IDisposable
    {
        protected UnitOfWork uow;

        public ProjectStatRepository()
        {
            uow = new UnitOfWork();
        }
        
        public ProjectStatModelToView GetProjectStats(int projectId, ClaimsIdentity identity = null)
        {
            bool isRequestorProjectCreator = false;

            if(identity != null)
            {
                ProjectRepository _prRepo = new ProjectRepository();
                isRequestorProjectCreator = _prRepo.IsProjectCreator(projectId, identity);
            }

            try
            {
                return uow.ProjectStatRepository
                      .SearchFor(e => e.ProjectId == projectId)
                      .Select(e => new ProjectStatModelToView()
                      {
                          Id = e.Id,
                          ProjectId = e.ProjectId,
                          BackersNo = e.BackersNo,
                          MoneyPledged = e.MoneyPledged,
                          SharesNo = e.SharesNo,
                          CommentsNo = e.CommentsNo,
                          IsRequestorProjectCreator = isRequestorProjectCreator,
                          FundingGoal = e.Project.FundingGoal,
                          FundingEndDate = e.Project.FundingEndDate
                      }).SingleOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Project stat screen lookup for project Id failed", ex);
            }
            
        }

        public bool CreateProjectStat(int projectId)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return false;
                }

                else { 
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

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ProjectStat GetProjectStatOfProject(int projectId)
        {
            try
            {
                var _projectStat = uow.ProjectStatRepository
                                  .SearchFor(e => e.ProjectId == projectId)
                                  .SingleOrDefault();

                return _projectStat;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Project stat lookup for project Id failed", ex);
            }
        }

        public bool UpdateProjectStatFromModel(ProjectStatModel source, int projectId)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return false;
                }

                var _projectStat = this.GetProjectStatOfProject(projectId);
                
                if (_projectStat == null)
                {
                    return false;
                }

                _projectStat.BackersNo = source.BackersNo;
                _projectStat.MoneyPledged = source.MoneyPledged;
                _projectStat.SharesNo = source.SharesNo;
                _projectStat.CommentsNo = source.CommentsNo;

                uow.ProjectStatRepository.Update(_projectStat, true);

                return true;
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IncrementProjectStatBackersNo(int projectId)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return false;
                }

                var _projectStat = this.GetProjectStatOfProject(projectId);

                if (_projectStat == null)
                {
                    return false;
                }

                _projectStat.BackersNo++;
                
                uow.ProjectStatRepository.Update(_projectStat, true);

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IncrementProjectStatMoneyPledged(int projectId, decimal amountPledged)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return false;
                }

                var _projectStat = this.GetProjectStatOfProject(projectId);

                if (_projectStat == null)
                {
                    return false;
                }

                _projectStat.MoneyPledged += amountPledged;

                uow.ProjectStatRepository.Update(_projectStat, true);

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IncrementProjectStatSharesNo(int projectId)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return false;
                }

                var _projectStat = this.GetProjectStatOfProject(projectId);

                if (_projectStat == null)
                {
                    return false;
                }

                _projectStat.SharesNo++;

                uow.ProjectStatRepository.Update(_projectStat, true);

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ChangeProjectStatCommentsNo(int projectId, bool willIncrement = true)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return false;
                }

                var _projectStat = this.GetProjectStatOfProject(projectId);

                if (_projectStat == null)
                {
                    return false;
                }

                if (willIncrement)
                    _projectStat.CommentsNo++;

                else
                    _projectStat.CommentsNo--;

                uow.ProjectStatRepository.Update(_projectStat, true);

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteProjectStat(int projectId)
        {
            try
            {
                //get the project
                var _project = uow.ProjectRepository.FindById(projectId);

                if (_project == null)
                {
                    return false;
                }

                var _projectStat = this.GetProjectStatOfProject(projectId);

                if (_projectStat == null)
                {
                    return false;
                }

                uow.ProjectStatRepository.Delete(_projectStat);

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
