using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using ContractorsDesk.DataStore.Client.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace ContractorsDesk.DataStore.Client.Models;

public partial class ClientDbContext : DbContext
{
	public int UserIdSession { get; set; }
	public virtual DbSet<ActiveConstructionJobSpResult> ActiveConstructionJobsSpResult { get; set; }
	public virtual DbSet<RevisedEstimateSpResult> RevisedEstimateSpResult { get; set; }
	public virtual DbSet<ActiveSpecJobSpResult> ActiveSpecJobSpResult { get; set; }
	public virtual DbSet<TransactionDetailReportSpResult> TransactionDetailReportSpResult { get; set; }
	public virtual DbSet<CDMonthlyBudgetReportSpResult> MonthlyBudgetReportSpResult { get; set; }
	public virtual DbSet<ProjectScheduleSpResult> ProjectScheduleSpResult { get; set; }
	public virtual DbSet<TotalCostSpResult> TotalCostSpResult { get; set; }
	public virtual DbSet<ProjectEstimateCategoriesSpResult> ProjectEstimateCategoriesSpResult { get; set; }
	public virtual DbSet<ProjectScheduleItemsSpResult> ProjectScheduleItemsSpResult { get; set; }
	public virtual DbSet<GetUserInboxSpResult> GetUserInboxSpResult { get; set; }
	public virtual DbSet<ClassTransactionsReportSpResult> ClassTransactionsReportSpResult { get; set; }
	public virtual DbSet<TransactionsReportSpResult> TransactionsReportSpResult { get; set; }
	partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
		modelBuilder.Entity<ActiveConstructionJobSpResult>(entity =>
		{
			entity.HasNoKey();
		}); 
		modelBuilder.Entity<RevisedEstimateSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<ActiveSpecJobSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<TransactionDetailReportSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<CDMonthlyBudgetReportSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<ProjectScheduleSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<TotalCostSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<ProjectEstimateCategoriesSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<ProjectScheduleItemsSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<GetUserInboxSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<ClassTransactionsReportSpResult>(entity =>
		{
			entity.HasNoKey();
		});
		modelBuilder.Entity<TransactionsReportSpResult>(entity =>
		{
			entity.HasNoKey();
		});

		modelBuilder.Entity<Proposal>().HasQueryFilter(e => !e.IsDeleted);
		modelBuilder.Entity<SysFolder>().HasQueryFilter(s => !s.IsDeleted);
		modelBuilder.Entity<ActionItem>().HasQueryFilter(c => !c.IsDeleted);
		modelBuilder.Entity<Qbclass>().HasQueryFilter(c => !c.IsDeleted);
		// Add filters for other entities that support soft delete

	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
		{
			if (entry.Entity is IAuditableEntity entityWithCreatedByUserId)
			{
				entityWithCreatedByUserId.CreatedBy = UserIdSession;
				entityWithCreatedByUserId.DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
				entityWithCreatedByUserId.DateUpdated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			}
		}

		foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
		{
			if (entry.Entity is IAuditableEntity entityWithCreatedByUserId)
			{
				entityWithCreatedByUserId.UpdatedBy = UserIdSession;
				entityWithCreatedByUserId.DateUpdated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}
}

