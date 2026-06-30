class CreateProposalView extends DomEventComponent {
	constructor() {
		super();
		this.proposalId = document.URL.split('/').pop();
		this.httpService = new httpService();
		this.service = new ProposalService();
		this.proposal = null;
		this.prevProposal = null;
		this.proposalLines = [];
		this.proposalLinesByGroup = [];
		this.swal = new SwalUtil();
		this.customers = [];
		this.onprojectnamesearch = null;
		this.projectNameExists = false;
		this.supervisors = [];
		this.tagifySelect = null;
		this.selectedSupervisors = [];
		this.templates = [];
		this.template = null;
		this.permissions = null;
		this.totalNonOverhead = 0;
		this.totalProjectAmount = 0;
		this.totalOverhead = 0;
		this.supervisorsDropdown = null;
		this.printer = new printer();
		this.onsearchLineItem = null;
		this.lineItemsUniqueNamesValidation = [];
		this.proposalTemplateId = 0;
		this.oncheckduplicatetimeout = null;
		this.estimateCategories = [];
		this.parentEstimateCategories = [];
		this.isinitialload = true;
		this.tabControl = new TabControl();
		this.onSequenceChangeTimeOut = null;
		this.isNewProposal = this.proposalId == Guid.empty;
		this.sendContractDrawer = new Drawer('#send-contract-drawer');
		this.saveTemplateDrawer = new Drawer('#save-template-drawer');
		this.addCategoryDrawer = new Drawer('#add-category-drawer');
		this.sendContractForm = document.getElementById('form-contract-details');
		this.sendContractFormValidator = this.#setContractFormValidator();
		this.templateForm = document.getElementById('form-template');
		this.templateFormValidator = this.#setTemplateFormValidator();
		this.addCategoryForm = document.getElementById('form-add-category');
		this.addCategoryFormValidator = this.#setAddCategoryFormValidator();

		this.printOptionForm = document.getElementById('form-print');
		this.printOptionDrawer = new Drawer('#print-option-drawer');
		this.includeZeroAmounts = false;
		this.companySettings = Auth.getCompanySettings();
		this.tempLineItems = [];
		this.lineItemNameChangeTimeout = null;
		this.isPrevAccepted = false;
		this.prevStatusId = 1;
		this.enableUnloadProtection = true;
		this.saveChangesMethod = (callback) => this.saveProposal(document.getElementById('btnSaveProposal'), callback);
		this.isInitializing = true;
		this.cachekey = 'proposal-form';
		this.emailTemplates = [];
		this.clientModel = new ClientModel();
		this.projectModel = new ProjectModel();
		this.emailSender = new EmailSender();
		this.matchingProjects = [];
		this.mergeProposalModal = null;
		this.selectedMergeProjectId = null;
		this.payload = null;
		this.editMode = false;
	}

	downloadProposalCsv(b) {
		$(b).prop("disabled", true);
		const spinner = $(b).find('.spinner-border');
		spinner.removeClass('d-none');

		fetch(`/api/proposals/${this.proposalId}/download-csv`)
			.then(response => response.blob())
			.then(blob => {
				$(b).prop("disabled", false);
				spinner.addClass('d-none');
				const url = URL.createObjectURL(blob);
				const link = document.createElement('a');
				link.href = url;
				link.download = `${this.proposal.project.name}.csv`;
				link.click();
			});
	}

	async onSendContract(button) {
		const validator = this.sendContractFormValidator;
		const sendContractForm = this.sendContractForm;
		validator.validate().then((status) => {
			if (status == 'Valid') {
				button.setAttribute('data-kt-indicator', 'on');
				button.disabled = true;
				const contractModel = {
					clientName: $("#txtClientName").val(),
					clientEmail: $("#txtClientEmail").val().trim(),
					clientAddress: $("#txtClientAddress").val(),
					clientCity: $("#txtClientCity").val(),
					clientState: $("#txtClientState").val(),
					estStartDate: $("#txtestStartDate").val(),
					estCompletionDate: $("#txtestCompletionDate").val(),
					genContractorsFeePercentage: $("#txtgenContractorsFeePercentage").val(),
					genContractorsFeeAmount: $("#txtgenContractorsFeeAmount").val(),
					projectCost: $("#txtprojectCost").val(),
					initialDepositPercentage: $("#txtinitialDepositPercentage").val(),
					contractor: $("#txtcontractor").val(),
					contractorEmailAddress: $("#txtcontractorEmail").val(),
				};

				this.service.sendContract(this.proposalId, contractModel)
					.then((response) => {
						this.swal.alert('Contract was sent successfully!');
						sendContractForm.reset();
						this.sendContractDrawer.toggle();
					})
					.finally(() => {
						button.removeAttribute('data-kt-indicator');
						button.disabled = false;
					});
				this.swal.alert('Contract was sent successfully!');
			}
		});
	}

	async print(b) {

		$(b).prop("disabled", true);
		const spinner = $(b).find('.spinner-border');
		spinner.removeClass('d-none');
		try {
			// Fetch the PDF from the API
			const response = await fetch(`/api/print/proposal/${this.proposalId}`);
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
			$(b).prop("disabled", false);
			spinner.addClass('d-none');
		}
	}

	onCancelBtn() {
		this.swal.confirm(`Are you sure to cancel all your changes?`,
			() => {
				location.reload();
			}
		);
	}

	onEditProposal() {
		$("#slcProposalStatus").prop("disabled", false);
		this.editMode = true;
		this.#toggleFormState(true);
		$(`button[evt-click="onEditProposal"]`).addClass('d-none');
	}

	createSchedule(b) {
		location.href = `/project/${this.proposal.project.id}/schedule`;
	}

	loadTemplateLineItems() {
		const id = this.template.id;

		$("#dv-tab-line-items").addClass('loading').removeClass('loaded');
		$(".category-row").remove();
		this.service.loadTemplateLineItems(id)
			.then((lineItems) => {
				this.proposalLinesByGroup = lineItems;
				this.proposalLinesByGroup.forEach(item => {
					item.lineItems.forEach((lineItem, index) => {
						lineItem.sequence = index + 1; // Set sequence to index + 1
					});
				});
			})
			.then(() => {
				this.#renderProposalDetails();
				this.#renderProposalLineCategories();
			});
	}

	onLineItemAmountChange(i) {
		const itemId = $(i).attr("data-item-id");
		let value = $(i).val() == '' ? '0' : $(i).val();
		value = value.replace(/,/g, '');

		const itemInGroup = this.proposalLinesByGroup.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;
		itemInGroup.amount = parseFloat(value);

		this.#resetSquareFeetAndMultiplier(itemId);
		this.#calculateCategoryTotals(itemInGroup.parentId);
	}

	onLineItemNameChangeCallback(i) {

		const input = $(i);
		const name = input.val();
		const currentItemId = input.attr("data-item-id");

		const categoryId = input.parents('tr').parents('tbody').data("category-id");
		const currentCategory = this.proposalLinesByGroup.find(line => line.id === categoryId);
		const categoryName = currentCategory.name;
		const lineItem = currentCategory.lineItems.find(line => line.name.trim().toLowerCase() === name.trim().toLowerCase());

		if (lineItem != null) {
			input.addClass('error');
			this.swal.error(`Duplicate line item found on ${categoryName}! Please select a different line item.`);
			this.lineItemsUniqueNamesValidation.push({ id: currentItemId, desc: `${categoryName} has duplicate line items.` });
		}
		else {
			const item = { id: currentItemId, name: name };
			input.removeClass('error');
			const tempLineItem = this.tempLineItems.find(li => li.id == currentItemId);
			if (tempLineItem) {
				currentCategory.lineItems.push(tempLineItem);
				this.tempLineItems = this.tempLineItems.filter(li => li.id != currentItemId);
			}

			this.lineItemsUniqueNamesValidation = this.lineItemsUniqueNamesValidation.filter(li => li.id != currentItemId);
			this.#setNewEstimateCategoryId(input, item, currentItemId);
		}
	}

	onLineItemNameChange(i) {
		const instance = this;
		clearTimeout(this.lineItemNameChangeTimeout);
		this.lineItemNameChangeTimeout = setTimeout(() => { instance.onLineItemNameChangeCallback(i); }, 500);
	}

	onPercentageAmountChange(i) {
		const id = $(i).attr("data-item-id");
		const percentageCtrl = $(`input.percentage[data-item-id='${id}']`);

		let value = $(i).val() == '' ? 0 : $(i).val();
		value = parseFloat(value.replace(/,/g, ''));

		let percentage = value / this.totalNonOverhead * 100;
		percentage = parseFloat(percentage.toFixed(2));
		percentageCtrl.val(percentage);

		var itemInGroup = this.proposalLinesByGroup.flatMap(items => items.lineItems).find(line => line.id === id) || null;
		itemInGroup.percentage = percentage;
		itemInGroup.amount = value;

		this.#calculateOverheadAndProjectTotals();
	}

	onSquareFootChange(i) {
		const input = $(i);
		const itemId = input.attr("data-item-id");
		let value = parseFloat(input.val() == '' ? '0' : input.val().replace(/,/g, ''));

		const multiplierAmountField = input.parent().parent().next().find('input.multiplier-amount');
		const multiplierAmountValue = parseFloat(multiplierAmountField.val() == '' ? '0' : multiplierAmountField.val().replace(/,/g, ''));
		const itemInGroup = this.proposalLinesByGroup.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;

		if (multiplierAmountField.val() != '') {
			const amountField = input.parent().parent().next().next().find('input.line-item-amount');
			const amount = value * multiplierAmountValue;
			itemInGroup.amount = amount;
			itemInGroup.sqFoot = value;
			amountField.val(amount).trigger('keyup');
			this.#calculateCategoryTotals(itemInGroup.parentId);
		}

		itemInGroup.sqFootLocked = true;
		itemInGroup.sqFoot = value == 0 ? null : value;
		input.addClass('sqFootLocked');
	}

	onMultiplierAmountChange(i) {
		const input = $(i);
		const itemId = input.attr("data-item-id");
		let value = parseFloat(input.val() == '' ? '0' : input.val().replace(/,/g, ''));

		const sqfField = input.parent().parent().prev().find('input.squarefoot');
		const sqfFieldValue = parseFloat(sqfField.val() == '' ? '0' : sqfField.val().replace(/,/g, ''));
		const itemInGroup = this.proposalLinesByGroup.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;
		if (sqfField.val() != '') {
			const amountField = input.parent().parent().next().find('input.line-item-amount');
			const amount = +(value * sqfFieldValue).toFixed(2);
			itemInGroup.amount = amount;
			amountField.val(amount).trigger('keyup');
		}

		this.#calculateCategoryTotals(itemInGroup.parentId);
		itemInGroup.multiplier = value == 0 ? null : value;
	}

	onAmountFocus(i) {
		const currentValue = $(i).val();
		if (currentValue == '0') $(i).val('');
	}

	onDescriptionChange(i) {
		var itemId = $(i).attr("data-item-id");
		let value = $(i).val();

		var itemInGroup = this.proposalLinesByGroup.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;
		itemInGroup.description = value;
	}

	onLineItemSequenceChange(i) {
		var lineItemId = $(i).attr("data-item-id");
		let newSequence = $(i).val();

		if (isNaN(newSequence) || newSequence < 1) {
			newSequence = 1;
		}

		let proposalLinesByGroup = this.proposalLinesByGroup;

		let parent = null;
		let lineItems = null;
		let targetIndex = -1;

		for (let item of proposalLinesByGroup) {
			targetIndex = item.lineItems.findIndex(li => li.id === lineItemId);
			if (targetIndex > -1) {
				parent = item;
				lineItems = item.lineItems;
				break;
			}
		}

		if (!parent || targetIndex === -1) {
			console.error(`LineItem with ID ${lineItemId} not found.`);
			return;
		}

		const [targetItem] = lineItems.splice(targetIndex, 1);
		lineItems.splice(newSequence - 1, 0, targetItem);

		lineItems.forEach((item, index) => {
			item.sequence = index + 1;
		});

		clearTimeout(this.onSequenceChangeTimeOut);

		this.onSequenceChangeTimeOut =
		setTimeout(() => {
			this.#renderProposalLineCategories();
		}, 500);

		$(`input[data-item-id="${lineItemId}"][data-model="sequence"]`).focus();
	}

	onProjectSqFeetChange(i) {
		const sqFeet = $(i).val();
		this.proposal.project.sqFeet = parseInt(sqFeet);
		$("input.squarefoot:not(.sqFootLocked)").val(sqFeet);
	}

	onPercentageChange(i) {
		this.#changepercentage(i);
		this.#calculateOverheadAndProjectTotals();
	}
	
	onTemplateChange(s) {
		const id = $(s).val();
		this.template = this.#findTemplate(id);

		this.loadTemplateLineItems();
	}

	onEmailToClientClick() {
		const proposalClient = this.proposal.client;
		const to = proposalClient.emailAddress ?? '';
		const emailTemplate = this.emailTemplates.find(e => e.emailType == 'Proposal Report');

		this.emailSender.resetEmailFields();

		this.emailSender.showGreetings = true;
		this.emailSender.addDummyAttachmentItem(`${this.proposal.project.name}.proposal.pdf`);
		this.emailSender.setTo(to);
		this.emailSender.setReplyTo(Auth.currentUser().email);
		this.emailSender.setSubject('Here is the cost estimate you requested');
		this.emailSender.setContent(`<p>&nbsp;</p>${emailTemplate.body}`);

		this.emailSender.toggle();
	}

	onSendContractClick() {
		const currentUser = Auth.currentUser();
		const companySettings = this.companySettings;
		const proposalProject = this.proposal.project;
		const proposalClient = this.proposal.client;

		const clientName = `${proposalClient.firstName} ${proposalClient.lastName ?? ''}`;
		const contractorName = companySettings.generalContractorName;
		const contractorEmail = companySettings.companyEmail;
		const state = proposalClient.state;
		const city = proposalClient.city;
		const address = proposalClient.address;
		const email = proposalClient.email;
		const gcFeePercentage = this.proposal.totalGCFeePercentage;	
		const gcFeeAmount = this.proposal.totalGCFeeAmount;

		//this.totalProjectAmount 
		$("#txtClientName").val(clientName);
		$("#txtClientAddress").val(address);
		$("#txtClientState").val(state);
		$("#txtClientCity").val(city);
		$("#txtClientEmail").val(email);
		$("#txtprojectCost").val(this.totalProjectAmount);
		$("#txtcontractor").val(contractorName);
		$("#txtcontractorEmail").val(contractorEmail);
		$("#txtgenContractorsFeePercentage").val(gcFeePercentage);
		$("#txtgenContractorsFeeAmount").val(gcFeeAmount);
	
		$("#txtestStartDate").flatpickr({
			dateFormat: "m/d/Y",
			onChange: (selectedDates, dateStr, instance) => {
				//this.startDate = dateStr;
				//this.#updateStartAndEndDates(dateStr);
			}
		});
		$("#txtestCompletionDate").flatpickr({
			dateFormat: "m/d/Y",
			onChange: (selectedDates, dateStr, instance) => {
				//this.startDate = dateStr;
				//this.#updateStartAndEndDates(dateStr);
			}
		});
		this.sendContractDrawer.toggle();
	}

	onSaveAsTemplateClick(b) {
		this.templateModel = {
			id: Guid.empty,
			name: '',
			overwrite: false,
			default: false
		};

		this.templateFormData = new Form();
		this.templateFormData.formId = "form-template";
		this.templateFormData.model = this.templateModel;
		this.templateFormData.init();

		var currentTemplate = this.template;
		this.templateForm.reset();

		const currentUser = Auth.currentUser();	
		$("#chkOverwrite").prop("disabled", currentTemplate.isDefault || currentTemplate.owner[0].id !== currentUser.id);

		this.saveTemplateDrawer.toggle();
		$("#txtTemplateName").focus();
	}

	onSaveTemplate(b) {
		var templateModel = this.templateModel;
		var currentTemplate = this.template;
		const validator = this.templateFormValidator;
		this.templateModel.name = $("#txtTemplateName").val();
		const currentUser = Auth.currentUser();

		validator.validate().then((status) => {

			if (status == 'Valid') {
				if (templateModel.overwrite) {
					if (currentTemplate.isDefault) {
						const message = `Cannot overwrite Ground-Up (Default). Please uncheck Overwrite Source.`;
						this.swal.alert(message);
					}
					else if (currentTemplate.owner[0].id !== currentUser.id) { 
						const message = `Cannot overwrite not owned template. Please uncheck Overwrite Source.`;
						this.swal.alert(message);
					}
					else {
						const message = `This action will overwrite ${currentTemplate.name} template. Would you like to proceed?`;
						this.swal.confirm(
							message,
							() => {
								this.#submitTemplate(b, true);
							}
						);
					}

				}
				else {
					this.#submitTemplate(b);
				}
			}
		});
	}

	onOverwriteSet(c) {
		var templateModel = this.templateModel;
		var currentTemplate = this.template;
		$("#txtTemplateName").val(templateModel.overwrite ? currentTemplate.name : '').trigger('change');
	}

	onChangeDocStatus(b) {
		const proposal = this.proposal;
		const prevStatus = proposal.status;
		const newStatus = parseInt($("#slcProposalStatus").val());
		const permissions = Auth.permissions();
		const canEditProposals = permissions.CanEditAcceptedProposals;

	
		if (prevStatus == 1 && newStatus == 2) {

			const hasClientName = this.proposalClientModel.clientName != null && this.proposalClientModel.clientName != '';
			const hasClientEmailAddress = this.proposalClientModel.emailAddress != null && this.proposalClientModel.emailAddress != '';

			if (!hasClientName || !hasClientEmailAddress) {
				let errorMessage = `Unable to accept proposal. `;
				if (!hasClientName) {
					errorMessage += `Client Name is required. `;
				}
				else if (!hasClientEmailAddress) {
					errorMessage += `Client Email Address is required.`;
				}

				errorMessage += `Please fill in the client details.`;
				this.swal.info(errorMessage);
				$("#slcProposalStatus").val(prevStatus);
				return;
			}

			this.swal.confirm("Accepting a proposal makes it locked for editing. Are you sure you want to proceed?",
				() => {
					this.service.updateProposalStatus(this.proposalId, newStatus);

					if (canEditProposals)
						$(`button[evt-click="onEditProposal"]`).show();

					this.#handleLockedStatus();
				},
				() => {
					$("#slcProposalStatus").val("1");
					this.#handleLockedStatus();
				}
			);

			return;
		}

		proposal.status = newStatus;
		this.#handleLockedStatus();
	}

	onCategorySequenceChange(i) {
		var id = $(i).attr("data-item-id");
		
		clearTimeout(this.onSequenceChangeTimeOut);
		
		this.onSequenceChangeTimeOut = setTimeout(() => {
			var sequence = parseInt($(i).val());	
			var category = this.proposalLinesByGroup.find(c => c.id == id);

			this.#setCategorySequence(category, sequence);
			this.#renderProposalLineCategories();
			this.#scrollToElement(id);
		}, 500);
	}

	onAddCategoryClick(b) {
		this.categoryModel = {
			name: '',
			sequence: 1
		};
		this.addCategoryFormData = new Form();
		this.addCategoryFormData.formId = "form-add-category";
		this.addCategoryFormData.model = this.categoryModel;
		this.addCategoryFormData.init();

		const options = [];
		const uniqueNames = [];
		this.parentEstimateCategories.forEach(ec => {
			const lowerCaseName = ec.name.toLowerCase();
			if (!uniqueNames.includes(lowerCaseName)) {
				uniqueNames.push(lowerCaseName);
				options.push({ id: ec.id, name: ec.name });
			}
		});
		const acOptions = {
			element: $("#txtCategoryName"),
			options: options,
			onselect: (item) => {
				this.categoryModel.name = item.text;
			}
		}
		const ac = new AutoComplete(acOptions);
		ac.init();

		this.addCategoryForm.reset();
		this.addCategoryDrawer.toggle();

		$("#txtCategoryName").focus();
	}

	onAddCategory(b) {
		var categoryModel = this.categoryModel;
		const validator = this.addCategoryFormValidator;

		validator.validate().then((status) => {
			if (status == 'Valid') {
				debugger;
				const categoryName = categoryModel.name;
				const existingCategory = this.proposalLinesByGroup.find(c => c.name.toLowerCase().trim() == categoryName.toLowerCase().trim());
				if (existingCategory) {
					$("#categoryName").addClass('invalid');
					this.swal.error(`A category with the name '${categoryName}' already exists in this template.`)
					return;
				}
				const id = Guid.new();
				this.categoryModel.id = id;
				this.categoryModel.lineItems = [];
				this.categoryModel.totalAmount = 0;

				const sequence = parseInt(categoryModel.sequence ?? 1);
				this.proposalLinesByGroup.push({ ...this.categoryModel });

				var category = this.proposalLinesByGroup.find(c => c.id == id); 
				this.#setCategorySequenceOnAdd(category, sequence);

				//this.#refreshSequence(2);
				//this.proposalLinesByGroup.push(this.categoryModel);
				//this.proposalLinesByGroup.sort((a, b) => a.sequence - b.sequence);
		
				this.#renderProposalLineCategories();
				this.#scrollToElement(id);
				this.addCategoryDrawer.toggle();
			}
		});
	}

	onAddLineItem(b) {
		const categoryId = $(b).attr("data-category-id");
		const category = this.proposalLinesByGroup.find(c => c.id == categoryId);

		const rowsSelector = `tbody.${categoryId} tr`;
		const newSequence = $(`${rowsSelector}.line-item`).length + 1;
		const isOverhead = category.name == 'Overhead'
		const amountInputClass = isOverhead ? 'overhead overhead-field' : 'line-item-amount';

		const newlineItem = this.#createLineItem(category, newSequence);
		const lineItemId = newlineItem.id;
		const sqFeet = this.proposal.project?.sqFeet ?? '';

		const multiplier = (id) => {
			return `<td style="padding:0 !important;padding-right: 4px !important;padding-top:5px !important;">
						<div class="input-group input-group-sm">
							<input type="text" class="form-control form-control-sm squarefoot" evt-input="onSquareFootChange" data-type="decimal" data-item-id="${id}" value="${sqFeet}">
						</div>
					</td>
					<td style="padding:0 !important;padding-top:5px !important;">
						<div class="input-group input-group-sm">
							<span class="input-group-text">$</span>
							<input type="text" class="form-control form-control-sm multiplier-amount" evt-input="onMultiplierAmountChange" data-type="money" data-item-id="${id}" value="">
						</div>
					</td>`;
		}

		const description = (id) => {
			return `<div class="d-flex">
						<textarea class="form-control textarea-dynamic form-control-sm" 
							data-model="description"
							data-item-id="${id}" 
							evt-input="onDescriptionChange"
							style="height:37px;"></textarea>
						<div class="input-group input-group-sm ms-2 h-37px d-none">
							<span class="input-group-text" id="inputGroup-sizing-sm">%</span>
							<input type="text" data-type="decimal" class="form-control percentage" 
							evt-input="onPercentageChange"
							data-item-id="${id}" 
							value="10">
						</div>
					</div>`;
		}

		const sequence = (id, sequence) => {
			return `<input type="text" class="form-control form-control-sm text-center" 
				data-model="sequence"
				data-type="int"
				data-item-id="${id}" 
				evt-input="onLineItemSequenceChange"
				value="${sequence}">`
		}

		const name = (id) => {
			return `<select
						class="form-control form-control-sm line-item-name"
						evt-input="onLineItemNameChange"
						data-model="name"
						data-parent-category-id="${categoryId}"
						data-item-id="${id}" value=""></select>`;
		}

		const amount = (id, cls) => {
			return `<div class="input-group input-group-sm">
						<span class="input-group-text">$</span>
						<input type="text" class="form-control form-control-sm ${cls}" 
						data-type="money" data-item-id="${id}" value="0" 
						evt-input="onLineItemAmountChange"
						evt-focus="onAmountFocus">
					</div>`;
		}

		const remove = (id) => {
			return `<div class="action-buttons">
						<a href="javascript:" class="btn btn-sm btn-icon btn-light-danger remove-item"
							title="Remove" data-item-id="${id}"
							evt-click="onRemoveLineItem">
							<i class="bi bi-trash"></i>
						</a>
					</div>`;
		}

		const template = `<tr class="line-item" data-id='${lineItemId}'>
							<td>
								${sequence(lineItemId, newSequence)}
							</td>
							<td>
								${name(lineItemId)}
							</td>
							<td>
								${description(lineItemId)}
							</td>
							${!isOverhead ? multiplier(lineItemId) : ''}
							<td style="padding-left:4px !important;padding-top:5px !important;">
								${amount(lineItemId, amountInputClass)}
							</td>
							<td>
								${remove(lineItemId)}
							</td>
						</tr>`;

		$(`${rowsSelector}.row-new-item`).before(template);
		const lineItemTextBox = $(`tr[data-id="${lineItemId}"]`).find('select.line-item-name');

		this.#initEstimateCategoryDropdown(lineItemTextBox);
		TextboxUtils.init();
		//category.lineItems.push(newLineItem);
		this.tempLineItems.push(newlineItem);

		$(lineItemTextBox).select2('open');
		setTimeout(() => {
			const searchField = document.querySelector('.select2-container--open .select2-search__field');
			if (searchField) {
				searchField.focus();
			}
		}, 100); // slight delay ensures DOM is ready


	}

	onMergeProposal(button) {
		this.payload.mergeWithProjectId = this.selectedMergeProjectId;

		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;

		this.httpService.post('/api/proposals/save-and-merge', this.payload)
			.then((response) => {
				this.proposal = response.proposal;
				this.proposalId = this.proposal.id;
				this.#initProposalData(this.proposal);
				this.mergeProposalModal.hide();
				this.swal.alert('Proposal has been saved successfully.');
			})
			.finally(() => {
				button.removeAttribute('data-kt-indicator');
				button.disabled = false;
			});
	}

	onOverwriteProposal(button) {
		this.payload.overwriteProposalId = this.selectedOverwriteProposalId;

		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;

		this.httpService.post('/api/proposals/save-and-overwrite', this.payload)
			.then((response) => {
				this.proposal = response.proposal;
				this.proposalId = this.proposal.id;
				this.#initProposalData(this.proposal);
				this.overwriteProposalModal.hide();
				this.swal.alert('Proposal has been saved successfully.');
			})
			.finally(() => {
				button.removeAttribute('data-kt-indicator');
				button.disabled = false;
			});
	}

	onSaveAsNewProposal(button) {
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;

		this.httpService.post('/api/proposals/save-as-new', this.payload)
			.then((response) => {
				this.proposal = response.proposal;
				this.proposalId = this.proposal.id;
				this.#initProposalData(this.proposal);

				if (this.mergeProposalModal)
					this.mergeProposalModal.hide();
				else if (this.overwriteProposalModal)
					this.overwriteProposalModal.hide();

				this.swal.alert('Proposal has been saved successfully.');
			})
			.finally(() => {
				button.removeAttribute('data-kt-indicator');
				button.disabled = false;
			});
	}

	onMergeProjectSelect(b) {
		this.selectedMergeProjectId = $(b).val();
		$("button.btn-merge-action").prop('disabled', false);
	}

	onOverwriteProposalSelect(b) {
		this.selectedOverwriteProposalId = $(b).val();
		$("button.btn-overwrite-action").prop('disabled', false);
	}

	onRemoveLineItem(a) {
		const id = $(a).attr("data-item-id");
		const lineItem = this.proposalLinesByGroup.flatMap(item => item.lineItems).find(line => line.id === id) || null;
		const categoryId = $(a).parents("tbody").data('category-id');
		const category = this.proposalLinesByGroup.find(c => c.id == categoryId);

		if (lineItem) {
			const name = lineItem.name == '' ? 'this new line item' : lineItem.name;
			this.swal.confirm(`Would you like to remove ${name}?`,
				() => {

					const index = category.lineItems.indexOf(lineItem);

					// If the index is valid, remove the lineItem from the array
					if (index !== -1) {
						category.lineItems.splice(index, 1);
					}
					this.#resolveLineItemSequences(category);
					$(`tr.line-item[data-id='${id}']`).remove();
					this.lineItemsUniqueNamesValidation = this.lineItemsUniqueNamesValidation.filter(item => item.id !== id);
					this.#calculateCategoryTotals(categoryId);
				}
			);
		}
		else {
			$(`tr.line-item[data-id='${id}']`).remove();
			this.#resolveLineItemSequences(category);
		}
	}

	onRemoveCategory(b) {
		const id = $(b).attr("data-item-id");
		const message = `Are you sure you want to remove this category including all its line items?`;
		this.swal.confirm(
			message,
			() => {
				this.proposalLinesByGroup = this.proposalLinesByGroup.filter(c => c.id !== id);
				this.proposalLinesByGroup.forEach((item, index) => {
					item.sequence = index + 1;
				});
				this.#renderProposalLineCategories();
				this.#calculateOverheadTotals();
			}
		);
	}

	onResetCustomer(b) {
		this.clientModel.id = null;
		$(`#txtCustomerName`).val('').trigger('keyup');
		$(`#txtCustomerCompanyName`).val('').trigger('keyup');
		$(`#txtCustomerAddress`).val('').trigger('keyup');
		$(`#txtCustomerPhone`).val('').trigger('keyup');
		$(`#txtCustomerEmail`).val('').trigger('keyup');
		$(`#txtCustomerCity`).val('').trigger('keyup');
		$(`#slcCustomerState`).val('').trigger('change');
		$("#slcCustomer").val('0').trigger('change');
	}

	onUseClientAddress(b) {
		const clientModel = this.clientModel;
		const projectModel = this.projectModel;
		projectModel.mapAddress(clientModel);

		this.projectForm.updateFormFromModel();
	}

	formatPhone(e) {
		let value = $(e).val().replace(/\D/g, ''); // remove non-numeric characters

		if (value.length > 10) value = value.slice(0, 10); // limit to 10 digits

		let formatted = '';
		if (value.length > 0) formatted += '(' + value.slice(0, 3);
		if (value.length >= 4) formatted += ') ' + value.slice(3, 6);
		if (value.length >= 7) formatted += '-' + value.slice(6);

		$(e).val(formatted);
	}

	updateProposalIncludeZeroAmount() {
		const isChecked = $("#include-zero-amount").is(":checked");
		this.proposal.includeLinesWithZeroAmount = isChecked;
		this.service.updateProposalIncludeZeroAmount(this.proposalId);
	}

	#setAddCategoryFormValidator() {
		return new FormValidator(this.addCategoryForm, {
			'categoryName': {
				validators: {
					notEmpty: {
						message: 'Category Name is required'
					}
				}
			}
		}).init();
	}

	#setNewEstimateCategoryId(input, item, currentItemId) {
		const name = item.name;
		const estimateCategoryId = item.id;
		//check from estimate categories by name
		var existingEstimateCategory = this.estimateCategories.find(line => line.id == estimateCategoryId);

		const lineItem = this.proposalLinesByGroup.flatMap(item => item.lineItems).find(line => line.id == currentItemId);
		lineItem.name = name;
		lineItem.estimateCategoryId = existingEstimateCategory ? existingEstimateCategory.id : Guid.new();

		// update table row data
		const itemrow = $(input).parents('tr');
		const inputs = $(itemrow).find('input');
		const textarea = $(itemrow).find('textarea');

		const isSpecialCase = ['ON SITE SUPERVISION', 'GENERAL CONTRACTOR FEE', 'ON-SITE SUPERVISION'].includes(lineItem.name.trim().toUpperCase());
		const isGeneralContractorFee = lineItem.name.trim().toUpperCase() == 'GENERAL CONTRACTOR FEE';
		const rate = isGeneralContractorFee ? 10 : 5;
		itemrow.attr("data-id", lineItem.id);

		const percentageInput = itemrow.find('input.percentage');
		percentageInput.val(rate).parent().toggleClass('d-none', !isSpecialCase);

		if (isSpecialCase) {
			const percentageAmountInput = itemrow.find('input.overhead-field');
			percentageAmountInput.removeClass('overhead').addClass('percentage-amount');
			percentageAmountInput.attr("evt-input", "onPercentageAmountChange");

			var event = new Event('input', {
				bubbles: true,
				cancelable: true
			});

			// Dispatch the event
			percentageInput.get(0).dispatchEvent(event);
		}

		for (let i = 0; i < inputs.length; i++) {
			$(inputs[i]).attr("data-item-id", lineItem.id);
		}

		for (let i = 0; i < textarea.length; i++) {
			$(textarea[i]).attr("data-item-id", lineItem.id);
		}

	}

	#setContractFormValidator() {
		return new FormValidator(this.sendContractForm, {
			'clientName': {
				validators: {
					notEmpty: {
						message: 'Client Name is required'
					}
				}
			},
			'clientEmail': {
				validators: {
					notEmpty: {
						message: 'Client Email is required'
					}
				}
			},
			'clientAddress': {
				validators: {
					notEmpty: {
						message: 'Address is required'
					}
				}
			},
			'clientCity': {
				validators: {
					notEmpty: {
						message: 'City is required'
					}
				}
			},
			'clientState': {
				validators: {
					notEmpty: {
						message: 'State is required'
					}
				}
			},
			'estStartDate': {
				validators: {
					notEmpty: {
						message: 'Estimated Start Date is required'
					}
				}
			},
			'estCompletionDate': {
				validators: {
					notEmpty: {
						message: 'Estimated Completion Date is required'
					}
				}
			},
			'initialDeposit': {
				validators: {
					notEmpty: {
						message: 'Initial Deposit is required'
					}
				}
			},
			'contractorEmail': {
				validators: {
					notEmpty: {
						message: 'Contractor Email Address is required'
					}
				}
			}
		}, 'input-group').init();
	}

	#setTemplateFormValidator() {
		return new FormValidator(this.templateForm, {
			'name': {
				validators: {
					notEmpty: {
						message: 'Please enter template name'
					}
				}
			},
		}, 'input-group').init();
	}

	#changepercentage(item) {
		const id = $(item).attr("data-item-id");
		const amountCtrl = $(`input.percentage-amount[data-item-id='${id}']`);
		const percentage = parseFloat($(item).val() == '' ? 0 : $(item).val());
		let amount = this.totalNonOverhead * (percentage / 100);
		amount = parseFloat(amount.toFixed(2));
		amountCtrl.val(amount).trigger('keyup');

		var itemInGroup = this.proposalLinesByGroup.flatMap(items => items.lineItems).find(line => line.id === id) || null;
		itemInGroup.percentage = percentage;
		itemInGroup.amount = amount;
	}

	#calculateCategoryTotals(categoryId) {
		const instance = this;
		var category = instance.proposalLinesByGroup.find(c => c.id == categoryId);
		var totalCategoryAmount = category.lineItems.reduce((total, line) => total + line.amount, 0);


		$(".amount-" + categoryId).html(StringUtils.formatMoney(totalCategoryAmount));

		const totalNonOverheadAmount = instance.proposalLinesByGroup
			.filter(item => item.name !== 'Overhead') // Exclude 'Overhead'
			.reduce((total, item) => {
				const proposalTotal = item.lineItems.reduce((sum, line) => sum + line.amount, 0);
				return total + proposalTotal;
			}, 0);

		// total non overhead
		instance.totalNonOverhead = parseFloat(totalNonOverheadAmount);

		//total project amount
		instance.totalProjectAmount = instance.proposalLinesByGroup.reduce((total, item) => {
			// Sum the amounts within each ProposalLines array
			const proposalTotal = item.lineItems.reduce((sum, line) => sum + line.amount, 0);
			return total + proposalTotal;
		}, 0);

		//$(".total-non-overhead-amount").html(instance.totalNonOverhead.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
		$(".total-non-overhead-amount").html(StringUtils.formatMoney(instance.totalNonOverhead));
		if (!this.isinitialload) {
			instance.#calculateFees();
		}

		instance.#calculateOverheadTotals();
	}

	#calculateOverheadTotals() {
		const instance = this;
		if (this.totalNonOverhead > 0) {
			$('div.input-group:not(.d-none) input.percentage').each((index, item) => {
				instance.#changepercentage(item);
			});
		}
		else {
			$("input.percentage-amount").val(0);
		}

		this.#calculateOverheadAndProjectTotals();
	}

	#calculateOverheadAndProjectTotals() {

		this.totalOverhead = 0;
		const overheadItems = $("input.overhead-field");

		overheadItems.map((index, item) => {
			const val = $(item).val() == '' ? '0' : $(item).val();
			this.totalOverhead += parseFloat(val.replace(/,/g, ''));
		});

		$('span.total-overhead').html(StringUtils.formatMoney(this.totalOverhead));

		//calculate the total project amount
		this.#calculateProjectTotals();
	}

	#calculateProjectTotals() {
		this.totalProjectAmount = this.totalOverhead + this.totalNonOverhead;
		$(".amount-project-totals").html(StringUtils.formatMoney(this.totalProjectAmount));
	}

	#calculateFees() {
		$("input.percentage").each((index, obj) => {
			this.#changepercentage(obj);
		});
	}

	#findTemplate(id) {
		return this.templates.find(t => t.id == id);
	}

	#renderCategories() {
		const categories = this.proposalLinesByGroup;
		const nonOverheadCategories = categories.filter(c => c.name.toUpperCase() != 'OVERHEAD');
		const overheadCategories = categories.filter(c => c.name.toUpperCase() == 'OVERHEAD');

		nonOverheadCategories.forEach((category, index) => {
			category.totalAmount = nonOverheadCategories[index].lineItems.reduce((cat, li) => cat + li.amount, 0);
		});

		this.totalNonOverhead = nonOverheadCategories.reduce((acc, category) => acc + category.totalAmount, 0);
		this.totalOverhead = overheadCategories.reduce((acc, category) => acc + category.totalAmount, 0);
		this.totalProjectAmount = this.totalNonOverhead + this.totalOverhead;

		const estimateGroup = new EstimateCategoryGroupRenderer();
		return this.proposalLinesByGroup.map((category, index, categories) =>
			`${estimateGroup.renderGroup(category, index, categories, this.proposal.project?.sqFeet)}`)
			.join('');

	}

	#renderState(selector, state, onChangeCallback) {
		// Set the initial value and trigger the change event
		this.isInitializing = true;
		$(selector)
			.val(state)
			.trigger('change');

		// Handle the 'change' event to update the state
		$(selector).on('change', function () {
			const selectedValue = $(this).val();
			if (typeof onChangeCallback === 'function') {
				onChangeCallback(selectedValue);
			}
		});

		// Handle the 'select2:open' event to focus on the search box
		$(selector).on('select2:open', function () {
			$('.select2-search__field').focus();
		});
		this.isInitializing = false;
	}

	#renderStates() {
		const instance = this;
		const clientModel = instance.clientModel;
		const projectModel = instance.projectModel;

		this.#renderState(
			'#slcCustomerState',
			clientModel.state,
			(selectedValue) => {
				clientModel.state = selectedValue;
			}
		);

		this.#renderState(
			'#slcProjectState',
			projectModel.state,
			(selectedValue) => {
				projectModel.state = selectedValue;
			}
		);
	}

	#checkHasProjectSchedule() {
		if (!this.proposal.hasProjectSchedule) {
			$(".sendContract")
				.addClass("disabled")

			$(".sendContract-wrapper")
				.attr({
					"data-bs-toggle": "tooltip",
					"data-bs-placement": "top",
					"data-bs-original-title": "You must create a project schedule before sending this contract."
				})
				.removeAttr("title") // Remove native title to avoid duplicate tooltip
				.tooltip(); // Initialize Bootstrap tooltip
		} else {
			$(".sendContract")
				.removeClass("disabled")

			$(".sendContract-wrapper")
				.removeAttr("data-bs-toggle data-bs-placement data-bs-original-title")
				.tooltip("dispose"); // Dispose of the tooltip
		}
	}

	#renderProposalDetails() {

		if (!this.proposal) return;

		const permissions = Auth.permissions();
		$(".proposal-details").removeClass('loading').addClass('loaded');

		const proposal = this.proposal;
		const project = proposal.project;
		this.prevStatusId = proposal.status;
		const isNewProposal = this.proposal.id == Guid.empty;
		const isAccepted = proposal.status === Enums.ProposalStatus.Accepted;
		const isDraft = proposal.status === Enums.ProposalStatus.Draft;
		const canEditProposals = permissions.CanEditAcceptedProposals;

		this.#checkHasProjectSchedule();

		$("#txtProposalDate").val(DateUtils.formatDate(proposal.date));
		$("#txtProposalNumber").val(proposal.number);
		$("#slcProposalStatus").val(proposal.status);

		const proposalName = isNewProposal ? 'New Proposal' : project.name;
		$("#lblProjectTitle").html(` - ${proposalName}`);

		if (isAccepted || !isNewProposal) {
			$("#toggleProjectClientDetails").trigger("click");
		}

		if (isDraft) {
			$('a[evt-click="createSchedule"]').parent().remove();
		}

		if (isNewProposal || (isAccepted && !canEditProposals)) {
			$("#slcProposalStatus").prop("disabled", true);
		}
		else {
			if (!isNewProposal && canEditProposals) {
				$("#slcProposalStatus").prop("disabled", false);
			}
		}

		if (isNewProposal || (isAccepted && !canEditProposals) || isDraft)
			$(`button[evt-click="onEditProposal"]`).hide();

		$("button.kt-menu-button").toggleClass('d-none', isNewProposal);
	}

	#renderProposalLineCategories() {
		this.isInitializing = true;
		const categories = this.#renderCategories();
		$("#dv-proposal-categories").html(categories);
		$("#dv-tab-line-items").removeClass('loading').addClass('loaded');

		TextboxUtils.init();

		$('html').on('input', '.textarea-dynamic', (a) => {
			const textarea = a.target;
			textarea.style.height = '37px'; // Reset height to initial value
			textarea.style.height = textarea.scrollHeight + 'px'; // Adjust height based on content
		});

		$(".textarea-dynamic").trigger('input');

		setTimeout(() => { this.#handleLockedStatus(); }, 500);
	}

	#handleLockedStatus() {
		const selectedStatus = parseInt($("#slcProposalStatus").val());
		const canmodifyForm = selectedStatus != Enums.ProposalStatus.Accepted;
		const isAccepted = selectedStatus == Enums.ProposalStatus.Accepted;
		const isEditMode = this.editMode;

		this.#toggleFormState(canmodifyForm || isEditMode);
		const permissions = Auth.permissions();

		if ((!permissions.CanEditAcceptedProposals && !canmodifyForm) || (isAccepted && !isEditMode) ) {
			$("#slcProposalStatus").prop("disabled", true);
		}

		this.#checkHasProjectSchedule();
	}

	#toggleFormState(canModifyForm) {

		const disabled = !canModifyForm;

		$("#frmClientDetails").find("input").prop("disabled", disabled);
		$("#frmClientDetails").find("select").prop("disabled", disabled);
		$("#frmClientDetails").find("textarea").prop("disabled", disabled);
		$("#frmClientDetails").find("button").prop("disabled", disabled);
		$("#frmProjectDetails").find("input").prop("disabled", disabled);
		$("#frmProjectDetails").find("select").prop("disabled", disabled);
		$("#frmProjectDetails").find("textarea").prop("disabled", disabled);
		$("#frmProjectDetails").find("button").prop("disabled", disabled);

		$(".proposal-form input,.proposal-form input select").prop("disabled", disabled);
		$(".clientProject-form input, .clientProject-form input select").prop("disabled", disabled);
		$(".proposal-form button:not(.always-enabled)").prop("disabled", disabled);
		$(`button[evt-click="saveProposal"]`).prop("disabled", disabled);
		$(`button[evt-click="onCancelBtn"]`).prop("disabled", disabled);
		$("#btnAddNewCategory").prop("disabled", disabled);
		$('#slcProjectState').prop('disabled', disabled);
		$('#slcCustomerState').prop('disabled', disabled);
		$(".textarea-dynamic").prop('disabled', disabled);
		$('#slcCustomer').prop('disabled', disabled);
		$('#slcSupervisors').prop('disabled', disabled);
		$(".proposal-form a.btn-add-item").prop('disabled', disabled);
		$(".proposal-form a.remove-item").removeClass('btn-light-danger').prop('disabled', true);

		if (disabled) {
			$('.remove-item').addClass('disabled');
			$('.btn-add-item').addClass('disabled');
			$(".proposal-form a.remove-item").removeClass('btn-light-danger').addClass('btn-light-secondary').prop('disabled', true);
			$(".proposal-form a.btn-add-item")
				.removeClass('btn-outline-primary')
				.removeClass('btn-active-light-primary')
				.addClass('btn-outline-secondary')
				.addClass('btn-active-light-secondary')
				.prop('disabled', true);
			$('div.proposal-details .select2-selection').addClass('disabled-select-bg');
		}
		else {
			$('.remove-item').removeClass('disabled');
			$('.btn-add-item').removeClass('disabled');
			$(".proposal-form a.remove-item").addClass('btn-light-danger').removeClass('btn-light-secondary').prop('disabled', false);
			$(".proposal-form a.btn-add-item")
				.addClass('btn-outline-primary')
				.addClass('btn-active-light-primary')
				.removeClass('btn-outline-secondary')
				.removeClass('btn-active-light-secondary')
				.prop('disabled', false);
			$('div.proposal-details .select2-selection').removeClass('disabled-select-bg');
		}
	}

	#buildStickyActions() {
		window.addEventListener('scroll', function () {
			const scrollY = window.scrollY || window.pageYOffset;
			const totalHeight = document.documentElement.scrollHeight - window.innerHeight;
			const scrollPercent = (scrollY / totalHeight) * 100;
			const actionBar = document.getElementById('scroll-action-bar');

			if (scrollPercent >= 7) {
				actionBar.style.bottom = '20px'; // slide up into view
			} else {
				actionBar.style.bottom = '-100px'; // slide down out of view
			}
		});
	}

	#initEstimateCategoryDropdown(elem) {
		debugger;
		const parentCategoryId = $(elem).attr("data-parent-category-id");
		const options = [];
		const uniqueNames = [];
		const instance = this;
		const addedLineItems = this.proposalLinesByGroup.find(cat => cat.id == parentCategoryId).lineItems;
		const addedLineItemNames = addedLineItems.map(item => item.name.toLowerCase());
		const estimateCategories = this.estimateCategories
			.filter(c => c.parentEstimateCategoryId != null && !addedLineItemNames.includes(c.name.toLowerCase()));

		//const estimateCategories = this.estimateCategories.filter(c => c.parentEstimateCategoryId != null);

		estimateCategories.forEach(ec => {
			if (!uniqueNames.includes(ec.name.toLowerCase())) {
				options.push({ id: ec.id, name: ec.name });
				uniqueNames.push(ec.name.toLowerCase());
			}
		});

		const option = `<option value="">&nbsp;</option>`;
		$(elem).append(option);

		options.forEach(ec => {
			const option = `<option value="${ec.id}">${ec.name}</option>`;
			$(elem).append(option);
		});

		$(elem).select2({
			placeholder: 'Choose an option',
			allowClear: true
		});

		$(elem).on('select2:select', function (e) {

			const selectedItem = {
				id: e.params.data.id,
				name: e.params.data.text
			};
			const currentItemId = elem.attr("data-item-id");
			const categoryId = elem.parents('tr').parents('tbody').data("category-id");
			const currentCategory = instance.proposalLinesByGroup.find(line => line.id === categoryId);
			const categoryName = currentCategory.name;
			const lineItem = currentCategory.lineItems.find(line => line.name.trim().toLowerCase() === selectedItem.name.trim().toLowerCase());

			if (lineItem != null) {
				elem.addClass('error');
				instance.swal.error(`Duplicate line item found on ${categoryName}! Please select a different line item.`);
				instance.lineItemsUniqueNamesValidation.push({ id: currentItemId, desc: `${categoryName} has duplicate line items.` });
			}
			else {
				elem.removeClass('error');
				const tempLineItem = instance.tempLineItems.find(li => li.id == currentItemId);
				if (tempLineItem) {
					currentCategory.lineItems.push(tempLineItem);
					instance.tempLineItems = instance.tempLineItems.filter(li => li.id != currentItemId);
				}

				instance.lineItemsUniqueNamesValidation = instance.lineItemsUniqueNamesValidation.filter(li => li.id != currentItemId);
				instance.#setNewEstimateCategoryId(elem, selectedItem, currentItemId);
			}
		});

		$(elem).on('select2:unselect', function (e) {
			const currentItemId = elem.attr("data-item-id");
			instance.lineItemsUniqueNamesValidation = instance.lineItemsUniqueNamesValidation.filter(li => li.id != currentItemId);
		});



		//      const acOptions = {
		//          element: elem,
		//	onselect: (item) => { instance.#onSelectAutoCompleteItem(item, elem); },
		//          options: options
		//      }
		//      const ac = new AutoComplete(acOptions);
		//      ac.init();
	}

	#initEmailSender() {
		this.emailSender.type = Enums.ProposalReport;
		this.emailSender.refId = this.proposalId;
		this.emailSender.clientName = this.clientModel.firstName;
		this.emailSender.senderName = Auth.currentUser().fullName;
		this.emailSender.companyName = 'CH Anderson Construction';
	}

	#initSupervisorControl() {

		const instance = this;
		const supervisors = this.supervisors;
		const sd = new SearchableDropdown();
		sd.data = supervisors;
		sd.iconText = (o) => { return o.initials }
		sd.optionText = (o) => { return `${o.firstName} ${o.lastName}` }
		sd.optionSelected = (o) => { return instance.selectedSupervisors.includes(o.id) }
		sd.onSelect = () => {
			$(".select-supervisor span.select2-container").removeClass('invalid');
			this.hasChanges = true;
		}
		sd.width = '1%';
		sd.init("#slcSupervisors");
		this.supervisorsDropdown = sd;

	}

	#initTabControl() {

		this.tabControl.actions = [
			{
				tabid: 'tab-proposal-lines',
				callback: () => {
					$("#addCategoryDiv").removeClass("d-none");
				}
			},
			{
				tabid: 'tab-project-details',
				callback: () => {
					$("#addCategoryDiv").addClass("d-none");
				}
			},
			{
				tabid: 'tab-client-details',
				callback: () => {
					$("#addCategoryDiv").addClass("d-none");
				}
			}
		];
	}

	#initCustomerControl(selector) {
		const instance = this;

		$(selector).select2({
			dropdownPosition: 'below',
			ajax: {
				url: '/api/customers',
				data: params => ({ key: params.term }),
				processResults: response => {
					instance.customers = response.data;
					return {
						results: response.data.map(item => ({
							id: item.id,
							text: item.name
						}))
					};
				}
			}
		});

		$(selector).on('select2:select', function (e) {
			const customer = e.params.data;
			const selectedCustomer = instance.customers.find(item => item.id === customer.id);

			if (selectedCustomer) {
				instance.clientModel.id = selectedCustomer.id;
				$(`#txtCustomerName`).val(`${selectedCustomer.name}`).trigger('keyup');
				$(`#txtCustomerCompanyName`).val(selectedCustomer.companyName).trigger('keyup');
				$(`#txtCustomerAddress`).val(selectedCustomer.address).trigger('keyup');
				$(`#txtCustomerPhone`).val(selectedCustomer.phone).trigger('keyup');
				$(`#txtCustomerEmail`).val(selectedCustomer.emailAddress).trigger('keyup');
				$(`#txtCustomerCity`).val(selectedCustomer.city).trigger('keyup');
				$(`#slcCustomerState`).val(selectedCustomer.state).trigger('change');
			}
		});
	}

	#populateTemplates() {
		const templateId = this.template ? this.template.id : Guid.empty;
		var templateOptions = `<option value="${Guid.empty}">&nbsp;</option>`;
		templateOptions += this.templates.map(template => `<option value="${template.id}">${template.name}</option>`).join('');

		$("#slcTemplates").html(templateOptions);
		$("#slcTemplates").val(templateId);

		if (this.proposalId != Guid.empty) {
			$("#slcTemplates").attr("disabled", true);
		}
	}

	#loadSupervisorUsers() {
		this.service.loadSupervisorUsers()
			.then((supervisors) => {
				this.supervisors = supervisors;
				this.#initSupervisorControl();
			});
	}

	#loadProposalTemplates() {
		this.service.loadTemplates()
			.then((templates) => {
				this.templates = templates;
				this.#populateTemplates();
			});
	}

	#resetSquareFeetAndMultiplier(id) {
		$(`input.multiplier-amount[data-item-id="${id}"]`).val('');
		$(`input.squarefoot[data-item-id="${id}"]`).val('');
	}

	#getEstimateCategories() {
		this.service.getEstimateCategories()
			.then((estimateCategories) => {
				this.estimateCategories = estimateCategories;
				this.parentEstimateCategories = this.estimateCategories.filter(ec => ec.parentEstimateCategoryId === null);
			});
	}

	#loadProposal() {
		const id = this.proposalId;
		this.service.loadProposal(id)
			.then((proposal) => {
				this.proposal = proposal;
				this.template = this.proposal.template;
				this.#initProposalData(proposal);
				this.isInitializing = false;
			});
	}

	#resolveLineItemSequences(category) {
		category.lineItems.forEach((item, index) => {
			item.sequence = index + 1;
			let $input = $(`input[data-model='sequence'][data-item-id='${item.id}']`);
			$input.val(item.sequence);
		});
	}

	#createProposalPayload() {
	
		var proposal = new ProposalModel();
		this.selectedMergeProjectId = null;
		this.selectedOverwriteProposalId = null;

		this.projectModel.sqFeet = this.projectModel.sqFeet == '' ? null : parseFloat(this.projectModel.sqFeet);

		proposal.client = this.clientModel;
		proposal.project = this.projectModel;
		proposal.template = { ...this.template };
		proposal.template.categories = this.proposalLinesByGroup;
		proposal.supervisors = this.supervisorsDropdown.getSelectedValues();
		
		proposal.id = this.proposalId;
		proposal.qbClassId = this.proposal.qbClassId;
		proposal.date = $("#txtProposalDate").val();
		proposal.number = parseInt($("#txtProposalNumber").val());
		proposal.status = parseInt($("#slcProposalStatus").val());

		this.selectedSupervisors = proposal.supervisors;
		return proposal;
	}

	saveProposal(button = null, proceedCallback = null) {
		const validateLineItems = () => {
			const inputs = $('input.line-item-name');
			let errors = '';
			inputs.each(function () {
				const value = $(this).val();
				if (value === '') {
					$(this).addClass('invalid');
					$(this).prevAll('h3').first().html();
					errors += `${$(this).parents('.card').find('h3').eq(0).text()}: Item Name is required<br/>`
				}
				else {
					$(this).removeClass('invalid');
				}
			});

			this.lineItemsUniqueNamesValidation.map(item => {
				errors += `${item.desc}<br/>`
			});

			return errors;
		}

		const validateSupervisors = () => {
			const hasValidSupervisors = this.supervisorsDropdown.getSelectedValues().length > 0;
			if (!hasValidSupervisors) {
				$(".select-supervisor span.select2-container").addClass('invalid');
			}
			return hasValidSupervisors;
		}
		
		$("#txtProposalProjectName").removeClass("error");
		const lineItemsError = validateLineItems();
		const hasValidSupervisors = validateSupervisors();
		const isValidProjectName = (this.projectModel.name != null && this.projectModel.name.trim() !== '');
		const hasSelectedTemplate = $("#slcTemplates").val() != Guid.empty;
		const hasClientName = this.clientModel.name != null && this.clientModel.name != '';
		const hasClientEmailAddress = this.clientModel.emailAddress != null && this.clientModel.emailAddress != '';
		const isValidEmail = !this.clientModel.emailAddress || VALIDATIONS.isValidEmail(this.clientModel.emailAddress.trim());
		const isValidPhone = !this.clientModel.phone || VALIDATIONS.isValidUSPhoneNumber(this.clientModel.phone.trim());
		const isValidForm = lineItemsError === '' && hasValidSupervisors && isValidProjectName && hasSelectedTemplate
			&& hasClientName && hasClientEmailAddress && isValidEmail && isValidPhone;


		if (isValidForm) {
			this.#submitProposalChanges(button, proceedCallback);
		} else {
			const errors = [
				lineItemsError,
				!hasValidSupervisors && `At least one (1) supervisor is required`,
				!isValidProjectName && `Project Name is required<br/>`,
				!hasSelectedTemplate && `Please select a proposal template<br/>`,
				!hasClientName && `Client Name is required<br/>`,
				!hasClientEmailAddress && `Client Email Address is required<br/>`,
				!isValidEmail && `Client Email Address is invalid<br/>`,
				!isValidPhone && `Phone Number is invalid<br/>`,
			].filter(Boolean).join(' ');

			this.swal.error("Please correct all errors below.", errors);

			if (!isValidProjectName) {
				const toggleProjectDetailsButton = $("#toggleProjectClientDetails");
				if (toggleProjectDetailsButton.hasClass("collapsed")) {
					toggleProjectDetailsButton.trigger("click");
					$("#txtProposalProjectName").addClass("error");
				}
			}

			SCROLLTOTOP();
		}
	}

	#submitProposalChanges(button, proceedCallback = null) {
		this.payload = this.#createProposalPayload();

		if (button) {
			button.setAttribute('data-kt-indicator', 'on');
			button.disabled = true;
		}

		this.httpService.post('/api/proposals/validate-client-email', this.clientModel)
			.then((isValid) => {
				debugger;
				if (isValid) {
					this.service.saveProposal(this.payload)
						.then((response) => {
							//this.proposalId = data.id;
							this.matchingProjects = response.matchingProjects;
							this.matchingDraftProposals = response.matchingDraftProposals;
							if (this.matchingProjects && this.matchingProjects.length > 0) {
								this.#showProjectMatchModal();
							}
							else if (this.matchingDraftProposals && this.matchingDraftProposals.length > 0) {
								this.#showExistingDraftProposalsMatchModal();
							}
							else {
								this.proposal = response.proposal;
								this.proposalId = this.proposal.id;
								this.#initProposalData(this.proposal);
								this.swal.alert('Proposal has been saved successfully.', () => {
									if (typeof proceedCallback === 'function') {
										proceedCallback();
									}
								});
							}
						})
						.finally(() => {
							if (button) {
								button.removeAttribute('data-kt-indicator');
								button.disabled = false;
							}
							this.hasChanges = false;
						});
				}
				else {
					this.swal.error('Unable to save proposal. A client with the same email address already exists.');
				}
			})
			.finally(() => {
				button.removeAttribute('data-kt-indicator');
				button.disabled = false;
			});

		
	}

	#showProjectMatchModal() {
		const matchingProjects = this.matchingProjects;
		this.mergeProposalModal = new bootstrap.Modal(document.getElementById('mdl-merge-proposal'), {
			keyboard: false,
			backdrop: 'static'
		});

		const checked = matchingProjects.length == 1 ? 'checked' : '';

		const matchingProjectsRow = matchingProjects.map(p => {
			if (checked) {
				this.selectedMergeProjectId = p.id;
			}

			return `<tr>
                        <td>${p.name}</td>
                        <td>
                            <div class="form-check form-check-custom form-check-solid">
                                <input class="form-check-input border-primary-1" evt-change="onMergeProjectSelect" ${checked} type="radio" value="${p.id}" name="mergeProject" />
                            </div>
                        </td>
                    </tr>`;
		}).join('');
		
		$("button.btn-merge-action").prop('disabled', !checked);
		$("#tbody-matching-projects").html(matchingProjectsRow);
		this.mergeProposalModal.show();
	}

	#showExistingDraftProposalsMatchModal() {
		const matchingDraftProposals = this.matchingDraftProposals;
		this.overwriteProposalModal = new bootstrap.Modal(document.getElementById('mdl-overwrite-proposal'), {
			keyboard: false,
			backdrop: 'static'
		});

		const checked = matchingDraftProposals.length == 1 ? 'checked' : '';

		const matchingProjectsRow = matchingDraftProposals.map(p => {
			if (checked) {
				this.selectedOverwriteProposalId = p.id;
			}

			return `<tr>
                        <td>${p.name}</td>
                        <td>
                            <div class="form-check form-check-custom form-check-solid">
                                <input class="form-check-input border-primary-1" evt-change="onOverwriteProposalSelect" ${checked} type="radio" value="${p.id}" name="overwriteProposal" />
                            </div>
                        </td>
                    </tr>`;
		}).join('');

		$("button.btn-overwrite-action").prop('disabled', !checked);
		$("#tbody-matching-proposals").html(matchingProjectsRow);
		this.overwriteProposalModal.show();
	}

	#setCategorySequence(category, sequence) {
		const targetItem = category
		if (!targetItem) return;

		// 💥 Remove the target item first
		let items = this.proposalLinesByGroup.filter(i => i.id !== category.id);

		// 🔄 Shift sequences if needed
		items.forEach(i => {
			if (i.sequence >= sequence) {
				i.sequence += 1;
			}
		});

		// 🧩 Reinsert the updated item
		targetItem.sequence = sequence;
		items.push(targetItem);

		this.proposalLinesByGroup = items;

		this.proposalLinesByGroup.sort((a, b) => a.sequence - b.sequence);
		this.#refreshSequence();
	}

    #setCategorySequenceOnAdd(category, sequence) {
        var nextSiblings = this.proposalLinesByGroup.filter(c => c.sequence >= sequence && c.name !== 'Overhead');
        nextSiblings.forEach(c => c.sequence = c.sequence + 1);

		var overheadCategory = this.proposalLinesByGroup.find(c => c.name == 'Overhead');
		overheadCategory.sequence = this.proposalLinesByGroup.length;
		category.sequence = sequence >= overheadCategory.sequence ? overheadCategory.sequence - 1 : sequence;

        this.proposalLinesByGroup.sort((a, b) => a.sequence - b.sequence);
        this.#refreshSequence();
    }

	#refreshSequence() {
		this.proposalLinesByGroup.forEach((item, index) => {
			item.sequence = index + 1;
		});
	}

	#submitTemplate(button, overwrite = false) {
	
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;

		const currentTemplate = this.template;
		const templateModel = this.templateModel;

		templateModel.lineItems = this.proposalLinesByGroup;
		var templateId = overwrite ? currentTemplate.id : '';
		templateModel.id = overwrite ? currentTemplate.id : Guid.empty;
	
		this.service.saveProposalTemplate(templateModel, templateId)
			.then((data) => {

				if (overwrite) {
					this.templates = this.templates.filter(template => template.id !== data.id);
				}
				this.template = data;
				this.templates.push(data);
				this.#populateTemplates();
				this.saveTemplateDrawer.toggle();
				this.templateForm.reset();
				this.swal.alert(`${templateModel.name} has been saved successfully!`, () => {
				});
			})
			.finally(() => {
				button.removeAttribute('data-kt-indicator');
				button.disabled = false;
			});
	}

	#scrollToElement(value) {
		const element = document.querySelector(`div.category-row[data-id="${value}"]`);
		if (element) {
			const elementPosition = element.getBoundingClientRect().top + window.scrollY;
			const offsetPosition = elementPosition - 100;

			window.scrollTo({
				top: offsetPosition,
				behavior: 'smooth'
			});

			element.classList.add('highlight-on-scroll');

			// Remove highlight after 3 seconds
			setTimeout(() => {
				element.classList.remove('highlight-on-scroll');
			}, 3000);

		} else {
			console.log("Element not found with data-id:", value);
		}
	}

	#createLineItem(category, newSequence) {
		const lineItemId = Guid.new();
		return {
			id: lineItemId,
			proposalTemplateId: this.template.id,
			name: "",
			description: "",
			amount: 0,
			sequence: newSequence,
			parentId: category.id,
			estimateCategoryId: lineItemId,
			lineItems: [],
			totalAmount: 0,
			percentage: 0
		};
	}

	#initUi() {
		this.#initTabControl();
		this.#initCustomerControl("#slcCustomer");
		this.#initEmailSender();
		this.#buildStickyActions();

		$("#dv-sub-header").removeClass('loading').addClass('loaded');
		this.#renderProposalDetails();
		this.#renderProposalLineCategories();
	}

	#initProposalData(proposal) {
		const currentUser = Auth.currentUser();
		this.proposal = proposal;
		this.proposalId = proposal.id;

		this.clientModel = new ClientModel();
		this.projectModel = new ProjectModel();

		if (this.proposalId != Guid.empty) {
			this.clientModel.map(this.proposal.client);
			this.projectModel.map(this.proposal.project);
		}

		this.proposalLinesByGroup = this.proposal.template.categories;

		this.proposalLinesByGroup.forEach((item, groupIndex) => {
			item.sequence = groupIndex + 1; // Set sequence to index + 1
			item.lineItems.forEach((lineItem, index) => {
				lineItem.sequence = index + 1; // Set sequence to index + 1
			});
		});

		this.selectedSupervisors = [...new Set([...this.selectedSupervisors, ...this.proposal.supervisors])];

		const templateId = this.template ? this.template.id : Guid.empty;
		$("#slcTemplates").val(templateId);

		if (this.proposalId == Guid.empty) {
			if (permissions.CanManageOwnEstimates || permissions.CanManageAllEstimates) {
				this.selectedSupervisors.push(currentUser.id);
			}
			$("#toggleDetails").trigger("click");
		}

		this.#initForms();
		this.#renderStates();
		this.#initSupervisorControl();
		this.#initUi();
	}

	#initForms() {
		this.clientForm = new Form();
		this.clientForm.formId = "frmClientDetails";
		this.clientForm.model = this.clientModel;
		this.clientForm.init();

		this.projectForm = new Form();
		this.projectForm.formId = "frmProjectDetails";
		this.projectForm.model = this.projectModel;
		this.projectForm.init();
	}

	#getEmailTemplates() {
		this.httpService.get("/api/email-template")
			.then(response => {
				this.emailTemplates = response;
			});
	}
	
	async init() {
		this.#loadSupervisorUsers();
		this.#loadProposalTemplates();
		this.#getEstimateCategories();
		this.#loadProposal();
		this.#getEmailTemplates();

		$("div.action-menu-items").find(".menu-item").bind("click", function () {
			const th = $(this);
			setTimeout(() => { th.removeClass("hover"); }, 500);
		});
    }
}
class EstimateCategoryGroupRenderer {
	#subTotalRow(categories) {
		const nonOverheadCategories = categories.filter(c => c.name.toUpperCase() != 'OVERHEAD');
		const totalNonOverhead = nonOverheadCategories.reduce((acc, category) => acc + category.totalAmount, 0);
		return `<tr class="bg-light">
					   <td colspan="7" class="subtotal">
						   <h3>Subtotal (Non-Overhead): <span class="total-non-overhead-amount">${StringUtils.formatMoney(totalNonOverhead) }</span></h3>
					   </td>
				   </tr>`;
	}
	#projectTotalsRow(categories) {

		const projectTotals = categories.reduce((acc, category) => acc + category.totalAmount, 0);
		return `<tr class="bg-light">
						<td colspan="7" class="subtotal">
							<h2>Project Totals: <span class="amount-project-totals">${StringUtils.formatMoney(projectTotals) }</span></h2>
						</td>
					</tr>`;
	}
	#totalCategoryRow(category) {
		return `
					<tr class="bg-light">
						<td colspan="7" class="subtotal">
							<h3>Total ${category.name}: <span class="amount-${category.id} total-${category.name.toLowerCase()}">${StringUtils.formatMoney(category.totalAmount) }</span></h3>
						</td>
					</tr>
				`;

	}
	#addNewItemRow (category) {
		const buttonClass = 'btn btn-sm btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary w-100 btn-add-item';
		return `
					<tr class="row-new-item">
						<td colspan="7">
							<a href="javascript:" class="${buttonClass}" evt-click="onAddLineItem" data-category-id='${category.id}'>+ Add Line Item</a>
						</td>
					</tr>`
	}
	#headerRow (category) {
		let row = `<tr class="fw-bolder fs-6 text-gray-800 bg-light">
							<th class="w-75px text-center">#</th>
							<th class="w-200px">Item</th>
							<th class="">Description</th>`;
		if (category.name != 'Overhead') {
			row += `<th class="w-100px text-center">SQ FT</th>
							<th class="w-175px text-center">Multiplier</th>`;
		}
		row += `<th class="w-175px text-center">Amount</th>
							<th class="w-50px text-center"></th>
						</tr>`;
		return row;
	}
	#percentage (id, percentage) {
		return `<div class="input-group input-group-sm ms-2 h-37px">
						<span class="input-group-text" id="inputGroup-sizing-sm">%</span>
						<input type="text" data-type="decimal" class="form-control percentage" evt-input="onPercentageChange" data-item-id="${id}" value="${percentage}">
					</div>`;
	}
	#description(category, lineItem) {

		const baseInput = `<textarea class="form-control textarea-dynamic form-control-sm" 
				data-model="description"
				data-item-id="${lineItem.id}" 
				evt-input="onDescriptionChange"
				style="height:37px; font-size:smaller;">${lineItem.description}</textarea>`;

		const isOverhead = category.name.toUpperCase() === 'OVERHEAD';
		const isSpecialLineItem = ['ON SITE SUPERVISION', 'GENERAL CONTRACTOR FEE'].includes(lineItem.name.trim().toUpperCase());
		
		if (isOverhead && isSpecialLineItem) {
			return `<div class="d-flex">
							${baseInput.replace('class="form-control form-control-sm"', 'class="form-control form-control-sm me-2 fw-70"')}
							${this.#percentage(lineItem.id, lineItem.percentage)}
						</div>`;
		}

		return baseInput;
	}
	#lineItemSequence(lineItem) {
		const baseInput = `<input type="text" class="form-control form-control-sm text-center" 
				data-model="sequence"
				data-type="int"
				data-item-id="${lineItem.id}" 
				evt-input="onLineItemSequenceChange"
				value="${lineItem.sequence}">`;
		return baseInput;
	}
	#amount(category, lineItem)  {
		const isOverhead = category.name.toUpperCase() === 'OVERHEAD';
		const isSpecialCase = ['ON SITE SUPERVISION', 'GENERAL CONTRACTOR FEE'].includes(lineItem.name.trim().toUpperCase());
		const lineItemInputClass = isOverhead ? `${category.name.toLowerCase()} overhead-field` : `line-item-amount ${category.name.toLowerCase()}`;
		const additionalClass = isSpecialCase ? 'percentage-amount overhead-field' : lineItemInputClass;
		const onChangeHandler = isSpecialCase ? 'onPercentageAmountChange' : 'onLineItemAmountChange';

		let formattedAmount = StringUtils.formatMoney(lineItem.amount, false);
		

		return `<input type="text" class="form-control form-control-sm ${additionalClass}" 
						data-type="money"
						evt-input="${onChangeHandler}"
						evt-focus="onAmountFocus"
						data-item-id="${lineItem.id}" 
						value="${formattedAmount}">`;
	}
	#squareFoot(lineItem, projectSqFeet) {
		const id = lineItem.id;
		const value = lineItem.sqFoot ?? projectSqFeet ?? '';
		const isLockedCls = lineItem.sqFootLocked ? 'sqFootLocked' : '';
		return `<div class="input-group input-group-sm h-37px">
						<input type="text" class="form-control form-control-sm squarefoot ${isLockedCls}" evt-input="onSquareFootChange" data-type="decimal" data-item-id="${id}" value="${value}">
					</div>`;
	}
	#multiplier(lineItem) {
		const id = lineItem.id;
		const value = lineItem.multiplier == null ? '' : lineItem.multiplier;
		return `<div class="input-group input-group-sm">
						<span class="input-group-text">$</span>
						<input type="text" class="form-control form-control-sm multiplier-amount" evt-input="onMultiplierAmountChange" data-type="money" data-item-id="${id}" value="${value}">
					</div>`;
	}
	#removeItemLink (id) {
		return `<a href="javascript:" 
						class="btn btn-sm btn-icon btn-light-danger remove-item"
						evt-click="onRemoveLineItem"
						title="Remove Item"
						data-item-id="${id}" >
							<i class="bi bi-trash"></i>
					</a>`;
	}
	#content (innerContent, cssClass) {
		return `<div class="${cssClass}">
						${innerContent}
					</div>`;
	}
	#cell(content, style = '') {
		return `<td style="${style}">${content}</td>`;
	}
	#lineItemsRows(category, projectSqFeet) {
		const lineItemRow = (category, lineItem) => {
			const isOverheadRow = category.name == 'Overhead';
			const dataRowType = isOverheadRow ? 'overhead' : 'non-overhead';
			return `<tr class="line-item" data-id='${lineItem.id}' data-row-type="${dataRowType}">
								${this.#cell(`${this.#lineItemSequence(lineItem)}`, 'text-align:center;')}
								${this.#cell(`<strong>${lineItem.name}</strong>`, 'padding-top:12px !important;')}
								${this.#cell(`${this.#description(category, lineItem)}`)}
								${!isOverheadRow ? this.#cell(`${this.#squareFoot(lineItem, projectSqFeet)}`, `padding:0 !important;padding-right: 4px !important;padding-top:5px !important;`) : ''}
								${!isOverheadRow ? this.#cell(`${this.#multiplier(lineItem, true)}`, `padding:0 !important;padding-top:5px !important;`) : ''}
								${this.#cell(`${this.#content(`<span class="input-group-text">$</span>${this.#amount(category, lineItem)}`, `input-group input-group-sm h-37px`)}`, `padding-left:4px !important;`)}
								${this.#cell(`${this.#content(`${this.#removeItemLink(lineItem.id)}`, `action-buttons`)}`, `text-center`)}
					</tr>`;
		}

		return category.lineItems.map(lineItem => `${lineItemRow(category, lineItem)}`).join('');
	}
	#categoryTable(category, index, categories, projectSqFeet) {
		const isSecondToLast = index === categories.length - 2;
		const isLastRow = index === categories.length - 1;

		return `<table class="table table table-row-dashed table-row-gray-300 proposal-items">
								<thead>
									${this.#headerRow(category)}
								</thead>
								<tbody class="${category.id}" data-category-id="${category.id}">
									${this.#lineItemsRows(category, projectSqFeet)}
									${this.#addNewItemRow(category)}
									${this.#totalCategoryRow(category)}
									${isSecondToLast ? this.#subTotalRow(categories) : ''}
									${isLastRow ? this.#projectTotalsRow(categories) : ''}
								</tbody>
							</table>`;
	}
	renderGroup(category, index, categories, projectSqFeet) {
		var isOverhead = category.name.trim().toUpperCase() == 'OVERHEAD';

		const removeButton = (category) => {
			return `<a href="javascript:" 
								class="btn btn-sm btn-icon btn-light-danger remove-item" 
								evt-click="onRemoveCategory" 
								title="Remove Item" 
								data-item-id="${category.id}"
								style="float:right;">
								<i class="bi bi-trash"></i>
					</a>`;
		}

		const sequenceField = (category) => {
			return `<input type="text"
					class="form-control form-control-sm text-center"
					data-model="sequence"
					data-type="int"
					data-item-id="${category.id}"
					evt-input="onCategorySequenceChange"
					value="${category.sequence}"
					style="float:left; margin-right:10px; width:50px;">`;
		}

		return `
				<div class="row mb-2 g-6 g-xl-9 category-row" data-id="${category.id}">
					<div class="col-lg-12">
						<div class="card card-flush h-lg-100" >
							<!--begin::Card header-->
							<div class="px-10 py-5 pb-0">
								<!--begin::Card title-->
								<div class="card-title flex-column">
									<h3 class="fw-bolder mb-1" style="height:40px; line-height:40px;">
										${ isOverhead ? '' : sequenceField(category) }
										${category.name}
										${ isOverhead ? '' : removeButton(category) }
									</h3>
								</div>

							</div>
							<!--end::Card header-->
							<!--begin::Card body-->
							<div class="card-body pt-0">
								${this.#categoryTable(category, index, categories, projectSqFeet)}
							</div>
							<!--end::Card body-->
						</div>
					</div>
				</div>
			`
	}
}


$(document).ready(() => {
	proposalView = new CreateProposalView();
	proposalView.init();
});