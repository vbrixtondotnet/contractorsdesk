using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("pdf")]
	public class PDFController : BaseController
	{
		private readonly IProposalService proposalService;
		private readonly IProjectsService projectsService;
		private readonly IEstimateService estimateService;
		private readonly ITransactionService transactionService;
		private readonly IScheduleService scheduleService;
		private readonly IActionItemsService actionItemsService;
		private readonly IInvoiceService invoiceService;
		private readonly IProjectJournalService projectJournalService;
		private readonly IPostMarkEmailService emailService;
		private readonly IRevisionsService revisionsService;
		private readonly IChangeOrderService changeOrderService;
		private readonly ICompanySettingService companySettingService;
		private readonly IReportsService reportsService;

		public PDFController(
			IProposalService proposalService, 
			IEstimateService estimateService, 
			ITransactionService transactionService, 
			IScheduleService scheduleService,
			IActionItemsService actionItemsService,
			IInvoiceService invoiceService,
			IProjectJournalService projectJournalService,
			IRevisionsService revisionsService,
			IChangeOrderService changeOrderService,
			IProjectsService projectsService,
			ICompanySettingService companySettingService,
			IPostMarkEmailService emailService,
			IReportsService reportsService,
			IServiceProvider provider) : base("PDF", provider)
		{
			this.proposalService = proposalService;
			this.estimateService = estimateService;
			this.transactionService = transactionService;
			this.scheduleService = scheduleService;
			this.actionItemsService = actionItemsService;
			this.invoiceService = invoiceService;
			this.projectJournalService = projectJournalService;
			this.emailService = emailService;
			this.projectsService = projectsService;
			this.revisionsService = revisionsService;
			this.changeOrderService = changeOrderService;
			this.companySettingService = companySettingService;
			this.reportsService = reportsService;
		}

		[HttpGet("proposal/{id}")]
		public async Task<IActionResult> Proposal(Guid id)
		{
            var includeZeroAmountFlag = await proposalService.CheckIfProposalIncludesZeroAmount(id);
            var proposal = await proposalService.GetProposalAsync(id, includeZeroAmountFlag);
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(proposal);
		}
		[HttpGet("revised-estimate/{id}")]
		public async Task<IActionResult> RevisedEstimate(Guid id)
		{
			var estimateToActual = await estimateService.GetEstimateToActualAsync(id);
			var proposal = await proposalService.GetProposalDetailsAsync(id);
			var vm = new PrintRevisedEstimateModel();
			vm.ProjectName = proposal.Project.Name;
			vm.Categories = estimateToActual.EstimateCategories;
			vm.Summary = estimateToActual.Summary;
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(vm);
		}
		[HttpGet("transaction-details")]
		public async Task<IActionResult> TransactionDetails(Guid proposalId, Guid? estimateCategoryId, Guid? parentEstimateCategoryId, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			var transactions = await transactionService.GetTransactionDetailsAsync(proposalId, estimateCategoryId, parentEstimateCategoryId, startDate, endDate);
			var proposalDetails = await proposalService.GetProposalDetailsAsync(proposalId);
			var projectDetails = await projectsService.GetProjectByProposalIdAsync(proposalId);

			var isOwnerDepositsReport = parentEstimateCategoryId == Guid.Parse("36fde9f1-dce7-4c5b-8fb8-4c23e1fe3f38");

			var totalCost = isOwnerDepositsReport ? await transactionService.GetTotalCost(projectDetails.Id) : 0;
			var transactionDate = startDate != null && endDate != null ? $"{startDate} - {endDate}" : null;
			var vm = new TransactionDetailsViewModel
			{
				TransactionDate = transactionDate,
				ItemName = string.Empty,
				TransactionDetails = transactions,
				ProjectName = projectDetails?.Name,
				IsOwnerDepositsReport = isOwnerDepositsReport,
				TotalCost = totalCost
			};

			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(vm);
		}

		[HttpGet("project-schedule/{projectId}")]
		public async Task<IActionResult> ProjectSchedule(Guid projectId)
		{
			var projectSchedule = await scheduleService.GetProjectScheduleAsync(projectId);
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(projectSchedule);
		}

		[HttpGet("change-order/{actionItemId}")]
		public async Task<IActionResult> ChangeOrder(int actionItemId)
		{
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();

			var actionItem = await actionItemsService.GetActionItemSummaryViewByIdAsync(actionItemId);
			var projectDetails = await projectsService.GetProjectShortDetailsAsync(actionItem.ProjectId.Value);
			var changeOrder = await changeOrderService.GetChangeOrderByActionItemIdAsync(actionItemId);

			var changeOrderType = actionItem.ActionTypeId switch
			{
				1 => "Cost Change Order",
				2 => "Schedule Change Order",
				_ => "General Change Order"
			};

			var vm = new ChangeOrderFormViewModel
			{
				ChangeOrderType = changeOrderType,
				Title = actionItem.Title,
				Description = actionItem.Description,
				EstimateCategory = actionItem.CostChangeItem,
				Amount = changeOrder.NewAmount,
				CurrentAmount = changeOrder.CurrentAmount,
				NewAmount = changeOrder.NewAmount,
				ScheduleItemName = actionItem.ScheduleChangeItem,
				NoOfDays = actionItem.NoOfDays,
				ClientName = projectDetails.ClientName,
				ContractorName = this.CompanySettings.GeneralContractorName,
				ProjectName = projectDetails.Name,
				ChangeOrderNumber = changeOrder.ChangeOrderNumber.ToString().PadLeft(5,'0'),
			};

			return View(vm);
		}

		[HttpGet("schedule-revision/{id}")]
		public async Task<IActionResult> ScheduleRevision(Guid id)
		{
			var scheduleRevisionDto = await scheduleService.GetScheduleRevision(id);
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(scheduleRevisionDto);
		}

		[HttpGet("cost-revision/{id}")]
		public async Task<IActionResult> CostRevision(Guid id)
		{
			var costRevisionDto = await revisionsService.GetCostRevisionAsync(id);
			var projectDetails = await projectsService.GetProjectShortDetailsAsync(costRevisionDto.ProjectId);
			var vm = new CostRevisionViewModel
			{
				ClientName = projectDetails.ClientName,
				CostRevisionDto = costRevisionDto,
				ProjectAddress = projectDetails.Address, ProjectName = projectDetails.Name, StartDate = projectDetails.StartDate
			};

			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(vm);
		}

		[HttpGet("invoice/{id}")]
		public async Task<IActionResult> Invoice(Guid id)
		{
			var invoiceDto = await invoiceService.GetInvoiceAsync(id);
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(invoiceDto);
		}

		[HttpGet("status-report/{id}")]
		public async Task<IActionResult> StatusReport(Guid id)
		{
			var projectJournalDto = await projectJournalService.GetByIdAsync(id);
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(projectJournalDto);
		}


		[HttpGet("client-email/{id}")]
		public async Task<IActionResult> ClientEmail(Guid id)
		{
			var emailSentDto = await emailService.GetSentEmailByIdAsync(id);
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(emailSentDto);
		}

        [HttpGet("change-order/{id}/project")]
        public async Task<IActionResult> ChangeOrderProject(Guid id)
        {
            var estimateToActual = await estimateService.GetEstimateToActualAsync(id);
            var proposal = await proposalService.GetProposalDetailsAsync(id);
            var vm = new PrintRevisedEstimateModel();
            vm.ProjectName = proposal.Project.Name;
            vm.Categories = estimateToActual.EstimateCategories;
            vm.Summary = estimateToActual.Summary;
			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(vm);
        }

		[HttpGet("active-construction-jobs")]
		public async Task<IActionResult> ActiveConstructionJobs(DateOnly start, DateOnly end)
		{
			var activeConstructionJobs = await reportsService.GetActiveConstructionJobReportAsync(start, end, "All");
			
			var vm = new ActiveConstructionJobsReportViewModel
			{
				StartDate = start,
				EndDate = end,
				ActiveConstructionJobs = activeConstructionJobs
			};

			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(vm);
		}

		[HttpGet("class-transactions")]
		public async Task<IActionResult> ClassTransactions(DateOnly start, DateOnly end, string className, string filter)
		{
			var classTransactions = await reportsService.GetClassTransactionsReportAsync(start, end, className, filter);

			var vm = new ClassTransactionsReportViewModel
			{
				StartDate = start,
				EndDate = end,
				ClassTransactions = classTransactions
			};

			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(vm);
		}

		[HttpGet("transactions")]
		public async Task<IActionResult> Transactions(DateOnly start, DateOnly end, string className, string filter)
		{
			var transactions = await reportsService.GetTransactionsReportAsync(start, end, className, filter);

			var vm = new TransactionsReportViewModel
			{
				StartDate = start,
				EndDate = end,
				Transactions = transactions
			};

			this.CompanySettings = await companySettingService.GetCompanySettingAsync();
			InitializeCompanySettings();
			return View(vm);
		}


	}
}
