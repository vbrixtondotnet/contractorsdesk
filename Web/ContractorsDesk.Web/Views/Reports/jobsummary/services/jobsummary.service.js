class JobSummaryService {
    constructor() {
        this.httpService = new httpService();
    }

    loadActiveJobs(id) {
        return this.httpService.get(`/api/reports/active-jobs`);
    }

    loadPendingJobs(id) {
        return this.httpService.get(`/api/reports/pending-jobs`);
    }
}
