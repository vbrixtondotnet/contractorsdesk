class DashboardModel {
    constructor() {
        this.tasks = [];
    }
}

class ActionItem {
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