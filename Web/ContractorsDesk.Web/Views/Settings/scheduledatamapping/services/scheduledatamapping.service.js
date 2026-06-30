class ScheduleDataMappingService {
    constructor() {
        this.httpService = new httpService();
    }

    loadConstructionTasks() {
        return this.httpService.get('/api/construction-tasks');
    }

    loadScheduleDataMappings()
    {
        return this.httpService.get('/api/schedule-data-mappings'); 
    }    

    saveScheduleDataMappings(mappings) {
        return this.httpService.post('/api/schedule-data-mappings', mappings);
    }
}
