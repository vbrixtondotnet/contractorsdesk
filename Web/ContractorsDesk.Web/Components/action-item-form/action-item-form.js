class ActionItemForm extends DomEventComponent {
	constructor() {
		super();
		this.actionItemDrawer = new Drawer(`#actionitem-form-drawer`);
		this.mode = "new"; // or "edit
		this.form = document.getElementById('actionitem-form-drawer-form');
		this.selectedProjectId = null;
		this.estimateCategories = [];
		this.httpService = new httpService();
		this.swal = new SwalUtil();
		this.onSaveActionItemCallback = null;
		this.currentUser = Auth.currentUser();
		this.init();
		this.projectId = null;
	}

	populateProjects(projects) {
		this.projects = projects;
		this.#setProjectDropdown();
	}

	createNew() {
		this.mode = "new";
		this.actionItemDrawer.toggle();
		this.#prepareForm();
	}

	close() {
		this.actionItemDrawer.toggle();
	}

	edit(actionItem) {
		this.actionItem = actionItem;
		this.mode = "edit";
		this.actionItemDrawer.toggle();
		this.#prepareEditForm();
	}

	assign(actionItem) {
		this.actionItem = actionItem;
		this.mode = "assign";
		this.actionItemDrawer.toggle();
		this.#prepareEditForm();
	}

	review(actionItem) {
		this.actionItem = actionItem;
		this.mode = "review";
		this.actionItemDrawer.toggle();
		this.#prepareEditForm();
		$("#actionitem-drawer-form-title").html("Review Action Item");
		$(".btn-actionitem-saveandstart").removeClass("d-none");
		$(".btn-actionitem-save").addClass("d-none");
	}

	#setProjectDropdown() {
		const projects = this.projects;

		if (projects && this.projects.length > 0) {
			$("#slc-action-item-form-project").empty();
			$("#slc-action-item-form-project").append(`<option value="" disabled selected>Select Project</option>`);
			projects.forEach(p => {
				$("#slc-action-item-form-project").append(new Option(p.name, p.id));
			});

			$("#slc-action-item-form-project").select2({ placeholder: 'Select Project' });

			this.#fixSelect2Width();
		}
	}

	#initProjectSearch() {
		const instance = this;
		$("#slc-action-item-form-project").select2({
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

		$("#slc-action-item-form-project").on('select2:select', function (e) {
			instance.selectedProjectId = e.params.data.id;
			instance.#loadEstimateCategoriesAndSchedule(instance.selectedProjectId);
		});
	}

	async #loadEstimateCategoriesAndSchedule(projectId) {
		if (projectId == null) return;

		this.#preloadDropdown("#slc-action-item-form-estimatecategory", "Select Estimate Category", "Loading Estimate Categories");
		this.#preloadDropdown("#slc-action-item-form-constructiontask", "Select Construction Task", "Loading Construction Tasks");

		await this.httpService.get("/api/projects/" + projectId + "/estimate-categories-and-schedules")
			.then((resp) => {
				this.estimateCategories = resp.estimateCategories;
				const scheduleTasks = resp.scheduleTasks;

				this.#populateEstimateCategories(this.estimateCategories);
				this.#populateConstructionTasks(scheduleTasks);
			});
	}

	#preloadDropdown(id, disabledText, loadingText) {
		this.#resetDropdown(id, disabledText);
		$(id).select2({ placeholder: loadingText });
		this.#fixSelect2Width();
	}

	#resetDropdown(id, disabledText) {
		$(id).prop('disabled', true);
		$(id).empty();
		$(id).append(`<option value="" disabled selected>${disabledText}</option>`);
	}

	#populateEstimateCategories(estimateCategories) {
		const instance = this;
		$("#slc-action-item-form-estimatecategory").empty();
		$("#slc-action-item-form-estimatecategory").prop("disabled", false);
		$("#slc-action-item-form-estimatecategory").append(`<option value="" disabled selected>Select Estimate Category</option>`);

		estimateCategories.forEach(ct => {
			$("#slc-action-item-form-estimatecategory").append(new Option(ct.name, ct.estimateCategoryID));
		});

		$("#slc-action-item-form-estimatecategory").select2({ placeholder: 'Select Estimate Category' });
		this.#fixSelect2Width();

		$("#slc-action-item-form-estimatecategory").on('select2:select', function (e) {
			var selectedId = e.currentTarget.value;
			instance.estimateCategoryID = selectedId;
			const estimateCategory = instance.estimateCategories.find(ec => ec.estimateCategoryID == selectedId);
			$("#txt-action-item-form-currentamount").val(estimateCategory.currentAmount);
			$("#txt-action-item-form-costchange").val('');
			$("#txt-action-item-form-revisedamount").val('');
		});

	}

	#populateConstructionTasks(scheduleTasks) {
		const instance = this;

		$("#slc-action-item-form-constructiontask").empty();
		$("#slc-action-item-form-constructiontask").prop("disabled", false);
		$("#slc-action-item-form-constructiontask").append(`<option value="" disabled selected>Select Construction Task</option>`);

		scheduleTasks.forEach(ct => {
			$("#slc-action-item-form-constructiontask").append(new Option(ct.name, ct.id));
		});

		$("#slc-action-item-form-constructiontask").select2({ placeholder: 'Select Construction Task' });
		this.#fixSelect2Width();

		$("#slc-action-item-form-constructiontask").on('select2:select', function (e) {
			var selectedId = e.currentTarget.value;
			instance.scheduleTaskId = selectedId;
		});

	}

	#initAssignedToSearch() {
		const instance = this;
		this.assignedToDropdown = $("#slc-action-item-form-assignedto").select2({
			minimumInputLength: 0,
			dropdownPosition: 'below',
			ajax: {
				url: '/api/users',
				data: params => ({ search: params.term }),
				processResults: response => {
					return {
						results: response.data.map(item => ({
							id: item.id,
							firstName: item.firstName,
							lastName: item.lastName,
							email: item.email,
							text: `${item.firstName} ${item.lastName}` // fallback for selection
						}))
					};
				}
			},
			templateResult: function (item) {
				if (!item.id) return item.text; // for placeholder
				return $(`<div>
                            <div><strong>${item.firstName} ${item.lastName}</strong></div>
                            <div style="font-size: 0.9em; color: #888;">${item.email}</div>
                        </div>`);
			},
			templateSelection: function (item) {
				return item.text || '';
			}
		});
		$("#slc-action-item-form-assignedto").on('select2:select', function (e) {
			instance.assignedTo = e.params.data;
		});
	}
	
	#prepareForm() {
		const formTitle = this.mode == "new" ? "New Action Item" : "Edit Action Item";
		$("#actionitem-drawer-form-title").html(formTitle);

		$("#actionitem-form-drawer-form div.input-group").removeClass('d-none');

		this.#setProjectDropdown();
		this.#resetDropdown("#slc-action-item-form-estimatecategory", "Select Estimate Category");
		this.#resetDropdown("#slc-action-item-form-constructiontask", "Select Construction Task");
		$("#slc-action-item-form-assignedto").empty();
		$("#slc-action-item-form-assignedto").append(`<option value="${this.currentUser.id}" selected>${this.currentUser.firstName} ${this.currentUser.lastName}</option>`);
		$("#actionitem-form-drawer-form .input-group.general-cost-change").addClass("d-none");
		$("#txt-action-item-form-duedate").flatpickr({
			minDate: new Date(),
			dateFormat: "Y-m-d"
		});

		this.form.reset();
		this.actionType = null;
		this.estimateCategoryID = null;
		this.scheduleTaskId = null;

		let now = new Date(),
			formattedDateTime = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, "0")}-${String(now.getDate()).padStart(2, "0")} `;
		//const selectedSupervisorIds = actionItem.assignedSupervisors.map(s => s.id);
		$("#txt-action-item-form-duedate").val(formattedDateTime);

		$("#txt-action-item-form-title").prop("disabled", false);
		$("#txt-action-item-form-description").prop("disabled", false);
		$("#slc-action-item-form-actiontype").prop("disabled", false);
		$("#slc-action-item-form-project").val(this.projectId).trigger('change');
		if (this.projectId) {
			this.#loadEstimateCategoriesAndSchedule(this.projectId);
			this.selectedProjectId = this.projectId;

		}
	}

	#toggleAssignFields() {
		if (this.mode == "assign") {
			$("#actionitem-form-drawer-form div.input-group").addClass('d-none');
			$("#actionitem-form-drawer-form div.input-group.show-on-assign").removeClass('d-none');
		}

		$("#txt-action-item-form-title").prop("disabled", this.mode == "assign");
		$("#txt-action-item-form-description").prop("disabled", this.mode == "assign");
	}

	async #prepareEditForm() {
		const actionItem = this.actionItem;

		this.actionType = actionItem.actionTypeId;
		this.estimateCategoryID = actionItem.costChangeItemId;
		this.scheduleTaskId = actionItem.scheduleChangeItemId;
		this.selectedProjectId = actionItem.projectId ?? null;

		this.#setProjectDropdown();

		$("#actionitem-drawer-form-title").html("Edit Action Item");
		$("#txt-action-item-form-title").val(actionItem.title);
		$("#txt-action-item-form-description").val(actionItem.description);
		$("#txt-action-item-form-duedate").val(actionItem.dueDate);
		$("#txt-action-item-form-currentamount").val(actionItem.currentAmount ?? '').trigger('keyup');
		$("#txt-action-item-form-costchange").val(actionItem.amount ?? '').trigger('keyup');
		$("#txt-action-item-form-revisedamount").val(actionItem.revisedAmount ?? '').trigger('keyup');
		$("#txt-action-item-form-numberofdays").val(actionItem.noOfDays ?? '');
		$("#slc-action-item-form-actiontype").val(actionItem.actionTypeId).prop("disabled", true);
		$("#slc-action-item-form-assignedto").empty();

		actionItem.assignedSupervisors.forEach(s => {
			$("#slc-action-item-form-assignedto").append(`<option value="${s.id}" selected>${s.firstName} ${s.lastName}</option>`);
		});

		$("#slc-action-item-form-project").val(actionItem.projectId).trigger('change');

		$("#actionitem-form-drawer-form div.input-group").removeClass('d-none');

		this.onActionTypeChange(document.getElementById('slc-action-item-form-actiontype'));
		this.#toggleAssignFields();
		
		await this.#loadEstimateCategoriesAndSchedule(actionItem.projectId);

		$("#slc-action-item-form-estimatecategory").val(actionItem.costChangeItemId).trigger('change');
		$("#slc-action-item-form-constructiontask").val(actionItem.scheduleChangeItemId).trigger('change');
	}

    #fixSelect2Width() {
		$("#actionitem-form-drawer-form span.select2.select2-container").css("width", "");
	}

	#isValidForm() {
		const form = this.form;
		const isValidFormPerActionType = this.#validatePerActionType();

		if (!isValidFormPerActionType || !form.checkValidity()) {
			form.reportValidity(); 
			return false;
		}

		return true
	}

	#validatePerActionType() {
		var retval = true;

		const estimateCategorySelect = document.getElementById('slc-action-item-form-estimatecategory');
		const costchangeInput = document.getElementById('txt-action-item-form-costchange');
		const constructiontaskSelect = document.getElementById('slc-action-item-form-constructiontask');
		const noOfDaysInput = document.getElementById('txt-action-item-form-numberofdays');

		estimateCategorySelect.setCustomValidity('');
		costchangeInput.setCustomValidity('');

		constructiontaskSelect.setCustomValidity('');
		noOfDaysInput.setCustomValidity('');

		this.estimateCategoryID = estimateCategorySelect.value;
		this.scheduleTaskId = constructiontaskSelect.value;

		switch (this.actionType) {
            case 1: // cost change
				//estimateCategorySelect.setAttribute('required', 'required');
				if (!this.estimateCategoryID || this.estimateCategoryID === '') {
					estimateCategorySelect.setCustomValidity('Estimate Category is required.');
					retval = false;
				}
				else {
					if(!costchangeInput.value || costchangeInput.value === '') {
                        costchangeInput.setCustomValidity('Cost Change is required.');
                        retval = false;
                    }
				}
				break;
			case 2: // schedule change
				//constructiontaskSelect.setAttribute('required', 'required');
				if (!this.scheduleTaskId || this.scheduleTaskId === '') {
					constructiontaskSelect.setCustomValidity('Schedule Task is required.');
					retval = false;
				}
				else {
					if (!noOfDaysInput.value || noOfDaysInput.value === '') {
						noOfDaysInput.setCustomValidity('Number of Days is required.');
						retval = false;
					}
				}
				break;
			case 8: // general cost change
				break;
		}

		return retval;
	}

	onCostChangeInput(e) {
		const currentAmount = TextboxUtils.getFloatValue($("#txt-action-item-form-currentamount").val());
		const costChange = TextboxUtils.getFloatValue(e.value);
		const revisedAmount = currentAmount + costChange;
		$("#txt-action-item-form-revisedamount").val(revisedAmount.toFixed(2)).trigger('keyup');
	}

	onRevisedAmountInput(e) {
		const currentAmount = TextboxUtils.getFloatValue($("#txt-action-item-form-currentamount").val());
		const revisedAmount = TextboxUtils.getFloatValue(e.value);
		const costChange = revisedAmount - currentAmount;
		$("#txt-action-item-form-costchange").val(costChange.toFixed(2)).trigger('keyup');	
	}

	onActionTypeChange(e) {
		const selectedValue = parseInt(e.value);
		// Hide all cost change related fields initially
		$("#actionitem-form-drawer-form .input-group.general-cost-change").addClass("d-none");

		switch (selectedValue) {
			case 1:
				$("#actionitem-form-drawer-form .input-group.cost-change").removeClass("d-none");
				break;
			case 2:
				$("#actionitem-form-drawer-form .input-group.schedule-change").removeClass("d-none");
				break;
			case 8:
				$("#actionitem-form-drawer-form .input-group.general-cost-change").removeClass("d-none");
                break;
		}

		this.actionType = selectedValue;

	}

	onActionItemFormSave(b) {
		debugger;
		if (this.#isValidForm()) {
			const formatDueDate = (date) => date !== '' ? new Date(date.replace(/-/g, '/')).toISOString().split('T')[0] : null;
			const description = $("#txt-action-item-form-description").val()
				.replace(/&/g, '&amp;')   // escape HTML
				.replace(/</g, '&lt;')
				.replace(/>/g, '&gt;')
				.replace(/\n/g, '<br>');  // convert line breaks to <br>


			const actionItem = {};
			actionItem.id = this.mode == "edit" || this.mode == "review" || this.mode == "assign" ? this.actionItem.id : 0; // Set ID only for edit mode
			actionItem.title = $("#txt-action-item-form-title").val();
			actionItem.description = description;
			actionItem.actionTypeId = this.actionType;
			actionItem.dueDate = formatDueDate($("#txt-action-item-form-duedate").val());
			actionItem.supervisors = this.assignedToDropdown.val().map(Number);
			actionItem.costChangeAmount = parseFloat($("#txt-action-item-form-costchange").val().replace(',', ''));
			actionItem.scheduleChangeNumberOfDays = parseInt($("#txt-action-item-form-numberofdays").val());

			actionItem.costChangeRequiresClientApproval = true;
			actionItem.projectId = this.selectedProjectId == "" ? null : this.selectedProjectId;
			actionItem.costChangeEstimateCategoryId = this.estimateCategoryID == "" ? null : this.estimateCategoryID;
			actionItem.scheduleChangeTaskId = this.scheduleTaskId == "" ? null : this.scheduleTaskId;
			actionItem.scheduleChangeRequiresClientApproval = actionItem.costChangeRequiresClientApproval;
			actionItem.startOnSaveChanges = this.mode == "review";
			
			b.setAttribute('data-kt-indicator', 'on');
			b.disabled = true;
			let onOk = null;

			this.httpService.post('/api/action-items', actionItem)
				.then((data) => {
					let message = "";
					switch (this.mode) {
						case "edit":
							message = "Action Item has been updated successfully!";
							break;
						case "review":
							message = "Action Item has been reviewed and started successfully!";
							break;
						default:
							message = "Action Item has been created successfully!";
                            break;
					}
				
					this.swal.alert(message, () => {
						if (this.onSaveActionItemCallback)
							this.onSaveActionItemCallback(data, this.mode);
					});
					debugger;
					this.actionItemDrawer.toggle();
				})
				.finally(() => {
					b.removeAttribute('data-kt-indicator');
					b.disabled = false;
				});
		}
	}

	init() {
		$("#txt-action-item-formdue-date").flatpickr({
			minDate: new Date(),
			enableTime: false,
			dateFormat: "Y-m-d h:i K",
			time_24hr: false
		});

		this.#initProjectSearch();
		this.#initAssignedToSearch();

		this.#prepareForm();
		TextboxUtils.init();

	}
}