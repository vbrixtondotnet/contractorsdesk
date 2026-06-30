class ReviseEstimateModel {
    constructor() {
        this.id = 0;
        this.number = 0;
        this.date = '';
        this.template = null;
        this.supervisors = [];
        this.client = null;
        this.project = null;
    }
}
class ReviseEstimateProjectModel {
    constructor() {
        this.name = '';
        this.address = '';
        this.description = '';
    }
}
class ReviseEstimateClientModel {
    constructor() {
        this.firstName = '';
        this.lastName = '';
        this.companyName = '';
        this.address = '';
        this.phone = '';
        this.email = '';
    }
}
