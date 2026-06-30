class EditRoleService {
    constructor() {
        this.httpService = new httpService();
    }

    getRole(roleId) {
        return this.httpService.get('/api/roles/' + roleId);
    }

    getPermissionsByCategory(categoryId) {
        return this.httpService.get('/api/permissions?categoryId=' + categoryId);
    }

    getRoleUsers(roleId) {
        return this.httpService.get('/api/roles/' + roleId + '/users');
    }
    saveRole(role) {
        debugger;
        return this.httpService.put('/api/roles', role);
    }
    
}
