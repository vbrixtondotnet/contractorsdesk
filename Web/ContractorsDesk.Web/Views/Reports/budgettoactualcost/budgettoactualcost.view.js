class BudgetToActualCostReportView extends DomEventComponent {
	constructor() {
		super();
		this.service = new BudgetToActualCostReportService();
		this.printer = new printer();
		this.tableWrapper = document.getElementById('report-table-wrapper');
		this.listOfProjects = [];
	}

	async init() {
		await this.#getProjects();
		this.#loadProjectsToSelect();
		this.#initEventHandlers();
	}

	async #getProjects() {
		const response = await this.httpService.get('/api/dashboard/projects');
		this.listOfProjects = response;
	}

	#loadProjectsToSelect() {
		const $select = $('#slcActiveJobs');

		this.listOfProjects.forEach(project => {
			$select.append($('<option>', {
				value: project.proposalId,
				text: project.name
			}));
		});
	}

	#initEventHandlers() {
		const instance = this;

		$('#slcActiveJobs').select2();
		
		$('#slcActiveJobs').on('change', function (e) {
			e.preventDefault();
			const selectedValue = $(e.currentTarget).val();

			const result = instance.listOfProjects.find(
				project => project.proposalId.toString() === selectedValue
			);

			if (result) {
				instance.#renderReport(result.proposalId, e.currentTarget);
			}
		});

		$('#btnPrint').on('click', function (e) {
			e.preventDefault();
			const selectedValue = $('#slcActiveJobs').val();

			const result = instance.listOfProjects.find(
				project => project.proposalId.toString() === selectedValue
			);

			if (result) {
				instance.#printReport(result.proposalId, e.currentTarget);
			}
			else {
				alert("Please select a project!");
			}
		});
	}

	async #printReport(proposalId, elmnt) {
		elmnt.setAttribute('data-kt-indicator', 'on');
		elmnt.disabled = true;

		try {
			const response = await fetch(`/api/print/revised-estimate/${proposalId}`);
			if (!response.ok) {
				throw new Error('Failed to fetch the PDF.');
			}

			printFile(response);

		} catch (error) {
			console.error("Error:", error);
			alert("Failed to load the PDF for printing.");
		}
		finally {
			elmnt.removeAttribute('data-kt-indicator');
			elmnt.disabled = false;
		}
	}

	async #renderReport(proposalId, emlnt) {
		$('.report-container').addClass('loading').removeClass('loaded');
		this.tableWrapper.innerHTML = '';
		emlnt.disabled = true;

		try {
			const url = `${location.origin}/pdf/revised-estimate/${proposalId}`;
			const modalBodyViewerHeight = $('body').height() - 200;
			const iframe = document.createElement('iframe');
			iframe.src = url;
			iframe.style.width = '100%';
			iframe.style.height = `${modalBodyViewerHeight}px`;
			iframe.style.border = 'none';
			
			this.tableWrapper.appendChild(iframe);

			iframe.onload = () => {
				$('.report-container').removeClass('loading').addClass('loaded');
				emlnt.disabled = false;
			};

		} catch (error) {
			console.error("Error loading report:", error);
			this.tableWrapper.innerHTML = `<div class="text-danger">Failed to load report.</div>`;
		}
	}
}

$(document).ready(() => {
	const view = new BudgetToActualCostReportView();
	view.init();
});