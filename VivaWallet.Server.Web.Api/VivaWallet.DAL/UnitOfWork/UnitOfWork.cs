using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL
{
   public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private Repository<User> _UserRepository;
        private Repository<UserFunding> _UserFundingRepository;
        private Repository<Project> _ProjectRepository;
        private Repository<ProjectCategory> _ProjectCategoryRepository;
        private Repository<FundingPackage> _FundingPackageRepository;
        private Repository<ProjectComment> _ProjectCommentreRepository;
        private Repository<ProjectUpdate> _ProjectUpdateRepository;
        private Repository<ProjectStat> _ProjectStatRepository;
        private Repository<Attachment> _AttachemntRepository;
        private VivaWalletEntities _dbContext;

        public UnitOfWork()
        {
            _dbContext = new VivaWalletEntities();
        }
        

        public Repository<Project> ProjectRepository
        {
            get {
                if (this._ProjectRepository == null)
                    this._ProjectRepository = new Repository<Project>(_dbContext);

                return this._ProjectRepository;
            }

            set { _ProjectRepository = value; }
        }

        public Repository<User> UserRepository
        {
            get { return _UserRepository ?? new Repository<User>(_dbContext); }
        }

        public Repository<UserFunding> UserFundingRepository
        {
            get { return _UserFundingRepository ?? new Repository<UserFunding>(_dbContext); }
        }

        public Repository<ProjectCategory> ProjectCategoryRepository
        {
            get { return _ProjectCategoryRepository ?? new Repository<ProjectCategory>(_dbContext); }
        }

        public Repository<ProjectStat> ProjectStatRepository
        {
            get { return _ProjectStatRepository ?? new Repository<ProjectStat>(_dbContext); }
        }

        public Repository<FundingPackage> FundingPackageRepository
        {
            get { return _FundingPackageRepository ?? new Repository<FundingPackage>(_dbContext); }         
        }
        
        public Repository<ProjectComment> ProjectCommentreRepository
        {
            get { return _ProjectCommentreRepository ?? new Repository<ProjectComment>(_dbContext); }
        }

        public Repository<ProjectUpdate> ProjectUpdateRepository
        {
            get { return _ProjectUpdateRepository ?? new Repository<ProjectUpdate>(_dbContext); }
        }

        public Repository<Attachment> AttachemntRepository
        {
            get { return _AttachemntRepository ?? new Repository<Attachment>(_dbContext); }
        }

        public void Dispose()
        {
            if (_FundingPackageRepository != null)
                _FundingPackageRepository.Dispose();

            if (_ProjectCategoryRepository != null)
                _ProjectCategoryRepository.Dispose();

            if (_AttachemntRepository != null)
                _AttachemntRepository.Dispose();

            if (_ProjectCommentreRepository != null)
                _ProjectCommentreRepository.Dispose();

            if (_ProjectUpdateRepository != null)
                _ProjectUpdateRepository.Dispose();

            if (_ProjectRepository != null)
                _ProjectRepository.Dispose();


            _dbContext.Dispose();
        }

        public void SaveChanges(bool save = true)
        {
            if (save)
                _dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync(bool save = true)
        {
            if (save)
                await _dbContext.SaveChangesAsync();
        }

        
    }
}
