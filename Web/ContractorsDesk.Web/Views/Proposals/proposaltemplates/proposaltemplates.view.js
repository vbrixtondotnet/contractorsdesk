class ProposalTemplateView extends DomEventComponent {
	constructor() {
		super();
		this.proposalTemplateId = document.URL.split('/').pop();
		this.httpService = new httpService();
		this.service = new ProposalTemplateService();
		this.templates = null;
		this.lineItems = [];
		this.lineItemsByGroup = [];
		this.swal = new SwalUtil();
		this.template = null;
		this.addCategoryModal = $("#modalAddCategory").modal();
		this.saveTemplateModal = $("#modalSaveProposalTemplate").modal();
		this.categoryForm = document.getElementById('frmCategory');
		this.proposalTemplateForm = document.getElementById('modalProposalTemplateForm');
		this.categoryFormValidator = this.#setCategoryFormValidator(this.categoryForm);
		this.saveas = false;
		this.popoverTimeout = null;
		this.currentCategory = null;
		this.onsearchitemname = null;
		this.lineItemsUniqueNamesValidation = [];
		this.estimateCategories = [];
		this.parentEstimateCategories = [];
		this.oncheckduplicatetimeout = null;

	}

	#setCategoryFormValidator(form) {
		const fields = {
			'categoryName': {
				validators: {
					notEmpty: {
						message: 'Category Name is required.'
					}
				}
			}
		};

		return new FormValidator(form, fields, 'fv-row').init();
	}

	#scrollToElement(attr, value) {
		const element = document.querySelector(`[${attr}="${value}"]`);
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

	#bindEventHandlers() {
		var instance = this;

		$("html").on("focus", '.line-item-amount', (e) => {
			const currentValue = $(e.currentTarget).val();
			if (currentValue == '0') $(e.currentTarget).val('');
		});

		$("html").on('click', '.btn-add-item', (b) => {
			const button = $(b.currentTarget);
			instance.#insertLineItem(button);
		});

		$("html").on("click", `a.move-item`, (a) => {
			const id = $(a.currentTarget).attr("data-item-id");
			const direction = $(a.currentTarget).attr("data-action");
			instance.#moveItem(id, direction);
		});

		$("html").on("click", `a.remove-item`, (a) => {
			const id = $(a.currentTarget).attr("data-item-id");
			instance.#removeItem(id);
		});
		$("html").on("change", '.line-item-amount', function () {
			var value = $(this).val().replace(',','');
			var itemId = $(this).attr("data-item-id");

			var itemInGroup = instance.lineItemsByGroup.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;
			itemInGroup.amount = parseFloat(value);
		});
		$("html").on("change", '.percentage', function () {
			debugger;
			var value = $(this).val();
			var itemId = $(this).attr("data-item-id");

			var itemInGroup = instance.lineItemsByGroup.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;
			itemInGroup.percentage = parseFloat(value);
		});
		$("html").on("change", '.percentage-amount', function () {
			var value = $(this).val();
			var itemId = $(this).attr("data-item-id");

			var itemInGroup = instance.lineItemsByGroup.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;
			itemInGroup.amount = parseFloat(value.replace(',',''));
		});
		
		$("#btnModalCancel, .dismiss-modal").on('click', () => {
			this.addCategoryForm.reset();
			this.addCategoryModal.modal('hide');
		});

		$("#btnModalTemplateCancel, .dismiss-modal-template").on('click', () => {
			this.closeTemplateModal();
		});

		$("#btnModalSaveTemplate").on("click", (b) => {
			const button = b.currentTarget;
			this.saveProposalTemplate(button);
		})

		$('html').on('input', '.textarea-dynamic', (a) => {
			const textarea = a.target;
			textarea.style.height = '37px';
			textarea.style.height = textarea.scrollHeight + 'px'; // Adjust height based on content
		});
	}

	onChangeDescription(i) {
		const itemId = $(i).attr("data-item-id");
		const value = $(i).val();
		const model = $(i).attr("data-model");
		// Find the item in the lineItemsByGroup
		const itemInGroup = this.lineItemsByGroup
			.flatMap(item => item.lineItems)
			.find(line => line.id === itemId) || null;

		// Update the corresponding property based on data-model value
		if (itemInGroup && model) {
			itemInGroup[model] = value;
		}

		console.log(itemId);
	}

	onLineItemNameChange(i) {
		const input = $(i);
		const name = input.val();
		const id = input.attr("data-item-id");
		const item = { id: id, name: name };
		this.#resetUniqueNamesValidation(input);
		this.#setNewEstimateCategoryId(input, item, id);
	}

	onRemoveCategory(b) {
		const id = $(b).attr("data-category-id");
		let swal = new SwalUtil();
		let categoryId = id;
		swal.confirm('Are you sure you want to remove this category?', () => {
			this.lineItemsByGroup = this.lineItemsByGroup.filter(item => item.id !== categoryId);
			this.renderCategories();
		});
	}

	showCategoryDrawer(b) {
		const button = $(b);
		const id = button.attr("data-category-id");
		$("#drawer-category .drawer-title").html('Add Category');

		if (id) {
			this.currentCategory = this.lineItemsByGroup.find(c => c.id == id);
			$("#modalTxtCategoryName").val(this.currentCategory.name);
			$("#drawer-category .drawer-title").html('Edit Category');
		}

		$("#btnAddCategoryFormToggle").click();
		$(".tooltip").remove();
		this.#populateCategoryList();
	}

	addCategory() {
		var instance = this;
		const insertAfter = (array, targetId, newItem) => {
			// Find the index of the item with the specified id
			const index = array.findIndex(item => item.id === targetId);

			// If the item is found, insert the new item after it
			if (index !== -1) {
				array.splice(index + 1, 0, newItem);
			}
		}

		this.categoryFormValidator.validate().then((status) => {
			if (status === 'Valid') {
				//$(".validators span.category-exist").addClass('d-none');
				$("#modalTxtCategoryName").removeClass('invalid');
				const categoryName = $("#modalTxtCategoryName").val();
				const categories = this.lineItemsByGroup;

				const existingCategory = categories.find(c => c.name.toLowerCase().trim() == categoryName.toLowerCase().trim() && this.currentCategory == null);
				if (existingCategory) {
					//$(".validators span.category-exist").removeClass('d-none');
					$("#modalTxtCategoryName").addClass('invalid');
					this.swal.error(`A category with the name '${categoryName}' already exists in this template.`)
					return;
				}

				let currentCategory = null;

				if (this.currentCategory != null) {
					currentCategory = this.currentCategory;
					currentCategory.name = categoryName;
				}
				else {
					currentCategory = {};
					currentCategory.id = Guid.new();
					currentCategory.name = categoryName;
					currentCategory.lineItems = [];

					const insertAfterId = $("#modalSlcInsertAfter").val();

					if (insertAfterId === '0')
						instance.lineItemsByGroup.unshift(currentCategory);
					else
						insertAfter(instance.lineItemsByGroup, insertAfterId, currentCategory);

					debugger;

					let sequence = 1;
					instance.lineItemsByGroup.forEach(item => {
						item.sequence = sequence;
						sequence++;
					});
				}

				$("#btnAddCategoryFormToggle").click();
				this.categoryForm.reset();
				instance.renderCategories();
				instance.#scrollToElement('data-category-id', currentCategory.id);
				this.currentCategory = null;
			}
		}); 
	}

	onAddLineItem(b) {
		const button = $(b);
		const categoryId = button.attr("data-category-id");
		const category = this.lineItemsByGroup.find(c => c.id == categoryId);
		const id = Guid.new();

		const newLineItem = {
			"id": id,
			"proposalTemplateId": this.template.id,
			"estimateCategoryId": id,
			"name": "",
			"description": "",
			"amount": 0,
			"sequence": category.lineItems.length + 1,
			"parentId": categoryId,
			"lineItems": [],
			"totalAmount": 0,
			"percentage": 0
		}

		category.lineItems.push(newLineItem);
		$(`tbody[data-category-id="${categoryId}"]`).html(this.#renderCategoryItems(category));
		$(`input[data-item-id="${id}"][data-model="name"]`).focus();
		this.#initAutoCompleteNewlineItems();
	}

	closeTemplateModal() {
		this.proposalTemplateForm.reset();
		this.saveTemplateModal.modal('hide');
	}

	getCategories() {
		return this.lineItemsByGroup.map((category) => {
			return `
			<div class="row mb-2 g-6 g-xl-9 category-row">
				<div class="col-lg-12">
					<div class="card card-flush h-lg-100">
						<div class="px-10 py-5 pb-0" data-category-id="${category.id}">
							<!--begin::Card title-->
							<div class="card-title flex-column">
								<div class="d-flex flex-stack">
									<div class="d-flex align-items-center">
										<h3 class="fw-bolder mb-1">${category.name}</h3>
									</div>
									<div class="d-flex align-items-center">
										<a href="javascript:" 
											class="btn btn-sm btn-icon btn-light-warning me-1" 
											data-bs-toggle="tooltip" data-bs-placement="top" title="Edit Category" 
											evt-click="showCategoryDrawer"
											data-category-id="${category.id}">
											<i class="bi bi-pencil-square"></i>
										</a>
										<a href="javascript:" 
											class="btn btn-sm btn-icon btn-light-danger" 
											data-bs-toggle="tooltip" data-bs-placement="top" title="Remove Category" 
											evt-click="onRemoveCategory"
											data-category-id="${category.id}">
											<i class="bi bi-trash"></i>
										</a>
									</div>
								</div>
							</div>

						</div>
						<!--end::Card header-->
						<!--begin::Card body-->
						<div class="card-body pt-0">
							<table class="table table table-row-dashed table-row-gray-300 proposal-items">
								<thead>
									<tr class="fw-bolder fs-6 text-gray-800 bg-light">
										<th class="w-35px">Sequence</th>
										<th class="w-250px">Item</th>
										<th class="">Description</th>
										<th class="w-150px">Amount</th>
										<th class="text-center w-150px">Actions</th>
									</tr>
								</thead>
								<tbody data-category-id="${category.id}">
									${this.#renderCategoryItems(category)}
								</tbody>
							</table>
						</div>
						<!--end::Card body-->
					</div>
				</div>
			</div>
			`}).join('');
	}

	renderCategories() {
		const categories = this.getCategories();
		$("#dv-categories").find(".category-row").remove();

		const spanElement = $("#templateAction");
		if (this.proposalTemplateId == Guid.empty) {
			spanElement.html(`Create New Proposal Template`);
		}
		else {
			spanElement.html(`Edit Proposal Template - ${this.template.name}`);
		}

		$("#proposal-template-header").removeClass('loading').addClass('loaded');
		$("#dv-categories").append(categories).removeClass('loading').addClass('loaded');
	
		TextboxUtils.init();
		this.#initAutoCompleteNewlineItems();
	}

	onsaveproposalTemplate(b) {
		const template = this.template;
		const isValid = this.#validate();

		b.setAttribute('data-kt-indicator', 'on');
		b.disabled = true;

		const resetButtonState = () => {
			b.removeAttribute('data-kt-indicator');
			b.disabled = false;
		};


		if (isValid) {
			if (template.isDefault) {
				const message = `Overwriting the default template is not allowed. Would you like to save a new copy of this as a new template instead?`;
				this.swal.confirm(
					message,
					() => {
						this.saveTemplateModal.modal('show');
						resetButtonState();
					},
					() => {
						resetButtonState();
					}
				);
			}
			else {
				const message = `This action will overwrite ${template.name} template. Would you like to proceed?`;
				this.swal.confirm(
					message,
					() => {
						this.updateProposalTemplate();
						resetButtonState();
					},
					() => {
						resetButtonState(); // Ensure resetButtonState is executed
					}
				);
			}
		}
	}

	onSaveAs() {
		const valid = this.#validate();
		if (valid) {
			this.saveTemplateModal.modal('show');
		}
	}

	saveProposalTemplate(button) {
		var proposalTemplate = {};
		proposalTemplate.name = $("#modalTxtTemplateName").val();
		proposalTemplate.lineItems = this.lineItemsByGroup;
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;
		this.service.saveProposalTemplate(proposalTemplate)
			.then((data) => {
				this.swal.alert('Proposal Template has been submitted successfully!', () => {

					if (this.proposalTemplateId == Guid.empty) {
						location.href = `/proposals/templates/${data.id}`;
					}
					else {
						this.closeTemplateModal();
						this.#loadProposalTemplate();
						button.removeAttribute('data-kt-indicator');
						button.disabled = false;
					}
				});
			})
			.catch(() => {
				button.removeAttribute('data-kt-indicator');
				button.disabled = false;
			});
	}

	updateProposalTemplate() {
		const template = this.template;
		var proposalTemplate = structuredClone(template);
		proposalTemplate.lineItems = this.lineItemsByGroup;
		debugger;
		this.service.saveProposalTemplate(proposalTemplate, template.id)
			.then((data) => {
				this.#loadProposalTemplate();
				this.swal.alert(`${this.template.name} has been updated successfully!`, () => {
				});
			})
			.catch(() => {

			});
	}
	
	#addNewLineItemRow(category) {
		return `<tr class="row-new-item">
						<td colspan="7">
							<a href="javascript:" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary w-100"
							evt-click="onAddLineItem"
							data-category-id="${category.id}">+ Add Line Item</a>
						</td>
				</tr>`;
	}

	#insertLineItem(button) {
		const $row = $(button).closest("tr");
		const $textbox = $row.find(".new-item-sequence");
		const sequenceInput = $textbox.val();
		const titleInput = $row.find(".new-item-title").val();
		const descriptionInput = $row.find(".new-item-description").val();
		const amountInput = $row.find(".new-item-amount").val();

		if (titleInput === '') return;

		$textbox.removeClass("invalid");
		var existingLineItem = this.#searchLineItemByName(titleInput);
		if (existingLineItem != null) {
			$textbox.addClass("invalid");
			this.swal.error(`An item named '${titleInput}' already exists in this template.`);
			return;
		}

		const insertItem = (arr, newItem) => {
			// Increment sequence for items with equal or greater sequence
			arr.forEach(item => {
				if (item.sequence >= newItem.sequence) {
					item.sequence++;
				}
			});

			// Add the new item to the array
			arr.push(newItem);

			// Sort the array based on sequence
			arr.sort((a, b) => a.sequence - b.sequence);
		}
		
		const categoryId = button.attr("data-category-id");

		const category = this.lineItemsByGroup.find(c => c.id == categoryId);
		const nextSeq = category.lineItems.length + 1;

		const newItem = {};
		newItem.id = Guid.new();
		newItem.sequence = sequenceInput == ''? parseInt(nextSeq) : parseInt(sequenceInput);
		newItem.name = titleInput;
		newItem.description = descriptionInput;
		newItem.amount = amountInput != '' ? parseFloat(amountInput) : 0;
		newItem.parentId = categoryId;

		insertItem(category.lineItems, newItem);
		const rows = this.#renderCategoryItems(category);
		$(`tbody[data-category-id="${categoryId}"]`).html(rows);
		this.#scrollToElement('data-row-id', newItem.id);
		this.#initAutoCompleteNewlineItems();
	}

	#buildStickyActions() {
		this.stickyActions = new StickyActions();
		this.stickyActions.actions = [
			new StickyActionsButton('Save', null, 'primary', null, false, (b) => { this.onsaveproposalTemplate(b);}),
			new StickyActionsButton('Save As', null, 'primary', null, false, () => { this.saveTemplateModal.modal('show'); }),
			new StickyActionsButton('Add Category', 'bi-plus', 'secondary', null, false, (b) => { this.showCategoryDrawer(b); })
		];
		this.stickyActions.init();	
	}

	#getActionButtons(lineItems, lineItem, index) {
		const isFirst = index === 0;
		const isLast = index === lineItems.length - 1;

		const upButtonStyle = isFirst ? 'visibility: hidden;' : 'visibility: visible;';
		const downButtonStyle = isLast ? 'visibility: hidden;' : 'visibility: visible;';

		// Define each button separately
		const moveUpButton = `
		<a href="javascript:" class="btn btn-sm btn-icon btn-secondary move-item" style="${upButtonStyle}"
			title="Move Up"
			data-action="up"
			data-item-id="${lineItem.id}">
			<span class="svg-icon">${doutune.up}</span>
		</a>`;

		const moveDownButton = `
		<a href="javascript:" class="btn btn-sm btn-icon btn-secondary move-item" style="${downButtonStyle}"
			title="Move Down"
			data-action="down"
			data-item-id="${lineItem.id}">
			<span class="svg-icon">${doutune.down}</span>
		</a>`;

		const removeButton = `
		<a href="javascript:" class="btn btn-sm btn-icon btn-light-danger remove-item"
			data-bs-toggle="tooltip"
			data-bs-placement="top"
			title="Remove"
			data-item-id="${lineItem.id}">
			<i class="bi bi-trash"></i>
		</a>`;

		// Assemble buttons based on the position of the item
		return `
			<div class="action-buttons">
				${moveUpButton}
				${moveDownButton}
				${removeButton}
			</div>
		`;
	};

	#getLineItems(category) {
		const description = (category, lineItem) => {
			const baseInput = `<textarea class="form-control textarea-dynamic form-control-sm" 
				style="height:37px; font-size:smaller;"
				data-model="description"
				data-item-id="${lineItem.id}"
				evt-input="onChangeDescription">${lineItem.description}</textarea>`;

			const isOverhead = category.name.toUpperCase() === 'OVERHEAD';
			const isSpecialLineItem = ['ON SITE SUPERVISION', 'GENERAL CONTRACTOR FEE'].includes(lineItem.name.trim().toUpperCase());

			if (isOverhead && isSpecialLineItem) {
				return `<div class="d-flex">
							${baseInput.replace('class="form-control form-control-sm"', 'class="form-control form-control-sm" style="flex: 0 0 85%; margin-right: 5px;"')}
							<div class="input-group input-group-sm">
								<span class="input-group-text" id="inputGroup-sizing-sm">%</span>
								<input type="text" data-type="int" class="form-control percentage" data-item-id="${lineItem.id}" value="${lineItem.percentage}">
							</div>
						</div>`;
			}

			return baseInput;
		};

		const amount = (category, lineItem) => {
			const isSpecialCase = ['ON SITE SUPERVISION', 'GENERAL CONTRACTOR FEE'].includes(lineItem.name.trim().toUpperCase());
			const additionalClass = isSpecialCase ? 'percentage-amount' : `line-item-amount ${category.name.toLowerCase()}`;

			return `<input type="text" class="form-control form-control-sm ${additionalClass}" data-type="money" data-item-id="${lineItem.id}" value="${lineItem.amount}">`;
		};

		return category.lineItems.map((lineItem, index) => `
			<tr data-row-id="${lineItem.id}">
				<td class="text-center">${lineItem.sequence}</td>
				<td>
					<input type="text" class="form-control form-control-sm new-item-title"
						evt-input="onLineItemNameChange"
						data-model="name"
						data-item-id="${lineItem.id}" 
						value="${lineItem.name}">
				</td>
				<td>
					${description(category, lineItem)}
				</td>
				<td>
					<div class="input-group input-group-sm">
						<span class="input-group-text">$</span>
							${amount(category, lineItem)}
					</div>
				</td>
				<td class="text-center">
					${this.#getActionButtons(category.lineItems, lineItem, index)}
				</td>
			</tr>
		`).join('');
	}

	#moveItem(elemId, direction) {
		const id = elemId;
		const item = this.#searchLineItemById(id);
		const category = this.lineItemsByGroup.find(g => g.id == item.parentId);
		const categoryId = category.id;
		const items = category.lineItems;
		const index = items.findIndex(item => item.id === id);

		// Return if the element is not found or if movement is out of bounds
		if (index === -1 ||
			(direction === 'up' && index === 0) ||
			(direction === 'down' && index === items.length - 1)) {
			return;
		}

		// Determine the target index based on direction
		const targetIndex = direction === 'up' ? index - 1 : index + 1;

		// Swap the elements at index and targetIndex
		[items[index], items[targetIndex]] = [items[targetIndex], items[index]];

		// Update the sequence values after the swap
		items[index].sequence = index + 1;
		items[targetIndex].sequence = targetIndex + 1;

		category.lineItems.forEach((lineItem, index) => {
			lineItem.sequence = index + 1; // Set sequence to index + 1
		});

		const rows = this.#renderCategoryItems(category);
		$(`tbody[data-category-id="${categoryId}"]`).html(rows);
		this.#scrollToElement('data-row-id', id);
		this.#initAutoCompleteNewlineItems();
	}

	#removeItem(id) {
		const item = this.#searchLineItemById(id);

		if (item) {
			const category = this.lineItemsByGroup.find(g => g.id == item.parentId);
			const categoryId = category.id;
			// Find the index of the lineItem within the lineItems array
			const index = category.lineItems.indexOf(item);

			// If the index is valid, remove the lineItem from the array
			if (index !== -1) {
				category.lineItems.splice(index, 1);
				this.lineItemsUniqueNamesValidation = this.lineItemsUniqueNamesValidation.filter(
					(item) => item.id !== id
				);
			}

			const rows = this.#renderCategoryItems(category);
			$(`tbody[data-category-id="${categoryId}"]`).html(rows);
			this.#initAutoCompleteNewlineItems();
		}

	}

	#renderCategoryItems(category) {
		//const addLineItemControl = this.newLineItemRow(category);
		const lineItems = this.#getLineItems(category);
		const addNewLineItemRow = this.#addNewLineItemRow(category);
		return lineItems + addNewLineItemRow;
	}

	async #getEstimateCategories() {
		await this.service.getEstimateCategories()
			.then((estimateCategories) => {
				this.estimateCategories = estimateCategories;
				this.parentEstimateCategories = this.estimateCategories.filter(ec => ec.parentEstimateCategoryId === null);
			});
	}

	async #loadProposalTemplate() {
		await this.service.loadProposalTemplate(this.proposalTemplateId)
			.then((template) => {
				this.template = template;
				this.lineItemsByGroup = this.template.categories;
				this.lineItemsByGroup.forEach(item => {
					item.lineItems.forEach((lineItem, index) => {
						lineItem.sequence = index + 1; // Set sequence to index + 1
					});
				});
			});
	}

	#populateCategoryList() {
		const categories = this.lineItemsByGroup;
		const emptyOption = `<option value="0">&nbsp;</option>`;
		const categoryOptions = emptyOption + categories.map(c => `<option value="${c.id}">${c.name}</option>`).join('');
		$("#modalSlcInsertAfter").html(categoryOptions);
	}

	#searchLineItemById(id) {
		for (const item of this.lineItemsByGroup) {
			// Check if lineItems exist in the current item
			if (item.lineItems) {
				// Search within lineItems for the specified id
				const lineItem = item.lineItems.find(line => line.id === id);
				if (lineItem) {
					return lineItem; // Return the lineItem if found
				}
			}
		}
		return null; // Return null if not found
	}

	#searchLineItemByName(searchName) {
		for (const item of this.lineItemsByGroup) {
			// Check if lineItems exist in the current item
			if (item.lineItems) {
				// Search within lineItems for the specified id
				const lineItem = item.lineItems.find(line => line.name.trim().toLowerCase() === searchName.trim().toLowerCase());
				if (lineItem) {
					return lineItem; // Return the lineItem if found
				}
			}
		}
		return null; 
	}

	#resetUniqueNamesValidation(input) {
		const currentItemId = input.attr("data-item-id");
		this.lineItemsUniqueNamesValidation = this.lineItemsUniqueNamesValidation.filter(item => item.id !== currentItemId);
		input.removeClass('invalid');
	}

	#setNewEstimateCategoryId(input, item, currentItemId) {
		debugger;
		const name = item.name;
		const id = item.id;
		////check from estimate categories by name
		var existingEstimateCategory = this.estimateCategories.find(line => line.name.toLowerCase().trim() == name.toLowerCase().trim());

		const lineItem = this.lineItemsByGroup.flatMap(item => item.lineItems).find(line => line.id == currentItemId);
		lineItem.name = name;
		lineItem.estimateCategoryId = lineItem.id = existingEstimateCategory ? existingEstimateCategory.id : id;

		//// update table row data
		const itemrow = $(input).parents('tr');
		const inputs = $(itemrow).find('input');
		const textareas = $(itemrow).find('textarea');
		const buttons = $(itemrow).find('a');

		itemrow.attr("data-row-id", lineItem.id);
		for (let i = 0; i < inputs.length; i++) {
			$(inputs[i]).attr("data-item-id", lineItem.id);
		}
		for (let i = 0; i < textareas.length; i++) {
			$(textareas[i]).attr("data-item-id", lineItem.id);
		}
		for (let i = 0; i < buttons.length; i++) {
			$(buttons[i]).attr("data-item-id", lineItem.id);
		}

		console.log(lineItem.id);
	}

	#initAutoCompleteNewlineItems() {
		const options = [];
		const uniqueNames = [];
		this.estimateCategories.forEach(ec => {
			const lowerCaseName = ec.name.toLowerCase();
			if (!uniqueNames.includes(lowerCaseName)) {
				uniqueNames.push(lowerCaseName);
				options.push({ id: ec.id, name: ec.name });
			}
		});

		$(".new-item-title").each((i,elem) => {
			const acOptions = {
				element: elem,
				options: options,
				onselect: (item) => {
					const selectedItem = {
						id: item.id,
						name: item.text
					};
					//this.#resetUniqueNamesValidation($(elem));
					//this.#setNewEstimateCategoryId($(elem), selectedItem, selectedItem.id);
					this.#checkIfLineItemExists(selectedItem, $(elem));
				},
			}
			const ac = new AutoComplete(acOptions);
			ac.init();
		});

		$(".textarea-dynamic").trigger('input');
	}

	#checkIfLineItemExists(item, input) {
		const name = item.name;
		if (name.length > 0) {
			const itemId = input.attr("data-item-id");
			const lineItem = this.lineItemsByGroup.flatMap(item => item.lineItems).find(line => line.name.toUpperCase().trim() === name.toUpperCase().trim() && itemId != line.id);

			this.#handleSearchFromProposalLinesResult(input, item, lineItem);
		}
		else {
			this.#resetUniqueNamesValidation(input);
		}
	}

	#handleSearchFromProposalLinesResult(input, item, result) {
		const name = item.name;
		const currentItemId = input.attr("data-item-id");
		this.#resetUniqueNamesValidation(input);
		this.#setNewEstimateCategoryId(input, item, currentItemId);

		//if (result) {
		//	const errorMessage = `An item with the name '${name}' already exists.`;
		//	const lineItemError = {
		//		id: currentItemId,
		//		error: errorMessage
		//	};
		//	this.lineItemsUniqueNamesValidation.push(lineItemError);
		//	this.swal.error(errorMessage);
		//	input.addClass('invalid');
		//}
		//else {
		//	this.#resetUniqueNamesValidation(input);
		//	this.#setNewEstimateCategoryId(input, item, currentItemId);
		//}
	}

	#initAutoCompleteCategoryGroup() {
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
			element: $("#modalTxtCategoryName"),
			options: options
		}
		const ac = new AutoComplete(acOptions);
		ac.init();
	}

	#validate() {
		let retval = true;
		let errors = '';
		this.lineItemsByGroup.map(category => {
			category.lineItems.map(lineItem => {
				if (lineItem.name.trim() == '') {
					errors += `${category.name} : Line Item. Name is Required<br/>`;
					retval = false;
				}
			}); 
		});

		this.lineItemsUniqueNamesValidation.map(li => {
			errors += `${li.error}<br/>`;
			retval = false;
		});
	
		if (!retval) {
			this.swal.error("Please correct all errors below.", errors);
		}

		return retval;
	}

	init() {
		this.#getEstimateCategories()
			.then(() => {
				this.#loadProposalTemplate()
					.then(() => {
						this.renderCategories();
						this.#bindEventHandlers();
						this.#buildStickyActions();
						this.#initAutoCompleteCategoryGroup();
						$(".textarea-dynamic").trigger('input');
					});
			});
    }
}

$(document).ready(() => {
	const view = new ProposalTemplateView();
    view.init();
});