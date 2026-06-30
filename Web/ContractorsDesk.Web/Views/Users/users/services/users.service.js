class UserService {
    constructor() {
        this.httpService = new httpService();
    }

    getCompanyUsers() {
        return this.httpService.get('/api/company/users');
    }
    getUsersFromCurrentRoleCategory() {
        return this.httpService.get('/api/role-category/users');
        
    }
    getRolesFromCurrentRoleCategory() {
        return this.httpService.get('/api/role-category/roles');
    }

    createUser(user) {
        return this.httpService.post('/api/users', user);
    }

    updateUser(user) {
        return this.httpService.put('/api/users', user);
    }

    deleteUser(user) {
        return this.httpService.delete('/api/users', user);
    }

    getUsersByManageJob()
    {
        return this.httpService.get('/api/getUsersByManageJob');
    }
    
}
