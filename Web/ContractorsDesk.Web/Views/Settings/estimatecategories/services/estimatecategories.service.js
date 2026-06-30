class EstimateCategoriesService {
    constructor() {
        this.httpService = new httpService();
    }

    loadEstimateCategories() {
        return this.httpService.get(`/api/estimate-categories/all`);
    }

    loadParentEstimateCategories() {
        return this.httpService.get('/api/estimate-data-mappings/parent-estimate-categories');
    }

    loadScheduleDataMappings()
    {
        return this.httpService.get('/api/schedule-data-mappings'); 
    }    

    saveEstimateCategories(estimateCategories) {
        return this.httpService.post('/api/estimate-data-mappings/parent-estimate-categories', estimateCategories);
    }

    saveNewEstimateCategory(estimateCategory) {
        return this.httpService.post('/api/estimate-categories', estimateCategory);
    }
}
