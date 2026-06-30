class TaskModel {
    constructor() {
        this.id = "";
        this.projectId = "00000000-0000-0000-0000-000000000000";
        this.constructionTaskId = "";
        this.name = "";
        this.sequence = 0;
        this.duration = 0;
        this.startDate = "";
        this.endDate = "";
        this.pred1 = "";
        this.lag1 = 0;
        this.pred2 = null;
        this.lag2 = 0;
        this.pred3 = null;
        this.lag3 = 0;
        this.startDateFormatted = '';
        this.endDateFormatted = '';
    }
}

class DelayModel {
    constructor() {
        this.id = Guid.empty;
        this.taskId = null;
        this.taskName = '';
        this.start = '';
        this.reason = '';
        this.description = '';
        this.days = 0;
        this.applyToOtherProjects = false;
    }
}