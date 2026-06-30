class EstimateDataMappingService {
    constructor() {
        this.httpService = new httpService();
    }

    loadEstimateDataMappings() {
        return this.httpService.get('/api/estimate-data-mappings');
    }

    loadEstimateCategories() {
        return this.httpService.get(`/api/estimate-categories/all`);
    }

    saveEstimateDataMappings(mappings) {
        return this.httpService.post('/api/estimate-data-mappings', mappings);
    }

    //updateProposal(proposal) {
    //    return this.httpService.post('/api/proposals', proposal);
    //}

    
}
