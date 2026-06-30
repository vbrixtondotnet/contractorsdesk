class ProposalManagementView {
    constructor() {
		this.httpService = new httpService();
		this.service = new ProposalManagementService();
		this.proposals = [];
		this.swal = new SwalUtil();
	}

	#bindEventHandlers() {
		var instance = this;
	}

	#getrows() {

		const status = (status) => {
			switch (status.toLowerCase()) {
				case 'draft':
					return `<span class="badge badge-secondary">${status}</span>`;
				case 'accepted':
					return `<span class="badge badge-light-success">${status}</span>`;
				case 'archived':
					return `<span class="badge badge-dark">${status}</span>`;
			}
		}

		return this.proposals.map(proposal => `
				<tr data-id="${proposal.id}">
					<td class="ps-9">${proposal.client}</td>
					<td class="">${proposal.project}</td>
					<td class="text-center">${proposal.number}</td>
					<td class="">${proposal.date}</td>
					<td class="">${StringUtils.formatMoney(proposal.totalAmount)}</td>
					<td class="">${status(proposal.docStatus)}</td>
					<td class="text-center">
						<div class="ms-auto">
							<a href="/proposals/${proposal.id}" class="btn btn-icon btn-light-warning" title="Edit Proposal">
								<span class="svg-icon svg-icon-muted svg-icon-2">
									<i class="bi bi-pencil"></i>
								</span>
							</a>
							<a href="#" class="btn btn-icon btn-light-danger" title="Delete Proposal">
								<!--begin::Svg Icon | path: assets/media/icons/duotune/general/gen014.svg-->
								<span class="svg-icon svg-icon-muted svg-icon-2">
									<i class="bi bi-trash"></i>

								</span>
								<!--end::Svg Icon-->
							</a>
							<a href="#" class="btn btn-icon btn-secondary" title="Archive Proposal">
								<span class="svg-icon svg-icon-muted svg-icon-2">
									<i class="bi bi-archive"></i>
								</span>
							</a>
						</div>
					</td>
				</tr>`).join('');
	}

	renderProposalsTable() {

		const rows = this.#getrows()

		$("#tbody-proposals").html(rows);
		$(".preloader").parents().removeClass('preload');
		$(".preloader").remove();
	}

	loadProposals() {
		this.service.loadProposals()
			.then((proposals) => {
				this.proposals = proposals;
			})
			.then(() => {
				this.renderProposalsTable();
			});
	}

	init() {
		this.loadProposals();
		this.#bindEventHandlers();
    }
}

$(document).ready(() => {
	const view = new ProposalManagementView();
    view.init();
});