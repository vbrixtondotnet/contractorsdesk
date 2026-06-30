class ProposalTemplatesView extends DomEventComponent {
	constructor() {
		super();
		this.proposalId = document.URL.split('/').pop();
		this.service = new ProposalTemplatesService();
		this.data = [];
		this.proposaltemplatesItemsLv = null;
		this.currentUser = Auth.currentUser();
		this.swal = new SwalUtil();
		this.defaultTemplateId = null;

	}


	loadData() {
		this.service.loadProposaTemplates()
			.then((template) => {
				console.log(template);
				this.data = template;
				this.renderList();
				this.triggerFirstFilter();
			});
		this.service.loadProposalTemplateDefaultByUser().then((template) => {
			this.defaultTemplateId = template.templateId;
		});
	}

	renderList() {
		const proposaltemplates = this.data;
		const rows = proposaltemplates.map(item => { return this.#proposaltemplateRow(item) }).join('');
		this.proposaltemplatesItemsLv.renderRows(rows);
		this.#initKtAppEventHandlers();
	}

	#proposaltemplateRow(item) {
		return `<tr data-action-item-row="${item.id}">
					<td class="py-1 ps-1 mb-2">
						${item.name}
					</td>
					<td class="py-1">${item.owner?.[0] ? `${item.owner[0].firstName} ${item.owner[0].lastName}` : ''}</td>
					<td class="py-1">${DateUtils.formatDateTime(item.dateCreated) ?? ''}</td>
					<td class="text-center py-1">
						<div class="ms-auto">
							${this.#generateActionMenu(item)}
						</div>
					</td>
				</tr>`;
	}

	triggerFirstFilter() {
		this.proposaltemplatesItemsLv.triggerFirstFilter();
	}

	#generateActionMenu(item) {
		const deleteButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-danger" data-id=${item.id} evt-click="onDelete">
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.trash}
						</span>
						&nbsp;Remove
					</a>
				</div>`;
		const setAsDefault = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onSetDefault">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.check}
						</span>
						&nbsp;Set as Default
					</a>
				</div>`;
		const disabledDefault = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-muted" data-id=${item.id} disabled>
						<span class="svg-icon svg-icon-muted svg-icon-1x">
							${doutune.check}
						</span>
						&nbsp;Default Template
					</a>
				</div>`;
		return `
			<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
				Actions <i class="fas fa-chevron-down"></i>
			</a>
			<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true">
				${item.id === this.defaultTemplateId
				? disabledDefault
				: setAsDefault}
				${item.canBeDeleted && item.id !== this.defaultTemplateId ? deleteButton : ""}
			</div>`;
	}

	render() {
		const lvOptions = {
			title: 'Proposal Templates',
			columnDefs: [
				{ class: 'ps-1', title: 'Template Name', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-200px', title: 'Owner Name', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-200px', title: 'Date Created', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-150px text-center', title: 'Action' },
			],
			shadow: true
		}

		this.proposaltemplatesItemsLv = new ListView(lvOptions);
		this.proposaltemplatesItemsLv.render('proposaltemplate-list');
	}

	#initKtAppEventHandlers() {
		KTApp.initBootstrapTooltips();
		KTMenu.init();
		KTMenu.initGlobalHandlers();
	}

	#sortList(col, direction) {
		switch (col) {
			case 'TEMPLATE NAME':
				this.data.sort((a, b) => {
					if (a.name === b.name) return 0;
					return direction === 'desc' ? (a.name < b.name ? 1 : -1) : (a.name > b.name ? 1 : -1);
				});
				break;
			case 'OWNER NAME':
				this.data.sort((a, b) => {
					const nameA = a.owner?.[0]
						? `${a.owner[0].firstName} ${a.owner[0].lastName}`.toLowerCase()
						: '';
					const nameB = b.owner?.[0]
						? `${b.owner[0].firstName} ${b.owner[0].lastName}`.toLowerCase()
						: '';

					if (nameA === nameB) return 0;
					return direction === 'desc'
						? nameA < nameB ? 1 : -1
						: nameA > nameB ? 1 : -1;
				});
				break;
			case 'DATE CREATED':
				this.data.sort((a, b) => {
					if (a.dateCreated === b.dateCreated) return 0;
					return direction === 'desc' ? (a.dateCreated < b.dateCreated ? 1 : -1) : (a.dateCreated > b.dateCreated ? 1 : -1);
				});
				break;

		}
		this.renderList();
	}

	onDelete(b) {
		const id = b.getAttribute('data-id');
		this.swal.confirm('Are you sure you want to delete this proposal template?', () => {

			this.httpService.patch(`/api/proposal-templates/delete/${id}`)
				.then((response) => {
					if (response) {
						this.swal.alert('Proposal template has been removed successfully!');
						$(`tr[data-action-item-row="${id}"]`).remove();
					}
				});
		});
	}

	onSetDefault(b) {
		const id = b.getAttribute('data-id');
		this.swal.confirm('Do you want to set this proposal template as your default?', () => {
			this.service.saveProposalTemplateUserDefault(id).then((response) => {
				this.swal.alert('Default proposal template updated successfully.');
				this.defaultTemplateId = id;
				this.renderList();
			});;
		});

	}

	init() {
		this.loadData();
		this.render();
    }
}

$(document).ready(() => {
	const view = new ProposalTemplatesView();
    view.init();
});