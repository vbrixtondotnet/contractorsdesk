class TemplateView {
    constructor() {
		this.proposalId = document.URL.split('/').pop();
		this.service = new TemplateService();
	}

	bindEventHandlers() {
		/*Event handlers here*/
	}

	loadData() {

	}

	saveData() {

	}

	updateData() {

	}

	init() {
		this.loadData();
		this.bindEventHandlers();
    }
}

$(document).ready(() => {
	const view = new TemplateView();
    view.init();
});