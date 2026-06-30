APP = null;
class App extends DomEventComponent {
    constructor() {
        super();
        this.actionItemForm = new ActionItemForm();
        this.selectedSupervisor = null;
        this.eventSubscriptions = [];
    }

    async onTriggerQBDataSync() {
        try {
            const response = await fetch("/api/sysbackgroundjobs/quickbooksdatasync", {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    containerId: 'div#background-process-messagescroll',
                    notifyOnStart: true
                })
            });

            if (!response.ok) {
                throw new Error("Failed to trigger QuickBooks data sync.");
            }

            const result = await response.json();

            $("#btnToggleBackgroundProcessMessages").click();

        } catch (error) {
            console.error("Error triggering QuickBooks data sync:", error);
            alert("Error triggering QuickBooks data sync: " + error.message);
        }
    }

    onAddActionItem(b, e) {
        this.actionItemForm.createNew();
    }

    resetActionItemForm(b, e) {
        this.actionItemForm.close();
    }

    #setActiveMenuItem() {
        const activeLinks = document.querySelectorAll(".menu-link.active");

        activeLinks.forEach(activeLink => {
            const menuSub = activeLink.closest(".menu-sub");

            if (menuSub) {
                const parentItem = menuSub.closest(".menu-item");

                if (parentItem && !parentItem.classList.contains("show")) {
                    parentItem.classList.add("show");

                    const trigger = parentItem.querySelector('[data-kt-menu-trigger]');

                    if (trigger) {
                        trigger.click();
                    }
                }
            }
        });
    }

    #initActiveMenu() {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", () => {
                this.#setActiveMenuItem();
            });
        } else {
            this.#setActiveMenuItem();
        }
    }

    initKtAppEventHandlers() {
        KTApp.initBootstrapTooltips();
        KTMenu.init();
        KTMenu.initGlobalHandlers();
    }

    #populateProjectManagersDropdown() {
        const instance = this;
        const supervisors = [
            ...new Map(
                this.projects
                    .flatMap(project => project.supervisors) // combine all supervisors into one array
                    .map(s => [s.id, s])                     // turn into [id, supervisor] pairs
            ).values()                                   // take only the unique supervisor objects
        ];

        const data = supervisors.map(s => { return { id: s.id, text: `${s.firstName} ${s.lastName}` } });
        
        
        //temp for manager projects
        const $select2 = $('#manager-projects-filter');
        // Clear existing options
        $select2.empty();

        $select2.append(new Option('All', 0, true, false));

        // Append new options
        data.forEach(item => {
            const newOption = new Option(item.text, item.id, false, false);
            $select2.append(newOption);
        });

        // Refresh Select2 to reflect changes
        $select2.trigger('change');

        $select2.on('select2:select', function (e) {
            instance.#filterProjects(e.params.data.id);
        });

    }

    #filterProjects(supervisorId) {
        let filteredData = [...this.projects];
        let id = parseInt(supervisorId);
        filteredData = filteredData.filter(item => id == 0 || item.supervisors?.some(s => s.id === id));

        this.#renderProjects(filteredData);
        //filteredData = filteredData.filter()
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
            return ``;
            return `<div class="mt-5">
						<div class="ms-auto">
							<span href="#" class="btn btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
								<i class="fas fa-chevron-down"></i>
							</span>
							<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true" style="">
								<div class="menu-item px-3">
									<span data-id="${item.proposalId}" evt-click="onOpenRevisedEstimate"  class="menu-link px-3 text-warning">
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
								<img src="/assets/media/stock/600x400/img-56.jpg" class="mw-100" alt="">
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

    #renderProjects(projects) {
        const instance = this;
        const projectRows = projects.map(item => {
            return instance.#projectRow(item);
        }).join('');

        //$("#listview-widget-active-projects").addClass('loaded').removeClass('loading');
        $("#manager-projects-active-projects-list").html(projectRows);
        
        this.initKtAppEventHandlers();
    }

    loadProjects() {
        this.httpService.get('/api/dashboard/projects')
            .then(response => {
                this.projects = response;
                this.#renderProjects(this.projects);
                this.actionItemForm.populateProjects(this.projects);
                this.#populateProjectManagersDropdown();

                let onprojectsLoadSubscription = this.eventSubscriptions.find(e => e.name == 'loadProjects');
                if (onprojectsLoadSubscription && onprojectsLoadSubscription.callback) {
                    onprojectsLoadSubscription.callback(this.projects);
                }
            });
    }

    subscribe(name, callback) {
        this.eventSubscriptions.push({ name: name, callback: callback });
    }

    setLoadingIndicator(elementId, isLoading) {
        $(elementId)
            .toggleClass("loading", isLoading)
            .toggleClass("loaded", !isLoading);
    }

    resetStickyActionBars() {
        $(".sticky-action-bar").css('bottom', '-100px');
    }

    #logPage(path) {
        this.httpService.post('/api/webhooks/userlog', { path: path });
    } 

    init() {
        this.#logPage(window.location.pathname);
        this.#initActiveMenu();
        this.loadProjects();
    }

}

$(document).ready(() => {
    APP = new App();
    APP.init();
});