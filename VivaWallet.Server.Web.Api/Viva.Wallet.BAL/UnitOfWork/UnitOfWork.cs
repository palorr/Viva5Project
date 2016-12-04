using System.Threading.Tasks;
using VivaWallet.DAL;

namespace Viva.Wallet.BAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<User> _UserRepository;
        private IRepository<UserFunding> _UserFundingRepository;
        private IRepository<Project> _ProjectRepository;
        private IRepository<ProjectCategory> _ProjectCategoryRepository;
        private IRepository<FundingPackage> _FundingPackageRepository;
        private IRepository<ProjectComment> _ProjectCommentreRepository;
        private IRepository<ProjectUpdate> _ProjectUpdateRepository;
        private IRepository<ProjectStat> _ProjectStatRepository;
        private IRepository<ProjectExternalShare> _ProjectExternalShareRepository;
        private IRepository<Attachment> _AttachmentRepository;
        private IRepository<AttachmentSet> _AttachmentSetRepository;
        private VivaWalletEntities _dbContext;


        public UnitOfWork()
        {
            _dbContext = new VivaWalletEntities();
        }
        public VivaWalletEntities dbContext
        {
            get
            {
               if(this._dbContext == null)
                    _dbContext = new VivaWalletEntities();

                return _dbContext;
            }
            set
            {
                this._dbContext = value;
            }
        }

        public IRepository<Attachment> AttachmentRepository
        {
            get
            {
                if (_AttachmentRepository == null)
                    _AttachmentRepository = new Repository<Attachment>(_dbContext);
                  
                    return this._AttachmentRepository;
            }

            set
            {
                _AttachmentRepository = value;
            }
        }

        public IRepository<AttachmentSet> AttachmentSetRepository
        {
            get
            {
                if (_AttachmentSetRepository == null)
                    _AttachmentSetRepository = new Repository<AttachmentSet>(_dbContext);

                return _AttachmentSetRepository;
            }

            set
            {
                _AttachmentSetRepository = value;
            }
        }

        public IRepository<FundingPackage> FundingPackageRepository
        {
            get
            {
                if (_FundingPackageRepository == null)
                    _FundingPackageRepository = new Repository<FundingPackage>(_dbContext);

                return _FundingPackageRepository;
            }

            set
            {
                _FundingPackageRepository = value;
            }
        }

        public IRepository<ProjectCategory> ProjectCategoryRepository
        {
            get
            {
                if (_ProjectCategoryRepository == null)
                    _ProjectCategoryRepository = new Repository<ProjectCategory>(_dbContext);

                return _ProjectCategoryRepository;
            }

            set
            {
                _ProjectCategoryRepository = value;

            }
        }

        public IRepository<ProjectComment> ProjectCommentreRepository
        {
            get
            {
                if (_ProjectCommentreRepository == null)
                    _ProjectCommentreRepository = new Repository<ProjectComment>(_dbContext);

                return _ProjectCommentreRepository;
            }

            set
            {
                _ProjectCommentreRepository = value;
            }
        }

        public IRepository<ProjectExternalShare> ProjectExternalShareRepository
        {
            get
            {
                if (_ProjectExternalShareRepository == null)
                    _ProjectExternalShareRepository = new Repository<ProjectExternalShare>(_dbContext);

                return _ProjectExternalShareRepository;
            }

            set
            {
                _ProjectExternalShareRepository = value;
            }
        }

        public IRepository<Project> ProjectRepository
        {
            get
            {
                if (_ProjectRepository == null)
                    _ProjectRepository = new Repository<Project>(_dbContext);

                return _ProjectRepository;
            }

            set
            {
                _ProjectRepository = value;
            }
        }

      

        public IRepository<ProjectStat> ProjectStatRepository
        {
            get
            {
                if (_ProjectStatRepository == null)
                    _ProjectStatRepository = new Repository<ProjectStat>(_dbContext);

                return _ProjectStatRepository;
            }

            set
            {
                _ProjectStatRepository = value;
            }
        }

        public IRepository<ProjectUpdate> ProjectUpdateRepository
        {
            get
            {
                if (_ProjectUpdateRepository == null)
                    _ProjectUpdateRepository = new Repository<ProjectUpdate>(_dbContext);

                return _ProjectUpdateRepository;
            }

            set
            {
                _ProjectUpdateRepository = value;
            }
        }

        public IRepository<UserFunding> UserFundingRepository
        {
            get
            {
                if (_UserFundingRepository == null)
                    _UserFundingRepository = new Repository<UserFunding>(_dbContext);

                return _UserFundingRepository;
            }

            set
            {
                _UserFundingRepository = value;
            }
        }

        public IRepository<User> UserRepository
        {
            get
            {
                if (_UserRepository == null)
                    _UserRepository = new Repository<User>(_dbContext);

                return _UserRepository;
            }

            set
            {
                _UserRepository = value;
            }
        }

        public void Dispose()
        {

            if (_FundingPackageRepository != null)
                _FundingPackageRepository.Dispose();

            if (_ProjectCategoryRepository != null)
                _ProjectCategoryRepository.Dispose();

            if (_AttachmentRepository != null)
                _AttachmentRepository.Dispose();

            if (_AttachmentSetRepository != null)
                _AttachmentSetRepository.Dispose();

            if (_ProjectCommentreRepository != null)
                _ProjectCommentreRepository.Dispose();

            if (_ProjectUpdateRepository != null)
                _ProjectUpdateRepository.Dispose();

            if (_ProjectRepository != null)
                _ProjectRepository.Dispose();

            if (_UserRepository != null)
                _UserRepository.Dispose();

            if (_UserFundingRepository != null)
                _UserFundingRepository.Dispose();

            if (_ProjectStatRepository != null)
                _ProjectStatRepository.Dispose();

            if (_ProjectExternalShareRepository != null)
                _ProjectExternalShareRepository.Dispose();

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
