class PermissionsView {
    constructor() { 
        this.httpService = new httpService();
    }
    
    #loadPermissions(){
        const pv = this;
        this.httpService.get('/api/permissions', 
            function (data){ 
                pv.#renderUserTable(data) 
            }, 
            ()=>{},
            ()=>{}
        );
    }

    #renderUserTable(data) {
        const tableDefinition = {
            cellSize: 'small',
            pageSize: 10,
            searchbar: true,
            recordName: 'Permission',
            addButton: true,
            dataSet: data,
            colDefs : [
                {
                    label: "Permissions",
                    data: "description",
                    cellStyle: "min-w-125px"
                },
                {
                    label: "Created On",
                    data: "dateCreatedShortString",
                    cellStyle: "min-w-125px"
                },
                {
                    label: "Created By",
                    data: "createdByUser",
                    cellStyle: "min-w-125px"
                },
            ],
            actions: [
                {
                    label: 'Edit',
                    command: `alert('Edit!');`
                },
                {
                    label: 'Delete',
                    command: `alert('Delete!');`
                }
            ],
            formFields: [
                { label: "Description", data: "description" }
            ],
            saveurl: '/api/permissions',
            onsavecallback: (data) => {
                console.log(data);
            }
        };
        const table = new Table(tableDefinition);
        table.render("#dvPermissions");
    }

    init() {
        this.#loadPermissions();
    }
}

$(document).ready(() => {
    const pv = new PermissionsView();
    pv.init();
});