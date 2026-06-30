class ArchivedActionItemsView extends DomEventComponent {
	constructor() {
		super();
		this.onSearchActionItemTimeOut = null;
	}

	#renderActionItems(actionItems) {
		const rows = actionItems.map(a => {
			const createdBy = a.createdByUser.firstName + ' ' + a.createdByUser.lastName;
			const assignedTo = a.assignedSupervisors.map(s => { return `${s.firstName} ${s.lastName}`; }).join(', ');
			const dateCreated = DateUtils.formatDateWithTimeDifference(a.dateCreated);
			return `<tr class="odd">
							<td class="text-dark ps-1">${a.title}</td>
							<td>${a.actionTypeName}</td>
							<td>${createdBy}</td>
							<td>${assignedTo}</td>
							<td>${dateCreated}</td>
							<td class="text-center">
								<a href="javascript:" data-id="${a.id}" evt-click="onUnarchiveActionItem" class="btn btn-sm btn-light-primary" data-bs-toggle="tooltip" data-bs-trigger="hover" data-bs-original-title="Unarchive">
									<i class="bi bi-upload pe-0"></i>
								</a>
								<a href="javascript:" data-id="${a.id}" evt-click="onDeleteActionItem" class="btn btn-sm btn-light-danger" data-bs-toggle="tooltip" data-bs-trigger="hover" data-bs-original-title="Delete">
									<i class="bi bi-trash pe-0"></i>
								</a>
							</td>
						</tr>`;
		}).join('');

		$("#tbody-archived-action-items").html(rows);
		APP.initKtAppEventHandlers();

	}

	#loadActionItems() {
		APP.setLoadingIndicator("#dv-archived-action-items", true);
		this.httpService.get(`/api/action-items/archived`)
			.then((actionItems) => {
				this.actionItems = actionItems;
				this.#renderActionItems(this.actionItems);
				APP.setLoadingIndicator("#dv-archived-action-items", false);
			});
	}

	#searchActionItem() {
		const searchTerm = $(`input[evt-keyup="onSearchActionItem"]`).val().trim();
		const results = this.actionItems.filter(item => {
			const term = searchTerm.toLowerCase();

			const titleMatch = item.title.toLowerCase().includes(term);

			const supervisorMatch = item.assignedSupervisors?.some(s =>
				`${s.firstName} ${s.lastName}`.toLowerCase().includes(term)
			);

			return titleMatch || supervisorMatch;
		});

		this.#renderActionItems(results);
	}

	onUnarchiveActionItem(b) {
		const id = parseInt(b.getAttribute('data-id'));
		this.actionItems = this.actionItems.filter(a => a.id != id);
		this.httpService.patch(`/api/action-items/${id}/unarchive`);
		this.#renderActionItems(this.actionItems);
	}

	onDeleteActionItem(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		this.swal.confirm('Are you sure you want to permanently delete this action item?', () => {
			this.actionItems = this.actionItems.filter(a => a.id != id);
			this.httpService.delete(`/api/action-items/${id}`);
			this.#renderActionItems(this.actionItems);
		});
	}

	onSearchActionItem(b, e) {
		const instance = this;
		clearTimeout(this.onSearchActionItemTimeOut);
		this.onSearchActionItemTimeOut = setTimeout(() => { instance.#searchActionItem(); }, 500);
	}

	init() {
		this.#loadActionItems();
	}
}

$(document).ready(() => {
	const view = new ArchivedActionItemsView();
	view.init();
});