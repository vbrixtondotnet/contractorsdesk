class ProposalManagementService {
    constructor() {
        this.httpService = new httpService();
    }

    loadProposals() {
        return this.httpService.get('/api/proposals');
    }

    saveProposal(proposal) {
        return this.httpService.post('/api/proposals', proposal);
    }

    //getPermissionsByCategory(categoryId) {
    //    return this.httpService.get('/api/permissions?categoryId=' + categoryId);
    //}

    //getRoleUsers(roleId) {
    //    return this.httpService.get('/api/roles/' + roleId + '/users');
    //}
    //saveRole(role) {
    //    debugger;
    //    return this.httpService.put('/api/roles', role);
    //}
    
}
