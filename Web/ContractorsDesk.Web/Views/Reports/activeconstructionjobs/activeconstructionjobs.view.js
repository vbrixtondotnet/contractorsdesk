class ActiveConstructionJobsView extends DomEventComponent {
	constructor() {
		super();
		const parts = document.URL.split('/');
		this.swal = new SwalUtil();
		this.printer = new printer();
		this.activeJobs = [];
		this.pendingJobs = [];
		this.onFilterReportTimeout = null;
		this.start = null;
		this.end = null;
	}

	async print(button) {
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;


		try {
			// Fetch the PDF from the API
			const response = await fetch(`/api/print/active-construction-jobs?start=${this.start}&end=${this.end}`);
			if (!response.ok) {
				throw new Error('Failed to fetch the PDF.');
			}
			//downloadFile(response);
			printFile(response);
		} catch (error) {
			console.error("Error:", error);
			alert("Failed to load the PDF for printing.");
		}
		finally {
			button.removeAttribute('data-kt-indicator');
			button.disabled = false;
		}
		//this.printer.printCurrentPage('/print-project-schedule/' + this.projectId);
	}

	onFilterReport(d) {

		clearTimeout(this.onFilterReportTimeout);
		this.onFilterReportTimeout = setTimeout(() => {
			clearTimeout(this.onFilterReportTimeout);

			$("#btnPrint").prop("disabled", true);
			var start = $("#startDate").val();
			var end = $("#endDate").val();

			if (start != '' && end != '') {

				this.start = start;
				this.end = end;

				$("#active-jobs-container")
					.removeClass("loaded")
					.addClass("loading");

				this.httpService.get(`/api/reports/active-construction-jobs?start=${start}&end=${end}`)
					.then(response => {
						this.activeJobs = response;
						this.renderActiveJobs();

						if (this.activeJobs.length > 3) {
							$("#btnPrint").prop("disabled", false);
						}
					});
			}
		}, 500);
	}

	#formatBalance(value) {
		const number = parseFloat(value);

		if (isNaN(number)) {
			return 'Invalid number';
		}

		const absValue = Math.abs(number);

		// Format with commas and two decimals
		const formatted = absValue.toLocaleString('en-US', {
			style: 'currency',
			currency: 'USD',
			minimumFractionDigits: 2,
			maximumFractionDigits: 2
		});

		// Wrap negatives in parentheses
		return number < 0 ? `(${formatted})` : formatted;

	}

	activeJobRow(a, index) {
		const nonItemRows = ['Total Job Balance:', 'QB CHA 9100 Acct Balance:', 'Difference:'];
		const style = a.jobBalance < 0 ? 'color:red;' : '';
		const itemNo = nonItemRows.indexOf(a.activeConstructionJobs) != -1 ? '' : (index + 1);
		const itemNameStyle = nonItemRows.indexOf(a.activeConstructionJobs) != -1 ? 'text-right fw-bolder' : 'text-left';
		return `<tr>
					<td class="ps-2 py-2 fw-bolder text-center">${itemNo}</td>
					<td class="${itemNameStyle}">${a.activeConstructionJobs}</td>
					<td class="text-right pe-5" style="${style}">${this.#formatBalance(a.jobBalance)}</td>
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
		const activeJobRows = this.activeJobs.map((activeJob, index) => {
			return this.activeJobRow(activeJob, index);
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

	#initControls() {
		const defStartDate = new Date('2011-10-24');
		//twoMonthsAgo.setMonth(twoMonthsAgo.getMonth() - 2);

		$("#startDate").flatpickr({
			dateFormat: "Y-m-d",
			defaultDate: defStartDate
		});

		$("#endDate").flatpickr({
			dateFormat: "Y-m-d",
			defaultDate: new Date()
		});
	}

	init() {
		this.#initControls();
		this.#buildStickyHeader();
		this.onFilterReport(null);
    }
}
$(document).ready(() => {
	const view = new ActiveConstructionJobsView();
	view.init();
});