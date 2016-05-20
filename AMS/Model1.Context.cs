﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AMS
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AMSEntities : DbContext
    {
        public AMSEntities()
            : base("name=AMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<AroundProvider> AroundProviders { get; set; }
        public virtual DbSet<AroundProviderCategory> AroundProviderCategories { get; set; }
        public virtual DbSet<HelpdeskRequest> HelpdeskRequests { get; set; }
        public virtual DbSet<HelpdeskRequestHelpdeskSupporter> HelpdeskRequestHelpdeskSupporters { get; set; }
        public virtual DbSet<HelpdeskRequestLog> HelpdeskRequestLogs { get; set; }
        public virtual DbSet<HelpdeskService> HelpdeskServices { get; set; }
        public virtual DbSet<HelpdeskServiceCategory> HelpdeskServiceCategories { get; set; }
        public virtual DbSet<House> Houses { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Receipt> Receipts { get; set; }
        public virtual DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public virtual DbSet<ResidentRateAroundProvider> ResidentRateAroundProviders { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ServiceFee> ServiceFees { get; set; }
        public virtual DbSet<Survey> Surveys { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAnswerSurvey> UserAnswerSurveys { get; set; }
        public virtual DbSet<UserInHouse> UserInHouses { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
    }
}
