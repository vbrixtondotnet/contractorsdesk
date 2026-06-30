class EmailTemplatesView extends DomEventComponent {
	constructor() {
		super();
		this.emailTemplateBodyEditor = null;
		this.emailTemplatesItemsLv = null;
		this.service = new EmailTemplatesService();
		this.enableAdd = false;
		this.mode = "new";
		this.form = document.getElementById('email-template-form-drawer-form');
		this.emailTemplateFormDrawer = new Drawer(`#email-template-form-drawer`);
		this.swal = new SwalUtil();
		this.onSaveCallback = null;
		this.currentUser = Auth.currentUser();
		this.emailTemplate = null;
		this.emailTemplates = [];
		this.isNew = true;
		this.emailTemplateId = Guid.empty;
	}

	createNew() {
		this.mode = "new";
		this.emailTemplateFormDrawer.toggle();
		this.#prepareForm();
	}

	async #prepareEmailBodyEditor() {
		if (this.emailTemplateBodyEditor) {
			const toolbarContainer = document.querySelector('#txt-email-template-body-component-editor-toolbar');
			if (this.emailTemplateBodyEditor.ui?.view?.toolbar?.element && toolbarContainer.contains(this.emailTemplateBodyEditor.ui.view.toolbar.element)) {
				toolbarContainer.removeChild(this.emailTemplateBodyEditor.ui.view.toolbar.element);
			}

			await this.emailTemplateBodyEditor.destroy();
			this.emailTemplateBodyEditor = null;
		}

		await DecoupledEditor
			.create(document.querySelector('#txt-email-template-body-component-editor-document'))
			.then(editor => {
				document.querySelector('#txt-email-template-body-component-editor-toolbar').appendChild(editor.ui.view.toolbar.element);
				this.emailTemplateBodyEditor = editor;
			})
			.catch(console.error);
	}

	async #prepareForm() {
		const formTitle = this.mode == "new" ? "Add New Email Template" : "Edit Email Template";
		$("#email-template-drawer-form-title").html(formTitle);

		$("#email-template-form-drawer-form div.input-group").removeClass('d-none');

		await this.#prepareEmailBodyEditor();

		this.isNew = true;
		this.form.reset();
	}

	#getContent() {
		return this.emailTemplateBodyEditor.getData();
	}

	async #prepareEditForm() {
		const emailTemplate = this.emailTemplate;
		this.isNew = false;

		$("#email-template-drawer-form-title").html("Edit Email Template");
		$("#txt-email-template-name").val(emailTemplate.name);
		/*$("#txt-email-template-type").val(emailTemplate.emailType);*/
		$("#email-template-form-drawer-form div.input-group").removeClass('d-none');

		this.#prepareEmailBodyEditor().then(() => {
			this.emailTemplateBodyEditor.setData(this.emailTemplate.body || "");
		});
	}

	edit(emailTemplate) {
		this.emailTemplate = emailTemplate;
		this.mode = "edit";
		this.#prepareEditForm();
		this.emailTemplateFormDrawer.toggle();
	}

	loadData() {
		this.httpService.get("/api/email-template")
			.then((data) => {
				this.emailTemplates = data;
				this.renderList();
				this.triggerFirstFilter();
			});
	}

	render() {
		const lvOptions = {
			title: 'Email Templates',
			columnDefs: [
				{ class: 'ps-1', title: 'Name', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-300px', title: 'Email Type', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-250px', title: 'Modified Date', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-150px text-center', title: 'Action' },
			],
			shadow: true
		}

		if (this.enableAdd) {
			lvOptions.onAdd = () => {
				this.createNew();
			}
		}

		this.emailTemplatesItemsLv = new ListView(lvOptions);
		this.emailTemplatesItemsLv.render('email-template-list');
	}

	#sortList(col, direction) {
		let data = this.emailTemplates;

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

		this.emailTemplates = data;
		this.renderList();
	}

	triggerFirstFilter() {
		this.emailTemplatesItemsLv.triggerFirstFilter();
	}

	#emailTemplateRow(item) {
		return `<tr data-action-item-row="${item.id}">
					<td class="py-1 ps-1 mb-2">
						${item.name}
					</td>
					<td class="py-1">${item.emailType ?? ''}</td>
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
					<a href="javascript:" class="menu-link px-3 text-danger" data-id=${item.id} evt-click="onEmailTemplateFormDelete">
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.trash}
						</span>
						&nbsp;Remove
					</a>
				</div>`;

		const editButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onEmailTemplateFormEdit">
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
		const emailTemplates = this.emailTemplates;
		const rows = emailTemplates.map(item => { return this.#emailTemplateRow(item) }).join('');
		this.emailTemplatesItemsLv.renderRows(rows);
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
			let emailTemplateBody = this.#getContent();

			if (!emailTemplateBody) {
				this.swal.error('Email template body is required.');
				return false;
			}

			return true;
		}

		form.reportValidity();
		return false;
	}

	#createPayload() {
		debugger;
		return {
			name: $("#txt-email-template-name").val(),
			/*type: $("#txt-email-template-type").val(),*/
			body: this.#getContent(),
			isNew: this.isNew,
			id: this.emailTemplate ? this.emailTemplate.id : null
		}
	}
	//
	onEmailTemplateFormSave(b) {
		debugger;
		if (this.#isValidForm()) {
			b.setAttribute('data-kt-indicator', 'on');
			b.disabled = true;
			const message = this.mode === "new" ? "Email Template created successfully." : "Email Template updated successfully.";
			const payload = this.#createPayload();

			this.httpService.post(`/api/email-template`, payload)
				.then((data) => {
					debugger;
					if (this.mode == "new") {
						this.emailTemplates.unshift(data);
						this.renderList();
					}
					else {
						const index = this.emailTemplates.findIndex(({ id }) => id === data.id);

						if (index >= 0) {
							this.emailTemplates.splice(index, 1, data);
						}
						this.renderList();
					}

				})
				.finally(() => {
					this.swal.alert(message);
					this.emailTemplateFormDrawer.toggle();
					b.removeAttribute('data-kt-indicator');
					b.disabled = false;
				});

		}
	}

	/*
	onEmailTemplateFormDelete(b) {
		this.swal.confirm('Are you sure you want to delete this email template?', () => {
			const id = b.getAttribute('data-id');
			this.httpService.patch(`/api/{controller}/delete/${id}`)
				.then((response) => {
					if (response) {
						this.swal.alert('Email template has been removed successfully!');
						$(`tr[data-action-item-row="${id}"]`).remove();
					}
				});
		});

	}
	*/

	onEmailTemplateFormEdit(b) {
		const id = b.getAttribute('data-id');
		const emailTemplate = this.emailTemplates.find(ai => ai.id == id);
		this.edit(emailTemplate);
	}

	init() {
		this.render();
		this.loadData();
		TextboxUtils.init();
	}
}

$(document).ready(() => {
	const view = new EmailTemplatesView();
	view.init();
});