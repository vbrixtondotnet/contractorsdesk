class ChangeOrderProjectReportView extends DomEventComponent {
	constructor() {
		super();
		this.proposalId = null;
		this.httpService = new httpService();
		this.service = new ChangeOrderService();
		this.projectDetails = null;
		this.swal = new SwalUtil();
		this.categories = [];
		this.summary = null;
		this.printer = new printer();
		this.reTableRowRenderer = null;
		this.formatter = new Formatter(this.proposalId);
		this.companySettings = Auth.getCompanySettings();
		this.selectedProjectId = null;
		this.projects = [];
	}


	#calculateRowTotals(item) {
		const estimateAmount = item.revised;//item.revised > 0 ? item.revised : item.original;
		//if (estimateAmount == 0) {
		//	$(`tr.${item.id} td.item-percentage`).html(`N/A`);
		//	return;
		//}
		var percentage = this.#getPercentage(estimateAmount, item.costToDate);
		item.balance = parseFloat(estimateAmount - item.costToDate);
		item.percentage = percentage ?? item.percentage;

		const percentageText = percentage == null ? 'N/A' : `${item.percentage}%`;

		$(`tr.${item.id} td.item-balance`).html(`${this.formatter.formatBalance(item.balance)}`);
		$(`tr.${item.id} td.item-percentage`).html(`${percentageText}`);

		this.#calculateCategoryTotals(item);
	}

	#calculateOriginalColumnTotals() {
		let totalProject = 0;
		this.categories.map(c => {
			const lineItems = c.lineItems;
			let totalOriginal = 0;
			lineItems.map(item => {
				totalOriginal += item.original;
			});

			$(`tr.${c.id} td.total-original`).html(`${StringUtils.formatMoney(totalOriginal)}`);
			totalProject += totalOriginal;
		});

		$(`tr.project-totals td.total-original`).html(`${this.formatter.formatBalance(totalProject)}`);
		//$(`tr.${item.id} td.item-percentage`).html(`${item.percentage.toFixed(2)}%`);
	}

	#calculateCategoryTotals(item) {
		const category = this.categories.find(c => c.id === item.parentId);
		if (!category) return;

		const { totalRevised, totalBalance } = category.lineItems.reduce(
			(totals, i) => {
				totals.totalRevised += i.revised;
				totals.totalBalance += i.balance;
				return totals;
			},
			{ totalRevised: 0, totalBalance: 0 }
		);

		let totalPercent = category.lineItems.reduce((sum, item) => sum + item.percentage, 0);
		totalPercent = totalPercent / category.lineItems.length;
		totalPercent = Math.round(totalPercent * 100) / 100;

		const formatPercent = (value) => {
			return value < 0 ? `<span class="text-danger">${value.toFixed(2)}%</span>` : `${value}%`;
		}

		category.totalBalance = totalBalance;
		category.totalRevised = totalRevised;
		category.totalPercentage = Math.round(totalPercent);

		$(`tr.${item.parentId} td.total-revised`).html(`${StringUtils.formatMoney(totalRevised)}`);
		$(`tr.${item.parentId} td.total-balance`).html(`${this.formatter.formatBalance(totalBalance)}`);
		$(`tr.${item.parentId} td.total-balance`).html(`${this.formatter.formatBalance(totalBalance)}`);
		$(`tr.${item.parentId} td.total-percent`).html(`${formatPercent(category.totalPercentage)}`);
		this.#calculateProjectTotals();
	}

	#calculateProjectTotals() {
		let totalOriginal = 0;
		let totalRevised = 0;
		let totalCostToDate = 0;

		//this.categories.map(c => {
		//	totalOriginal += c.totalOriginal;
		//	totalRevised += c.totalRevised;
		//});

		const categories = this.categories;
		categories.map(c => {
			totalOriginal += c.totalOriginal;
			totalRevised += c.totalRevised;
			if (c.name != "JOB BALANCE" && c.name != "OWNER DEPOSITS" && c.name != "TOTAL COST TO DATE") {
				totalCostToDate += c.totalCostToDate == null ? 0 : c.totalCostToDate;
			}
		});

		const formatPercent = (value) => {
			return value < 0 ? `<span class="text-danger">${value.toFixed(2)}%</span>` : `${value}%`;
		}

		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';

		if (totalRevised == 0) {
			$(".project-totals td.total-percent").html(`N/A`).attr('style', blackBorderStyle);
		}
		else {
			let totalPercentage = parseFloat(((totalCostToDate / totalRevised) * 100));
			totalPercentage = Math.round(totalPercentage);
			$(".project-totals td.total-percent").html(`${formatPercent(totalPercentage)}`).attr('style', blackBorderStyle);
		}

		$(".project-totals td.total-original").html(`${StringUtils.formatMoney(totalOriginal)}`).attr('style', blackBorderStyle);
		$(".project-totals td.total-adjustment").html(`${StringUtils.formatMoney(totalRevised - totalOriginal)}`).attr('style', blackBorderStyle);
		$(".project-totals td.total-revised").html(`${StringUtils.formatMoney(totalRevised)}`).attr('style', blackBorderStyle);
		$(".project-totals td.total-balance").html(`${this.formatter.formatBalance(totalRevised - totalCostToDate)}`).attr('style', blackBorderStyle);
		$(".project-totals td.reconcile-cell").attr('style', blackBorderStyle);
	}

	#getPercentage(estimateAmount, costToDate) {
		if (estimateAmount == costToDate) {
			return 100;
		}
		else if (estimateAmount == 0) {
			return null;
		}
		else {
			let percentage = parseFloat(((costToDate / estimateAmount) * 100));
			return Math.round(percentage);
		}
	}

	#renderProposalDetails() {
		const project = this.projectDetails;

		if (project != null) {
			$("#lblProjectName").html(project.name);
			$("#lblStartDate").html(project.startDate);
			$("#lblEstimatedCompletionDate").html(project.endDate);
		}

		$(".proposal-details").removeClass('loading').addClass('loaded');
	}

	#renderRevisedEstimateRows() {
		$(".tooltip").remove();
		$("#dv-proposal-categories").removeClass('loading').addClass('loaded');

		//this.reTableRowRenderer = new RevisedEstimateTableRowRenderer(this.categories, this.summary, this.proposalId, this.proposal.locked);
		this.reTableRowRenderer = new RevisedEstimateTableRowRenderer(this.categories, this.summary, this.proposalId, this.projectDetails.locked);
		const estimateRows = this.reTableRowRenderer.generateRows();
		$("#body-categories").html(estimateRows);

		this.categories.map(c => {
			const lineItems = c.lineItems;
			lineItems.map(item => {
				this.#calculateRowTotals(item);
			})
		});

		this.#calculateProjectTotals();
		TextboxUtils.init();
		KTApp.initBootstrapTooltips();

	}

	loadRevisedEstimate(ids) {
		this.service.loadRevisedEstimate(ids)
			.then((data) => {
				this.categories = data.estimateCategories;
				this.summary = data.summary;
				this.projectDetails = data.projectDetails;
			})
			.then(() => {

				this.#renderProposalDetails();
				this.#renderRevisedEstimateRows();
			});
	}

	async print(button) {
		if (this.proposalId == null) {
			return this.swal.error("Please select a project");
		}
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;
		try {
			// Fetch the PDF from the API
			const response = await fetch('/api/print/change-order/' + this.proposalId + '/project');
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
	}

	#buildStickyActions() {
		this.stickyActions = new StickyActions('min-w-lg-150px');
		this.stickyActions.actions = [
			new StickyActionsButton('Print', null, 'secondary', null, true,
				(b) => {
					this.print(b);
				}
			)
		];

		this.stickyActions.init();
	}

	#buildStickyHeader() {
		const options = {
			selector: ".sticky-header",
			top: '74px',
			showOnScrollTop: 336
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	#initProjectSearch() {
		const instance = this;
		$("#slc-cost-revision-project").select2({
			minimumInputLength: 2,
			dropdownPosition: 'below',
			ajax: {
				url: '/api/projects/search',
				data: params => ({ search: params.term }),
				processResults: response => {
					return {
						results: response.data.map(item => ({
							id: item.id,
							text: `${item.name}` // fallback for selection
						}))
					};
				}
			},
			templateResult: function (item) {
				if (!item.id) return item.text; // for placeholder
				return $(`<div>
                            <div><strong>${item.text}</strong></div>
                        </div>`);
			},
			templateSelection: function (item) {
				return item.text || '';
			}
		});

		$("#slc-cost-revision-project").on('select2:select', function (e) {
			instance.proposalId = e.params.data.id;
			$("#dv-proposal-categories").removeClass('loaded').addClass('loading');

			if ($("#dv-proposal-categories").hasClass("d-none")) {
				$("#dv-proposal-categories").removeClass("d-none");
			}
			instance.loadRevisedEstimate(instance.proposalId);
		});
	}

	#setProjectDropdown(projects) {
		if (projects.length > 0) {
			$("#slc-cost-revision-project").empty();
			$("#slc-cost-revision-project").append(`<option value="" disabled selected>Select Project</option>`);
			projects.forEach(p => {
				$("#slc-cost-revision-project").append(new Option(p.name, p.proposalId));
			});

			$("#slc-cost-revision-project").select2({ placeholder: 'Select Project' });

			this.#fixSelect2Width();
		}
	}

	#fixSelect2Width() {
		$('#slc-cost-revision-project').select2({ width: 'auto' });
	}

	#adjustDropdownWidth(dropdownId) {
		const select = document.getElementById(dropdownId);
		if (!select) return;

		// Create a temporary span for measuring text width
		const tempSpan = document.createElement("span");
		tempSpan.style.visibility = "hidden";
		tempSpan.style.whiteSpace = "nowrap";
		tempSpan.style.font = window.getComputedStyle(select).font;
		document.body.appendChild(tempSpan);

		let maxWidth = 0;

		// Measure each option's width
		for (let i = 0; i < select.options.length; i++) {
			tempSpan.textContent = select.options[i].text;
			maxWidth = Math.max(maxWidth, tempSpan.offsetWidth);
		}

		document.body.removeChild(tempSpan);

		// Add some padding for dropdown arrow
		select.style.width = (maxWidth + 40) + "px";
	}


	loadProjects() {
		this.httpService.get('/api/projects')
			.then(response => {
				this.#setProjectDropdown(response);
			}).then(response => {
				this.#renderProposalDetails()
			});
	}

	init() {
		this.#initProjectSearch();
		this.loadProjects();
		this.#buildStickyActions();
		this.#buildStickyHeader();
	}
}
class RevisedEstimateTableRowRenderer {
	constructor(categories, summary, proposalId, locked) {
		this.categories = categories;
		this.summary = summary;
		this.locked = locked;
		this.textboxStyle = 'height: 30px !IMPORTANT;min-height: 30px !important;';
		this.nonCategoryItemNames = ['OWNER DEPOSITS', 'JOB BALANCE', 'TOTAL COST TO DATE'];
		this.formatter = new Formatter(proposalId);
	}

	#createCells(columns, index = 0, len = 0) {
		//border-top: 2px solid black !important;border-bottom: 5px double black !important
		if (index == len - 1) {
			return columns.map(col => `<td${col.class ? ` class="${col.class}"` : ''} style="border-bottom: 2px solid black !important;">${col.content}</td>`).join('');
		}
		else {
			return columns.map(col => `<td${col.class ? ` class="${col.class}"` : ''} style="${col.style ?? ''}">${col.content}</td>`).join('');
		}
	}

	#createRow(columns, classes = '', styles = '', index = 0, len = 0) {
		return `<tr class="${classes}" style="${styles}">
				${this.#createCells(columns, index, len)}
			</tr>`;
	}

	#categoryRow(c) {
		return this.#createRow(
			[{ content: c.name, class: 'fw-bolder py-2 ps-2' }, ...Array(5).fill({ content: '&nbsp;' })],
			'',
			'text-transform:uppercase;padding:5px 0px 5px 0px'
		);
	}

	#totalCategoryRow(c) {
		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';
		const totalColumns = [
			{ content: `Total ${c.name}`, class: 'fw-bolder py-2 ps-2', style: `text-transform:uppercase;${blackBorderStyle}` },
			{ content: `${this.formatter.formatBalance(c.totalOriginal)}`, class: 'text-center fw-bold total-original py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatBalance(c.totalRevised - c.totalOriginal)}`, class: 'text-center fw-bold total-original py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatBalance(c.totalRevised)}`, class: 'text-center fw-bold total-revised py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatCostToDate(null, c.id, c.totalCostToDate)}`, class: 'text-center fw-bold py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatBalance(c.totalBalance)}`, class: 'text-center fw-bold total-balance py-2', style: `${blackBorderStyle}` },
		];

		const emptyRow = Array(5).fill({ content: '&nbsp;', class: 'text-center' });
		return this.#createRow(totalColumns, `${c.id} total-category-row`);
	}

	#itemRow(c, index, len) {
		const revisedAmount = (item) => {
			return `<span>${this.formatter.formatBalance(item.revised)}</span>`;
		}
		const originalAmountCell = (c) => {
			return this.formatter.formatBalance(c.original);
		}

		const totalAdjustmentAmountCell = (c) => {
			return `<span>${this.formatter.formatBalance(c.revised - c.original)}</span>`;
		}

		const formatName = (name) => {
			if (name.toUpperCase() == 'UNMAPPED TRANSACTIONS') {
				return '<span class="text-danger">Unmapped Transactions</span>';
			}
			else {
				return name;
			}
		}

		return this.#createRow(
			[
				{ content: formatName(c.name), style: '', class: "ps-10" },
				{ content: originalAmountCell(c), class: 'text-center' },
				{ content: totalAdjustmentAmountCell(c), class: 'text-center' },
				{ content: revisedAmount(c), class: 'text-center revised-amount' },
				{ content: this.formatter.formatCostToDate(c.estimateCategoryId, null, c.costToDate), class: 'text-center' },
				{ content: this.formatter.formatBalance(c.balance), class: 'text-center item-balance' },
			],
			`${c.id}`,
			'',
			index,
			len
		);
	}

	#nonCategoryRow(name, cls = '') {
		const costToDate = (name) => {
			switch (name) {
				case 'OWNER DEPOSITS':
					return this.formatter.formatCostToDate(null, '36fde9f1-dce7-4c5b-8fb8-4c23e1fe3f38', this.summary.ownerDeposits, name);
				case 'JOB BALANCE':
					return this.formatter.formatBalance(this.summary.jobBalance);
			}
		}

		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';
		return `<tr class="${cls}" style="">
					   <td class="fw-bolder py-2 ps-2" style="text-transform:uppercase;${blackBorderStyle}">${name}</td>
					   <td class="text-center fw-bold py-3 total-original"></td>
					   <td class="text-center fw-bold py-3 total-adjustment"></td>
					   <td class="text-center fw-bold total-revised py-3" style=""></td>
					   <td class="text-center fw-bold py-3 total-costtodate" style="${blackBorderStyle}">${costToDate(name)}</td>
					</tr>`
	}

	#projectTotalRow(name, cls = '') {

		const totalCostToDate = () => {
			const costToDate = this.summary.totalCostToDate;

			return this.formatter.formatCostToDate(null, '0934406a-64db-432b-a01c-5d7573a0f3ce', costToDate, name);
		}

		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';
		return `<tr class="${cls}" style="">
					   <td class="fw-bolder py-2 ps-2" style="text-transform:uppercase;${blackBorderStyle}">${name}</td>
					   <td class="text-center fw-bold py-3 total-original"></td>
					    <td class="text-center fw-bold py-3 total-adjustment"></td>
					   <td class="text-center fw-bold total-revised py-3" style=""></td>
					   <td class="text-center fw-bold py-3 total-costtodate" style="${blackBorderStyle}">${totalCostToDate()}</td>
					   <td class="text-center fw-bold total-balance py-3" style=""></td>
					</tr>`
	}

	generateRows() {
		const categories = this.categories.filter(item => !this.nonCategoryItemNames.includes(item.name));

		const categoryRows = categories.map(c => {
			let rowHtml = this.#categoryRow(c);
			rowHtml += c.lineItems.map((i, index) => this.#itemRow(i, index, c.lineItems.length)).join('');
			rowHtml += this.#totalCategoryRow(c);
			return rowHtml;
		}).join('');

		return `
			${categoryRows}
			${this.#projectTotalRow('PROJECT TOTALS', 'project-totals')}
			${this.#nonCategoryRow(this.nonCategoryItemNames[0])}
			${this.#nonCategoryRow(this.nonCategoryItemNames[1])}
		`
	}
}
class Formatter {
	constructor(proposalId) {
		this.proposalId = proposalId;
	}

	formatDecimal(value) {
		return value ? value.toFixed(2) : '';
	}

	formatPercent(value) {
		let val = value == null ? 0 : value;
		return value < 0 ? `<span class="text-danger">${val}%</span>` : `${val}%`;
	}

	formatBalance(value) {
		const formattedValue = StringUtils.formatMoney(value);
		return value < 0 ? `<span class="text-danger">(${formattedValue.replace('-', '')})</span>` : `${formattedValue}`;
	}

	formatCostToDate(estimateCategoryId, parentEstimateCategoryId, value, name = '') {
		const formattedValue = StringUtils.formatMoney(value);
		const fontColor = value < 0 ? 'text-danger' : '';

		//Unmapped Transactions
		if (estimateCategoryId && estimateCategoryId.toUpperCase() == "821DBDD6-A6FE-4ED3-8F98-2AA63DABFEBA") {
			return `<a href="javascript:" evt-click="toggleMappingDrawer">${formattedValue}</a>`;
		}

		return value != 0 ? `<span class="${fontColor}">${formattedValue}</span>` : `${formattedValue}`;

	}
}

$(document).ready(() => {
	const view = new ChangeOrderProjectReportView();
	view.init();
});