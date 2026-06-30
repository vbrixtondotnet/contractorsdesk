class ChangeOrderService {
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

    loadEstimateCategories() {
        return this.httpService.get(`/api/estimate-categories/all`);
    }

    saveRevisedEstimatesMapping(payload) {
        return this.httpService.post(`/api/revised-estimates/mapping`, payload);
    }

    sendEmail(body) {
        return this.httpService.post('/api/email/send/', body);
    }

    hasQuickBooksAccountConnected(categoryId) {
        return this.httpService.get('/api/quickbooks');
    }

}
