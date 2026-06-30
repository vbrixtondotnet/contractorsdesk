class RolesView {
    constructor() {
        this.httpService = new httpService();
    }

    #loadRoles() {
		this.httpService.get('/api/role-category/roles')
			.then((roles) => {
				this.#renderRoles(roles)
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

    #renderRoles(roles) {
		const rolesListHtml = `<div class="row row-cols-1 row-cols-md-2 row-cols-xl-3 g-5 g-xl-9">
	                              ${roles.map(role => `
                                        <div class="col-md-4">
										<!--begin::Card-->
										<div class="card card-flush h-md-100 geeks">
											<!--begin::Card header-->
											<div class="card-header">
												<!--begin::Card title-->
												<div class="card-title">
													<h2>${role.name}</h2>
												</div>
												<!--end::Card title-->
											</div>
											<!--end::Card header-->
											<!--begin::Card body-->
											<div class="card-body pt-1">
												<!--begin::Users-->
												<div class="fw-bolder text-gray-600 mb-5">Permissions:</div>
												<!--end::Users-->
												<!--begin::Permissions-->
												${this.#renderPermissions(role.permissions)}
												<!--end::Permissions-->
											</div>
											<!--end::Card body-->
											<!--begin::Card footer-->
											<div class="card-footer flex-wrap pt-0">
												<a href="/roles/${role.id}" class="btn btn-light btn-active-primary my-1 me-2">View Role</a>
												<a href="/roles/${role.id}/edit" class="btn btn-light btn-active-light-primary my-1">Edit Role</a>
											</div>
											<!--end::Card footer-->
										</div>
										<!--end::Card-->
									</div>
                                  `).join('')}
                               </div>`;
		$("#dvRolesView").html(rolesListHtml);
    }

    init() {
		this.#loadRoles();
    }
}

$(document).ready(() => {
    const rolesView = new RolesView();
    rolesView.init();
});