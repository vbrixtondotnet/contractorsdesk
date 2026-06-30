class TransactionsView extends DomEventComponent {
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
		this.selectedClass = [];
		this.selectedClassesStr = [];
	}

	async print(button) {
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;


		try {
			// Fetch the PDF from the API
			const selectedClass = `${this.selectedClass} |Class`;
			const response = await fetch(`/api/print/transactions?start=${this.start}&end=${this.end}&className=${selectedClass}&filter=All`);
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
			const start = $("#startDate").val();
			const end = $("#endDate").val();
			const selectedClass = this.selectedClass;

			if (start != '' && end != '' && selectedClass != '') {

				this.start = start;
				this.end = end;
				const cls = `${selectedClass} |Class`;

				$("#active-jobs-container")
					.removeClass("loaded")
					.addClass("loading");

				this.httpService.get(`/api/reports/transactions?start=${start}&end=${end}&className=${cls}&filter=All`)
					.then(response => {
						this.activeJobs = response;
						this.renderActiveJobs();

						if (this.activeJobs.length > 0) {
							$("#btnPrint").prop("disabled", false);
						}
					});
			}
		}, 500);
	}

	#initClassesDropdown() {
		const instance = this;
		this.assignedToDropdown = $("#txtClasses").select2({
			minimumInputLength: 0,
			dropdownPosition: 'below',
			ajax: {
				url: '/api/dashboard/projects',
				delay: 500,
				data: params => ({ search: params.term }),
				processResults: response => {
					return {
						results: response.data.map(item => ({
							id: item.id,
							name: item.name // fallback for selection
						}))
					};
				}
			},
			templateResult: function (item) {
				if (!item.id) return item.name; // for placeholder
				return $(`<div>
                            <div><span>${item.name}</span></div>
                        </div>`);
			},
			templateSelection: function (item) {
				return item.name || '';
			}
		});

		$("#txtClasses").on('select2:select', function (e) {
			instance.selectedClass = e.params.data.name;
			instance.onFilterReport(null);
		});
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
		return `<tr>
					<td class="ps-2">${a.date}</td>
					<td>${a.type}</td>
					<td>${a.num}</td>
					<td>${a.clearStatus}</td>
					<td>${this.#formatBalance(a.amount)}</td>
					<td>${a.class}</td>
					<td>${a.division}</td>
					<td>${a.category}</td>
					<td>${a.memo}</td>
					<td>${a.customerPayee}</td>
					<td>${this.#formatBalance(a.balance)}</td>
				</tr>`;
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

	#initControls() {
		const twoMonthsAgo = new Date();
		twoMonthsAgo.setMonth(twoMonthsAgo.getMonth() - 2);

		$("#startDate").flatpickr({
			dateFormat: "Y-m-d",
			defaultDate: twoMonthsAgo
		});

		$("#endDate").flatpickr({
			dateFormat: "Y-m-d"
		});

		this.#initClassesDropdown();
	}

	init() {
		this.#initControls();
		//this.#buildStickyHeader();
    }
}
$(document).ready(() => {
	const view = new TransactionsView();
	view.init();
});