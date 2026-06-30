class CompanySettingsService {
    constructor() {
        this.httpService = new httpService();
    }

    loadCompanySettings() {
        return this.httpService.get('/api/company/settings');
    }

    saveCompanySettings(model) {
        return this.httpService.post('/api/company/settings', model);
    }

    
}
