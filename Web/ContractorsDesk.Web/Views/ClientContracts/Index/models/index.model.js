class ActionItemModel {
    constructor() {
    }
}

class ApplicationUserShortDetailsModel {
    constructor(id = 0, firstName = '', lastName = '') {
        this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
    }

    get initials() {
        const firstInitial = this.firstName?.charAt(0).toUpperCase() || '';
        const lastInitial = this.lastName?.charAt(0).toUpperCase() || '';
        return `${firstInitial}${lastInitial}`;
    }
}

class ActionItemSummaryModel {
    constructor() {
        this.id = null;
        this.projectId = null;
        this.title = '';
        this.description = '';
        this.projectName = '';
        this.dateCreated = null;
        this.dueDate = null;
        this.statusId = null;
        this.amount = '';
        this.actionTypeId = null;
        this.actionTypeName = '';
        this.estimateCategory = '';
        this.constructionTask = '';
        this.noOfDays = null;
        this.requiresClientApproval = false;
        this.assignedTo = null
        this.createdBy = null
    }
}

