﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VivaWallet.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VivaWalletEntities : DbContext
    {
        public VivaWalletEntities()
            : base("name=VivaWalletEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<AttachmentSet> AttachmentSets { get; set; }
        public virtual DbSet<FundingPackage> FundingPackages { get; set; }
        public virtual DbSet<ProjectCategory> ProjectCategories { get; set; }
        public virtual DbSet<ProjectComment> ProjectComments { get; set; }
        public virtual DbSet<ProjectExternalShare> ProjectExternalShares { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectStat> ProjectStats { get; set; }
        public virtual DbSet<ProjectUpdate> ProjectUpdates { get; set; }
        public virtual DbSet<UserFunding> UserFundings { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
