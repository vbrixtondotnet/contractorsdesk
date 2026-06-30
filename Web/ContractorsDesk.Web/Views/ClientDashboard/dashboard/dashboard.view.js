class ClientDashboard extends DomEventComponent {
	constructor() {
		super();
	}

	#renderProjectRow(p) {
		const address = p.address == '' ? '&nbsp;' : p.address;
		const completionDate = p.estimatedCompletionDate == null ? 'N/A' : DateUtils.toStandardDate(p.estimatedCompletionDate);

		const renderSupervisor = (a, i) => {
			const ii = i > 2 ? 0 : i;
			const bg = ['dark', 'warning', 'primary'];

			return `<div class="symbol symbol-35px symbol-circle me-3" data-bs-toggle="tooltip" title="" data-bs-original-title="${a.firstName} ${a.lastName}">
						<span class="symbol-label bg-${bg[ii]} text-inverse-${bg[ii]} fw-bolder">${a.initials}</span>
					</div>`;
		};

		const getCompletionPercentage = (s, e) => {
			if (s == null || e == null) return 0;

			const now = new Date();
			const start = new Date(s);
			const end = new Date(e);

			const totalDuration = end - start;
			const elapsed = now - start;

			if (totalDuration <= 0) return 0;
			const percentage = Math.min(Math.max((elapsed / totalDuration) * 100, 0), 100);
			return percentage.toFixed(2);
		};

		const getStatusBadge = (p) => {
			return p.status == 'In Progress' ? 'badge-light-primary' : 'badge-secondary';
		}

		const completionPercent = getCompletionPercentage(p.startDate, p.estimatedCompletionDate);
		const supervisors = p.assignedSupervisors;
		const supervisorsHtml = supervisors.map((a, i) => { return renderSupervisor(a, i) }).join('');

		return `<div class="col-md-6 col-xl-4">
		<!--begin::Card-->
			<a href="/client-project/${p.id}" class="card border-hover-primary">
				<!--begin::Card header-->
				<div class="card-header border-0 pt-9">
					<!--begin::Card Title-->
					<div class="card-title m-0">
						<!--begin::Avatar-->
						<div class="symbol symbol-50px w-50px bg-light">
							<img src="assets/media/stock/600x400/img-56.jpg" alt="">
						</div>
						<!--end::Avatar-->
					</div>
					<!--end::Car Title-->
					<!--begin::Card toolbar-->
					<div class="card-toolbar">
						<span class="badge ${getStatusBadge(p)} fw-bolder me-auto px-4 py-3">${p.status}</span>
					</div>
					<!--end::Card toolbar-->
				</div>
				<!--end:: Card header-->
				<!--begin:: Card body-->
				<div class="card-body p-9">
					<!--begin::Name-->
					<div class="fs-3 fw-bolder text-dark">${p.name} </div>
					<!--end::Name-->
					<!--begin::Description-->
					<p class="text-gray-400 fw-bold fs-5 mt-1 mb-7">${address}</p>
					<!--end::Description-->
					<!--begin::Info-->
					<div class="d-flex flex-wrap mb-5">
						<!--begin::Due-->
						<div class="border border-gray-300 border-dashed rounded min-w-125px py-3 px-4 me-7 mb-3">
							<div class="fs-6 text-gray-800 fw-bolder">${completionDate}</div>
							<div class="fw-bold text-gray-400">Est. Completion Date</div>
						</div>
						<!--end::Due-->
						<!--begin::Budget-->
						<div class="border border-gray-300 border-dashed rounded min-w-125px py-3 px-4 mb-3">
							<div class="fs-6 text-gray-800 fw-bolder">${StringUtils.formatMoney(p.budget)}</div>
							<div class="fw-bold text-gray-400">Budget</div>
						</div>
						<!--end::Budget-->
					</div>
					<!--end::Info-->
					<!--begin::Progress-->
					<div class="h-4px w-100 bg-light mb-5" data-bs-toggle="tooltip" title="This project ${completionPercent}% completed">
						<div class="bg-primary rounded h-4px" role="progressbar" style="width: ${completionPercent}%" aria-valuenow="${completionPercent}" aria-valuemin="0" aria-valuemax="100"></div>
					</div>
					<!--end::Progress-->
					<!--begin::Users-->
					<div class="symbol-group symbol-hover">
						${supervisorsHtml}
					</div>
					<!--end::Users-->
				</div>
				<!--end:: Card body-->
			</a>
			<!--end::Card-->
		</div>`;
	}

	#renderProjects() {
		const projectRows = this.projects.map(p => {
			return this.#renderProjectRow(p);
		}).join('');

		$("#dv-client-dashboard").html(projectRows);

		this.#initKtAppEventHandlers();
	}

	#loadProjects() {
		this.httpService.get('/api/dashboard/client-projects')
			.then(data => {
				this.projects = data;
				this.#renderProjects();
			});
	}

	#initKtAppEventHandlers() {
		KTApp.initBootstrapTooltips();
		KTMenu.init();
		KTMenu.initGlobalHandlers();
	}
	
	init() {
		this.#loadProjects();
    }
}


$(document).ready(() => {
	const dashboard = new ClientDashboard();
	dashboard.init();
});