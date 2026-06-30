class Dashboard extends DomEventComponent {
	constructor() {
		super();
		this.service = new DashboardService();
		this.templates = new DashboardTemplates();
		this.companySettings = Auth.getCompanySettings();
		this.userSession = Auth.currentUser;
		this.projects = [];
		this.actionItems = [];
		this.bookmarks = [];
		this.constructionTasks = [];
		this.estimateCategories = [];
		this.onsearchTimeOut = null;
		this.filteredActionListStatuses = [];
		this.proposals = [];
		this.supervisors = [];
		this.currentProjects = [];
		this.dashboardStats = null;
		this.onSearchProjectTimeOut = null;
		this.onSearchProposalTimeOut = null;
		this.onSearchActionItemTimeOut = null;
		this.currentActionItemFilter = 'all';
		this.currentProposalFilter = 'all';
		this.currentProjectsFilter = 'all';
	}
	
	#initEventHandlers() {
		const instance = this;
		$("#kt_datepicker_3").flatpickr({
			minDate: new Date(),
			enableTime: true,
			dateFormat: "Y-m-d h:i K",
			time_24hr: false
		});
		
		$("html").on("click", ".menu-trigger", function () {
			$(this).next().toggleClass('show');
		});
		$("html").on("click", "a.proposal-action", function () {
			var dataAction = $(this).attr("data-action");
			var dataId = $(this).attr("data-id");
			instance.#updateProposalStatus(dataId, dataAction);
		});
		
	}
	#loadProposals(status) {
		//return;
		//this.proposalsLv.preload();

		this.service.getProposals(status)
			.then((proposals) => {
				this.proposals = proposals;
				this.#renderProposals(proposals);
				//this.#renderProposalsList(proposals);
				//this.proposalsLv.onsearch?.();
				//KTApp.initBootstrapTooltips();
				//KTMenu.init();
				//KTMenu.initGlobalHandlers();
			});
	}
	#loadActionItems() {
		this.httpService.get('/api/dashboard/action-items')
			.then(response => {
				this.actionItems = response;
				this.#renderActionItems(this.actionItems);
			});
	}
	#loadDashboardStats() {
		let preloadIntervals = [];

		// Start the random counting animation
		const startStatAnimation = ()=> {
			const preloadElements = document.querySelectorAll('[data-preload="true"]');
			preloadElements.forEach((el) => {
				const interval = setInterval(() => {
					const randomValue = Math.floor(Math.random() * 900) + 100; // 100–999
					el.textContent = randomValue;
				}, 10); // 1ms for fast counting
				preloadIntervals.push({ el, interval });
			});

		}

		// Stop animation and show actual value
		const stopStatAnimation = ()=> {
			preloadIntervals.forEach(({ el, interval }) => {
				clearInterval(interval);
			});
			preloadIntervals = []; // Clear for reuse
		}
		
		startStatAnimation();
		this.httpService.get('/api/dashboard/stats')
			.then(response => {
				stopStatAnimation();
				this.dashboardStats = response;
				this.#renderStatistics();
			});
	}
	#updateProposalStatus(id, action) {
		let status;
		const accept = action == 'accept';

		switch (action) {
			case 'draft':
				status = 'Draft';
				break;
			case 'archive':
				status = 'Archived';
				break;
			default:
				status = 'Accepted';
				break;
		}
		const onUpdateProposal = () => {
			this.service.updateProposalStatus(id, status)
				.then((data) => {
					const proposal = this.proposals.find(item => item.id === id);
					proposal.docStatus = status;
					const updatedRow = this.templates.proposalRow(proposal);
					$(`tr[data-id="${id}"]`).replaceWith(updatedRow);
					KTApp.initBootstrapTooltips();
					KTMenu.init();
					KTMenu.initGlobalHandlers();
					if (accept) APP.loadProjects();
				});
		}

		const onRemoveProposal = () => {
			this.service.removeProposal(id)
				.then((data) => {
					this.#loadProposals('all');
					this.swal.alert('Proposal has been removed successfully!');
				})
		}

		if (action == 'accept') {
			this.swal.confirm('This proposal will now be moved to your Active Projects. Would you like to proceed?', () => { onUpdateProposal() });
		}
		else if (action == 'remove'){
			this.swal.confirm('Are you sure you want to delete this proposal?', () => { onRemoveProposal() });
		}
		else {
			onUpdateProposal();
		}
	}
	#populateProjectStatusDropdown() {
		$('#listview-widget-active-projects-filter-options-status').select2({
			data: [
				{ id: 0, text: 'All', class: 'text-primary' },
				{ id: 1, text: 'Above Threshold', class: 'text-success' },
				{ id: 2, text: 'Below Threshold', class: 'text-warning' },
				{ id: 3, text: 'Negative Balance', class: 'text-danger' }
			],
			templateResult: function (data) {
				if (!data.id) return data.text; // for placeholders
				return $('<span>').text(data.text).addClass(data.class);
			},
			templateSelection: function (data) {
				if (!data.id) return data.text;
				return $('<span>').text(data.text).addClass(data.class);
			}
		});
	}
	#populateActionItemStatusDropdown() {
		$('#listview-widget-actionitems-filter-options-status').select2({
			data: [
				{ id: 0, text: 'All', class: 'text-primary' },
				{ id: 1, text: 'Not Started', class: 'text-muted' },
				{ id: 2, text: 'For Review', class: 'text-info' },
				{ id: 3, text: 'In Progress', class: 'text-primary' },
				{ id: 4, text: 'Pending Client Response', class: 'text-danger' },
				{ id: 5, text: 'Client Approved', class: 'text-warning' },
				{ id: 6, text: 'Completed', class: 'text-success' },
				{ id: 7, text: 'Archived', class: 'text-dark' }
			],
			templateResult: function (data) {
				if (!data.id) return data.text; // for placeholders
				return $('<span>').text(data.text).addClass(data.class);
			},
			templateSelection: function (data) {
				if (!data.id) return data.text;
				return $('<span>').text(data.text).addClass(data.class);
			}
		});
	}
	onOpenSchedule(b, e) {
		const id = $(b).attr('data-id');
		e.preventDefault();
		e.stopPropagation();

		location.href = `/project/${id}?t=schedule`;
	}
	onOpenRevisedEstimate(b, e) {
		const id = $(b).attr('data-id');
		e.preventDefault();
		e.stopPropagation();

		location.href = `/project/${id}?t=estimate-to-actual`;
	}
	onSearchProject(b, e) {
		const instance = this;
		clearTimeout(this.onSearchProjectTimeOut);
		this.onSearchProjectTimeOut = setTimeout(() => { instance.#searchProject(); }, 500);
	}
	onSearchProposal(b, e) {
		const instance = this;
		clearTimeout(this.onSearchProposalTimeOut);
		this.onSearchProposalTimeOut = setTimeout(() => { instance.#searchProposal(); }, 500);
	}
	onSearchActionItem(b, e) {
		const instance = this;
		clearTimeout(this.onSearchActionItemTimeOut);
		this.onSearchActionItemTimeOut = setTimeout(() => { instance.#searchActionItem(); }, 500);
	}
	onFilterProjects(b) {
		const $selectedFilterText = $(b).text();
		const $container = $(b).closest('.ms-auto'); // find the container
		const $selectedSpan = $container.find('.selected-filter');
		const $filterText = $container.find('a.filter-text');
		const $activeClass = b.getAttribute('data-class-active');

		this.currentProjectsFilter = $selectedFilterText.trim().toLowerCase();
		$filterText.removeClass().addClass(`btn btn-light btn-sm filter-text text-${$activeClass}`);
		$selectedSpan.text($selectedFilterText);
		this.#searchProject();

	}
	onFilterProposals(b, e) {
		const status = b.getAttribute('data-value');
		
		const $container = $(b).closest('.ms-auto'); // find the container
		const $selectedSpan = $container.find('.selected-filter');
		const $filterText = $container.find('a.filter-text');
		const $activeClass = b.getAttribute('data-class-active');

		this.currentProposalFilter = status.toLowerCase();

		this.#searchProposal();

		$filterText.removeClass().addClass(`btn btn-light btn-sm filter-text text-${$activeClass}`);
		$selectedSpan.text(status)
	}
	onFilterActionItems(b, e) {
		const status = b.getAttribute('data-value');

		const $selectedFilterText = $(b).text();
		const $container = $(b).closest('.ms-auto');
		const $selectedSpan = $container.find('.selected-filter');
		const $filterText = $container.find('a.filter-text');
		const $activeClass = b.getAttribute('data-class-active');

		this.currentActionItemFilter = status.toLowerCase();
		this.#searchActionItem();

		$filterText.removeClass().addClass(`btn btn-light btn-sm filter-text text-${$activeClass}`);
		$selectedSpan.text($selectedFilterText);
		
	}
	onMarkAcceptedProposal(b, e) {
		e.preventDefault();
		e.stopPropagation();

		const proposalId = b.getAttribute('data-id');
		const proposal = this.proposals.find(item => item.id == proposalId);

		const missingFields = [];
		if (proposal.client == '') missingFields.push('Client Name');
		if (proposal.clientEmailAddress == '') missingFields.push('Client Email Address');

		if (missingFields.length) {
			this.swal.info(`Unable to accept proposal. ${missingFields.join(' and ')} ${missingFields.length > 1 ? 'are' : 'is'} missing. Please update the proposal with a valid client before proceeding.`);
			return;
		}

		const status = "Accepted";

		Swal.fire({
			title: 'Are you sure?',
			text: 'An Accepted proposal cannot be reverted to Draft. Are you sure you want to proceed?',
			icon: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Yes, Proceed',
			cancelButtonText: 'Cancel',
			reverseButtons: true,
			showLoaderOnConfirm: true,
			preConfirm: () => {
				Swal.update({
					title: 'Accepting Proposal',
					text: 'Please wait...',
					icon: 'info',
					showCancelButton: false,
					showConfirmButton: false
				});

				return this.service.updateProposalStatus(proposalId, status)
					.then(data => {
						proposal.docStatus = status;
						const updatedRow = this.#proposalRow(proposal);
						$(`div#listview-widget-proposals`)
							.find(`a.proposal-item[data-id="${proposalId}"]`)
							.replaceWith(updatedRow);

						this.projects.unshift(data);
						this.#renderProjects(this.projects);
						this.#initKtAppEventHandlers();
					})
					.catch(error => {
						Swal.showValidationMessage(`Request failed: ${error}`);
					});
			},
			allowOutsideClick: () => !Swal.isLoading()
		}).then(result => {
			if (result.isConfirmed) {
				Swal.fire({
					title: 'Success',
					text: 'Proposal has been Accepted!',
					icon: 'success'
				});
			}
		});
		
	}
	onArchiveProject(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const projectId = b.getAttribute('data-id');
		this.projects = this.projects.filter(item => item.id !== projectId);
		this.service.archiveProject(projectId);
		this.#searchProject();
	}
	onDeleteProject(b, e) {
		e.preventDefault();
		e.stopPropagation();
		this.swal.confirm('Are you sure you want to delete this project?', () => {
			const projectId = b.getAttribute('data-id');
			this.projects = this.projects.filter(item => item.id !== projectId);
			this.service.deleteProject(projectId);
			this.#searchProject();
		});
	}
	onArchiveProposal(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const proposalId = b.getAttribute('data-id');
		this.proposals = this.proposals.filter(a => a.id != proposalId);
		this.httpService.patch(`/api/proposals/${proposalId}/archive`);
		this.#searchProposal();
	}
	onDeleteProposal(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		this.swal.confirm('Are you sure you want to delete this proposal?', () => {
			this.proposals = this.proposals.filter(a => a.id != id);
			this.httpService.delete(`/api/proposals/${id}`);
			this.#searchProposal();
		});
	}
	onArchiveActionItem(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		this.actionItems = this.actionItems.filter(a => a.id != id);
		this.httpService.patch(`/api/action-items/${id}/archive`);
		this.#searchActionItem();
	}
	onDeleteActionItem(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		this.swal.confirm('Are you sure you want to delete this action item?', () => {
			this.actionItems = this.actionItems.filter(a => a.id != id);
			this.httpService.delete(`/api/action-items/${id}`);
			this.#searchActionItem();
		});
	}
	onToggleStatistics(b, e) {
		const $panel = $("#dv-dashboard-statistics");
		const $button = $(".btn-toggle-stats"); // or use $(b) if already defined

		$panel.stop(true, true).slideToggle('fast', function () {
			const isShown = $panel.is(':visible');

			if (isShown) {
				$button
					.addClass('shown btn-outline-default')
					.removeClass('btn-outline-primary')
					.contents().filter(function () {
						return this.nodeType === 3; // text node
					}).last().replaceWith('Hide Statistics');
			} else {
				$button
					.removeClass('shown btn-outline-default')
					.addClass('btn-outline-primary')
					.contents().filter(function () {
						return this.nodeType === 3;
					}).last().replaceWith('Show Statistics');
			}

			$button.blur();
		});

		//$("#dv-dashboard-statistics").toggleClass('d-none');

		//const isShown = !$("#dv-dashboard-statistics").hasClass('d-none');

		//if (isShown) {
		//	$(b).addClass('shown');
		//	$(b).removeClass('btn-outline-primary');
		//	$(b).addClass('btn-outline-default');
		//}
		//else {
		//	$(b).removeClass('shown');
		//	$(b).removeClass('btn-outline-default');
		//	$(b).addClass('btn-outline-primary');
		//}

		//$(b).blur();
	}
	onStartActionItem(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		const actionItem = this.actionItems.find(a => a.id == id);
		actionItem.status = "In Progress";
		this.httpService.patch(`/api/action-items/${id}/accept`);
		this.#filterActionItems();
	}
	onReviewActionItem(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");

		location.href = `/action-items/${id}`;
	}
	onCompleteActionItem(b, e) {
		e.preventDefault();
		e.stopPropagation();
		const id = $(b).attr("data-id");
		const actionItem = this.actionItems.find(a => a.id == id);
		actionItem.status = "Completed";
		this.httpService.patch(`/api/action-items/${id}/complete`);
		this.#filterActionItems();
	}
	#projectRow(item) {
		const trimWithEllipses = (str, len) => {
			if (typeof str !== "string") return "";
			return str.length > len ? str.substring(0, len) + "..." : str;
		}
		const projectManagerIcons = (projectManagers) => {
			return projectManagers.map(pm => `<span class="fs-9 badge badge-circle badge-dark me-1"
					 data-bs-toggle="tooltip"
					 title=""
					 data-bs-original-title="${pm.firstName} ${pm.lastName}">
				  ${pm.initials}
				</span>`
			).join('');
		};
		const formatToDollars = (amount) => {
			return `$${amount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}`;
		}
		const formatBalance = (item) => {
			let cssClass = '';
			const jobBalance = item.jobBalance;
			const threshold = item.threshold;
			let jobBalanceText = formatToDollars(Math.abs(jobBalance));
			if (jobBalance < 0) {
				cssClass = 'text-danger';
				jobBalanceText = `-${jobBalanceText}`;
			} else if (jobBalance < threshold) {
				cssClass = 'text-warning';
			} else {
				cssClass = 'text-success';
			}
			return `<span href="/revised-estimates/${item.proposalId}" class="fw-boldest fs-5 ${cssClass}">${jobBalanceText}</span>`;
		}
		const actionMenu = (item) => {
			return `<div class="mt-5">
						<div class="ms-auto">
							<span href="#" class="btn btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
								<i class="fas fa-chevron-down"></i>
							</span>
							<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true" style="">
								<div class="menu-item px-3">
									<span data-id="${item.id}" evt-click="onOpenRevisedEstimate"  class="menu-link px-3 text-warning">
										<span class="svg-icon svg-icon-warning svg-icon-1x">
											${doutune.file1} 
										</span>
										&nbsp;Manage Estimates
									</span>
								</div>
								<div class="menu-item px-3">
									<span data-id="${item.id}" evt-click="onOpenSchedule" class="menu-link px-3 text-primary">
										<span class="svg-icon svg-icon-primary svg-icon-1x">
											${doutune.calendar} 
										</span>
										&nbsp;Manage Schedule
									</span>
								</div>
								<div class="menu-item px-3">
									<span class="menu-link px-3" evt-click="onArchiveProject" data-id="${item.id}">
										<span class="svg-icon svg-icon-muted svg-icon-1x">
											${doutune.archive} 
										</span>
										&nbsp;Archive
									</span>
								</div>
								<div class="menu-item px-3">
									<span class="menu-link px-3" evt-click="onDeleteProject" data-id="${item.id}">
										<span class="svg-icon svg-icon-danger svg-icon-1x">
											${doutune.trash}
										</span>
										&nbsp;Delete
									</span>
								</div>
							</div>
						</div>
					</div>`;
		}

		return `<a href="/project/${item.id}">
						<div class="d-flex mb-2 p-3 bg-hover-light-primary border-secondary border-bottom-1 border-bottom-dashed">
							<!-- Symbol -->
							<div class="symbol flex-shrink-0 me-4">
								<img src="${item.photoUrl}" class="mw-100" alt="">
							</div>
							<div class="flex-grow-1">
								<!-- Top row: Title + Balance -->
								<div class="d-flex align-items-start justify-content-between pe-3">
									<div>
										<span class="text-gray-800 fw-boldest fs-7 text-hover-primary fw-bolder" data-bs-toggle="tooltip" title="" data-bs-original-title="${item.name}">${trimWithEllipses(item.name, 20)}</span>
										  <!-- Owner -->
										<div class="text-gray-400 fw-bold fs-8 d-block m-0">
											Owner:<br>
											<span class="text-dark fw-bold">${trimWithEllipses(item.clientName, 20)}</span>
										</div>

										<!-- Project Managers -->
										<div class="text-gray-400 fw-bold fs-8 mt-2 d-block">
											<span class="mb-1 d-flex">Project Manager(s):</span>
											${projectManagerIcons(item.supervisors)}
										</div>
									</div>
									<div class="text-end">
										${formatBalance(item)}
										<span class="text-gray-400 fs-7 fw-bold d-block">Balance</span>
										${actionMenu(item)}
									</div>
								</div>
							</div>
						</div>

					</a>`;
	}
	#proposalRow(item) {
		const trimWithEllipses = (str, len) => {
			if (typeof str !== "string") return "";
			return str.length > len ? str.substring(0, len) + "..." : str;
		}
		const renderActionMenu = (item) => {

			const markAcceptedButton = (item) => {
				return item.docStatus.toLowerCase() != 'accepted' ? `<div class="menu-item px-3">
												<span data-id="${item.id}" evt-click="onMarkAcceptedProposal" class="menu-link px-3 text-success">
													<span class="svg-icon svg-icon-success svg-icon-1x">
														<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
															<path opacity="0.3" d="M10.3 14.3L11 13.6L7.70002 10.3C7.30002 9.9 6.7 9.9 6.3 10.3C5.9 10.7 5.9 11.3 6.3 11.7L10.3 15.7C9.9 15.3 9.9 14.7 10.3 14.3Z" fill="black"></path>
															<path d="M22 12C22 17.5 17.5 22 12 22C6.5 22 2 17.5 2 12C2 6.5 6.5 2 12 2C17.5 2 22 6.5 22 12ZM11.7 15.7L17.7 9.70001C18.1 9.30001 18.1 8.69999 17.7 8.29999C17.3 7.89999 16.7 7.89999 16.3 8.29999L11 13.6L7.70001 10.3C7.30001 9.89999 6.69999 9.89999 6.29999 10.3C5.89999 10.7 5.89999 11.3 6.29999 11.7L10.3 15.7C10.5 15.9 10.8 16 11 16C11.2 16 11.5 15.9 11.7 15.7Z" fill="black"></path>
														</svg>
													</span>
													&nbsp;Mark as Accepted
												</span>
											</div>` : '';
			}

			return `<div class="mt-10">
									<div class="ms-auto">
										<span href="#" class="btn btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
											<i class="fas fa-chevron-down"></i>
										</span>
										<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true" style="">
											${markAcceptedButton(item)}
											<div class="menu-item px-3">
												<span data-id="${item.id}" class="menu-link px-3" evt-click="onArchiveProposal">
													<span class="svg-icon svg-icon-muted svg-icon-1x">
														<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
															<rect opacity="0.3" width="12" height="2" rx="1" transform="matrix(-1 0 0 1 15.5 11)" fill="black"></rect>
															<path d="M13.6313 11.6927L11.8756 10.2297C11.4054 9.83785 11.3732 9.12683 11.806 8.69401C12.1957 8.3043 12.8216 8.28591 13.2336 8.65206L16.1592 11.2526C16.6067 11.6504 16.6067 12.3496 16.1592 12.7474L13.2336 15.3479C12.8216 15.7141 12.1957 15.6957 11.806 15.306C11.3732 14.8732 11.4054 14.1621 11.8756 13.7703L13.6313 12.3073C13.8232 12.1474 13.8232 11.8526 13.6313 11.6927Z" fill="black"></path>
															<path d="M8 5V6C8 6.55228 8.44772 7 9 7C9.55228 7 10 6.55228 10 6C10 5.44772 10.4477 5 11 5H18C18.5523 5 19 5.44772 19 6V18C19 18.5523 18.5523 19 18 19H11C10.4477 19 10 18.5523 10 18C10 17.4477 9.55228 17 9 17C8.44772 17 8 17.4477 8 18V19C8 20.1046 8.89543 21 10 21H19C20.1046 21 21 20.1046 21 19V5C21 3.89543 20.1046 3 19 3H10C8.89543 3 8 3.89543 8 5Z" fill="#C4C4C4"></path>
														</svg>
													</span>
													&nbsp;Archive
												</span>
											</div>
											<div class="menu-item px-3">
												<span data-id="${item.id}" class="menu-link px-3" evt-click="onDeleteProposal">
													<span class="svg-icon svg-icon-danger svg-icon-1x">
														<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
															<path d="M5 9C5 8.44772 5.44772 8 6 8H18C18.5523 8 19 8.44772 19 9V18C19 19.6569 17.6569 21 16 21H8C6.34315 21 5 19.6569 5 18V9Z" fill="black"></path>
															<path opacity="0.5" d="M5 5C5 4.44772 5.44772 4 6 4H18C18.5523 4 19 4.44772 19 5V5C19 5.55228 18.5523 6 18 6H6C5.44772 6 5 5.55228 5 5V5Z" fill="black"></path>
															<path opacity="0.5" d="M9 4C9 3.44772 9.44772 3 10 3H14C14.5523 3 15 3.44772 15 4V4H9V4Z" fill="black"></path>
														</svg>
													</span>
													&nbsp;Delete
												</span>
											</div>
										</div>
									</div>
								</div>`;
		}
		const renderStatusBadge = (status) => {
			const statusText = status.toUpperCase();
			let badgeClass = 'badge-secondary';
			switch (statusText) {
				case 'DRAFT':
					badgeClass = 'badge-secondary';
					break;
				case 'ACCEPTED':
					badgeClass = 'badge-success';
					break;
			}
			return `<span class="badge ${badgeClass} fw-bold fs-8 px-2 cursor-default">${statusText}</span>`;
		}
		const renderCreatedBy = (item) => {
			if (item.createdBy == null) return '';

			return `<span class="fs-9 badge badge-circle badge-dark me-1" data-bs-toggle="tooltip" title="" data-bs-original-title="${item.createdBy.firstName} ${item.createdBy.lastName}">
						${item.createdBy.initials}
					</span>`;
		}
		
		const proposalUrl = item.docStatus == 'Draft' ? `/proposals/${item.id}` : `/project/${item.qbClassId}?t=proposal`;
		return `<a href="${proposalUrl}" data-id="${item.id}" class="proposal-item">
				<div class="d-flex mb-2 p-3 bg-hover-light-primary border-secondary border-bottom-1 border-bottom-dashed">
					<!-- Symbol -->
					<div class="symbol flex-shrink-0 me-4">
						<img src="assets/media/logos/proposal-logo.png" class="mw-100" alt="">
					</div>
					<div class="flex-grow-1">
						<!-- Top row: Title + Balance -->
						<div class="d-flex align-items-start justify-content-between pe-3">
							<div>
								<span class="text-gray-800 fw-boldest fs-7 text-hover-primary fw-bolder" data-bs-toggle="tooltip" title="" data-bs-original-title="${item.project}">${trimWithEllipses(item.project, 20)}</span>
								<!-- Owner -->
								<div class="text-gray-400 fw-bold fs-8 d-block m-0">
									Client:<br>
									<span class="text-dark fw-bold">${trimWithEllipses(item.client, 15)}</span>
								</div>

								<!-- Project Managers -->
								<div class="text-gray-400 fw-bold fs-8 mt-2 d-block">
									<span class="mb-1 d-flex">Estimator:</span>
									${renderCreatedBy(item)}
								</div>
							</div>
							<div class="text-end">
								${renderStatusBadge(item.docStatus)}
								${renderActionMenu(item)}
							</div>
						</div>
					</div>
				</div>

			</a>`;
	}
	#actionItemRow(item) {
		const trimWithEllipses = (str, len) => {
			if (typeof str !== "string") return "";
			return str.length > len ? str.substring(0, len) + "..." : str;
		}
		const renderAssignedTo = (item) => {
			const supervisors = item.assignedSupervisors || [];
			return supervisors.map(s => `
                <span class="fs-9 badge badge-circle badge-dark me-1" data-bs-toggle="tooltip" title="" data-bs-original-title="${s.firstName} ${s.lastName}">
                    ${s.initials}
                </span>
            `).join('');
		}
		const renderCreatedBy = (item) => {
			const createdBy = item.createdByUser;
			return `<span class="fs-9 badge badge-circle badge-secondary me-1" data-bs-toggle="tooltip" title="" data-bs-original-title="${createdBy.firstName} ${createdBy.lastName}">
                    ${createdBy.initials}
                </span>`;
		}
		const renderActionMenu = (item) => {

			const archiveMenuItem = (i) => {
				return `<div class="menu-item px-3">
							<span data-id="${i.id}" class="menu-link px-3" evt-click="onArchiveActionItem">
								<span class="svg-icon svg-icon-muted svg-icon-1x">
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										<rect opacity="0.3" width="12" height="2" rx="1" transform="matrix(-1 0 0 1 15.5 11)" fill="black"></rect>
										<path d="M13.6313 11.6927L11.8756 10.2297C11.4054 9.83785 11.3732 9.12683 11.806 8.69401C12.1957 8.3043 12.8216 8.28591 13.2336 8.65206L16.1592 11.2526C16.6067 11.6504 16.6067 12.3496 16.1592 12.7474L13.2336 15.3479C12.8216 15.7141 12.1957 15.6957 11.806 15.306C11.3732 14.8732 11.4054 14.1621 11.8756 13.7703L13.6313 12.3073C13.8232 12.1474 13.8232 11.8526 13.6313 11.6927Z" fill="black"></path>
										<path d="M8 5V6C8 6.55228 8.44772 7 9 7C9.55228 7 10 6.55228 10 6C10 5.44772 10.4477 5 11 5H18C18.5523 5 19 5.44772 19 6V18C19 18.5523 18.5523 19 18 19H11C10.4477 19 10 18.5523 10 18C10 17.4477 9.55228 17 9 17C8.44772 17 8 17.4477 8 18V19C8 20.1046 8.89543 21 10 21H19C20.1046 21 21 20.1046 21 19V5C21 3.89543 20.1046 3 19 3H10C8.89543 3 8 3.89543 8 5Z" fill="#C4C4C4"></path>
									</svg>
								</span>
								&nbsp;Archive
							</span>
						</div>`;
			}

			const deleteMenuItem = (i) => {
				return `<div class="menu-item px-3">
							<span data-id="${i.id}" class="menu-link text-danger px-3" evt-click="onDeleteActionItem">
								<span class="svg-icon svg-icon-danger svg-icon-1x">
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										<path d="M5 9C5 8.44772 5.44772 8 6 8H18C18.5523 8 19 8.44772 19 9V18C19 19.6569 17.6569 21 16 21H8C6.34315 21 5 19.6569 5 18V9Z" fill="black"></path>
										<path opacity="0.5" d="M5 5C5 4.44772 5.44772 4 6 4H18C18.5523 4 19 4.44772 19 5V5C19 5.55228 18.5523 6 18 6H6C5.44772 6 5 5.55228 5 5V5Z" fill="black"></path>
										<path opacity="0.5" d="M9 4C9 3.44772 9.44772 3 10 3H14C14.5523 3 15 3.44772 15 4V4H9V4Z" fill="black"></path>
									</svg>
								</span>
								&nbsp;Delete
							</span>
						</div>`;
			}

			const startMenuItem = (i) => {
				return i.status == 'Not Started' ? `<div class="menu-item px-3">
							<span data-id="${i.id}" class="menu-link text-success px-3" evt-click="onStartActionItem">
								<span class="svg-icon svg-icon-success svg-icon-1x">
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										<path opacity="0.3" d="M4.05424 15.1982C8.34524 7.76818 13.5782 3.26318 20.9282 2.01418C21.0729 1.98837 21.2216 1.99789 21.3618 2.04193C21.502 2.08597 21.6294 2.16323 21.7333 2.26712C21.8372 2.37101 21.9144 2.49846 21.9585 2.63863C22.0025 2.7788 22.012 2.92754 21.9862 3.07218C20.7372 10.4222 16.2322 15.6552 8.80224 19.9462L4.05424 15.1982ZM3.81924 17.3372L2.63324 20.4482C2.58427 20.5765 2.5735 20.7163 2.6022 20.8507C2.63091 20.9851 2.69788 21.1082 2.79503 21.2054C2.89218 21.3025 3.01536 21.3695 3.14972 21.3982C3.28408 21.4269 3.42387 21.4161 3.55224 21.3672L6.66524 20.1802L3.81924 17.3372ZM16.5002 5.99818C16.2036 5.99818 15.9136 6.08615 15.6669 6.25097C15.4202 6.41579 15.228 6.65006 15.1144 6.92415C15.0009 7.19824 14.9712 7.49984 15.0291 7.79081C15.0869 8.08178 15.2298 8.34906 15.4396 8.55884C15.6494 8.76862 15.9166 8.91148 16.2076 8.96935C16.4986 9.02723 16.8002 8.99753 17.0743 8.884C17.3484 8.77046 17.5826 8.5782 17.7474 8.33153C17.9123 8.08486 18.0002 7.79485 18.0002 7.49818C18.0002 7.10035 17.8422 6.71882 17.5609 6.43752C17.2796 6.15621 16.8981 5.99818 16.5002 5.99818Z" fill="black"/>
										<path d="M4.05423 15.1982L2.24723 13.3912C2.15505 13.299 2.08547 13.1867 2.04395 13.0632C2.00243 12.9396 1.9901 12.8081 2.00793 12.679C2.02575 12.5498 2.07325 12.4266 2.14669 12.3189C2.22013 12.2112 2.31752 12.1219 2.43123 12.0582L9.15323 8.28918C7.17353 10.3717 5.4607 12.6926 4.05423 15.1982ZM8.80023 19.9442L10.6072 21.7512C10.6994 21.8434 10.8117 21.9129 10.9352 21.9545C11.0588 21.996 11.1903 22.0083 11.3195 21.9905C11.4486 21.9727 11.5718 21.9252 11.6795 21.8517C11.7872 21.7783 11.8765 21.6809 11.9402 21.5672L15.7092 14.8442C13.6269 16.8245 11.3061 18.5377 8.80023 19.9442ZM7.04023 18.1832L12.5832 12.6402C12.7381 12.4759 12.8228 12.2577 12.8195 12.032C12.8161 11.8063 12.725 11.5907 12.5653 11.4311C12.4057 11.2714 12.1901 11.1803 11.9644 11.1769C11.7387 11.1736 11.5205 11.2583 11.3562 11.4132L5.81323 16.9562L7.04023 18.1832Z" fill="black"/>
									</svg>
								</span>
								&nbsp;Start
							</span>
						</div>` : '';
			}

			const reviewMenuItem = (i) => {
				return i.status == 'For Review' ? `<div class="menu-item px-3">
							<span data-id="${i.id}" class="menu-link text-info px-3" evt-click="onReviewActionItem">
								<span class="svg-icon svg-icon-info svg-icon-1x">
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										<path d="M21.7 18.9L18.6 15.8C17.9 16.9 16.9 17.9 15.8 18.6L18.9 21.7C19.3 22.1 19.9 22.1 20.3 21.7L21.7 20.3C22.1 19.9 22.1 19.3 21.7 18.9Z" fill="black"/>
										<path opacity="0.3" d="M11 20C6 20 2 16 2 11C2 6 6 2 11 2C16 2 20 6 20 11C20 16 16 20 11 20ZM11 4C7.1 4 4 7.1 4 11C4 14.9 7.1 18 11 18C14.9 18 18 14.9 18 11C18 7.1 14.9 4 11 4ZM8 11C8 9.3 9.3 8 11 8C11.6 8 12 7.6 12 7C12 6.4 11.6 6 11 6C8.2 6 6 8.2 6 11C6 11.6 6.4 12 7 12C7.6 12 8 11.6 8 11Z" fill="black"/>
									</svg>
								</span>
								&nbsp;Review
							</span>
						</div>` : '';
			}

			const completeMenuItem = (i) => {
				const completableStatuses = ['Client Approved', 'In Progress', 'Client Acknowledged'];
			
				return completableStatuses.includes(i.status) ? `<div class="menu-item px-3">
							<span data-id="${i.id}" class="menu-link text-primary px-3" evt-click="onCompleteActionItem">
								<span class="svg-icon svg-icon-primary svg-icon-1x">
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										<rect opacity="0.3" x="2" y="2" width="20" height="20" rx="10" fill="black"/>
										<path d="M10.4343 12.4343L8.75 10.75C8.33579 10.3358 7.66421 10.3358 7.25 10.75C6.83579 11.1642 6.83579 11.8358 7.25 12.25L10.2929 15.2929C10.6834 15.6834 11.3166 15.6834 11.7071 15.2929L17.25 9.75C17.6642 9.33579 17.6642 8.66421 17.25 8.25C16.8358 7.83579 16.1642 7.83579 15.75 8.25L11.5657 12.4343C11.2533 12.7467 10.7467 12.7467 10.4343 12.4343Z" fill="black"/>
									</svg>
								</span>
								&nbsp;Complete
							</span>
						</div>` : '';
			}

			return `<div class="mt-10">
									<div class="ms-auto">
										<span href="#" class="btn btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
											<i class="fas fa-chevron-down"></i>
										</span>
										<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true" style="">
											${startMenuItem(item)}
											${reviewMenuItem(item)}
											${completeMenuItem(item)}
											${archiveMenuItem(item)}
											${deleteMenuItem(item)}
										</div>
									</div>
								</div>`;
		}
		const renderBadges = (item) => {
			const status = item.status;
			const statusText = status.toUpperCase();
			const actionType = item.actionTypeName.toUpperCase();
			let badgeClass = 'badge-secondary';
			let statusIconClass = '';
			let actionTypeBadgeClass = 'badge-muted';
			let actionTypeIconClass = 'bi-question-square-fill';

			switch (statusText) { 
				case 'NOT STARTED':
					badgeClass = 'badge-secondary';
					statusIconClass = 'bi-dash-circle';
					break;
				case 'FOR REVIEW':
					badgeClass = 'badge-info';
					statusIconClass = 'bi-eye';
					break;
				case 'IN PROGRESS':
					badgeClass = 'badge-primary';
					statusIconClass = 'bi-arrow-repeat';
					break;
				case 'PENDING CLIENT RESPONSE':
				case 'PENDING CLIENT ACKNOWLEDGEMENT':
					badgeClass = 'badge-danger';
					statusIconClass = 'bi-envelope-open';
					break;
				case 'COMPLETED':
					badgeClass = 'badge-success';
					statusIconClass = 'bi-check-circle-fill';
					break;
				case 'CLIENT APPROVED':
				case 'CLIENT ACKNOWLEDGED':
					badgeClass = 'badge-warning';
					statusIconClass = 'bi-check-circle';
					break;
			}

			switch (actionType) {
				case "COST CHANGE":
					actionTypeBadgeClass = "badge-primary";
					actionTypeIconClass = 'bi-currency-dollar';
					break;
				case "NOTE":
					actionTypeBadgeClass = "badge-success";
					actionTypeIconClass = 'bi-card-text';
					break;
			}

			const actionTypeIcon = `<span class="badge ${actionTypeBadgeClass} fw-bold fs-8 px-2 cursor-default" data-bs-toggle="tooltip" title="" data-bs-original-title="${actionType}">
										<i class="bi ${actionTypeIconClass} text-white"></i></span>`;

			const statusIcon = `<span class="badge ${badgeClass} fw-bold fs-8 px-2 cursor-default" data-bs-toggle="tooltip" title="" data-bs-original-title="${statusText}">
									<i class="bi ${statusIconClass} text-white"></i>
								</span>`;

			return `${actionTypeIcon} ${statusIcon}`;
		}

		return `<a href="/action-items/${item.id}" data-id="${item.id}" class="action-item">
				<div class="d-flex mb-2 p-3 ps-5 bg-hover-light-primary border-secondary border-bottom-1 border-bottom-dashed">
					<div class="flex-grow-1">
					<div class="text-gray-500 fw-semibold fs-8 pe-3 mb-2 text-right">
						${DateUtils.formatDateWithTimeDifference(item.dateCreated)}
						</div>
						<div class="d-flex align-items-start justify-content-between pe-3">
							<div>
								<span class="text-gray-800 fw-boldest fs-7 text-hover-primary fw-bolder" data-bs-toggle="tooltip" title="" data-bs-original-title="${item.title}">
								${trimWithEllipses(item.title, 30)}
								</span>
								<div class="text-gray-400 fw-bold fs-7 d-block m-0">
									<span>${trimWithEllipses(item.projectName, 30)}</span>
								</div>
								<div class="d-flex gap-5 mt-2">
								  <!-- Created By -->
								  <div class="d-flex flex-column text-gray-400 fw-bold fs-8">
									<span>Created By:</span>
									 <div class="mt-1 d-flex flex-wrap gap-1 justify-content-center">
										${renderCreatedBy(item)}
									 </div>
								  </div>

								  <!-- Assigned To -->
								  <div class="d-flex flex-column text-gray-400 fw-bold fs-8">
									<span>Assigned To:</span>	
									<div class="mt-1 d-flex flex-wrap gap-1  justify-content-center">
										${renderAssignedTo(item)}
									</div>
								  </div>
								</div>
							</div>
							<div class="text-end">
								${renderBadges(item)}
								${renderActionMenu(item)}
							</div>
						</div>
					</div>
				</div>

			</a>`;
	}
	#filterProjects(projects = null) {
		let filteredProjects = projects ?? [...this.projects];
		const status = this.currentProjectsFilter;

		switch (status) {
			case 'above threshold': // Above Threshold
				filteredProjects = filteredProjects.filter(p => p.jobBalance >= p.threshold);
				break;
			case 'below threshold': // Below Threshold
				filteredProjects = filteredProjects.filter(p => p.jobBalance < p.threshold && p.jobBalance >= 0);
				break;
			case 'negative balance': // Negative Balance
				filteredProjects = filteredProjects.filter(p => p.jobBalance < 0);
				break;
		}
		
		this.#renderProjects(filteredProjects);
	}
	#filterProposals(proposals = null) {

		const filterValue = this.currentProposalFilter;
		let filteredProposals = proposals ?? [...this.proposals];
		if (filterValue != 'all') {
			filteredProposals = filteredProposals.filter(item =>
				item.docStatus.toLowerCase().includes(filterValue));
		}
		this.#renderProposals(filteredProposals);
	}
	#filterActionItems(actionItems = null) {

		const status = this.currentActionItemFilter;
		let filteredActionItems = actionItems ?? [...this.actionItems];

		const pendingFilter = ['pending client acknowledgement', 'pending client response'];
		const approvedFilter = ['client approved', 'client acknowledged'];

		if (status != 'all' && status != 'awaiting response' && status != 'client responded') {
			filteredActionItems = filteredActionItems.filter(item => item.status.toLowerCase().includes(status));
		}
		else {
			if (status == 'awaiting response') {
				filteredActionItems = filteredActionItems.filter(item => pendingFilter.includes(item.status.toLowerCase()));
			}
			else if (status == 'client responded') {
				filteredActionItems = filteredActionItems.filter(item => approvedFilter.includes(item.status.toLowerCase()));
			}
		}
		
		this.#renderActionItems(filteredActionItems);
	}
	#searchProject() {
		// Case-insensitive search
		const searchTerm = $(`input[evt-keyup="onSearchProject"]`).val().trim();
		const results = this.projects.filter(item => {
			const term = searchTerm.toLowerCase();

			const nameMatch = item.name.toLowerCase().includes(term);
		
			const supervisorMatch = item.supervisors?.some(s =>
				`${s.firstName} ${s.lastName}`.toLowerCase().includes(term)
			);

			return nameMatch || supervisorMatch;
		});

		this.#filterProjects(results);

	}
	#searchProposal() {
		const searchTerm = $(`input[evt-keyup="onSearchProposal"]`).val().trim();
		const results = this.proposals.filter(item =>
			item.project.toLowerCase().includes(searchTerm.toLowerCase()) ||
			item.createdBy?.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
			item.createdBy?.lastName?.toLowerCase().includes(searchTerm.toLowerCase())
		);

		this.#filterProposals(results);

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

		this.#filterActionItems(results);

	}
	#initKtAppEventHandlers() {
		KTApp.initBootstrapTooltips();
		KTMenu.init();
		KTMenu.initGlobalHandlers();
	}
	#checkQuickbooksSettings() {
		const isQuickbooksConnected = this.companySettings.quickbooksConnected;

		if (isQuickbooksConnected === false) {
			$("#div-quickbooks-account").removeClass('d-none');
		}
		else {
			$("#div-quickbooks-account").remove();
		}
	}
	#renderProjects(projects) {
		const instance = this;
		const projectRows = projects.map(item => {
			return instance.#projectRow(item);
		}).join('');

		$("#listview-widget-active-projects").addClass('loaded').removeClass('loading');
		$("#listview-widget-active-projects div.listview-widget-content").html(projectRows);

		const totalCount = this.projects.length;
		const countByFilter = projects.length;
		$("#listview-widget-active-projects-counter").html(`${countByFilter} of ${totalCount}`);

		this.#initKtAppEventHandlers();
	}
	#renderProposals(proposals) {
		const instance = this;
		const proposalRows = proposals.map(item => {
			return instance.#proposalRow(item);
		}).join('');

		$("#listview-widget-proposals").addClass('loaded').removeClass('loading');
		$("#listview-widget-proposals div.listview-widget-content").html(proposalRows);

		const totalCount = this.proposals.length;
		const countByFilter = proposals.length;
		$("#listview-widget-proposals-counter").html(`${countByFilter} of ${totalCount}`);

		this.#initKtAppEventHandlers();
	}
	#renderActionItems(actionItems) {
		const instance = this;
		const actionItemRows = actionItems.map(item => {
			return instance.#actionItemRow(item);
		}).join('');

		$("#listview-widget-actionitems").addClass('loaded').removeClass('loading');
		$("#listview-widget-actionitems div.listview-widget-content").html(actionItemRows);

		const totalCount = this.actionItems.length;
		const countByFilter = actionItems.length;
		$("#listview-widget-actionitems-counter").html(`${countByFilter} of ${totalCount}`);

		this.#populateActionItemStatusDropdown();
		this.#initKtAppEventHandlers();
	}
	#renderStatistics() {
		const stats = this.dashboardStats.projectStats;
		const activeCount = stats.activeCount;
		const completedCount = stats.completedCount;
		const archivedCount = stats.archivedCount;

		$(`div[data-stat-id="projectStats"]`).addClass('loaded').removeClass('loading');
		$(`div[data-stat-id="projectStats-total"]`).html(stats.totalCount);
		$(`div[data-stat-id="projectStats-active-count"]`).html(activeCount);
		$(`div[data-stat-id="projectStats-completed-count"]`).html(completedCount);
		$(`div[data-stat-id="projectStats-archived-count"]`).html(archivedCount);

		const projectBalances = this.dashboardStats.projectBalances;
		const aboveThresholdCount = projectBalances.aboveThresholdCount;
		const belowThresholdCount = projectBalances.belowThresholdCount;
		const negativeBalanceCount = projectBalances.negativeBalanceCount;
		const totalCount = projectBalances.totalCount;


		$(`div[data-stat-id="activeprojects"]`).addClass('loaded').removeClass('loading');
		$(`div[data-stat-id="active-projects-total"]`).html(totalCount);
		$(`div[data-stat-id="active-projects-aboveth"]`).html(aboveThresholdCount);
		$(`div[data-stat-id="active-projects-belowth"]`).html(belowThresholdCount);
		$(`div[data-stat-id="active-projects-negative"]`).html(negativeBalanceCount);

		const totalClients = this.dashboardStats.totalClients;
		const recentClients = this.dashboardStats.recentClients;
		const moreCount = totalClients - recentClients.length;

		$(`div[data-stat-id="recentclients"]`).addClass('loaded').removeClass('loading');
		$(`span[data-stat-id="recentclients-list-count"]`).html(`+${moreCount}`);
		$(`div[data-stat-id="recentclients-list-total"]`).html(`${totalClients}`);

		var clientIcons = recentClients.map(rc => {
			return `<div class="symbol symbol-35px symbol-circle" data-bs-toggle="tooltip" title="" data-bs-original-title="${rc.name}">
						<span class="symbol-label bg-warning text-inverse-warning fw-bolder">${rc.initials}</span>
					</div>`;
		}).join('');
		

		$(`div[data-stat-id="recentclients-list"]`).prepend(clientIcons);
		
		var t = document.getElementById("kt_project_list_chart");
		if (t) {
			var e = t.getContext("2d");
			new Chart(e, {
				type: "doughnut",
				data: { datasets: [{ data: [activeCount, completedCount, archivedCount], backgroundColor: ["#00A3FF", "#50CD89", "#E4E6EF"] }], labels: ["Active", "Completed", "Archived"] },
				options: {
					chart: { fontFamily: "inherit" },
					cutout: "75%",
					cutoutPercentage: 65,
					responsive: !0,
					maintainAspectRatio: !1,
					title: { display: !1 },
					animation: { animateScale: !0, animateRotate: !0 },
					tooltips: {
						enabled: !0,
						intersect: !1,
						mode: "nearest",
						bodySpacing: 5,
						yPadding: 10,
						xPadding: 10,
						caretPadding: 0,
						displayColors: !1,
						backgroundColor: "#20D489",
						titleFontColor: "#ffffff",
						cornerRadius: 4,
						footerSpacing: 0,
						titleSpacing: 0,
					},
					plugins: { legend: { display: !1 } },
				},
			});
		}
	}
	#subscribeToActionItemCreated() {
		const instance = this;
		signalRConnection.on("OnActionItemCreated", function (actionItem) {
			
			instance.actionItems.unshift(actionItem);

			instance.#filterActionItems();
		});
	}
	
	init() {
		if (Auth.permissions().UserRole != 'Client') {
			this.#loadProposals('all');
			this.#loadActionItems();
		}

		APP.subscribe('loadProjects', (response) => {
			this.projects = response;
			this.#populateProjectStatusDropdown();
			this.#renderProjects(this.projects);
		});

		this.#loadDashboardStats();
		this.#initEventHandlers();
		this.#checkQuickbooksSettings();
		this.#subscribeToActionItemCreated();
    }
}

class DashboardTemplates {
	constructor() {
		this.emptyBookmark = `<div class="col-xl-6">
								<div class="notice d-flex bg-light-warning rounded border-warning border border-dashed rounded-3 p-6">
									<div class="d-flex flex-stack flex-grow-1">
										<div class="fw-bold">
											<h4 class="text-gray-900 fw-bolder">No Bookmarks Found!</h4>
											<div class="fs-6 text-gray-700">
												To add a bookmark item, click on the 'Add to Bookmark' button in each page you visit
											</div>
										</div>
									</div>
								</div>
							  </div>`;
	}
	#formatToDollars(amount) {
		return `$${amount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}`;
	}
	#formatJobBalance(jobBalance, threshold, id = null) {
		let jobBalanceText = this.#formatToDollars(Math.abs(jobBalance));
		let cssClass = '';
		
		if (jobBalance < 0) {
			cssClass = 'text-danger';
			jobBalanceText = `(${jobBalanceText})`;
		} else if (jobBalance < threshold) {
			cssClass = 'text-warning';
		} else {
			cssClass = 'text-success';
		}

		return `<span class="${cssClass}">${jobBalanceText}</span>`;
	}
	options(arr, label) {
		let options = `<option value="">--Select Option--</option>`;
		options += arr.map(ar =>
			`<option value="${ar.id}">${ar[label]}</option>`
		).join('');

		return options;
	}
	#contextMenu(contextMenuItems) {
		return `<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
					Actions <i class="fas fa-chevron-down"></i>
				</a>
				<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true">
					${contextMenuItems}
				</div>`;
	}
	projectRow(item, canManageAllJobs) {
		const actionButtons = (item) => {
			const status = '';

			const manageEstimatesButton = item.isAccepted ? `<div class="menu-item px-3">
					<a href="/revised-estimates/${item.proposalId}" class="menu-link px-3 text-warning">
						<span class="svg-icon svg-icon-warning svg-icon-1x">
							${doutune.file1} 
						</span>
						&nbsp;Manage Estimates
					</a>
				</div>` : '';
			const manageScheduleButton = item.isAccepted ? `<div class="menu-item px-3">
					<a href="/project/${item.id}/schedule" class="menu-link px-3 text-primary">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.calendar} 
						</span>
						&nbsp;Manage Schedule
					</a>
				</div>` : '';

			const archiveButton = !item.isArchived ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3" evt-click="onArchiveProject" data-id=${item.id}>
						<span class="svg-icon svg-icon-muted svg-icon-1x">
							${doutune.archive} 
						</span>
						&nbsp;Archive
					</a>
				</div>` : `<div class="menu-item px-3">
				<a href="javascript:" class="menu-link px-3" evt-click="onUnArchiveProject" data-id=${item.id}>
					<span class="svg-icon svg-icon-muted svg-icon-1x">
						${doutune.archive}
					</span>
					&nbsp;Unarchive
				</a>
			</div>`;
			const deleteButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3" evt-click="onDeleteProject" data-id=${item.id}>
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.checkCircle} 
						</span>
						&nbsp;Delete
					</a>
				</div>`;
			const contextMenuItems = `${manageEstimatesButton}
									${manageScheduleButton}
									${archiveButton}
									${deleteButton}`;

			return this.#contextMenu(contextMenuItems);
		};

		return `<tr data-project-row-id="${item.id}" data-threshold="${item.threshold}">
					<td class="py-1">
						<a href="/project/${item.id}" class="fs-5 fw-bolder text-gray-900 text-hover-primary">${item.name}</a>
						${item.supervisors.map(i => `<span class="text-muted fw-bold d-block fs-8">${i.firstName ?? ''} ${i.lastName ?? ''}</span>`).join('')}
					</td>
					<td class="text-center py-1">
						<a href="/revised-estimates/${item.proposalId}" class="fw-bold">${this.#formatJobBalance(item.jobBalance, item.threshold, item.id)}</a>
					</td>
					<td class="text-center text-muted fw-bold py-1">
						<div class="ms-auto">
							${actionButtons(item)}
						</div>
					</td>
				</tr>`;
	}
	projectClientRow(item) {
		const supervisorsColumn = (item) => {
			return `<td class="w-200px text-center"">
							${item.supervisors.map(i => `<span class="badge badge-light-primary me-1">${i.firstName} ${i.lastName}</span>`).join('')}
						</td>`;
		}

		return `<tr>
					<td class="py-1">
						<a href="/project/${item.id}" class="fs-5 fw-bolder text-gray-900 text-hover-primary">${item.name}</a>
					</td>
					${supervisorsColumn(item)}
				</tr>`;
	}
	proposalRow(item) {
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
		const actionButtons = (item) => {
			const status = item.docStatus.toLowerCase();
			
			const acceptButton = status != "accepted" && status != "archived" ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 accept proposal-action" data-action="accept" data-id=${item.id}>
						<span class="svg-icon svg-icon-success svg-icon-1x">
							${doutune.checkCircle} 
						</span>
						&nbsp;Accept
					</a>
				</div>` : '';
			const archiveButton = status != 'archived' ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 archive proposal-action" data-action="archive" data-id=${item.id}>
						<span class="svg-icon svg-icon-muted svg-icon-1x">
							${doutune.archive} 
						</span>
						&nbsp;Archive
					</a>
				</div>` : '';
			const createScheduleButton = status != 'archived' ? `<div class="menu-item px-3">
					<a href="/project/${item.projectId}/schedule" class="menu-link px-3" data-id=${item.id}>
						<span class="svg-icon svg-icon-muted svg-icon-1x">
							${doutune.calendar} 
						</span>
						&nbsp;Create Schedule
					</a>
				</div>` : '';
			const deleteButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 remove proposal-action text-danger" data-action="remove" data-id=${item.id}>
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.checkCircle} 
						</span>
						&nbsp;Delete
					</a>
				</div>`;
			const contextMenuItems = `${acceptButton}
									${createScheduleButton}
									${archiveButton}
									${deleteButton}`;

			return this.#contextMenu(contextMenuItems);
		};

		const projectName = (item) => {
			let p = `<a href="/proposals/${item.id}" class="fs-5 fw-bolder text-gray-900 text-hover-primary">${item.project}</a>`;
			if (item.supervisors.length > 0) {
				const supervisorNames = item.supervisors.map(s => `<span class="text-muted fw-bold d-block fs-8">${s.firstName} ${s.lastName}</span>`).join('');
				p += supervisorNames;
			}

			if (item.template) {
				p += `<span class="text-muted fw-bold d-block fs-8">Template: ${item.template}</span>`;
			}

			return p;
		}
		return `<tr data-id="${item.id}">
					<td class="py-1">
						${projectName(item)}
					</td>
					<td class="py-1">${status(item.docStatus)}</td>
					<td class="text-center py-1">
						${actionButtons(item)}
					</td>
				</tr>`;
	}
	bookmarkItems(bookmarks) {
		//return bookmarks.map(item => `<div class="col-xl-2 ribbon-triangle ribbon-top-start border-primary padding-right-0 d-flex" data-bookmark-id="${item.id}">

		//									<div class="overflow-hidden position-relative card-rounded d-flex has-shadow flex-1">
		//										<!--begin::Ribbon-->
		//										<div class="ribbon ribbon-triangle ribbon-top-start border-primary">
		//											<!--begin::Ribbon icon-->
		//											<div class="ribbon-icon mt-n5 ms-n6">
		//												<i class="bi bi-bookmarks-fill fs-2 text-white"></i>
		//											</div>
		//											<!--end::Ribbon icon-->
		//										</div>
		//										<!--end::Ribbon-->

		//										<!--begin::Card-->
		//										<div class="card card-bordered  flex-1">
		//											<!--begin::Header-->
		//											<div class="card-header ribbon ribbon-top ribbon-vertical">
		//											<a href="${item.url}" class="text-dark text-hover-primary" target="_blank">
		//												<div class="card-title px-5 fs-7">${item.title}</div>
		//											</a>
		//												<button data-bs-toggle="tooltip" data-bs-dismiss="click" data-bs-placement="top" title="Remove"
		//													class="btn btn-icon btn-sm btn-active-icon-dark remove-bookmark" data-bookmark-id="${item.id}" style="position: absolute;
		//													z-index: 100;
		//													right: -5px;
		//													align-items: center;>
		//													<span class="svg-icon svg-icon-1 svg-icon-primary">
		//														<i class="bi bi-trash"></i>
		//													</span>
		//												</button>
		//											</div>
		//											<!--end::Header-->
		//										</div>
		//										<!--end::Card-->
		//									</div>
		//							  </div>`).join('');
		return bookmarks.map(item => `<a href="${item.url}" class="text-dark text-hover-primary" target="_blank">
											<span class="badge badge-secondary me-2 mt-2" data-bookmark-id="${item.id}">${item.title}</span>
									  </a>`).join('');
	}
}

$(document).ready(() => {
	DashboardView = new Dashboard();
	DashboardView.init();
});