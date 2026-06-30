class ViewRoleService {
    constructor() {
        this.httpService = new httpService();
    }

    getRole(roleId) {
        return this.httpService.get('/api/roles/' + roleId);
    }

    getRoleUsers(roleId) {
        return this.httpService.get('/api/roles/' + roleId + '/users');
    }
    
}
