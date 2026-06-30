class ScheduleDataMappingView extends DomEventComponent {
	constructor() {
		super();
		this.service = new ScheduleDataMappingService();
		this.scheduleDataMappings = [];
		this.estimateCategories = [];
		this.tasks = [];
		this.swal = new SwalUtil();
		this.TasksDropdown = null;
		this.selectedSupervisors = [];
		this.dropdownInstances = [];
	}

	#renderMappingRow(data) {
		const dropdownInstances = this.dropdownInstances || {};
		const renderTaskController = (data) =>
		{
			const tasks = this.tasks;
			const sd = new SearchableDropdown2();
			sd.data = tasks;
			sd.optionText = (o) => { return `${o.name}` }
			sd.optionSelected = (o) => { return data?.tasks?.some(task => task.constructionTaskId === o.id); }

			sd.onSelect = (o) => {
				const estimateCategory = this.scheduleDataMappings.find(sdm => sdm.id === data.id);

				if (estimateCategory) {
					const existingTask = estimateCategory.tasks?.find(task => task.constructionTaskId === o.id);
					if (!existingTask) {
						estimateCategory.tasks.push({
							constructionTaskId: o.id,
							estimateCategoryId: estimateCategory.id,
							added: true,
							updated: false,
						});
					}
					else if (existingTask.id) {
						existingTask.added = false;
						existingTask.updated = false;
					}
					else if (existingTask.updated) {
						existingTask.added = true;
						existingTask.updated = false;
					}
				}

				$(".select-supervisor span.select2-container").removeClass('invalid');
			};

			sd.onDeselect = (o) => {
				const parentCategory = this.scheduleDataMappings.find(sdm => sdm.id === data.id);
				if (parentCategory) {
					const existingTask = parentCategory.tasks?.find(task => task.constructionTaskId === o.id);

					if (existingTask) {
						if (existingTask.id) {
							existingTask.added = false;
							existingTask.updated = true;
						} else {
							const existingTaskIndex = parentCategory.tasks.findIndex(task => task.constructionTaskId === o.id);
							parentCategory.tasks.splice(existingTaskIndex, 1);
						}
					}
				}
			};

			sd.width = '1%';

			setTimeout(() => {
				sd.init(`#${data.id}`);
			}, 0);

			dropdownInstances[data.id] = sd;

			return `<div class="input-group input-group-xs select-supervisor">
					<select class="form-select" data-control="select2" multiple="multiple" data-placeholder="Select construction tasks" id="${data.id}">
					</select>
				</div>`
		}

		return `<tr class="" style="text-transform:uppercase;padding:5px 0px 5px 0px" data-account-id="${data.id}">
                                <td class="fw-bolder ps-2">
									${data.parentEstimateCategory ? `${data.parentEstimateCategory} - ${data.name}` : data.name}
								</td>
								<td>${renderTaskController(data)} </td>
                            </tr>`;
	}

	async #loadConstructionTasks() {
		await this.service.loadConstructionTasks()
			.then((constructionTasks) => {
				this.tasks = constructionTasks;
			});
	}

	async #loadScheduleDataMappingAsync() {
		await this.service.loadScheduleDataMappings()
			.then((scheduleDataMapping) => {
				this.scheduleDataMappings = scheduleDataMapping;
			});
	}

	#renderScheduleDataMappings() {
		$("#dv-datamappings").removeClass("loading").addClass("loaded");
		const rows = this.scheduleDataMappings.map(item => { return this.#renderMappingRow(item); }).join('');
		$("#body-data-mappings").html(rows);
	}

	#refreshScheduleDataMappings(filteredData) {
		const rows = filteredData.map(item => { return this.#renderMappingRow(item); }).join('');
		$("#body-data-mappings").html(rows);
	}

	saveEstimateMappings(b) {

		b.setAttribute('data-kt-indicator', 'on');
		b.disabled = true;
		const tasksToSend = this.scheduleDataMappings
			.flatMap(category =>
				category.tasks
					.filter(task => task.added || task.updated)
					.map(task => ({
						constructionTaskId: task.constructionTaskId,
						added: task.added,
						updated: task.updated,
						estimateCategoryId: task.estimateCategoryId,
						id: task.id
					}))
		);

		this.service.saveScheduleDataMappings(tasksToSend)
			.then((scheduleDataMappings) => {
				b.removeAttribute('data-kt-indicator');
				b.disabled = false;
				this.scheduleDataMappings = scheduleDataMappings;

				this.#renderScheduleDataMappings();
				this.swal.alert('Schedule Task Mapping has been saved successfully.', () => {
				
				});
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
		//const filteredRows = this.scheduleDataMappings.filter(row => row.name.toLowerCase().includes(filter));
		const filteredRows = [];
		this.scheduleDataMappings.forEach(row => {
			if (row.name.toLowerCase().includes(filter)) {
				filteredRows.push(row);
			}
			else {
				if (row.tasks != null && row.tasks.length > 1) {
					row.tasks.forEach(task => {
						if (task.name.toLowerCase().includes(filter)) {
							filteredRows.push(row);
						}
					});
				}
			}
		});

		this.#refreshScheduleDataMappings(filteredRows);
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
			showOnScrollTop: 260
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	init() {
		this.#loadScheduleDataMappingAsync()
			.then(() => {
				this.#loadConstructionTasks()
					.then(() => {
						this.#renderScheduleDataMappings();
						this.#buildStickyActions();
						this.#buildStickyHeader();
					});
			});
		
    }
}

$(document).ready(() => {
	const view = new ScheduleDataMappingView();
    view.init();
});