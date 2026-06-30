class ConstructionTasksView extends DomEventComponent {
	constructor() {
		super();
		this.service = new ConstructionTasksService();
		this.constructionTasks = [];
		this.addItemForm = document.getElementById('frmAddItem');
		this.addConstructionTaskDrawer = new Drawer('#add-constructiontask-drawer');
		this.addItemFormValidator = this.#setAddItemValidator(this.addItemForm);
		this.swal = new SwalUtil();
	}

	#setAddItemValidator(form) {
		return new FormValidator(form, {
			'name': {
				validators: {
					notEmpty: {
						message: 'Name is required'
					}
				}
			},
			'duration': {
				validators: {
					notEmpty: {
						message: 'Duration is required'
					}
				}
			}
		}).init();
	}

	async #loadConstructionTasks() {
		await this.service.loadConstructionTasks()
			.then((response) => {
                this.constructionTasks = response;
            })
            .catch((error) => {
                console.error(error);
            });
	}

	#renderConstructionTaskRow(item) {
		return `<tr class="${item.id}">
							<td class="ps-2 text-center ">
								${item.sequence}
							</td>
							<td class=" ">
								${item.name}
							</td>
							<td class="text-center">
								${item.duration}
							</td>
							<td class="">
								${item.parentTaskName}
							</td>
						</tr>`;
	}

	#renderConstructionTasks() {
		$("#dv-construction-tasks").removeClass("loading").addClass("loaded");
		const rows = this.constructionTasks.map(item => { return this.#renderConstructionTaskRow(item); }).join('');
		$("#construction-tasks-body").html(rows);
	}

	cancelChanges() {
		this.swal.confirm(`Are you sure to cancel all your changes?`,
			() => {
				location.reload();
			}
		);
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

	#initPredecessorSelect() {
		const predecessorOptions = `<option value="${Guid.empty}">&nbsp;</option>
									${this.constructionTasks.map(c => { return c.name != '' ? `<option value="${c.id}"}>${c.name}</option>` : '';
		}).join('')}`;
		
		$("#slcAddItemPred1").each(function () {
			$(this).html(predecessorOptions);
		});

		$("#slcAddItemPred1").select2();

		$('#slcAddItemPred1').on('select2:open', function () {
			requestAnimationFrame(() => {
				let searchField = document.querySelector('.select2-container--open .select2-search__field');
				if (searchField) {
					searchField.focus();
				}
			});
		});
	}

	onNewConstructionTaskSave() {
		const validator = this.addItemFormValidator;
		const addItemForm = this.addItemForm;
		let constructionTask = new ConstructionTaskModel();

		validator.validate().then((status) => {
			if (status == 'Valid') {
				const existingItem = this.constructionTasks.find(c => c.name.trim().toLowerCase() == addItemForm.name.value.trim().toLowerCase());
				if (existingItem != null) {
					$("#txtAddItemName").next().html(`<div data-field="name" data-validator="notEmpty">${addItemForm.name.value} already exists!</div>`);
					$("#txtAddItemName").parents('fv-row').removeClass('fv-plugins-bootstrap5-row-valid').addClass('fv-plugins-bootstrap5-row-invalid');
					return;
				}
				else {
					$("#txtAddItemName").next().html('');
					$("#txtAddItemName").parents('fv-row').removeClass('fv-plugins-bootstrap5-row-invalid').addClass('fv-plugins-bootstrap5-row-valid');

					const highestSequenceTask = this.constructionTasks.reduce((maxTask, task) =>
						task.sequence > maxTask.sequence ? task : maxTask
					);

					const selectedValue = $("#slcAddItemPred1").val();
					constructionTask.name = addItemForm.name.value;
					constructionTask.duration = addItemForm.duration.value;
					constructionTask.sequence = highestSequenceTask.sequence + 1;
					constructionTask.parentTaskId = selectedValue ? selectedValue : null;

					this.service.saveConstructionTask(constructionTask)
						.then((data) => {
							this.swal.alert('Construction Task has been submitted successfully!', () => {
								location.reload();
							});
						});
				}
			}
		});
	}

	toggleAddDrawer() {
		this.addConstructionTaskDrawer.toggle();
	}

	clearFilter() {
		var input = $(`input[evt-input="filterRows"]`);
		input.val('');
		this.filterRows(input);
	}


	filterRows(i) {
		const filter = $(i).val().toLowerCase().trim(); // Get the search value
		$('tbody#construction-tasks-body tr').each(function () {
			const rowText = $(this).text().toLowerCase(); // Get all text in the row
			if (rowText.includes(filter)) {
				$(this).show(); // Show rows that match the filter
			} else {
				$(this).hide(); // Hide rows that don't match
			}
		});
	}

	init() {
		this.#loadConstructionTasks()
			.then(() => {
				this.#renderConstructionTasks();
		/*		this.#buildStickyActions();*/
				this.#buildStickyHeader();
				this.#initPredecessorSelect();
				TextboxUtils.init();
			});
		
    }
}

$(document).ready(() => {
	const view = new ConstructionTasksView();
    view.init();
});