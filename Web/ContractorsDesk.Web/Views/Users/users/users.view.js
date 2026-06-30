class UsersView {
    constructor() {
		const urlObject = new URL(document.URL);
		this.templates = new UsersTemplates();
        this.service = new UserService();
        this.users = [];
        this.user = [];
        this.roleAssignments = [];
        this.swal = new SwalUtil();
        this.addUserForm = document.getElementById('frmUser');
        this.editUserForm = document.getElementById('frmEditUser');
        this.addUserFormValidator = FormValidation.formValidation(
            this.addUserForm,
            {
                fields: {
                    'firstName': {
                        validators: {
                            notEmpty: {
                                message: 'First Name is required'
                            }
                        }
                    },
                    'lastName': {
                        validators: {
                            notEmpty: {
                                message: 'Last Name is required'
                            }
                        }
                    },
                    'email': {
                        validators: {
                            notEmpty: {
                                message: 'Email is required'
                            }
                        }
                    },
                    password: {
                        validators: {
                            notEmpty: {
                                message: 'Password is required.'
                            }
                        }
                    },
                    'confirmPassword': {
                        validators: {
                            notEmpty: {
                                message: 'Confirm Password is required.'
                            }
                        }
                    },
                    'role': {
                        validators: {
                            notEmpty: {
                                message: 'Role is required.'
                            }
                        }
                    }
                },

                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
        this.editUserFormValidator = FormValidation.formValidation(
            this.editUserForm,
            {
                fields: {
                    'firstName': {
                        validators: {
                            notEmpty: {
                                message: 'First Name is required'
                            }
                        }
                    },
                    'lastName': {
                        validators: {
                            notEmpty: {
                                message: 'Last Name is required'
                            }
                        }
                    },
                    'email': {
                        validators: {
                            notEmpty: {
                                message: 'Email is required'
                            }
                        }
                    },
                    'role': {
                        validators: {
                            notEmpty: {
                                message: 'Role is required.'
                            }
                        }
                    }
                },

                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
    }

    #loadRoleAssignments() {
        this.service.getRolesFromCurrentRoleCategory()
            .then((roleAssignment) => {
                this.roleAssignments = roleAssignment.map(role => ({
                    id: role.id,
                    text: role.name // Rename 'name' to 'text'
                }));
            })
    }

    #loadUsers() {
        this.userLv.preload();
        this.service.getCompanyUsers()
            .then((users) => {
                this.users = users;
                return this.users;
            })
            .then((users) => {
                this.#loadRoleAssignments();
                const userRows = this.users.map(item => { return this.templates.userRow(item) }).join('');
                this.userLv.renderRows(userRows);
                KTApp.initBootstrapTooltips();
                KTMenu.init();
                KTMenu.initGlobalHandlers();
            });
    }
    async #renderProposalsListView() {
        const lvOptions = {
            title: 'Users',
            columnDefs: [
                { class: 'ps-1', title: 'User' },
                { class: 'min-w-125px', title: 'Role' },
                { class: 'min-w-100px', title: 'DateAdded' },
                { class: 'w-200px text-center', title: 'Actions' },
            ],
            onAdd: () => {
                $("#btnAddUserToggle").trigger('click');
                this.#onCreateRole();
            },
            shadow: true,
            minHeight: '400px'

        }
        this.userLv = new ListView(lvOptions);
        this.userLv.render('users-listview');
    }

    async renderListViews() {
        await this.#renderProposalsListView();
    }

    #onCreateRole() {
        const roleOptions = this.templates.options(this.roleAssignments, 'text');
        $("#slcAddUserRole").html(roleOptions);
        $("#slcAddUserRole option[value='10']").prop('disabled', true);
        $("#slcAddUserRole").select2();
    }

    #onEditRole() {
        const roleOptions = this.templates.options(this.roleAssignments, 'text');
        $("#slcEditUserRole").html(roleOptions);
        $("#slcEditUserRole option[value='10']").prop('disabled', true);
        $("#slcEditUserRole").select2();
    }

    #initEventHandlers() {
        const instance = this;

        $("#btnSaveUser").on('click', () => {
            this.#onSaveUser();
        });

        $("html").on("click", ".edit", function () {
            const userId = $(this).data("id");
            instance.#onEditUser(userId);
        });

        $("#btnUpdateUser").on('click', () => {
            this.#onUpdatetUser();
        });

        $("html").on("click", ".delete", function () {
            const userId = $(this).data("id");
            instance.#onDeleteUser(userId);
        });
    }

    #onEditUser(id)
    {
        this.#onEditRole();
        const matchedData = this.users?.find(item => item.id === id);

        if (matchedData) {
            this.user = matchedData;
            $("#editFirstName").val(matchedData.firstName || '');
            $("#editLastName").val(matchedData.lastName || '');
            $("#editEmail").val(matchedData.email || '');
            $("#slcEditUserRole").val(matchedData.roleId).trigger('change');
        }

        $("#btnEditUserToggle").trigger('click');
    }

    #onSaveUser() {
        const validator = this.addUserFormValidator;
        validator.validate().then((status) => {
            if (status == 'Valid') {

                const submitButton = document.getElementById('btnSaveUser');

                const getSelect2Value = (selector) => $(selector).select2('data')[0]?.id || null;
                const getValue = (selector) => $(selector).val();

                const user = new UsersModel();
                user.firstName = getValue("#firstName");
                user.lastName = getValue("#lastName");
                user.email = getValue("#email");
                user.password = getValue("#password");
                user.confirmPassword = getValue("#confirmPassword");
                user.roleId = getSelect2Value('#slcAddUserRole');

                submitButton.setAttribute('data-kt-indicator', 'on');
                submitButton.disabled = true;

                this.service.createUser(user)
                    .then((data) => {
                        this.swal.alert('User has been submitted successfully!', () => {
                            this.addUserForm.reset();
                            this.#loadUsers();
                            $("#kt_user_close").click();
                        });
                    })
                    .finally(() => {
                        submitButton.removeAttribute('data-kt-indicator');
                        submitButton.disabled = false;
                    })
                    ;
            }
        });
    }

    #onUpdatetUser() {
        const validator = this.editUserFormValidator;
        validator.validate().then((status) => {
            if (status == 'Valid') {

                const submitButton = document.getElementById('btnUpdateUser');

                const getSelect2Value = (selector) => $(selector).select2('data')[0]?.id || null;
                const getValue = (selector) => $(selector).val();

                const updateUserModel = new UpdateUserModel();
                updateUserModel.firstName = getValue("#editFirstName");
                updateUserModel.lastName = getValue("#editLastName");
                updateUserModel.email = getValue("#editEmail");
                updateUserModel.roleId = getSelect2Value('#slcEditUserRole');
                updateUserModel.id = this.user.id;

                submitButton.setAttribute('data-kt-indicator', 'on');
                submitButton.disabled = true;

                this.service.updateUser(updateUserModel)
                    .then((data) => {
                        this.swal.alert('User has been submitted successfully!', () => {
                            this.editUserForm.reset();
                            submitButton.removeAttribute('data-kt-indicator');
                            submitButton.disabled = false;
                            this.#loadUsers();
                            $("#kt_editUser_close").click();
                        });
                    });
            }
        });
    }

    #onDeleteUser(id) {
        this.swal.confirm("Would you like to delete this record?",
            async () => {
                const updateUserModel = new UpdateUserModel();
                updateUserModel.id = id;

                const data = await this.service.deleteUser(updateUserModel);

                this.swal.alert('User has been deleted successfully!', () => {
                    this.#loadUsers();
                });
            }
        );
    }

    init() {
        this.renderListViews().then(() => {
            this.#loadUsers();
            this.#initEventHandlers();
        });
    }
}

class UsersTemplates {
    constructor() { }

    options(arr, label) {
        let options = `<option value="">--Select Option--</option>`;
        options += arr.map(ar =>
            `<option value="${ar.id}">${ar[label]}</option>`
        ).join('');

        return options;
    }

    userRow(item) {
        const role = (r) => {
            const role = r == null ? '' : r.trim();
            let p = `<div class="badge badge-dark fw-bolder">${role}</div>`;
            switch (role.toUpperCase()) {
                case 'CUSTOMER SUPPORT':
                    html = html.replace('badge-dark', 'badge-primary');
                    break;
                case 'SUPER IT':
                    html = html.replace('badge-dark', 'badge-success');
                    break;
            }
            return p;
        }
        const actionButtons = (item) => {;
            return `<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                        Actions
                        <i class="fas fa-chevron-down"></i>
                    </a>
                    <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-125px py-4" data-kt-menu="true" style="">
                        
                        <div class="menu-item px-3">
                            <a href="javascript:" class="menu-link px-3 edit" data-action="Edit" data-id=${item.id}>Edit</a>
                        </div>
                        <div class="menu-item px-3">
                            <a href="javascript:" class="menu-link px-3 delete" data-action="Delete" data-id=${item.id}>Delete</a>
                        </div>
                    </div>`;
        };
        const userInfo = (item) => {
            let p = `<div class="d-flex align-items-center">
                <div class="symbol symbol-circle symbol-50px overflow-hidden me-3 p-4">
                    <div class="symbol-label fs-3 bg-dark text-white">${item.initials}</div>
                </div>
                <div class="d-flex flex-column text-gray-600 fw-bold">
                    <a href="#" class="text-gray-800 text-hover-primary mb-1">${item.fullName}</a>
                    <span>${item.email}</span>
                </div>
            </div>`;

            const role = item.role == null ? '' : item.role.trim().toUpperCase();
            switch (role) {
                case 'CUSTOMER SUPPORT':
                    html = html.replace('badge-dark', 'badge-primary');
                    break;
                case 'SUPER IT':
                    html = html.replace('badge-dark', 'badge-success');
                    break;
            }
            return p;
        }
        return `<tr data-id="${item.id}">
					<td class="py-1">
						${userInfo(item)}
					</td>
					<td class="py-1">${role(item.role)}</td>
                    <td class="py-1">${item.createdDateString}</td>
					<td class="text-center py-1">
						${actionButtons(item)}
					</td>
				</tr>`;
    }
}

$(document).ready(() => {
    const view = new UsersView();
    view.init();
});