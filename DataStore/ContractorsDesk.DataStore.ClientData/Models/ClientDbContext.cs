using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ClientDbContext : DbContext
{
    public ClientDbContext(DbContextOptions<ClientDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionItem> ActionItems { get; set; }

    public virtual DbSet<ActionItemComment> ActionItemComments { get; set; }

    public virtual DbSet<ActionItemCostChange> ActionItemCostChanges { get; set; }

    public virtual DbSet<ActionItemScheduleChange> ActionItemScheduleChanges { get; set; }

    public virtual DbSet<ActionItemsSupervisor> ActionItemsSupervisors { get; set; }

    public virtual DbSet<ActionType> ActionTypes { get; set; }

    public virtual DbSet<ActivityStream> ActivityStreams { get; set; }

    public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }

    public virtual DbSet<AppConfiguration> AppConfigurations { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<AudioUpload> AudioUploads { get; set; }

    public virtual DbSet<ChangeOrder> ChangeOrders { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientDocument> ClientDocuments { get; set; }

    public virtual DbSet<ClientProject> ClientProjects { get; set; }

    public virtual DbSet<CompanySetting> CompanySettings { get; set; }

    public virtual DbSet<ConstructionTask> ConstructionTasks { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<CostRevision> CostRevisions { get; set; }

    public virtual DbSet<CostRevisionItem> CostRevisionItems { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<EmailAttachment> EmailAttachments { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Estimate> Estimates { get; set; }

    public virtual DbSet<EstimateCategory> EstimateCategories { get; set; }

    public virtual DbSet<EstimateHistory> EstimateHistories { get; set; }

    public virtual DbSet<EstimateMapping> EstimateMappings { get; set; }

    public virtual DbSet<EstimateSummary> EstimateSummaries { get; set; }

    public virtual DbSet<Hash> Hashes { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobBalance> JobBalances { get; set; }

    public virtual DbSet<JobParameter> JobParameters { get; set; }

    public virtual DbSet<JobQueue> JobQueues { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<MismatchesJobTransaction> MismatchesJobTransactions { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<ProjectDocument> ProjectDocuments { get; set; }

    public virtual DbSet<ProjectJournal> ProjectJournals { get; set; }

    public virtual DbSet<ProjectManagement> ProjectManagements { get; set; }

    public virtual DbSet<ProjectManagementLine> ProjectManagementLines { get; set; }

    public virtual DbSet<ProjectNote> ProjectNotes { get; set; }

    public virtual DbSet<ProjectSchedule> ProjectSchedules { get; set; }

    public virtual DbSet<ProjectScheduleDelay> ProjectScheduleDelays { get; set; }

    public virtual DbSet<ProjectScheduleTask> ProjectScheduleTasks { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<ProjectSubContractor> ProjectSubContractors { get; set; }

    public virtual DbSet<ProjectSupervisor> ProjectSupervisors { get; set; }

    public virtual DbSet<ProjectThreshold> ProjectThresholds { get; set; }

    public virtual DbSet<ProjectTotal> ProjectTotals { get; set; }

    public virtual DbSet<Proposal> Proposals { get; set; }

    public virtual DbSet<ProposalLine> ProposalLines { get; set; }

    public virtual DbSet<ProposalLinesHistory> ProposalLinesHistories { get; set; }

    public virtual DbSet<ProposalProject> ProposalProjects { get; set; }

    public virtual DbSet<ProposalSupervisor> ProposalSupervisors { get; set; }

    public virtual DbSet<ProposalTemplate> ProposalTemplates { get; set; }

    public virtual DbSet<ProposalTemplateUserDefault> ProposalTemplateUserDefaults { get; set; }

    public virtual DbSet<ProposalTemplatesLineItem> ProposalTemplatesLineItems { get; set; }

    public virtual DbSet<Qbaccount> Qbaccounts { get; set; }

    public virtual DbSet<Qbbill> Qbbills { get; set; }

    public virtual DbSet<Qbclass> Qbclasses { get; set; }

    public virtual DbSet<QbclassesExcel> QbclassesExcels { get; set; }

    public virtual DbSet<QbclassesExcelNew> QbclassesExcelNews { get; set; }

    public virtual DbSet<QbcreditMemo> QbcreditMemos { get; set; }

    public virtual DbSet<Qbcustomer> Qbcustomers { get; set; }

    public virtual DbSet<Qbinvoice> Qbinvoices { get; set; }

    public virtual DbSet<Qbitem> Qbitems { get; set; }

    public virtual DbSet<QbjournalEntry> QbjournalEntries { get; set; }

    public virtual DbSet<Qblocation> Qblocations { get; set; }

    public virtual DbSet<QbolastModifiedTimestamp> QbolastModifiedTimestamps { get; set; }

    public virtual DbSet<QbsalesReceipt> QbsalesReceipts { get; set; }

    public virtual DbSet<Qbtransaction> Qbtransactions { get; set; }

    public virtual DbSet<Qbvendor> Qbvendors { get; set; }

    public virtual DbSet<QbvendorCredit> QbvendorCredits { get; set; }

    public virtual DbSet<QuickBooksToken> QuickBooksTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<ScheduleRevision> ScheduleRevisions { get; set; }

    public virtual DbSet<ScheduleRevisionItem> ScheduleRevisionItems { get; set; }

    public virtual DbSet<ScheduleTaskMapping> ScheduleTaskMappings { get; set; }

    public virtual DbSet<Schema> Schemas { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<SpErrorLog> SpErrorLogs { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<SubContractor> SubContractors { get; set; }

    public virtual DbSet<SubContractorCategory> SubContractorCategories { get; set; }

    public virtual DbSet<SysBackgroundJob> SysBackgroundJobs { get; set; }

    public virtual DbSet<SysDataSyncSetting> SysDataSyncSettings { get; set; }

    public virtual DbSet<SysFolder> SysFolders { get; set; }

    public virtual DbSet<Test1> Test1s { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<TransactionIndexDb5d640bC9784222B1f5319e2688640a> TransactionIndexDb5d640bC9784222B1f5319e2688640as { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBookmark> UserBookmarks { get; set; }

    public virtual DbSet<UserLog> UserLogs { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    public virtual DbSet<UserResetPasswordRequest> UserResetPasswordRequests { get; set; }

    public virtual DbSet<UserUpload> UserUploads { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<VendorCategory> VendorCategories { get; set; }

    public virtual DbSet<VwActionItemsSummary> VwActionItemsSummaries { get; set; }

    public virtual DbSet<VwActiveConstructionJobDetail> VwActiveConstructionJobDetails { get; set; }

    public virtual DbSet<VwActiveJob> VwActiveJobs { get; set; }

    public virtual DbSet<VwActiveJobs2> VwActiveJobs2s { get; set; }

    public virtual DbSet<VwActiveSpecJobDetail> VwActiveSpecJobDetails { get; set; }

    public virtual DbSet<VwEstimateDataMapping> VwEstimateDataMappings { get; set; }

    public virtual DbSet<VwProjectShortDetail> VwProjectShortDetails { get; set; }

    public virtual DbSet<VwSupervisorAndClientActiveJob> VwSupervisorAndClientActiveJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionItem>(entity =>
        {
            entity.Property(e => e.Source).HasDefaultValue(1);
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.AcceptedByNavigation).WithMany(p => p.ActionItemAcceptedByNavigations).HasForeignKey(d => d.AcceptedBy);

            entity.HasOne(d => d.ActionType).WithMany(p => p.ActionItems).HasForeignKey(d => d.ActionTypeId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ActionItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ActionItems)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ActionItems_ActionItems_QBClasses");
        });

        modelBuilder.Entity<ActionItemComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActionIt__3214EC07D32BBD4D");

            entity.HasOne(d => d.ActionItem).WithMany(p => p.ActionItemComments)
                .HasForeignKey(d => d.ActionItemId)
                .HasConstraintName("FK_ActionItemComments_ActionItems");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ActionItemComments)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActionItemComments_Users");
        });

        modelBuilder.Entity<ActionItemCostChange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActionIt__3214EC078675B491");

            entity.ToTable("ActionItemCostChange");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ActionItem).WithMany(p => p.ActionItemCostChanges)
                .HasForeignKey(d => d.ActionItemId)
                .HasConstraintName("FK_ActionItemCostChange_ActionItem");
        });

        modelBuilder.Entity<ActionItemScheduleChange>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActionIt__3214EC0760F9A3CA");

            entity.ToTable("ActionItemScheduleChange");

            entity.HasOne(d => d.ActionItem).WithMany(p => p.ActionItemScheduleChanges)
                .HasForeignKey(d => d.ActionItemId)
                .HasConstraintName("FK_ActionItemScheduleChange_ActionItems");
        });

        modelBuilder.Entity<ActionItemsSupervisor>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");

            entity.HasOne(d => d.ActionItem).WithMany(p => p.ActionItemsSupervisors)
                .HasForeignKey(d => d.ActionItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActionItemsSupervisors_ActionItemsSupervisors");

            entity.HasOne(d => d.Supervisor).WithMany(p => p.ActionItemsSupervisors)
                .HasForeignKey(d => d.SupervisorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActionItemsSupervisors_Users");
        });

        modelBuilder.Entity<ActivityStream>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activity__3214EC075B1A25FD");

            entity.ToTable("ActivityStream");

            entity.Property(e => e.Ref).HasMaxLength(150);
            entity.Property(e => e.StepName).HasMaxLength(100);
        });

        modelBuilder.Entity<AggregatedCounter>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK_HangFire_CounterAggregated");

            entity.ToTable("AggregatedCounter", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<AppConfiguration>(entity =>
        {
            entity.ToTable("AppConfiguration");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AudioUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AudioUpl__3214EC0785946BBE");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Aistatus)
                .HasMaxLength(50)
                .HasColumnName("AIStatus");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.FileUrl).HasMaxLength(250);
            entity.Property(e => e.StorageStatus).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithMany(p => p.AudioUploads)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_AudioUploads_QbClasses");
        });

        modelBuilder.Entity<ChangeOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChangeOr__3214EC070FF573C7");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostChangeName).HasMaxLength(100);
            entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NewAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScheduleChangeItem).HasMaxLength(100);

            entity.HasOne(d => d.ActionItem).WithMany(p => p.ChangeOrders)
                .HasForeignKey(d => d.ActionItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChangeOrders_ActionItems");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC0722DE6264");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsFixedLength();
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.EmailAddress).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.SecondaryEmailAddress).HasMaxLength(250);
            entity.Property(e => e.State).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Clients)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Clients_Users");
        });

        modelBuilder.Entity<ClientDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientDo__3214EC07070E30D0");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.FileExtension).HasMaxLength(50);
            entity.Property(e => e.FileName).HasMaxLength(150);
            entity.Property(e => e.Url).HasMaxLength(250);
            entity.Property(e => e.Version).HasDefaultValue(1);

            entity.HasOne(d => d.Folder).WithMany(p => p.ClientDocuments)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientDocuments_SysFolders");

            entity.HasOne(d => d.Subcontractor).WithMany(p => p.ClientDocuments)
                .HasForeignKey(d => d.SubcontractorId)
                .HasConstraintName("FK_ClientDocuments_Subcontractors");
        });

        modelBuilder.Entity<ClientProject>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.ClientProjects)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientProjects_QbClasses");
        });

        modelBuilder.Entity<CompanySetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CompanyS__3214EC07C86B667A");

            entity.Property(e => e.Address1).IsUnicode(false);
            entity.Property(e => e.Address2).IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CompanyEmail).HasMaxLength(200);
            entity.Property(e => e.CompanyLogoUrl).HasMaxLength(200);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.GeneralContractorName).HasMaxLength(200);
            entity.Property(e => e.Zip)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ConstructionTask>(entity =>
        {
            entity.HasIndex(e => e.ParentTaskId, "IX_ConstructionTasks_ParentTaskID");

            entity.HasIndex(e => e.QbclassId, "IX_ConstructionTasks_QBClassID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ParentTaskId).HasColumnName("ParentTaskID");
            entity.Property(e => e.Pred1Id).HasColumnName("Pred1ID");
            entity.Property(e => e.Pred2Id).HasColumnName("Pred2ID");
            entity.Property(e => e.Pred3Id).HasColumnName("Pred3ID");
            entity.Property(e => e.QbclassId).HasColumnName("QBClassID");

            entity.HasOne(d => d.ParentTask).WithMany(p => p.InverseParentTask).HasForeignKey(d => d.ParentTaskId);

            entity.HasOne(d => d.Qbclass).WithMany(p => p.ConstructionTasks).HasForeignKey(d => d.QbclassId);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contacts__3214EC07D1CACD08");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("Contract");

            entity.HasIndex(e => e.Name, "UQ_Contract_Name").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateModified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DefaultFolderName).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(300);
        });

        modelBuilder.Entity<CostRevision>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CostRevi__3214EC07961703CA");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.StatusId).HasDefaultValue(1);

            entity.HasOne(d => d.Project).WithMany(p => p.CostRevisions)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostRevisions_QbClasses");
        });

        modelBuilder.Entity<CostRevisionItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CostRevi__3214EC07C0303987");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NewAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CostRevision).WithMany(p => p.CostRevisionItems)
                .HasForeignKey(d => d.CostRevisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CostRevisions_CostRevisionItems");
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_Counter");

            entity.ToTable("Counter", "HangFire");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC0795183F91");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Bcc).IsUnicode(false);
            entity.Property(e => e.Body).IsUnicode(false);
            entity.Property(e => e.Cc).IsUnicode(false);
            entity.Property(e => e.From).HasMaxLength(150);
            entity.Property(e => e.MessageId).HasMaxLength(250);
            entity.Property(e => e.ReplyToMessageId).HasMaxLength(250);
            entity.Property(e => e.Subject).IsUnicode(false);
            entity.Property(e => e.To).HasMaxLength(150);
        });

        modelBuilder.Entity<EmailAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailAtt__3214EC0790FC427A");

            entity.Property(e => e.FileName).HasMaxLength(100);
            entity.Property(e => e.FileUrl).HasMaxLength(250);

            entity.HasOne(d => d.Email).WithMany(p => p.EmailAttachments)
                .HasForeignKey(d => d.EmailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailAttachments_Emails");
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.ToTable("EmailTemplate");

            entity.HasIndex(e => new { e.Name, e.EmailType, e.OwnerId }, "UQ_EmailTemplate_Name_Type").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateModified).HasColumnType("datetime");
            entity.Property(e => e.EmailType).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(300);
        });

        modelBuilder.Entity<Estimate>(entity =>
        {
            entity.HasIndex(e => e.EstimateSubCategoryId, "IX_Estimates_EstimateSubCategoryID");

            entity.HasIndex(e => e.QbclassId, "IX_Estimates_QBClassID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateSubCategoryId).HasColumnName("EstimateSubCategoryID");
            entity.Property(e => e.QbclassId).HasColumnName("QBClassID");

            entity.HasOne(d => d.EstimateSubCategory).WithMany(p => p.Estimates).HasForeignKey(d => d.EstimateSubCategoryId);

            entity.HasOne(d => d.Qbclass).WithMany(p => p.Estimates).HasForeignKey(d => d.QbclassId);
        });

        modelBuilder.Entity<EstimateCategory>(entity =>
        {
            entity.HasIndex(e => e.ParentEstimateCategoryId, "IX_EstimateCategories_ParentEstimateCategoryID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name).HasDefaultValue("");
            entity.Property(e => e.ParentEstimateCategoryId).HasColumnName("ParentEstimateCategoryID");

            entity.HasOne(d => d.ParentEstimateCategory).WithMany(p => p.InverseParentEstimateCategory).HasForeignKey(d => d.ParentEstimateCategoryId);
        });

        modelBuilder.Entity<EstimateHistory>(entity =>
        {
            entity.HasIndex(e => e.EstimateId, "IX_EstimateHistories_EstimateID");

            entity.HasIndex(e => e.QbclassId, "IX_EstimateHistories_QBClassID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateId).HasColumnName("EstimateID");
            entity.Property(e => e.QbclassId).HasColumnName("QBClassID");

            entity.HasOne(d => d.Estimate).WithMany(p => p.EstimateHistories).HasForeignKey(d => d.EstimateId);

            entity.HasOne(d => d.Qbclass).WithMany(p => p.EstimateHistories).HasForeignKey(d => d.QbclassId);
        });

        modelBuilder.Entity<EstimateMapping>(entity =>
        {
            entity.HasIndex(e => e.EstimateSubCategoryId, "IX_EstimateMappings_EstimateSubCategoryID");

            entity.HasIndex(e => e.QbaccountId, "IX_EstimateMappings_QBAccountID");

            entity.HasIndex(e => e.QbclassId, "IX_EstimateMappings_QBClassID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.EstimateSubCategoryId).HasColumnName("EstimateSubCategoryID");
            entity.Property(e => e.QbaccountId).HasColumnName("QBAccountID");
            entity.Property(e => e.QbclassId).HasColumnName("QBClassID");

            entity.HasOne(d => d.EstimateSubCategory).WithMany(p => p.EstimateMappings).HasForeignKey(d => d.EstimateSubCategoryId);

            entity.HasOne(d => d.Qbaccount).WithMany(p => p.EstimateMappings).HasForeignKey(d => d.QbaccountId);

            entity.HasOne(d => d.Qbclass).WithMany(p => p.EstimateMappings).HasForeignKey(d => d.QbclassId);
        });

        modelBuilder.Entity<EstimateSummary>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateCategoryId).HasColumnName("EstimateCategoryID");
            entity.Property(e => e.ParentEstimateCategoryId).HasColumnName("ParentEstimateCategoryID");
            entity.Property(e => e.Percentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RevisedEstimateAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalToDate).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Hash>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Field }).HasName("PK_HangFire_Hash");

            entity.ToTable("Hash", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Field).HasMaxLength(100);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invoices__3214EC070D4E15BA");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(5);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Client).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_QbClasses");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InvoiceI__3214EC07190CF820");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceItems_Invoice");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Job");

            entity.ToTable("Job", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName").HasFilter("([StateName] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            entity.Property(e => e.StateName).HasMaxLength(20);
        });

        modelBuilder.Entity<JobBalance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobBalan__3214EC07A0933FEE");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");

            entity.HasOne(d => d.Job).WithMany(p => p.JobBalances)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_JobBalances_QbClass");
        });

        modelBuilder.Entity<JobParameter>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Name }).HasName("PK_HangFire_JobParameter");

            entity.ToTable("JobParameter", "HangFire");

            entity.Property(e => e.Name).HasMaxLength(40);

            entity.HasOne(d => d.Job).WithMany(p => p.JobParameters)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_JobParameter_Job");
        });

        modelBuilder.Entity<JobQueue>(entity =>
        {
            entity.HasKey(e => new { e.Queue, e.Id }).HasName("PK_HangFire_JobQueue");

            entity.ToTable("JobQueue", "HangFire");

            entity.Property(e => e.Queue).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FetchedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_List");

            entity.ToTable("List", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Level).HasMaxLength(15);
            entity.Property(e => e.Timestamp).HasMaxLength(100);
            entity.Property(e => e.Ts).HasColumnName("_ts");
        });

        modelBuilder.Entity<MismatchesJobTransaction>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("MismatchesJobTransactions");

            entity.Property(e => e.AccountCategory)
                .HasMaxLength(255)
                .HasColumnName("Account/Category");
            entity.Property(e => e.Amount).HasColumnType("decimal(21, 2)");
            entity.Property(e => e.BankingAccountApp)
                .HasMaxLength(255)
                .HasColumnName("BankingAccountAPP");
            entity.Property(e => e.BankingAccountQbo)
                .HasMaxLength(255)
                .HasColumnName("BankingAccountQBO");
            entity.Property(e => e.Class).HasMaxLength(255);
            entity.Property(e => e.DivisionLocation)
                .HasMaxLength(255)
                .HasColumnName("Division/Location");
            entity.Property(e => e.Payee).HasMaxLength(255);
            entity.Property(e => e.RefNo).HasMaxLength(255);
            entity.Property(e => e.TxnId)
                .HasMaxLength(255)
                .HasColumnName("TxnID");
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC07D4CF1FB9");

            entity.Property(e => e.Description).HasMaxLength(250);
        });

        modelBuilder.Entity<ProjectDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectD__3214EC07088C4FDB");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.FileUrl).HasMaxLength(250);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectDocuments)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectDocuments_ToTable");
        });

        modelBuilder.Entity<ProjectJournal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectJ__3214EC07B05AC7FB");

            entity.ToTable("ProjectJournal");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectJournals)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectJournal_QbClasses");
        });

        modelBuilder.Entity<ProjectManagement>(entity =>
        {
            entity.Property(e => e.ProjectManagementId)
                .ValueGeneratedNever()
                .HasColumnName("ProjectManagementID");
            entity.Property(e => e.ConstructionTaskId).HasColumnName("ConstructionTaskID");
            entity.Property(e => e.Pred1Id).HasColumnName("Pred1ID");
            entity.Property(e => e.Pred1LagId).HasColumnName("Pred1LagID");
            entity.Property(e => e.Pred2Id).HasColumnName("Pred2ID");
            entity.Property(e => e.Pred2LagId).HasColumnName("Pred2LagID");
            entity.Property(e => e.Pred3Id).HasColumnName("Pred3ID");
            entity.Property(e => e.Pred3LagId).HasColumnName("Pred3LagID");
            entity.Property(e => e.ProgressPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.QbclassId).HasColumnName("QBClassID");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<ProjectManagementLine>(entity =>
        {
            entity.HasIndex(e => e.ProjectManagementId, "IX_ProjectManagementLines_ProjectManagementID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Pred1Id).HasColumnName("Pred1ID");
            entity.Property(e => e.Pred1LagId).HasColumnName("Pred1LagID");
            entity.Property(e => e.Pred2Id).HasColumnName("Pred2ID");
            entity.Property(e => e.Pred2LagId).HasColumnName("Pred2LagID");
            entity.Property(e => e.Pred3Id).HasColumnName("Pred3ID");
            entity.Property(e => e.Pred3LagId).HasColumnName("Pred3LagID");
            entity.Property(e => e.ProgressPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ProjectManagementId).HasColumnName("ProjectManagementID");
        });

        modelBuilder.Entity<ProjectNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectN__3214EC07EBF628ED");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectNoteCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectNotes_CreatedByUser");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectNotes)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectNotes_QbClasses");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProjectNoteUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_ProjectNotes_UpdatedByUser");
        });

        modelBuilder.Entity<ProjectSchedule>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectSchedules)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectSchedules_QbClass");
        });

        modelBuilder.Entity<ProjectScheduleDelay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectS__3214EC0751F3B843");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApplyToOtherProjects).HasDefaultValue(false);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Reason).HasMaxLength(150);

            entity.HasOne(d => d.ProjectSchedule).WithMany(p => p.ProjectScheduleDelays)
                .HasForeignKey(d => d.ProjectScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectScheduleDelays_ToTable");

            entity.HasOne(d => d.Task).WithMany(p => p.ProjectScheduleDelays)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_ProjectScheduleDelays_ProjectScheduleTasks");
        });

        modelBuilder.Entity<ProjectScheduleTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectS__3214EC079B9630E2");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.ProjectSchedule).WithMany(p => p.ProjectScheduleTasks)
                .HasForeignKey(d => d.ProjectScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectScheduleTasks_ToTable");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.ToTable("ProjectStatus");

            entity.HasIndex(e => e.ProjectManagementId, "IX_ProjectStatus_ProjectManagementID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ProjectManagementId).HasColumnName("ProjectManagementID");
        });

        modelBuilder.Entity<ProjectSubContractor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectS__3214EC0744A0502A");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectSubContractors)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectSubContractors_QbClasses");

            entity.HasOne(d => d.SubContractor).WithMany(p => p.ProjectSubContractors)
                .HasForeignKey(d => d.SubContractorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectSubContractors_SubContractors");
        });

        modelBuilder.Entity<ProjectSupervisor>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.ProjectSupervisors)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectSupervisors_QBClasses");
        });

        modelBuilder.Entity<ProjectThreshold>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectT__3214EC07DEFCFE72");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Threshold).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectThresholds)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectThresholds_QbClass");
        });

        modelBuilder.Entity<ProjectTotal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectT__3214EC072FF8A370");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CostToDate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinimumRequestedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OwnerDeposits).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Proposal>(entity =>
        {
            entity.HasIndex(e => e.QbclassId, "IX_Proposals_QBClassID");

            entity.HasIndex(e => e.ClientId, "IX_Proposals_QBCustomerID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasDefaultValue(1);
            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DocStatus).HasDefaultValue("");
            entity.Property(e => e.QbclassId).HasColumnName("QBClassId");
            entity.Property(e => e.QbcustomerId).HasColumnName("QBCustomerId");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Client).WithMany(p => p.Proposals)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Proposals_Clients");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProposalCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProposalProject).WithMany(p => p.Proposals)
                .HasForeignKey(d => d.ProposalProjectId)
                .HasConstraintName("FK_Proposals_ProposalProjects");

            entity.HasOne(d => d.Qbclass).WithMany(p => p.Proposals)
                .HasForeignKey(d => d.QbclassId)
                .HasConstraintName("FK_Proposals_QBClasses");

            entity.HasOne(d => d.Template).WithMany(p => p.Proposals)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("FK_Proposals_ProposalTemplates");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProposalUpdatedByNavigations).HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ProposalLine>(entity =>
        {
            entity.HasIndex(e => e.EstimateCategoryId, "IX_ProposalLines_EstimateCategoryID");

            entity.HasIndex(e => e.ParentEstimateCategoryId, "IX_ProposalLines_ParentEstimateCategoryID");

            entity.HasIndex(e => e.ProposalId, "IX_ProposalLines_ProposalID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateCategoryId).HasColumnName("EstimateCategoryID");
            entity.Property(e => e.Multiplier).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.ParentEstimateCategoryId).HasColumnName("ParentEstimateCategoryID");
            entity.Property(e => e.ProposalId).HasColumnName("ProposalID");
            entity.Property(e => e.SqFoot).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.EstimateCategory).WithMany(p => p.ProposalLineEstimateCategories).HasForeignKey(d => d.EstimateCategoryId);

            entity.HasOne(d => d.ParentEstimateCategory).WithMany(p => p.ProposalLineParentEstimateCategories).HasForeignKey(d => d.ParentEstimateCategoryId);

            entity.HasOne(d => d.Proposal).WithMany(p => p.ProposalLines).HasForeignKey(d => d.ProposalId);
        });

        modelBuilder.Entity<ProposalLinesHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("ProposalLinesHistory");

            entity.HasIndex(e => e.ProposalLineId, "IX_ProposalLinesHistory_ProposalLineID");

            entity.Property(e => e.HistoryId)
                .ValueGeneratedNever()
                .HasColumnName("HistoryID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimateCategoryId).HasColumnName("EstimateCategoryID");
            entity.Property(e => e.ParentEstimateCategoryId).HasColumnName("ParentEstimateCategoryID");
            entity.Property(e => e.ProposalId).HasColumnName("ProposalID");
            entity.Property(e => e.ProposalLineId).HasColumnName("ProposalLineID");
        });

        modelBuilder.Entity<ProposalProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC0736E644DD");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
        });

        modelBuilder.Entity<ProposalSupervisor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Proposal__3214EC0700C676AB");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Proposal).WithMany(p => p.ProposalSupervisors)
                .HasForeignKey(d => d.ProposalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProposalSupervisors_Proposals");

            entity.HasOne(d => d.User).WithMany(p => p.ProposalSupervisors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProposalSupervisors_Users");
        });

        modelBuilder.Entity<ProposalTemplate>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ProposalTemplateUserDefault>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Proposal__3214EC07187D4081");

            entity.ToTable("ProposalTemplateUserDefault");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Template).WithMany(p => p.ProposalTemplateUserDefaults)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TemplateId");
        });

        modelBuilder.Entity<ProposalTemplatesLineItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_ProposalTemplatesLineItems_ProposalTemplatesLineItems");

            entity.HasOne(d => d.ProposalTemplate).WithMany(p => p.ProposalTemplatesLineItems)
                .HasForeignKey(d => d.ProposalTemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProposalTemplatesLineItems_ProposalTemplates");
        });

        modelBuilder.Entity<Qbaccount>(entity =>
        {
            entity.ToTable("QBAccounts");

            entity.HasIndex(e => e.Id, "IX_QBAccounts_ID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<Qbbill>(entity =>
        {
            entity.ToTable("QBBills");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmt).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Qbclass>(entity =>
        {
            entity.ToTable("QBClasses");

            entity.HasIndex(e => e.QbaccountId, "IX_QBClasses_QBAccountID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.MinimumRequestedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.QbaccountId).HasColumnName("QBAccountID");

            entity.HasOne(d => d.Qbaccount).WithMany(p => p.Qbclasses).HasForeignKey(d => d.QbaccountId);
        });

        modelBuilder.Entity<QbclassesExcel>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("QBClassesExcel");

            entity.Property(e => e.QbaccountId).HasColumnName("QBAccountID");
        });

        modelBuilder.Entity<QbclassesExcelNew>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("QBClassesExcelNew");

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.QbaccountId).HasColumnName("QBAccountID");
        });

        modelBuilder.Entity<QbcreditMemo>(entity =>
        {
            entity.ToTable("QBCreditMemos");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RemainingCredit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTax).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Qbcustomer>(entity =>
        {
            entity.ToTable("QBCustomers");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.Nace).HasColumnName("NACE");
            entity.Property(e => e.Nuinumber).HasColumnName("NUINumber");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.Vatnumber).HasColumnName("VATNumber");
        });

        modelBuilder.Entity<Qbinvoice>(entity =>
        {
            entity.ToTable("QBInvoices");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Deposit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EinvoiceStatus).HasColumnName("EInvoiceStatus");
            entity.Property(e => e.NetAmount0).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetAmount18).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NetAmount8).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount18).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount8).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTax).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Qbitem>(entity =>
        {
            entity.ToTable("QBItems");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ExpenseAccountId).HasColumnName("ExpenseAccountID");
            entity.Property(e => e.ListId).HasColumnName("ListID");
        });

        modelBuilder.Entity<QbjournalEntry>(entity =>
        {
            entity.ToTable("QBJournalEntries");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreditAccountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DebitAccountAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Qblocation>(entity =>
        {
            entity.ToTable("QBLocations");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ListId).HasColumnName("ListID");
        });

        modelBuilder.Entity<QbolastModifiedTimestamp>(entity =>
        {
            entity.ToTable("QBOLastModifiedTimestamps");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<QbsalesReceipt>(entity =>
        {
            entity.ToTable("QBSalesReceipts");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTax).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Qbtransaction>(entity =>
        {
            entity.ToTable("QBTransactions");

            entity.HasIndex(e => e.AccountId, "IX_QBTransactions_AccountID");

            entity.HasIndex(e => e.ClassId, "IX_QBTransactions_ClassID");

            entity.HasIndex(e => e.CustomerId, "IX_QBTransactions_CustomerID");

            entity.HasIndex(e => e.VendorId, "IX_QBTransactions_VendorID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.SplitAccountId).HasColumnName("SplitAccountID");
            entity.Property(e => e.TxnId).HasColumnName("TxnID");
            entity.Property(e => e.VendorId).HasColumnName("VendorID");

            entity.HasOne(d => d.Account).WithMany(p => p.Qbtransactions).HasForeignKey(d => d.AccountId);

            entity.HasOne(d => d.Class).WithMany(p => p.Qbtransactions).HasForeignKey(d => d.ClassId);

            entity.HasOne(d => d.Customer).WithMany(p => p.Qbtransactions).HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Vendor).WithMany(p => p.Qbtransactions).HasForeignKey(d => d.VendorId);
        });

        modelBuilder.Entity<Qbvendor>(entity =>
        {
            entity.ToTable("QBVendors");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.Nace).HasColumnName("NACE");
            entity.Property(e => e.Nuinumber).HasColumnName("NUINumber");
            entity.Property(e => e.Vatnumber).HasColumnName("VATNumber");
        });

        modelBuilder.Entity<QbvendorCredit>(entity =>
        {
            entity.ToTable("QBVendorCredits");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApaccountName).HasColumnName("APAccountName");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmt).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<QuickBooksToken>(entity =>
        {
            entity.ToTable("QuickBooksToken");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccessToken).HasColumnName("AccessToken ");
            entity.Property(e => e.ExpiryTime)
                .HasColumnType("datetime")
                .HasColumnName("ExpiryTime ");
            entity.Property(e => e.RealmId).HasMaxLength(200);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07158BEE8A");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolePerm__3214EC07CB1DF07F");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Permissions");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Roles");
        });

        modelBuilder.Entity<ScheduleRevision>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC07985008A7");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.StatusId).HasDefaultValue(1);

            entity.HasOne(d => d.Project).WithMany(p => p.ScheduleRevisions)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScheduleRevisions_QbClasses");
        });

        modelBuilder.Entity<ScheduleRevisionItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC071F65BD28");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.ScheduleRevision).WithMany(p => p.ScheduleRevisionItems)
                .HasForeignKey(d => d.ScheduleRevisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScheduleRevisionItems_ScheduleRevisions");
        });

        modelBuilder.Entity<ScheduleTaskMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3214EC078D05B24D");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");

            entity.HasOne(d => d.ConstructionTask).WithMany(p => p.ScheduleTaskMappings)
                .HasForeignKey(d => d.ConstructionTaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScheduleTaskMappings_ConstructionTasks");

            entity.HasOne(d => d.EstimateCategory).WithMany(p => p.ScheduleTaskMappings)
                .HasForeignKey(d => d.EstimateCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScheduleTaskMappings_EstimateCategories");
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PK_HangFire_Schema");

            entity.ToTable("Schema", "HangFire");

            entity.Property(e => e.Version).ValueGeneratedNever();
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Server");

            entity.ToTable("Server", "HangFire");

            entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

            entity.Property(e => e.Id).HasMaxLength(200);
            entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Value }).HasName("PK_HangFire_Set");

            entity.ToTable("Set", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(256);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<SpErrorLog>(entity =>
        {
            entity.ToTable("SpErrorLogs", "audit");

            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.SpName).HasColumnName("SP_Name");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Id }).HasName("PK_HangFire_State");

            entity.ToTable("State", "HangFire");

            entity.HasIndex(e => e.CreatedAt, "IX_HangFire_State_CreatedAt");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.Job).WithMany(p => p.States)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_State_Job");
        });

        modelBuilder.Entity<SubContractor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SubContr__3214EC077963F916");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.BondInsurancePolicyNo).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(150);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CompInsurancePolicyNo).HasMaxLength(100);
            entity.Property(e => e.Company).HasMaxLength(100);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LiabilityInsurancePolicyNo).HasMaxLength(100);
            entity.Property(e => e.LicenseNo).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Zip).HasMaxLength(20);
        });

        modelBuilder.Entity<SubContractorCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SubContr__3214EC07EC01374F");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
        });

        modelBuilder.Entity<SysBackgroundJob>(entity =>
        {
            entity.HasKey(e => e.SysJobsId).HasName("PK__SysBackg__CE00FE478A511204");

            entity.Property(e => e.ApiEndpoint).HasMaxLength(150);
            entity.Property(e => e.DateOfExecution)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.JobName).HasMaxLength(100);
            entity.Property(e => e.LastStatus).HasMaxLength(50);
        });

        modelBuilder.Entity<SysDataSyncSetting>(entity =>
        {
            entity.ToTable("SysDataSyncSetting");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DataSyncName).HasMaxLength(400);
        });

        modelBuilder.Entity<SysFolder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SysFolde__3214EC07AC721AA9");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy).HasDefaultValue(1);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Test1>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Test1");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Token");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<TransactionIndexDb5d640bC9784222B1f5319e2688640a>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK___Transac__3214EC078D394F71");

            entity.ToTable("_TransactionIndex_db5d640b-c978-4222-b1f5-319e2688640a");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07D6ECB251");

            entity.Property(e => e.AvatarUrl).HasMaxLength(200);
            entity.Property(e => e.ConfirmationCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.Phone).HasMaxLength(150);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UserBookmark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserBook__3214EC079959F8BF");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.Url).HasMaxLength(150);
        });

        modelBuilder.Entity<UserLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLogs__3214EC07535EE28D");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Url).HasMaxLength(150);
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.ToTable("UserNotification");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.NextActionEnum).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(300);
        });

        modelBuilder.Entity<UserResetPasswordRequest>(entity =>
        {
            entity.ToTable("UserResetPasswordRequest");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateSent).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.SentStatus).HasMaxLength(100);
        });

        modelBuilder.Entity<UserUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserUplo__3214EC07A59367CE");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Aistatus)
                .HasMaxLength(50)
                .HasColumnName("AIStatus");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.FileUrl).HasMaxLength(250);
            entity.Property(e => e.StorageStatus).HasMaxLength(50);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vendors__3214EC07B18B53A6");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(150);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Company).HasMaxLength(100);
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Zip).HasMaxLength(50);
        });

        modelBuilder.Entity<VendorCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VendorCa__3214EC0731F9254E");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
        });

        modelBuilder.Entity<VwActionItemsSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwActionItemsSummary");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostChangeItem).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(101);
            entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScheduleChangeItem).HasMaxLength(150);
        });

        modelBuilder.Entity<VwActiveConstructionJobDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwActiveConstructionJobDetails");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<VwActiveJob>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwActiveJobs");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.JobBalance).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<VwActiveJobs2>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwActiveJobs2");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.JobBalance).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<VwActiveSpecJobDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwActiveSpecJobDetails");

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Amount).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
        });

        modelBuilder.Entity<VwEstimateDataMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwEstimateDataMapping");
        });

        modelBuilder.Entity<VwProjectShortDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwProjectShortDetails");

            entity.Property(e => e.ClientEmailAddress).HasMaxLength(100);
            entity.Property(e => e.ClientName).HasMaxLength(100);
            entity.Property(e => e.JobBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SupervisorEmail).HasMaxLength(150);
            entity.Property(e => e.SupervisorFirstName).HasMaxLength(50);
            entity.Property(e => e.SupervisorLastName).HasMaxLength(50);
        });

        modelBuilder.Entity<VwSupervisorAndClientActiveJob>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwSupervisorAndClientActiveJobs");

            entity.Property(e => e.ClientEmailAddress).HasMaxLength(100);
            entity.Property(e => e.ClientFirstName).HasMaxLength(50);
            entity.Property(e => e.ClientFullName).HasMaxLength(100);
            entity.Property(e => e.ClientLastName).HasMaxLength(50);
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.JobBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SupervisorFirstName).HasMaxLength(50);
            entity.Property(e => e.SupervisorLastName).HasMaxLength(50);
            entity.Property(e => e.Threshold).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
