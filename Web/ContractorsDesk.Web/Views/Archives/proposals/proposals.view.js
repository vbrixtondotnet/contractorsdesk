class ArchivedProposalsView extends DomEventComponent {
	constructor() {
		super();
		this.onSearchProposalTimeOut = null;
	}

	#renderProposals(proposals) {
	
		const rows = proposals.map(a => {
			const client = a.client ?? '';
			const assignedTo = a.supervisors ? a.supervisors.map(s => { return `${s.firstName} ${s.lastName}`; }).join(', ') : '';
			return `<tr class="odd">
							<td class="text-dark ps-1">${a.project}</td>
							<td>${client}</td>
							<td>${assignedTo}</td>
							<td class="text-center">
								<a href="javascript:" data-id="${a.id}" evt-click="onUnarchiveProposal" class="btn btn-sm btn-light-primary" data-bs-toggle="tooltip" data-bs-trigger="hover" data-bs-original-title="Unarchive">
									<i class="bi bi-upload pe-0"></i>
								</a>
								<a href="javascript:" data-id="${a.id}" evt-click="onDeleteProposal" class="btn btn-sm btn-light-danger" data-bs-toggle="tooltip" data-bs-trigger="hover" data-bs-original-title="Delete">
									<i class="bi bi-trash pe-0"></i>
								</a>
							</td>
						</tr>`;
		}).join('');

		$("#tbody-archived-proposals").html(rows);
		APP.initKtAppEventHandlers();

	}

	#loadProposals() {
		APP.setLoadingIndicator("#dv-archived-proposals", true);
		this.httpService.get(`/api/proposals?status=archived`)
			.then((proposals) => {
				this.proposals = proposals;
				this.#renderProposals(this.proposals);
				APP.setLoadingIndicator("#dv-archived-proposals", false);
			});
	}

	#searchProposal() {
		const searchTerm = $(`input[evt-keyup="onSearchProposal"]`).val().trim();
		const results = this.proposals.filter(item => {
			const term = searchTerm.toLowerCase();

			const titleMatch = item.project.toLowerCase().includes(term);

			const supervisorMatch = item.supervisors?.some(s =>
				`${s.firstName} ${s.lastName}`.toLowerCase().includes(term)
			);

			return titleMatch || supervisorMatch;
		});

		this.#renderProposals(results);
	}

	onUnarchiveProposal(b) {
		const id = b.getAttribute('data-id');
		this.proposals = this.proposals.filter(a => a.id != id);
		this.httpService.patch(`/api/proposals/${id}/unarchive`);
		this.#renderProposals(this.proposals);
	}

	onDeleteProposal(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		this.swal.confirm('Are you sure you want to permanently delete this proposal?', () => {
			this.proposals = this.proposals.filter(a => a.id != id);
			this.httpService.delete(`/api/proposals/${id}`);
			this.#renderProposals(this.proposals);
		});
	}

	onSearchProposal(b, e) {
		const instance = this;
		clearTimeout(this.onSearchProposalTimeOut);
		this.onSearchProposalTimeOut = setTimeout(() => { instance.#searchProposal(); }, 500);
	}

	init() {
		this.#loadProposals();
	}
}

$(document).ready(() => {
	const view = new ArchivedProposalsView();
	view.init();
});