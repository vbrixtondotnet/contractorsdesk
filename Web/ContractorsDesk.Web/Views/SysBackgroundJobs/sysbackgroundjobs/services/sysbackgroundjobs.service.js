class SysBackgroundJobsService {
    constructor() {
        this.httpService = new httpService();
    }
    loadAllSysBackgroundJobs() {
        return this.httpService.get('/api/sysbackgroundjobs');
    }
    loadSysBackgroundJob(id) {
        return this.httpService.get('/api/sysbackgroundjobs' + id);

    }
    createSysBackgroundJobs(sysbackgroundjobs) {
        return this.httpService.post('/api/sysbackgroundjobs', sysbackgroundjobs);
    }
    executeSysBackgroundJob()
    {
        return this.httpService.post('/api/sysbackgroundjobs/quickbooksdatasync');
    }

}
