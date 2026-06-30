class ArchivedProjectsView extends DomEventComponent {
	constructor() {
		super();
		this.onSearchProjectTimeOut = null;
	}

	#renderProjects(projects) {
		const rows = projects.map(p => {
			const assignedTo = p.supervisors ? p.supervisors.map(s => { return `${s.firstName} ${s.lastName}`; }).join(', ') : '';
			const clientName = p.clientName ? p.clientName : '';
			return `<tr class="odd">
							<td class="text-dark ps-1">${p.name}</td>
							<td>${clientName}</td>
							<td>${assignedTo}</td>
							<td class="text-center">
								<a href="javascript:" data-id="${p.id}" evt-click="onUnarchiveProject" class="btn btn-sm btn-light-primary" data-bs-toggle="tooltip" data-bs-trigger="hover" data-bs-original-title="Unarchive">
									<i class="bi bi-upload pe-0"></i>
								</a>
								<a href="javascript:" data-id="${p.id}" evt-click="onDeleteProject" class="btn btn-sm btn-light-danger" data-bs-toggle="tooltip" data-bs-trigger="hover" data-bs-original-title="Delete">
									<i class="bi bi-trash pe-0"></i>
								</a>
							</td>
						</tr>`;
		}).join('');

		$("#tbody-archived-projects").html(rows);
		APP.initKtAppEventHandlers();

	}

	#loadArchivedProjects() {
		APP.setLoadingIndicator("#dv-archived-projects", true);
		this.httpService.get(`/api/projects/archived`)
			.then((projects) => {
				this.projects = projects;
				this.#renderProjects(this.projects);
				APP.setLoadingIndicator("#dv-archived-projects", false);
			});
	}

	#searchProject() {
		const searchTerm = $(`input[evt-keyup="onSearchProject"]`).val().trim();
		const results = this.projects.filter(item => {
			const term = searchTerm.toLowerCase();

			const titleMatch = item.name.toLowerCase().includes(term);

			const supervisorMatch = item.supervisors?.some(s =>
				`${s.firstName} ${s.lastName}`.toLowerCase().includes(term)
			);

			return titleMatch || supervisorMatch;
		});

		this.#renderProjects(results);
	}

	onUnarchiveProject(b) {
		const id = b.getAttribute('data-id');
		this.projects = this.projects.filter(a => a.id != id);
		this.httpService.patch(`/api/projects/${id}/unarchive`);
		this.#renderProjects(this.projects);
	}

	onDeleteProject(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		this.swal.confirm('Are you sure you want to permanently delete this project?', () => {
			this.projects = this.projects.filter(a => a.id != id);
			this.httpService.delete(`/api/projects/${id}`);
			this.#renderProjects(this.projects);
		});
	}

	onSearchProject(b, e) {
		const instance = this;
		clearTimeout(this.onSearchProjectTimeOut);
		this.onSearchProjectTimeOut = setTimeout(() => { instance.#searchProject(); }, 500);
	}

	init() {
		this.#loadArchivedProjects();
	}
}

$(document).ready(() => {
	const view = new ArchivedProjectsView();
	view.init();
});