class TransactionDetailView extends DomEventComponent {
	constructor() {
		super();
		const parts = document.URL.split('/');
		this.proposalId = parts[parts.length - 3];
		this.estimateCategoryId = parts[parts.length - 2];
		this.parentEstimateCategoryId = parts[parts.length - 1];
		this.httpService = new httpService();
		this.proposal = null;
		this.swal = new SwalUtil();
		this.categories = [];
		this.printer = new printer();
	}

	async print(button) {
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;
		try {
			const baseUrl = `/api/print/transaction-details?proposalId=${this.proposalId}`;
			let apiUrl = this.estimateCategoryId != "null" ? `${baseUrl}&estimateCategoryId=${this.estimateCategoryId}` : `${baseUrl}&parentEstimateCategoryId=${this.parentEstimateCategoryId}`
			const response = await fetch(apiUrl);

			if (!response.ok) {
				throw new Error('Failed to fetch the PDF.');
			}
			//downloadFile(response);
			printFile(response);
		} catch (error) {
			console.error("Error:", error);
			alert("Failed to load the PDF for printing.");
		}
		finally {
			button.removeAttribute('data-kt-indicator');
			button.disabled = false;
		}
	}

	#buildStickyActions() {
		this.stickyActions = new StickyActions('min-w-lg-200px');
		this.stickyActions.actions = [
			new StickyActionsButton('<i class="bi bi-printer-fill text-white"></i> Print', null, 'primary', null, false,
				(b) => {
					this.print(b);
				}
			)
		];

		this.stickyActions.init();
	}

	init() {
		this.#buildStickyActions();
    }
}
$(document).ready(() => {
	const view = new TransactionDetailView();
	view.init();
});