class ProposalModel {
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
class ClientProjectModel {
    constructor() {
        this.id = null;
        this.projectName = '';
        this.description = '';
        this.sqFeet = null;
        this.clientName;
        this.address = '';
        this.city = '';
        this.state = '';
        this.companyName = '';
        this.emailAddress = '';
        this.phone = '';
    }
}
class ProjectModel {
    constructor() {
        this.name = '';
        this.address = '';
        this.description = '';
        this.state = '';
        this.city = '';
        this.sqFeet = null;
    }

    map(dto) {
        if (dto != null) {
            this.name = dto.name;
            this.address = dto.address;
            this.description = dto.description;
            this.state = dto.state;
            this.city = dto.city;
            this.sqFeet = dto.sqFeet;
        }
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
    }

    map(dto) {
        if (dto != null) {
            this.id = dto.id;
            this.name = dto.name;
            this.companyName = dto.companyName;
            this.address = dto.address;
            this.phone = dto.phone;
            this.emailAddress = dto.emailAddress;
            this.state = dto.state;
            this.city = dto.city;
        }
    }
}
