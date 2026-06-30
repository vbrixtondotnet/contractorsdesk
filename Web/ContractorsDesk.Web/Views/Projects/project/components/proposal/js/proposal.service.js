class ProposalService {
    constructor() {
        this.httpService = new httpService();
    }

    loadProposal(id) {
        return this.httpService.get('/api/proposals/' + id);
    }

    checkProjectName(name) {
        return this.httpService.get('/api/proposals/project?name=' + name);
    }

    saveProposal(proposal) {
        if (proposal.id == Guid.empty)
            return this.httpService.post('/api/proposals', proposal);
        else
            return this.httpService.put('/api/proposals', proposal);
    }

    loadSupervisorUsers() {
        return this.httpService.get('/api/roles/supervisors');
    }

    loadTemplates() {
        return this.httpService.get('/api/proposal-templates');
    }

    loadTemplateLineItems(id) {
        return this.httpService.get(`/api/proposal-templates/${id}/line-items`);
    }

    getLineItemsByNameAndTemplate(templateId, itemName) {
        return this.httpService.get(`/api/proposals/templates/${templateId}/items/${itemName}`);
    }

    getProposalTemplateUserDefault() {
        return this.httpService.get('/api/proposals/templates/default/');
    }

    loadUsersByManageJob() {
        return this.httpService.get('/api/getUsersByManageJob');
    }

    isLineItemAdded(id, name) {
        return this.httpService.get(`/api/proposals/${id}/line-item?name=${name}`);
    }

    getCompanySettings() {
        return this.httpService.get(`/api/company/settings`);
    }

    getUsersByPermission(id) {
        return this.httpService.get(`/api/permissions/${id}/users`);
    }

    getEstimateCategories() {
        ///api/estimate-items
        return this.httpService.get(`/api/estimate-categories/all`);
    }

    sendEmail(body) {
        return this.httpService.post('/api/email/send/', body);
    }

    sendContract(id, contractModel) {
        return this.httpService.post(`/api/proposals/${id}/send-contract`, contractModel);
    }

    loadSupervisorUsers() {
        return this.httpService.get(`/api/users/supervisors`);
    }

    updateProposalStatus(id, status) {
        let statusText = 'Draft';

        if (status === 2) {
            statusText = 'Accepted';
        } else if (status === 3) {
            statusText = 'Archived';
        } 

        const payload = {
            id: id,
            docStatus: statusText
        };

        return this.httpService.patch(`/api/proposals/${id}/status`, payload);
    }

    saveProposalTemplate(proposalTemplate, id = '') {
        if (id === '')
            return this.httpService.post('/api/proposal-templates', proposalTemplate);
        else
            return this.httpService.put('/api/proposal-templates/' + id, proposalTemplate);
    }

    updateProposalIncludeZeroAmount(id) {
        return this.httpService.patch(`/api/proposals/${id}/include-zero-amount`);
    }
    
}
