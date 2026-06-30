class UsersView {
    constructor() { }
    
    #renderUserTable() {
        const tableDefinition = {
            searchbar: true,
            addButton: true,
            recordName: 'Users',
            filters: [{ label: 'Role', dataCol: 'role' }],
            colDefs : [
                {
                    label: "User",
                    html: `<div class="symbol symbol-circle symbol-50px overflow-hidden me-3">
					            <a href="#">
						            <div class="symbol-label">
							            <img src="{avatar}" alt="SB" class="w-100">
						            </div>
					            </a>
				            </div>
                            <div class="d-flex flex-column">
					            <a href="#" class="text-gray-800 text-hover-primary mb-1">{fullname}</a>
					            <span>{emailAddress}</span>
				            </div>`,
                    cellStyle: "min-w-125px",
                    tdStyle: "d-flex align-items-center"
                },
                {
                    label: "Role",
                    html: `<div class="badge badge-dark fw-bolder">{role}</div>`,
                    onrenderHtml: (html, data) => {
                        switch (data.role.toUpperCase()) {
                            case 'SUPERVISOR':
                                html = html.replace('badge-dark', 'badge-primary');
                                break;
                            case 'BOOKKEEPER':
                                html = html.replace('badge-dark', 'badge-secondary');
                                break;
                        }
                        return html;
                    },
                    cellStyle: "min-w-125px"
                },
                { label: "Last Login", html: `<div class="badge badge-light fw-bolder">{lastLogin}</div>`, cellStyle: "min-w-125px", },
                { label: "Joined Date", data: "joinedDate", cellStyle: "min-w-100px" }
            ],
            dataSet : [
                { fullname: "2 Ray Villanueva", role: "Administrator", lastLogin: "5 Hours Ago", joinedDate: "12/12/2024", avatar: "assets/media/avatars/150-4.jpg", emailAddress: "brix@gmail.com" },
                { fullname: "2 Ray Villanueva", role: "Supervisor", lastLogin: "4 Hours Ago", joinedDate: "11/12/2024", avatar: "assets/media/avatars/150-4.jpg", emailAddress: "brix2@gmail.com" },
                { fullname: "3 Ray Villanueva", role: "Bookkeeper", lastLogin: "30 Mins Ago", joinedDate: "10/12/2024", avatar: "assets/media/avatars/150-4.jpg", emailAddress: "brix3@gmail.com" },
                { fullname: "4 Ray Villanueva", role: "Bookkeeper", lastLogin: "40 Seconds Ago", joinedDate: "10/12/2024", avatar: "assets/media/avatars/150-4.jpg", emailAddress: "brix3@gmail.com" },
            ],
            formFields: [
                { label: "First Name", data: "firstName" },
                { label: "Last Name", data: "lastName" },
                { label: "Email Address", data: "emailAddress" },
                { label: "Password", data: "password" }
            ]
        };
        const table = new Table(tableDefinition);
        table.render("#dvUsers");
    }

    init() {
        this.#renderUserTable();
    }
}

$(document).ready(() => {
    const usersView = new UsersView();
    usersView.init();
});