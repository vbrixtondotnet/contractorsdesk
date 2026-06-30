class VendorsView extends DomEventComponent {
	constructor() {
		super();
		this.vendorsItemsLv = null;
		this.service = new VendorService();
		this.enableAdd = true;
		this.mode = "new";
		this.form = document.getElementById('vendor-form-drawer-form');
		this.vendorFormDrawer = new Drawer(`#vendor-form-drawer`);
		this.swal = new SwalUtil();
		this.onSaveCallback = null;
		this.currentUser = Auth.currentUser();
		this.vendor = null;
		this.vendors = [];
		this.isNew = true;
		this.vendorId = Guid.empty;
		this.types = [];
	}

	createNew() {
		this.mode = "new";
		this.vendorFormDrawer.toggle();
		this.#prepareForm();
	}

	#prepareForm() {
		const formTitle = this.mode == "new" ? "Add New Vendor" : "Edit Vendor";
		$("#vendor-drawer-form-title").html(formTitle);

		$("#vendor-form-drawer-form div.input-group").removeClass('d-none');

		this.#resetDropdown("#slc-vendor-state", "Select a state");
		this.isNew = true;
		this.form.reset();
	}

	async #prepareEditForm() {
		const vendor = this.vendor;
		this.isNew = false;
		$("#vendor-drawer-form-title").html("Edit Vendor");
		$(`#slc-vendor-category`).val(`${vendor.category}`).trigger("change");
		$("#txt-vendor-name").val(vendor.name);
		$("#txt-vendor-email").val(vendor.email);
		$("#txt-vendor-phone").val(vendor.phone);
		$("#txt-vendor-address").val(vendor.address);
		$(`#txt-vendor-zip`).val(vendor.zip);
		$("#txt-vendor-city").val(vendor.city);
		$(`#txt-vendor-company`).val(vendor.company);
		$("#slc-vendor-state").val(vendor.state).trigger('change');
		$("#vendor-form-drawer-form div.input-group").removeClass('d-none');
	}

	edit(vendor) {
		this.vendor = vendor;
		this.mode = "edit";
		this.#prepareEditForm();
		this.vendorFormDrawer.toggle();
	}


	#resetDropdown(id,) {
		$(id).val("").trigger("change");
	}

	loadData() {
		this.httpService.get("/api/vendors")
			.then((data) => {
				this.vendors = data;
				this.renderList();
				this.triggerFirstFilter();
			});
	}

	render() {
		const lvOptions = {
			title: 'Vendors',
			columnDefs: [
				{ class: 'ps-1', title: 'Name', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-200px', title: 'Category', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-200px', title: 'Company', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-lg-200px', title: 'Phone', sortable: true, onsort: (colname, direction) => { this.#sortList(colname, direction); } },
				{ class: 'w-150px text-center', title: 'Action' },
			],
			shadow: true
		}

		if (this.enableAdd) {
			lvOptions.onAdd = () => {
				this.createNew();
			}
		}

		this.vendorsItemsLv = new ListView(lvOptions);
		this.vendorsItemsLv.render('vendors-list');
	}

	#sortList(col, direction) {
		debugger;
		switch (col) {
			case 'NAME':
				this.vendors.sort((a, b) => {
					if (a.name === b.name) return 0;
					return direction === 'desc' ? (a.name < b.name ? 1 : -1) : (a.name > b.name ? 1 : -1);
				});
				break;
			case 'CATEGORY':
				this.vendors.sort((a, b) => {
					if (a.category === b.category) return 0;
					return direction === 'desc' ? (a.category < b.category ? 1 : -1) : (a.category > b.category ? 1 : -1);
				});
				break;
			case 'COMPANY':
				this.vendors.sort((a, b) => {
					if (a.company === b.company) return 0;
					return direction === 'desc' ? (a.company < b.company ? 1 : -1) : (a.company > b.company ? 1 : -1);
				});
				break;
			case 'PHONE':
				this.vendors.sort((a, b) => {
					if (a.phone === b.phone) return 0;
					return direction === 'desc' ? (a.phone < b.phone ? 1 : -1) : (a.phone > b.phone ? 1 : -1);
				});
				break;

		}
		this.renderList();
	}

	triggerFirstFilter() {
		this.vendorsItemsLv.triggerFirstFilter();
	}

	#vendorRow(item) {
		return `<tr data-action-item-row="${item.id}">
					<td class="py-1 ps-1 mb-2">
					<div class="d-flex flex-column ps-1 mb-2">
						<span class="fs-5 fw-bolder text-gray-900">${item.name}</span>
						<span>${item.email}</span>
					</div>
					</td>
					<td class="py-1">${item.category ?? ''}</td>
					<td class="py-1">${item.company ?? ''}</td>
					<td class="py-1">${item.phone ?? ''}</td>
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
					<a href="javascript:" class="menu-link px-3 text-danger" data-id=${item.id} evt-click="onvendorFormDelete">
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.trash}
						</span>
						&nbsp;Remove
					</a>
				</div>`;

		const editButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onvendorFormEdit">
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
					${deleteButton}
				</div>`;
	}

	renderList() {
		const vendors = this.vendors;
		const rows = vendors.map(item => { return this.#vendorRow(item) }).join('');
		this.vendorsItemsLv.renderRows(rows);
		this.#initKtAppEventHandlers();
	}

	#initKtAppEventHandlers() {
		KTApp.initBootstrapTooltips();
		KTMenu.init();
		KTMenu.initGlobalHandlers();
	}

	#isValidForm() {
		const form = this.form;

		let isValid = true;

		const categorySelect = document.getElementById('slc-vendor-category');
		const categoryTextbox = document.getElementById('txt-vendor-category');

		const selectValue = categorySelect.value;
		const textboxValue = categoryTextbox.value;
		this.selectedCategory = selectValue;

		categorySelect.removeAttribute("required");
		categoryTextbox.removeAttribute("required");
		categorySelect.setCustomValidity('');
		categoryTextbox.setCustomValidity('');

		if (!selectValue && !textboxValue) {
			categorySelect.setCustomValidity('Category is required.');
			categoryTextbox.setCustomValidity('Category is required.');
			isValid = false;
		}
		else if (selectValue) {
			categorySelect.setAttribute("required", "required");
		}
		else if (textboxValue) {
			categoryTextbox.setAttribute("required", "required");
		}

		if (!form.checkValidity() || !isValid) {
			if (!isValid) {
				this.swal.error("Category is required.");
			}
			else {
				form.reportValidity();
			}

			return false;
		}

		return true;
	}

	#createPayload() {
		let categoryValue = $("#txt-vendor-category").val() || $("#slc-vendor-category").val() || null;

		return {
			name: $("#txt-vendor-name").val(),
			address: $("#txt-vendor-address").val(),
			city: $("#txt-vendor-city").val(),
			state: $("#slc-vendor-state").val(),
			email: $("#txt-vendor-email").val(),
			phone: $("#txt-vendor-phone").val(),
			category: categoryValue,
			zip: $("#txt-vendor-zip").val(),
			company: $("#txt-vendor-company").val(),
			isNew: this.isNew,
			id: this.vendor ? this.vendor.id : null
		}
	}

	onvendorFormSave(b) {
		if (this.#isValidForm()) {
			b.setAttribute('data-kt-indicator', 'on');
			b.disabled = true;
			const message = this.mode === "new" ? "Vendor created successfully." : "Vendor updated successfully.";
			const payload = this.#createPayload();
		
			this.httpService.post(`/api/vendors`, payload)
				.then((data) => {
					if (this.mode == "new") {
						this.vendors.unshift(data);
						this.renderList();
					}
					else {
						const index = this.vendors.findIndex(({ id }) => id === data.id);

						if (index >= 0) {
							this.vendors.splice(index, 1, data);
						}
						this.renderList();
					}
				
				})
				.finally(() => {
					this.swal.alert(message);
					this.vendorFormDrawer.toggle();
					this.form.reset();
					this.#initTypes();
					b.removeAttribute('data-kt-indicator');
					b.disabled = false;
					
				});
			
		}
	}

	onvendorFormDelete(b) {
		this.swal.confirm('Are you sure you want to delete this vendor?', () => {
			const id = b.getAttribute('data-id');
			this.httpService.patch(`/api/vendors/delete/${id}`)
				.then((response) => {
					if (response) {
						this.swal.alert('Vendor has been removed successfully!');
						$(`tr[data-action-item-row="${id}"]`).remove();
					}
				});
		});

	}


	onvendorFormEdit(b) {
		const id = b.getAttribute('data-id');
		const vendor = this.vendors.find(ai => ai.id == id);
		this.edit(vendor);
	}

	#initTypes() {
		this.httpService.get("/api/vendors/types")
			.then((data) => {
				this.types = data;
				this.#populateCategories(data);
			});
	}


	#populateCategories(data) {
		$("#slc-vendor-category").empty();
		$("#slc-vendor-category").prop("disabled", false);
		$("#slc-vendor-category").append(`<option value="" disabled selected>Select Category</option>`);

		data.forEach(t => {
			$("#slc-vendor-category").append(new Option(t, t));
		});

		this.#fixSelect2Width();
	}

	#fixSelect2Width() {
		$("#select2-slc-vendor-category-container").parent().parent().parent().css("width", "");
	}

	init() {
		this.loadData();
		this.render();
		this.#initTypes();
		TextboxUtils.init();
    }
}

$(document).ready(() => {
	const view = new VendorsView();
    view.init();
});