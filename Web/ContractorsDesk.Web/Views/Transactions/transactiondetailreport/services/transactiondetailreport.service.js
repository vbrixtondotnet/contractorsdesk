class ReviseEstimateService {
    constructor() {
        this.httpService = new httpService();
    }

    loadProposal(id) {
        return this.httpService.get(`/api/proposals/${id}/details`);
    }

    loadRevisedEstimate(id) {
        return this.httpService.get('/api/revised-estimates/' + id);
    }
    
    saveRevisedEstimate(proposalId, categories) {
        return this.httpService.post('/api/revised-estimates/' + proposalId, categories);
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
