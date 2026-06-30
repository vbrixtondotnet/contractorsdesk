class SubContractorListView extends DomEventComponent {
	constructor() {
		super();
	}

	#initSubcontractorListView() {
		this.subcontractorList = new SubContractorList();
		this.subcontractorList.enableAdd = true;
		this.subcontractorList.loadSubContractors();
		this.subcontractorList.onSaveCallback = (data) => {
			this.subcontractorList.subContractors.unshift(data);
			this.subcontractorList.renderList();
		}
		this.subcontractorList.render();
	}

	init() {
		this.#initSubcontractorListView();
	}
}

$(document).ready(() => {
	const view = new SubContractorListView();
	view.init();
});