class CompanySettingsService {
    constructor() {
        this.httpService = new httpService();
    }

    loadConstructionTasks() {
        return this.httpService.get('/api/construction-tasks');
    }

    saveConstructionTask(payload) {
        return this.httpService.post('/api/construction-tasks', payload);
    }
}
