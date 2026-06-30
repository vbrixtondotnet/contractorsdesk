class ProposalTemplatesService {
    constructor() {
        this.httpService = new httpService();
    }

    loadProposaTemplates() {
        return this.httpService.get('/api/proposal-templates');
    }

    loadProposalTemplateDefaultByUser() {
        return this.httpService.get('/api/proposal-templates/default');
    }

    saveProposalTemplateUserDefault(id) {
        return this.httpService.post('/api/proposal-templates/default/' + id);
    }

    //updateProposal(proposal) {
    //    return this.httpService.post('/api/proposals', proposal);
    //}

    
}
