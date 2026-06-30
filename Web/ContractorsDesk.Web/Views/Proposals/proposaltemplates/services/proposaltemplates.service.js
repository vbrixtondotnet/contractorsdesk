class ProposalTemplateService {
    constructor() {
        this.httpService = new httpService();
    }

    loadProposalTemplate(id) {
        return this.httpService.get(`/api/proposal-templates/${id}`);
    }

    loadTemplates() {
        return this.httpService.get('/api/proposal-templates');
    }

    loadLineItems(id) {
        return this.httpService.get(`/api/proposal-templates/${id}/line-items`);
    }

    saveProposalTemplate(proposalTemplate, id = '') {
        if (id === '')
            return this.httpService.post('/api/proposal-templates', proposalTemplate);
        else
            return this.httpService.put('/api/proposal-templates/' + id, proposalTemplate);
    }

    getLineItemsByName(name) {
        return this.httpService.get(`/api/estimate-categories/${name}`);
    }

    getEstimateCategories() {
        ///api/estimate-items
        return this.httpService.get(`/api/estimate-categories/all`);
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
