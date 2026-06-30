class ViewRoleView {
    constructor() {
        const urlObject = new URL(document.URL);
        this.roleId = urlObject.pathname.split('/').pop();
		this.httpService = new httpService();
		this.service = new ViewRoleService();
    }

    #loadRole() {
		this.service.getRole(this.roleId)
			.then((role) => { this.#renderRole(role); });
	}

	#loadRoleUsers() {
		this.service.getRoleUsers(this.roleId)
			.then((users) => { this.#renderRoleUsers(users); });
	}

	#renderUserRows(users) {
		return users.map(user => `
						<tr>
							<td class="d-flex align-items-center">
								<div class="symbol symbol-50px mx-5">
								  <div class="symbol-label fs-2 fw-bold text-success">BV</div>
								</div>
								<div class="d-flex flex-column">
									<a href="../../demo8/dist/apps/user-management/users/view.html" class="text-gray-800 text-hover-primary mb-1">
										${user.firstName} ${user.lastName}
									</a>
									<span>	${user.email}</span>
								</div>
								<!--begin::User details-->
							</td>
							<!--end::user=-->
							<!--begin::Joined date=-->
							<td>${user.createdDateString}</td>
							<!--end::Joined date=-->
							<!--begin::Action=-->
							<td class="text-end">
								<a href="#" class="btn-active-danger">
									<span class="badge badge-light-danger fs-7 fw-bolder">Remove</span>
								</a>
							</td>
							<!--end::Action=-->
					</tr>
					`).join('');
	}

	#renderRoleUsers(users) {
		const usersTableHtml = `<div class="card card-flush mb-6 mb-xl-9">
											<!--begin::Card header-->
											<div class="card-header">
												<!--begin::Card title-->
												<div class="card-title">
													<h2 class="d-flex align-items-center">Users Assigned
													<span class="text-gray-600 fs-6 ms-1">(${users.length})</span></h2>
												</div>
												<!--end::Card title-->
												<!--begin::Card toolbar-->
												<div class="card-toolbar">
													<!--begin::Search-->
													<div class="d-flex align-items-center position-relative my-1" data-kt-view-roles-table-toolbar="base">
														<!--begin::Svg Icon | path: icons/duotune/general/gen021.svg-->
														<span class="svg-icon svg-icon-1 position-absolute ms-6">
															<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
																<rect opacity="0.5" x="17.0365" y="15.1223" width="8.15546" height="2" rx="1" transform="rotate(45 17.0365 15.1223)" fill="black" />
																<path d="M11 19C6.55556 19 3 15.4444 3 11C3 6.55556 6.55556 3 11 3C15.4444 3 19 6.55556 19 11C19 15.4444 15.4444 19 11 19ZM11 5C7.53333 5 5 7.53333 5 11C5 14.4667 7.53333 17 11 17C14.4667 17 17 14.4667 17 11C17 7.53333 14.4667 5 11 5Z" fill="black" />
															</svg>
														</span>
														<!--end::Svg Icon-->
														<input type="text" data-kt-roles-table-filter="search" class="form-control form-control-solid w-250px ps-15" placeholder="Search Users"/>
														
													</div>
												</div>
											</div>
											<!--end::Card header-->
											<!--begin::Card body-->
											<div class="card-body pt-0">
												<!--begin::Table-->
												<table class="table align-middle table-row-dashed fs-6 gy-5 mb-0" id="kt_roles_view_table">
													<!--begin::Table head-->
													<thead>
														<!--begin::Table row-->
														<tr class="text-start text-muted fw-bolder fs-7 text-uppercase gs-0">
															<th class="min-w-150px">User</th>
															<th class="min-w-125px">Date Added</th>
															<th class="text-end min-w-100px">Actions</th>
														</tr>
														<!--end::Table row-->
													</thead>
													<!--end::Table head-->
													<!--begin::Table body-->
													<tbody class="fw-bold text-gray-600">
														${this.#renderUserRows(users)}
													</tbody>
													<!--end::Table body-->
												</table>
												<!--end::Table-->
											</div>
											<!--end::Card body-->
										</div>`;
		$("#dvRoleUsers").html(usersTableHtml);
		var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
		var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
			return new bootstrap.Tooltip(tooltipTriggerEl);
		});
	}

	#renderPermissions(permissions) {
		return `<div class="d-flex flex-column text-gray-600">
					${permissions.map(p =>
						`<div class="d-flex align-items-center py-2">
							<span class="bullet bg-primary me-3"></span>${p.description}
						</div>`).join('')}
				</div>`; 
	}

    #renderRole(role) {
		const rolesListHtml = `<div class="card card-flush">
											<!--begin::Card header-->
											<div class="card-header">
												<!--begin::Card title-->
												<div class="card-title">
													<h2 class="mb-0">${role.name}</h2>
												</div>
												<!--end::Card title-->
											</div>
											<!--end::Card header-->
											<!--begin::Card body-->
											<div class="card-body pt-0">
												<div class="fw-bolder text-gray-600 mb-5">Permissions:</div>
												${this.#renderPermissions(role.permissions)}
											</div>
											<!--end::Card body-->
											<!--begin::Card footer-->
											<div class="card-footer pt-0">
												<a href="/roles/${role.id}/edit" class="btn btn-primary btn-active-primary w-100">Edit Role</a>
											</div>
											<!--end::Card footer-->
										</div>`;
        $("#dvRoleDetails").html(rolesListHtml);
    }

    init() {
		this.#loadRole();
		this.#loadRoleUsers();
    }
}

$(document).ready(() => {
    const view = new ViewRoleView();
    view.init();
});