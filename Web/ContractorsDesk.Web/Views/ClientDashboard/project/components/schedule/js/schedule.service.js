class ProjectScheduleService {
    constructor() {
        this.httpService = new httpService();
    }

    loadProject(projectId) {
        return this.httpService.get(`/api/projects/${projectId}/short-details`);
    }

    loadProposal(projectId) {
        return this.httpService.get(`/api/projects/${projectId}/proposal`);
    }

    loadSchedule(projectId) {
        return this.httpService.get(`/api/projects/${projectId}/schedule`);
    }

    loadUnscheduled(projectId) {
        return this.httpService.get(`/api/unschedule/${projectId}`);
    }

    saveSchedule(projectId, schedules) {
        return this.httpService.put(`/api/schedule/${projectId}`, schedules);
    }

    saveDelay(projectId, scheduleDelay) {
        return this.httpService.put(`/api/schedule/${projectId}/delay`, scheduleDelay);
    }

    sendEmail(body) {
        return this.httpService.post('/api/email/send/', body);
    }
    
}
