class EditRoleView {
    constructor() {
        const urlObject = new URL(document.URL);
		this.roleId = urlObject.pathname.split('/')[4 - 2];
		this.httpService = new httpService();
		this.service = new EditRoleService();
		this.permissionsByCategory = [];
		this.editRoleModel = new EditRoleModel();
		this.role = null;
		this.swal = new SwalUtil();
    }

    #loadRole() {
		const vw = this;
		this.service.getRole(this.roleId)
			.then((role) => {
				this.role = role;
			})
			.then(() => {
				const role = this.role;
				vw.#renderRoleDetails(role);
				vw.#loadPermissionsByCategory();
				vw.#bindEventHandlers();
			});
	}

	#loadPermissionsByCategory() {
		const vw = this;
		const categoryId = this.role.categoryId;
		this.service.getPermissionsByCategory(categoryId)
			.then((permissions) => {
				this.permissionsByCategory = permissions;
			})
			.then(() => {

				vw.#renderPermissions();
			});
	}

	#checkPermissionIfSelected(permissionId) {
		var checked = this.role.permissions.find(p => p.id === permissionId) ? 'checked' : '';
		return `<input class="form-check-input rolepermission" data-permission-id="${permissionId}"" type="checkbox" ${checked}>`;
	}
	
	#renderPermissions() {
		const permissionsByCategory = this.permissionsByCategory.chunk(6);
		const permissionsHtml = `<div class="row row-cols-1 row-cols-md-2 row-cols-xl-3 g-5 g-xl-9">
									${permissionsByCategory.map(pg => `
										<div class="col-md-4">
											${pg.map(p => `<div class="d-flex align-items-center position-relative mb-7">
												<div class="form-check form-check-custom form-check-solid ms-6 me-4">
													${this.#checkPermissionIfSelected(p.id)}
												</div>
												<!--end::Checkbox-->
												<!--begin::Details-->
												<div class="fw-bold">
													<span class="fs-6 fw-bolder text-gray-900">${p.description}</span>
												</div>
											</div>`).join('') }
										</div>
									`).join('')}
								</div>`;

		$("#dvPermissions").html(permissionsHtml);
	}

    #renderRoleDetails(role) {
		const roleDetailsHtml = `<div class="col-lg-12">
								<!--begin::Tasks-->
								<div class="card card-flush h-lg-100">
									<!--begin::Card header-->
									<div class="card-header mt-6">
										<!--begin::Card title-->
										<div class="card-title flex-column">
											<div class="input-group mb-5">
												<span class="input-group-text" id="basic-addon3">Role Name</span>
												<input type="text" class="form-control form-control-lg" id="txtEditRoleName" aria-describedby="basic-addon3" value="${role.name}">
											</div>
										</div>
										<!--end::Card title-->
										<!--begin::Card toolbar-->
										<div class="card-toolbar">
											<button class="btn btn-sm btn-primary me-2" id="btnSaveRole">Save Changes</button>
											<a href="/roles/${role.id}" class="btn btn-sm btn-light-danger">Cancel</a>
										</div>
										<!--end::Card toolbar-->
									</div>
									<!--end::Card header-->
									<!--begin::Card body-->
									<div class="card-body d-flex flex-column mb-9 p-9 pt-3">
										<!--begin::Item-->
										<h3 class="mb-10">Permissions:</h3>
										<div id="dvPermissions">
											
										</div>
									</div>
									<!--end::Card body-->
								</div>
								<!--end::Tasks-->
							</div>`;
		$("#dvEditRole").html(roleDetailsHtml);
	}

	#saveRole() {
		
		const selectedCheckboxes = document.querySelectorAll('.rolepermission:checked');
		const permissionIds = Array.from(selectedCheckboxes).map(checkbox => parseInt(checkbox.getAttribute('data-permission-id')));
		const roleName = $("#txtEditRoleName").val();

		const selectedPermissions = permissionIds.map(permissionId =>
			this.permissionsByCategory.find(p => p.id == permissionId)
		);

		this.editRoleModel.id = this.role.id;
		this.editRoleModel.name = roleName;
		this.editRoleModel.permissions = selectedPermissions;
		
		this.service.saveRole(this.editRoleModel)
			.then(() => {
				this.swal.alert("Role has been successfully updated!", () => { });
			});

	}

	#bindEventHandlers() {
		$("#btnSaveRole").click(() => {
			this.#saveRole();
		});
	}

	init() {
		this.#loadRole();
		//this.#loadRoleUsers();
    }
}

$(document).ready(() => {
	const view = new EditRoleView();
    view.init();
});