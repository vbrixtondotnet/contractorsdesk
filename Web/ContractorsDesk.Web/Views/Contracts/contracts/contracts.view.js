class ContractsView extends DomEventComponent {
	#debouncedManualSave;

	constructor() {
		super();
		this.contractBodyEditor = null;
		this.contractsItemsLv = null;
		this.service = new ContractsService();
		this.enableAdd = false;
		this.mode = "new";
		this.form = document.getElementById('contract-form-drawer-form');
		this.contractFormDrawer = new Drawer(`#contract-form-drawer`);
		this.swal = new SwalUtil();
		this.onSaveCallback = null;
		this.currentUser = Auth.currentUser();
		this.contract = null;
		this.contracts = [];
		this.isNew = true;
		this.contractId = Guid.empty;
		this.#debouncedManualSave = debouncedManualSave(html => this.#saveData(html), 3000);
	}

	createNew() {
		this.mode = "new";
		this.contractFormDrawer.toggle();
		this.#prepareForm();
	}

	async #prepareEmailBodyEditor() {
		if (this.contractBodyEditor) {
			const toolbarContainer = document.querySelector('#txt-contract-body-component-editor-toolbar');
			if (this.contractBodyEditor.ui?.view?.toolbar?.element && toolbarContainer.contains(this.contractBodyEditor.ui.view.toolbar.element)) {
				toolbarContainer.removeChild(this.contractBodyEditor.ui.view.toolbar.element);
			}

			await this.contractBodyEditor.destroy();
			this.contractBodyEditor = null;
		}

		await DecoupledEditor
			.create(document.querySelector('#txt-contract-body-component-editor-document'), {
				extraPlugins: [function (editor) {
					editor.plugins.get('FileRepository').createUploadAdapter = loader =>
						new ContractsCKEditorInserImageAdapter(loader);
				}],
				autosave: {
					save: (editor) => this.#saveData(editor.getData())
				}
			})
			.then(editor => {
				document
					.querySelector('#txt-contract-body-component-editor-toolbar')
					.appendChild(editor.ui.view.toolbar.element);

				editor.model.document.on('change:data', () => {
					this.#debouncedManualSave(editor.getData());
				});

				this.contractBodyEditor = editor;
			})
			.catch(console.error);
	}

	async #prepareForm() {
		const formTitle = this.mode == "new" ? "Add New Email Template" : "Edit Email Template";
		$("#contract-drawer-form-title").html(formTitle);

		$("#contract-form-drawer-form div.input-group").removeClass('d-none');

		await this.#prepareEmailBodyEditor();

		this.isNew = true;
		this.form.reset();
		this.contractBodyEditor.setData('');
	}

	#getContent() {
		return this.contractBodyEditor.getData();
	}

	async #prepareEditForm() {
		const contract = this.contract;
		this.isNew = false;

		$("#contract-drawer-form-title").html("Edit Email Template");
		$("#txt-contract-name").val(contract.name);
		$("#contract-form-drawer-form div.input-group").removeClass('d-none');

		await this.#prepareEmailBodyEditor();

		this.contractBodyEditor.setData(this.contract.bodyTemplate || "");
	}

	edit(contract) {
		this.contract = contract;
		this.mode = "edit";
		this.#prepareEditForm();
		this.contractFormDrawer.toggle();
	}

	loadData() {
		this.httpService.get("/api/contracts")
			.then((data) => {
				this.contracts = data;
				this.renderList();
				this.triggerFirstFilter();
			});
	}

	render() {
		const lvOptions = {
			title: 'Contracts',
			columnDefs: [
				{ class: 'ps-1', title: 'Name', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-300px', title: 'Modified Date', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-150px text-center', title: 'Action' },
			],
			shadow: true
		}

		if (this.enableAdd) {
			lvOptions.onAdd = () => {
				this.createNew();
			}
		}

		this.contractsItemsLv = new ListView(lvOptions);
		this.contractsItemsLv.render('contract-list');
	}

	#sortList(col, direction) {
		let data = this.contracts;

		switch (col) {
			case 'NAME':
				data.sort((a, b) => {
					if (a.name === b.name) return 0;
					return direction === 'desc' ? (a.name < b.name ? 1 : -1) : (a.name > b.name ? 1 : -1);
				});
				break;
			case 'EMAIL TYPE':
				data.sort((a, b) => {
					if (a.emailType === b.emailType) return 0;
					return direction === 'desc' ? (a.emailType < b.emailType ? 1 : -1) : (a.emailType > b.emailType ? 1 : -1);
				});
				break;
			case 'MODIFIED DATE':
				data.sort((a, b) => {
					if (a.dateModified === b.dateModified) return 0;
					return direction === 'desc' ? (a.dateModified < b.dateModified ? 1 : -1) : (a.dateModified > b.dateModified ? 1 : -1);
				});
				break;

		}

		this.contracts = data;
		this.renderList();
	}

	triggerFirstFilter() {
		this.contractsItemsLv.triggerFirstFilter();
	}

	#contractRow(item) {
		return `<tr data-action-item-row="${item.id}">
					<td class="py-1 ps-1 mb-2">
						${item.name}
					</td>
					<td class="py-1">${item.dateModified ?? ''}</td>
					<td class="text-center py-1">
						<div class="ms-auto">
							${this.#generateActionMenu(item)}
						</div>
					</td>
				</tr>`;
	}

	#generateActionMenu(item) {
		const currentUser = Auth.currentUser();
		const role = currentUser.role;

		const deleteButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-danger" data-id=${item.id} evt-click="onContractFormDelete">
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.trash}
						</span>
						&nbsp;Remove
					</a>
				</div>`;

		const editButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onContractFormEdit">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.pencil}
						</span>
						&nbsp;Update
					</a>
				</div>`;

		return `<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
					Actions <i class="fas fa-chevron-down"></i>
				</a>
				<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true">
					${editButton}
				</div>`;
	}

	renderList() {
		const contracts = this.contracts;
		const rows = contracts.map(item => { return this.#contractRow(item) }).join('');
		this.contractsItemsLv.renderRows(rows);
		this.#initKtAppEventHandlers();
	}

	#initKtAppEventHandlers() {
		KTApp.initBootstrapTooltips();
		KTMenu.init();
		KTMenu.initGlobalHandlers();
	}

	#isValidForm() {
		const form = this.form;

		if (form.checkValidity()) {
			return true;
		}

		form.reportValidity();
		return false;
	}

	#createPayload() {
		return {
			name: $("#txt-contract-name").val(),
			bodyTemplate: this.#getContent(),
			isNew: this.isNew,
			id: this.contract ? this.contract.id : null
		}
	}

	onContractFormSave(b) {
		if (this.#isValidForm()) {
			b.setAttribute('data-kt-indicator', 'on');
			b.disabled = true;
			const message = this.mode === "new" ? "Email Template created successfully." : "Email Template updated successfully.";
			const payload = this.#createPayload();

			this.httpService.post(`/api/contracts`, payload)
				.then((data) => {
					if (this.mode == "new") {
						this.contracts.unshift(data);
						this.renderList();
					}
					else {
						const index = this.contracts.findIndex(({ id }) => id === data.id);

						if (index >= 0) {
							this.contracts.splice(index, 1, data);
						}
						this.renderList();
					}

				})
				.finally(() => {
					this.swal.alert(message);
					this.contractFormDrawer.toggle();
					b.removeAttribute('data-kt-indicator');
					b.disabled = false;
				});

		}
	}

	onContractFormEdit(b) {
		const id = b.getAttribute('data-id');
		const contract = this.contracts.find(ai => ai.id == id);
		this.edit(contract);
	}

	#saveData(html) {
		if (this.mode === 'edit') {
			const payload = this.#createPayload();

			this.httpService.post(`/api/contracts`, payload)
				.then((data) => {
					const index = this.contracts.findIndex(({ id }) => id === data.id);

					if (index >= 0) {
						this.contracts.splice(index, 1, data);
					}
				});
		}

		return Promise.resolve();
	}

	init() {
		this.render();
		this.loadData();
		TextboxUtils.init();
	}
}

$(document).ready(() => {
	const view = new ContractsView();
	view.init();
});