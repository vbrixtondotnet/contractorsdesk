class DashboardService {
    constructor() {
        this.httpService = new httpService();
    }

    getProjects() {
        return this.httpService.get('/api/projects');
    }
    
    getActionTypes() {
        return this.httpService.get('/api/action-types');
    }

    getUserBookmarks() {
        return this.httpService.get('/api/user-bookmarks');
    }


    createActionItem(actionItem) {
        return this.httpService.post('/api/action-items', actionItem);
    }

    getProposals(status) {
        return this.httpService.get('/api/proposals?status=' + status);
    }

    loadSupervisorUsers() {
        return this.httpService.get('/api/roles/supervisors');
    }

    updateProposalStatus(id, status) {
        const payload = {
            id: id,
            docStatus: status
        };

        return this.httpService.patch(`/api/proposals/${id}/status`, payload);
    }

    loadUsersByManageJob() {
        return this.httpService.get('/api/getUsersByManageJob');
    }

    getUsersByPermission(id) {
        return this.httpService.get(`/api/permissions/${id}/users`);
    }

    removeProposal(id) {
        return this.httpService.delete(`api/proposals/${id}`)
    }

    getEstimateCategoriesByProjectId(id) {
        return this.httpService.get(`/api/estimate-categories/project/${id}`);
    }

    getProjectScheduleByProjectId(id) {
        return this.httpService.get(`/api/schedule/${id}/construction-tasks`);
    }
    
    getProjectSupervisors(projectId) {
        return this.httpService.get(`/api/projects/${projectId}/supervisors`);
    }
    
    deleteProject(id) {
        return this.httpService.delete(`/api/projects/${id}`);
    }

    archiveProject(id) {
        return this.httpService.patch(`/api/projects/${id}/archive`);
    }

    hasQuickBooksAccountConnected() {
        return this.httpService.get('/api/quickbooks');
    }

    loadActionItemSummary(id) {
        return this.httpService.get('/api/action-item/summary/' + id);
    }

    updateActionItem(id, actionItem) {
        return this.httpService.patch(`/api/action-item/update/${id}`, actionItem);
    }

    updateActionItemSupervisor(id, actionItem) {
        return this.httpService.patch(`/api/action-item/update/supervisors/${id}`, actionItem);
    }

    unArchiveProject(id) {
        return this.httpService.patch(`/api/projects/${id}/unarchive`);
    }
}
