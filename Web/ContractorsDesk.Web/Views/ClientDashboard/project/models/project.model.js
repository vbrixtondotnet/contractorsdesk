class ProjectModel {
	constructor() {
		this.name = '';
		this.address = '';
		this.state = '';
		this.city = '';
		this.jobTypeId = null;
		this.statusId = null;
	}

	map(dto) {
		this.name = dto.name;
		this.address = dto.address;
		this.state = dto.state;
		this.city = dto.city;
	}
	mapAddress(dto) {
		this.address = dto.address;
		this.state = dto.state;
		this.city = dto.city;
	}
}
class ClientModel {
	constructor() {
		this.id = null;
		this.name = '';
		this.companyName = '';
		this.address = '';
		this.phone = '';
		this.emailAddress = '';
		this.state = '';
		this.city = '';
		this.secondaryEmailAddress = '';
	}

	map(dto) {
		this.id = dto.id;
		this.name = dto.name;
		this.companyName = dto.companyName;
		this.address = dto.address;
		this.phone = dto.phone;
		this.emailAddress = dto.emailAddress;
		this.state = dto.state;
		this.city = dto.city;
		this.secondaryEmailAddress = dto.secondaryEmailAddress;
	}
}

class ActionItemUpdateProject {
	constructor() {
		this.title = "";
		this.description = "";
		this.projectId = 0;
		this.actionTypeId = 0;
		this.supervisors = [];
		this.status = 1;
		this.dueDate = '';
		this.costChangeEstimateCategoryId = null;
		this.scheduleChangeTaskId = null;
		this.scheduleChangeNumberOfDays = 0;
		this.costChangeAmount = 0;
		this.scheduleChangeRequiresClientApproval = false;
		this.costChangeRequiresClientApproval = false;
	}
}

class ProjectActionItemModel {
	constructor() {
		this.title = "";
		this.description = "";
		this.projectId = 0;
		this.actionTypeId = 0;
		this.supervisors = [];
		this.status = 1;
		this.dueDate = '';
		this.costChangeEstimateCategoryId = null;
		this.scheduleChangeTaskId = null;
		this.scheduleChangeNumberOfDays = 0;
		this.costChangeAmount = 0;
		this.scheduleChangeRequiresClientApproval = false;
		this.costChangeRequiresClientApproval = false;
	}
}
class SubContractorModel {
	constructor() {
		this.id = "";
		this.name = "";
		this.address = "";
		this.city = "";
		this.state = "";
		this.company = "";
		this.email = "";
		this.phone = "";
		this.licenseNo = "";
		this.licenseExp = null; // Use string (ISO date) or null
		this.createdBy = 0;
		this.updatedBy = null;
		this.dateCreated = null; 
		this.dateUpdated = null; 
		this.isActive = true; // Default to true
	}
}