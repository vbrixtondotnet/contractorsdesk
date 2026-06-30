class ProposalModel {
    constructor() {
        this.id = 0;
        this.number = 0;
        this.date = '';
        this.proposalLines = [];
        this.client = null;
        this.project = null;
    }
}
class ProposalProjectModel {
    constructor() {
        this.name = '';
        this.address = '';
        this.description = '';
    }
}
class ProposalClientModel {
    constructor() {
        this.name = '';
        this.fullName = '';
        this.companyName = '';
        this.address = '';
        this.phone = '';
        this.email = '';
    }
}
