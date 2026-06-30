class ProjectsService {
    constructor() {
        this.httpService = new httpService();
    }

    loadProject(projectId) {
        return this.httpService.get('/api/projects/' + projectId);
    }

    getActionItems(projectId) {
        return this.httpService.get('/api/action-items/project/' + projectId);
    }
    loadSubContractors(projectId) {
        return this.httpService.get(`/api/projects/${projectId}/sub-contractors`);
    }
    loadSubContractorById(Id) {
        return this.httpService.get('/api/sub-contractors/' + Id);
    }
    addSubContractor(subContractor) {
        return this.httpService.post('/api/sub-contractors', subContractor);
    }
    updateSubContractor(subcontractor) {
        return this.httpService.put('/api/sub-contractors/',  subcontractor);
    }
    deleteSubContractor(Id) {
        return this.httpService.delete('/api/sub-contractors/' + Id);
    }

    loadSupervisorUsers() {
        return this.httpService.get('/api/roles/supervisors');
    }

    updateProject(project, projectId) {
        return this.httpService.put('/api/projects/' + projectId, project);
    }

    updateClientDetails(clientDetails, projectId) {
        return this.httpService.put('/api/projects/' + projectId + '/client', clientDetails);
    }

    loadUsersByManageJob() {
        return this.httpService.get('/api/getUsersByManageJob');
    }

    getUsersByPermission(id) {
        return this.httpService.get(`/api/permissions/${id}/users`);
    }

    sendEmail(body) {
        return this.httpService.post('/api/email/send/', body);
    }

    getClientDocuments(projectId) {
        return this.httpService.get(`/api/client-documents/${projectId}`);
    }
    
    loadSupervisorUsers() {
        return this.httpService.get(`/api/users/supervisors`);
    }

    loadProjectJournal(projectId) {
        return this.httpService.get(`/api/projects/${projectId}/project-journal`);
    }

    saveProjectJournal(projectId, journalData) {
        return this.httpService.put(`/api/projects/${projectId}/project-journal`, journalData);
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
    getProjectSupervisors(projectId) {
        return this.httpService.get(`/api/projects/${projectId}/supervisors`);
    }
    getProjects() {
        return this.httpService.get('/api/projects');
    }
    getEstimateCategoriesByProjectId(id) {
        return this.httpService.get(`/api/estimate-categories/project/${id}`);
    }
    getProjectScheduleByProjectId(id) {
        return this.httpService.get(`/api/schedule/${id}/construction-tasks`);
    }
    createActionItem(actionItem) {
        return this.httpService.post('/api/action-items', actionItem);
    }
    getActionTypes() {
        return this.httpService.get('/api/action-types');
    }
    archiveActionItem(id) {
        return this.httpService.patch(`/api/action-items/${id}/archive`);
    }
    deleteActionItem(id) {
        return this.httpService.delete(`/api/action-items/${id}`);
    }
}

