
using AutoMapper;
using Azure.Storage.Blobs;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ContractorsDesk.WebPortal.Middlewares
{
	public static class DependencyInjectionMiddleware
    {
        public static void ConfigureDependencies(this WebApplicationBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isProductionEnvironment = environment?.ToUpper() == "PRODUCTION";

			builder.Services.AddScoped<MasterDbContext>();
			builder.Services.AddScoped<ClientDbContext>();
			builder.Services.AddSingleton(_ => new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]));

			builder.Services.AddScoped<IActionItemsService, ActionItemsService>();
			builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
			builder.Services.AddScoped<IPermissionsService, PermissionsService>();
			builder.Services.AddScoped<IProjectsService, ProjectsService>();
			builder.Services.AddScoped<IRolesService, RolesService>();
			builder.Services.AddScoped<IUserBookmarkService, UserBookmarkService>();
			builder.Services.AddScoped<IProposalService, ProposalService>();
			builder.Services.AddScoped<ICustomerService, CustomerService>();
			builder.Services.AddScoped<IEstimateService, EstimateService>();
			builder.Services.AddScoped<ITransactionService, TransactionService>();
			builder.Services.AddScoped<IScheduleService, ScheduleService>();
			builder.Services.AddScoped<IAudioUploadsService, AudioUploadsService>();
			builder.Services.AddScoped<IEstimateDataMappingService, EstimateDataMappingService>();
			builder.Services.AddScoped<IQbAccountService, QbAccountService>();
			builder.Services.AddScoped<IScheduleteDataMappingService, ScheduleDataMappingService>();
			builder.Services.AddScoped<IProposalTemplatesService, ProposalTemplatesService>();
			builder.Services.AddScoped<IReportsService, ReportsService>();
			builder.Services.AddScoped<IDocuSignService, DocuSignService>();
			builder.Services.AddScoped<ISignNowService, SignNowService>();
			builder.Services.AddScoped<IClientDocumentService, ClientDocumentService>();
			builder.Services.AddScoped<ISysFolderService, SysFolderService>();
			builder.Services.AddScoped<IConstructionTasksService, ConstructionTasksService>();
			builder.Services.AddScoped<INotificationService, NotificationService>();
			builder.Services.AddScoped<IActivityStreamService, ActivityStreamService>();
			builder.Services.AddScoped<IProjectJournalService, ProjectJournalService>();
			builder.Services.AddScoped<IQuickBooksService, QuickBooksService>();
			builder.Services.AddScoped<IWeeklyNoteActionItemService, WeeklyNoteActionItemService>();
			builder.Services.AddScoped<IQbClassService, QbClassService>();
			builder.Services.AddScoped<IImportDataService, ImportDataService>();
			builder.Services.AddScoped<ISubcontractorsService, SubcontractorsService>();
			builder.Services.AddScoped<IInvoiceService, InvoiceService>();
			builder.Services.AddScoped<ISysBackgroundJobsService, SysBackgroundJobsService>();
			builder.Services.AddScoped<IChangeOrderService, ChangeOrderService>();
            builder.Services.AddScoped<IVendorService, VendorService>();
			builder.Services.AddScoped<IRevisionsService, RevisionsService>();
			builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
			builder.Services.AddScoped<IContractService, ContractService>();
			builder.Services.AddScoped<IDashboardService, DashboardService>();
			builder.Services.AddScoped<IUserLogService, UserLogService>();
			builder.Services.AddScoped<IPostMarkEmailService, PostMarkEmailService>();
			builder.Services.AddScoped<ICompanySettingService, CompanySettingService>();
			builder.Services.AddScoped<IEmailsService, EmailsService>();
			builder.Services.AddScoped<ITenantService, TenantService>();
			builder.Services.AddScoped<TenantResolver>();
			builder.Services.AddScoped<ImportDataService>();
			builder.Services.AddScoped<IQuickBooksService, QuickBooksService>();
			builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

			//builder.Services.AddScoped<IPostMarkEmailService, IPostMarkEmailService>(provider =>
			//{
			//	var configuration = provider.GetRequiredService<IConfiguration>();
			//	var itenantService = provider.GetRequiredService<ITenantService>();
			//	var tenantResolver = provider.GetRequiredService<TenantResolver>();
			//	var azureStorageService = provider.GetRequiredService<IAzureStorageService>();
			//	var mapper = provider.GetRequiredService<IMapper>();
			//	var clientDbContext = provider.GetRequiredService<ClientDbContext>();
			//	var subDomain = tenantResolver.GetSubDomain();
			//	return new PostMarkEmailService(itenantService, clientDbContext, azureStorageService, mapper, configuration, subDomain);
			//});

			builder.Services.AddScoped<ITelegramService>(provider =>
			{
				var telegramSettings = provider.GetRequiredService<IOptions<TelegramSettingsModel>>().Value;
				var clientDbContext = provider.GetRequiredService<ClientDbContext>();
				
				telegramSettings.DatabaseName = clientDbContext.Database.GetDbConnection().Database;
				return new TelegramService(telegramSettings);
			});

			// Add Transient for services that do not inject DBContext
			builder.Services.AddTransient<IPDFService, PDFService>();

			//builder.Services.AddScoped(serviceProvider =>
			//{
			//	var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			//	var quickbooksService = serviceProvider.GetRequiredService<IQuickBooksService>();
			//	var iNotificationService = serviceProvider.GetRequiredService<ITelegramService>();
			//	var clientDbContext = serviceProvider.GetRequiredService<ClientDbContext>();

			//	return new JobSyncService(configuration, quickbooksService, itelegramService, clientDbContext);
			//});

		}
    }
}
