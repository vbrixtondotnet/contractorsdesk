using ContractorsDesk.Core.Utilities;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Services;
using Hangfire;
using Hangfire.SqlServer;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class HangfireMiddleware
	{
		public static void AddHangfire(this WebApplicationBuilder builder)
		{
			
			builder.Services.AddHangfire((serviceProvider, config) =>
			{
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();

				config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
						.UseSimpleAssemblyNameTypeSerializer()
						.UseRecommendedSerializerSettings()
						.UseSqlServerStorage(configuration.GetConnectionString("default"));

			});

			builder.Services.AddHangfireServer();
		}

		public static void SetupHangfireService(this WebApplication app, IServiceProvider serviceProvider)
		{
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var clientDbConnection = new SqlServerStorage(configuration.GetConnectionString("default"));

			app.UseHangfireDashboard("/data-sync", new DashboardOptions
			{
				Authorization = new[] { new DataSyncDashboardAuthorization() },
				DashboardTitle = "Data Sync Dashboard"
			}, clientDbConnection);

			app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				Authorization = new[] { new HangfireDashboardAuthorization() }
			});

            app.MapHangfireDashboard();
			app.DataSyncServiceSync();
			app.WeeklyJobs();
		}

		public static void DataSyncServiceSync(this WebApplication app)
		{
			var timeZone = TimezoneUtils.GetDefaultCaliforniaTimezoneInfo();
			var cronDataScedule = app.Configuration.GetSection("SystemSettings")["DataSync_CRON"];
			var environment = (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production").ToLower();
			var isProductionEnvironment = environment == "production";

			if (isProductionEnvironment)
			{
				var recurringJobOptions = new RecurringJobOptions
				{
					TimeZone = timeZone, 
				};

				RecurringJob.AddOrUpdate<IQuickBooksService>("QuickBooks Data Sync",
					syncService => syncService.RunDataSync(default),
					cronDataScedule,
					recurringJobOptions);
			}
			else
			{
				RecurringJob.AddOrUpdate<ProdToTestDbSyncUtilityService>(
				"Sync Production DB to Test DB",
				job => job.ExecuteSync(),
				Cron.Never);
			}
		}

		public static void WeeklyJobs(this WebApplication app)
		{
			var timeZone = TimezoneUtils.GetDefaultCaliforniaTimezoneInfo();
			var recurringJobOptions = new RecurringJobOptions
			{
				TimeZone = timeZone,
			};

			RecurringJob.AddOrUpdate<IWeeklyNoteActionItemService>(
				"Weekly Note Action Item",
				job => job.CreateWeeklyNoteActionItem(null),
				Cron.Weekly(DayOfWeek.Monday, 1, 0),
				recurringJobOptions);
		}
	}

}
