class ProjectSettingsModel {
    constructor() {
        this.id = '';
		this.description = '';
		this.openedDate = '';
		this.closedDate = '';
		this.notes = '';
		this.supervisors = [];
		this.allowedForJobReports = '';
		this.allowedForBudgetReports = '';
		this.activeJobs = '';
		this.activeSpecJobs = '';
    }
}

class ClientDetailsModel {
	constructor() {
		this.id = 0;
		this.projectId = "";
		this.name = "";
		this.fullName = "";
		this.companyName = "";
		this.address = "";
		this.phone= "";
		this.email= "";
	}
}
