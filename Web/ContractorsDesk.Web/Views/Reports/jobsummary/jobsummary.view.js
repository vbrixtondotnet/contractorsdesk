class JobSummaryReportView extends DomEventComponent {
	constructor() {
		super();
		const parts = document.URL.split('/');
		this.service = new JobSummaryService();
		this.swal = new SwalUtil();
		this.printer = new printer();
		this.activeJobs = [];
		this.pendingJobs = [];
	}

	loadActiveJobs() {
		this.service.loadActiveJobs()
			.then((activeJobs) => {
				this.activeJobs = activeJobs;
				this.renderActiveJobs();
			});
	}

	loadPendingJobs() {
		this.service.loadPendingJobs()
			.then((pendingJobs) => {
				this.pendingJobs = pendingJobs;
				this.renderPendingJobs();
			});
	}

	activeJobRow(activeJob) {
		return `<tr>
					<td class="ps-2 py-2 fw-bolder">${activeJob.jobAddress}</td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
				</tr>`
	}

	pendingJobRow(pendingJob) {
		return `<tr>
					<td class="ps-2 py-2">${pendingJob.jobAddress}</td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
					<td></td>
				</tr>`
	}

	renderActiveJobs() {
		const activeJobRows = this.activeJobs.map(activeJob => {
			return this.activeJobRow(activeJob);
		}).join('');

		$("#body-active-jobs").html(activeJobRows);

		$("#active-jobs-container")
			.removeClass("loading")
			.addClass("loaded");
	}

	renderPendingJobs() {
		const pendingJobRows = this.pendingJobs.map(pendingJob => {
			return this.activeJobRow(pendingJob);
		}).join('');

		$("#body-pending-jobs").html(pendingJobRows);

		$("#pending-jobs-container")
			.removeClass("loading")
			.addClass("loaded");
	}

	#buildStickyHeader() {
		const options = {
			selector: ".sticky-header-active",
			top: '74px',
			showOnScrollTop: 240
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	//#buildStickyActions() {
	//	this.stickyActions = new StickyActions('min-w-lg-200px');
	//	this.stickyActions.actions = [
	//		new StickyActionsButton('<i class="bi bi-printer-fill text-white"></i> Print', null, 'primary', null, false,
	//			(b) => {
	//				this.printer.printCurrentPage('/print-transaction-details-report/' + this.proposalId + '/' + this.itemName);
	//			}
	//		)
	//	];

	//	this.stickyActions.init();
	//}

	init() {
		this.#buildStickyHeader();
		this.loadActiveJobs();
		this.loadPendingJobs();
    }
}
$(document).ready(() => {
	const view = new JobSummaryReportView();
	view.init();
});