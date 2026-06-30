class SubContractorList extends DomEventComponent {
	constructor() {
		super();
		this.subcontractorsItemsLv = null;
		this.subcontractorForm = new SubcontractorForm();
		this.enableAdd = false;
		this.currentFilter = null;
		this.onSaveCallback = null;
		this.httpService = new httpService();
		this.projectId = Guid.empty;
		this.subContractors = [];
		this.swal = new SwalUtil();
		this.subContractorProjectDrawer = new Drawer('#subcontractor-projects-drawer');
		this.viewOnly = false;
	}

	#generateActionMenu(item) {
		const currentUser = Auth.currentUser();
		const role = currentUser.role;

		const deleteButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-danger" data-id=${item.id} evt-click="onDelete">
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.trash}
						</span>
						&nbsp;Remove
					</a>
				</div>`;

		const editButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onEdit">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.pencil}
						</span>
						&nbsp;Update
					</a>
				</div>`;

		const viewProjectButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-name="${item.name}" data-id=${item.id} data-category="${item.category}" evt-click="showSubcontractorsProjects">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.file1}
						</span>
						&nbsp;View Projects
					</a>
				</div>`;

		return `<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
					Actions <i class="fas fa-chevron-down"></i>
				</a>
				<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true">
					${viewProjectButton}
					${editButton}	
					${deleteButton}
				</div>`;
	}
	
	#subContractorRow(item) {
		return `<tr data-action-item-row="${item.id}">
					<td class="py-1">
						<div class="d-flex flex-column  ps-1 mb-2">
							<a href="javascript:" class="menu-link text-primary" data-name="${item.name} "data-category="${item.category}" data-id=${item.id} evt-click="showSubcontractorsProjects">
								<span class="fs-5 fw-bolder text-gray-900">${item.name}</span>
							</a>
							<span>${item.email}</span>
						</div>
					</td>
					<td class="py-1">${item.category ?? ''}</td>
					<td class="py-1">${item.company ?? ''}</td>
					<td class="py-1">${item.phone ?? ''}</td>
					${this.viewOnly ? '' : `<td class="text-center py-1">
						<div class="ms-auto">
							${this.#generateActionMenu(item)}
						</div>
					</td>`}
				</tr>`;
	}

	#sortList(col, direction) {
		switch (col) {
			case 'NAME':
				this.subContractors.sort((a, b) => {
					if (a.name === b.name) return 0;
					return direction === 'desc' ? (a.name < b.name ? 1 : -1) : (a.name > b.name ? 1 : -1);
				});
				break;
			case 'COMPANY':
				this.subContractors.sort((a, b) => {
					if (a.company === b.company) return 0;
					return direction === 'desc' ? (a.company < b.company ? 1 : -1) : (a.company > b.company ? 1 : -1);
				});
				break;
			case 'CATEGORY':
				this.subContractors.sort((a, b) => {
					if (a.category === b.category) return 0;
					return direction === 'asc' ? (a.category < b.category ? 1 : -1) : (a.category > b.category ? 1 : -1);
				});
				break;
			case 'PHONE':
				this.subContractors.sort((a, b) => {
					if (a.phone === b.phone) return 0;
					return direction === 'desc' ? (a.phone < b.phone ? 1 : -1) : (a.phone > b.phone ? 1 : -1);
				});
				break;

		}
		this.renderList();
	}

	#initKtAppEventHandlers() {
		KTApp.initBootstrapTooltips();
		KTMenu.init();
		KTMenu.initGlobalHandlers();
	}

	renderList() {
		const subContractors = this.subContractors;
		const subContractorRows = subContractors.map(item => { return this.#subContractorRow(item) }).join('');
		this.subcontractorsItemsLv.renderRows(subContractorRows);
		this.#initKtAppEventHandlers();
	}

	triggerFirstFilter() {
		this.subcontractorsItemsLv.triggerFirstFilter();
	}

	setProjects(projects) {
		this.subcontractorForm.populateProjects(projects);
	}

	setProjectId(projectId) {
		this.projectId = projectId;
		this.subcontractorForm.projectId = projectId;
	}

	loadProjects() {
		this.httpService.get('/api/projects')
			.then(response => {
				this.subcontractorForm.populateProjects(response);
			});
	}

	loadSubContractors() {
		this.httpService.get("/api/sub-contractors?projectId=" + this.projectId)
			.then((data) => {
				this.subContractors = data;
				this.renderList();
				this.triggerFirstFilter();
			});
	}
	
	preload() {
		this.subcontractorsItemsLv.preload();
	}

	showSubcontractorsProjects(i) {

		const $target = $(i);
		const id = $target.attr("data-id");
		const name = $target.attr("data-name");
		const category = $target.attr("data-category");

		$("#tbody-projects").empty();
		$("#lblSubcontractorName").text(""); 
		this.httpService.get("/api/sub-contractors/" + id + "/projects")
			.then(data => {
				if (data.projects.length > 0) {
					const rows = data.projects.map(d => `<tr>
													<td class="ps-2 ${d.isActive == false ? 'text-danger' : ''}">${d.name}</td>
													<td class="ps-2 ${d.isActive == false ? 'text-danger' : ''}">${d.startDate}</td>
													<td class="ps-2 ${d.isActive == false ? 'text-danger' : ''}">${d.endDate}</td>
													<td class="ps-2 ${d.isActive == false ? 'text-danger' : ''}">${d.isActive == true ? 'Active' : 'Inactive'}</td>
												</tr>`).join('');
					$("#tbody-projects").html(rows);
				}
				else {
					$("#tbody-projects").html(`<tr>
						<td colspan="4" class="text-center fs-8 text-muted">No assigned project.</td>
					</tr>`)
				}
			})
			.then(data => {
				$("#lblSubcontractorName").text(
					(name || "Subcontractor") + (category && category !== "null" ? " - " + category : "")
				);
				this.subContractorProjectDrawer.toggle();
			})
	}

	onEdit(b) {	
		const id = b.getAttribute('data-id');
		const record = this.subContractors.find(ai => ai.id == id);
		this.subcontractorForm.onSaveCallback = (data) => {
			const index = this.subContractors.findIndex(item => item.id === data.id);
			if (index !== -1) {
				this.subContractors[index] = data;
				this.renderList();
			}
		}
		this.subcontractorForm.edit(record);
	}

	onDelete(b) {
		this.swal.confirm('Are you sure you want to delete this subcontractor?', () => {
			const id = b.getAttribute('data-id');
			let request;
			if (this.projectId !== Guid.empty) {
				request = this.httpService.delete(`/api/sub-contractors/project/${this.projectId}/delete/${id}`);
			} else {
				// soft delete
				request = this.httpService.patch(`/api/sub-contractors/delete/${id}`);
			}

			request
				.then((response) => {
					if (response) {
						this.swal.alert('Subcontractor has been removed successfully!');
						$(`tr[data-action-item-row="${id}"]`).remove();
					}
				});
		});

	}

	render() {
		const lvOptions = {
			title: 'Sub-Contractors',
			shadow: true
		}

		let colDefs = [
			{ class: 'ps-1', title: 'Name', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
			{ class: 'w-lg-200px', title: 'Category', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
			{ class: 'w-lg-200px', title: 'Company', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
			{ class: 'w-lg-200px', title: 'Phone', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } }
		];

		if (!this.viewOnly) {
			colDefs.push({ class: 'w-150px text-center', title: 'Action' });
		}

		lvOptions.columnDefs = colDefs;

		if (this.enableAdd) {
			lvOptions.onAdd = () => {
				this.subcontractorForm.createNew();
			}
		}

		this.subcontractorsItemsLv = new ListView(lvOptions);
		this.subcontractorsItemsLv.render('comp-subcontractor-list');

		this.subcontractorForm.onSaveCallback = this.onSaveCallback;
		this.subcontractorForm.init();
	}
}