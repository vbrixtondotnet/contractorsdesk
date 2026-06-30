class ProposalTemplateModel {
    constructor() {
        this.id = 0;
        this.name = 0;
        this.lineItems = [];
    }
}
class LineItem {
    constructor() {
        this.id = Guid.empty;
        this.proposalTemplateId = Guid.empty;
        this.categoryName = '';
        this.name = '';
        this.description = '';
        this.parentId = 0;
        this.sequence = 0;
        this.amount = 0;
    }
}

