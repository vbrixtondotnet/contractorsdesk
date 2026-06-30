class TemplateService {
    constructor() {
        this.httpService = new httpService();
    }

    loadTemplates() {
        return this.httpService.get('/api/proposal-templates');
    }

    saveProposalTemplateUserDefault(id) {
        return this.httpService.post('/api/proposal-templates/default/' + id);
    }

    getProposalTemplateUserDefault() {
        return this.httpService.get('/api/proposal-templates/default');
    }

    removeProposalTemplate(id) {
        return this.httpService.delete(`/api/proposal-templates/${id}`);
    }
    
}
