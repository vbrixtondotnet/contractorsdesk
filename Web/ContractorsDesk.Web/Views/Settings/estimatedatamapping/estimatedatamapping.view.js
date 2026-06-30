class TemplateView extends DomEventComponent {
	constructor() {
		super();
		this.service = new EstimateDataMappingService();
		this.estimateDataMappings = [];
		this.estimateCategories = [];
		this.swal = new SwalUtil();
	}

	#renderMappingRow(data) {
		const renderestimateCategorySelectField = (item) => {
			return `<a href="javascript:" class="virtual-select" data-account-id="${item.accountId}">${item.estimateCategory == '' ? '&nbsp;' : item.parentCategory + ' > ' + item.estimateCategory}<span class="arrow"></span></a>`;
		};

		return `<tr class="" style="text-transform:uppercase;padding:5px 0px 5px 0px" data-account-id="${data.accountId}">
                                <td class="fw-bolder ps-2" style="">${data.fullyQualifiedName}</td>
                                <td class="w-300px" style="width:300px !important;">
									${renderestimateCategorySelectField(data)}
								</td>
                            </tr>`;
	}

	async #loadEstimateDataMappings() {
		await this.service.loadEstimateDataMappings()
			.then((estimateDataMappings) => {
				this.estimateDataMappings = estimateDataMappings;
			});
	}

	async #loadEstimateCategories() {
		await this.service.loadEstimateCategories()
			.then((estimateCategories) => {
				this.estimateCategories = estimateCategories;
			});
	}

	#sortEstimateDataMapping(col, direction) {
		const estimateDataMappings = this.estimateDataMappings;

		const sortByField = (field) => {
			estimateDataMappings.sort((a, b) => {
				if (a[field] === b[field]) return 0;
				return direction === 'desc'
					? (a[field] < b[field] ? 1 : -1)
					: (a[field] > b[field] ? 1 : -1);
			});
		};

		switch (col) {
			case 'ACCOUNT NAME':
				sortByField('fullyQualifiedName');
				break;
			case 'ESTIMATE CATEGORY':
				sortByField('estimateCategory');
				break;
			case 'PARENT CATEGORY':
				sortByField('parentCategory');
				break;
		}

		this.#renderEstimateDataMappings();
	}

	onSort = (c) => {
		const sortingIndex = c.getAttribute('data-sort-index');
		const colName = $(c).text().trim().toUpperCase();
		const sortDir = $(c).hasClass('asc') ? 'desc' : 'asc';

		this.#sortEstimateDataMapping(colName, sortDir);

		$(`th.sortable[data-sort-index="${sortingIndex}"]`)
			.removeClass('asc desc')
			.addClass(sortDir);
	};

	#renderEstimateDataMappings() {
		$("#dv-datamappings").removeClass("loading").addClass("loaded");
		const rows = this.estimateDataMappings.map(item => { return this.#renderMappingRow(item); }).join('');
		$("#body-data-mappings").html(rows);
		
		const estimateCategories = [];
		this.estimateCategories.map(e => {
			if (e.parent != null) {
				estimateCategories.push({ id: e.id, text: `${e.parent.name} > ${e.name}` });
			}
		});

		$('.virtual-select').each((ind, obj) => {
			var virtualSelectOptions = {
				options: estimateCategories,
				element: $(obj),
				onselect: (item) => {
					let accountId = $(obj).attr("data-account-id");
					const estimateDataMapping = this.estimateDataMappings.find(e => e.accountId == accountId);
					const estimateData = this.estimateCategories.find(e => e.id == item.id);
					const estimateDataParent = this.estimateCategories.find(e => e.id == estimateData.parentEstimateCategoryId);
					
					estimateDataMapping.updated = estimateDataMapping.estimateCategoryId != null;
					estimateDataMapping.added = estimateDataMapping.estimateCategoryId == null;
					estimateDataMapping.estimateCategoryId = item.id;
					
					$(`tr[data-account-id='${accountId}']`).find('td').addClass('bg-light-warning');
					$(`tr[data-account-id='${accountId}']`).find('td.parent').html(estimateDataParent.name);
					//console.log(`CATEGORYID: ${datamappingId}, ESTIMATECATEGORYID: ${item.id}`);
				}
			};
			const virtualSelect = new VirtualSelect(virtualSelectOptions);
			virtualSelect.init();
		});
	}

	saveEstimateMappings(b) {

		b.setAttribute('data-kt-indicator', 'on');
		b.disabled = true;

		this.service.saveEstimateDataMappings(this.estimateDataMappings)
			.then((estimateDataMappings) => {
				this.estimateDataMappings = estimateDataMappings;
				this.#renderEstimateDataMappings();
			})
			.finally(() => {
				b.removeAttribute('data-kt-indicator');
				b.disabled = false;
			});
	}

	cancelChanges() {
		this.swal.confirm(`Are you sure to cancel all your changes?`,
			() => {
				location.reload();
			}
		);
	}

	clearFilter() {
		var input = $(`input[evt-input="filterRows"]`);
		input.val('');
		this.filterRows(input);
	}

	filterRows(i) {
		const filter = $(i).val().toLowerCase().trim(); // Get the search value
		$('tbody#body-data-mappings tr').each(function () {
			const rowText = $(this).text().toLowerCase(); // Get all text in the row
			if (rowText.includes(filter)) {
				$(this).show(); // Show rows that match the filter
			} else {
				$(this).hide(); // Hide rows that don't match
			}
		});
	}

	#buildStickyActions() {
		this.stickyActions = new StickyActions('min-w-lg-250px');
		this.stickyActions.actions = [
			new StickyActionsButton('Save', null, 'primary', null, false,
				(b) => {
					this.saveEstimateMappings(b);
				}
			),
			new StickyActionsButton('Cancel', null, 'light', null, false,
				(b) => {
					this.cancelChanges();
				}
			)
		];
		this.stickyActions.init();
	}

	#buildStickyHeader() {
		const options = {
			selector: ".sticky-header",
			top: '74px',
			showOnScrollTop: 200
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	init() {
		this.#loadEstimateCategories()
			.then(() => {
				this.#loadEstimateDataMappings()
					.then(() => {
						this.#renderEstimateDataMappings();
						this.#buildStickyActions();
						this.#buildStickyHeader();
					});
			});
		
    }
}

$(document).ready(() => {
	const view = new TemplateView();
    view.init();
});