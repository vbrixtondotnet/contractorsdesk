class ActionItemList extends DomEventComponent {
	constructor() {
		super();
		this.actionItems = [];
		this.actionItemsLv = null;
		this.emailSender = new EmailSender();
		this.actionItemForm = APP.actionItemForm;
		this.enableAdd = false;
		this.currentFilter = null;
		this.swal = new SwalUtil();
		this.projectId = null;
		this.projects = [];
		this.eventSubscriptions = [];
	}

	#generateActionItemActionMenu(item) {
		const currentUser = Auth.currentUser();
		const role = currentUser.role;
		const statusId = item.statusId;
		const isUserAssigned = item.assignedSupervisors.some(s => s.id === currentUser.id);
		const isForReview = statusId === Enums.ActionItemStatus.ForReview;
		const isInProgress = statusId == Enums.ActionItemStatus.InProgress;
		const isUserCreator = item.createdById === currentUser.id;
		const isPendingApproval = statusId == Enums.ActionItemStatus.PendingClientResponse;
		const isCompleted = statusId == Enums.ActionItemStatus.Completed;
		const isClientApproved = statusId == Enums.ActionItemStatus.ClientApproved;
		const isArchived = item.isArchived;
		const isUserCompanyOwner = role == "Company Owner";

		const startButton = statusId === Enums.ActionItemStatus.NotStarted && isUserAssigned && !isArchived ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-success" data-id=${item.id} evt-click="onAcceptActionItem">
						<span class="svg-icon svg-icon-success svg-icon-1x">
							${doutune.checkCircle} 
						</span>
						&nbsp;Start
					</a>
				</div>` : '';
		const reviewButton = isForReview && isUserAssigned && !isArchived ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onReviewActionItem">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.search} 
						</span>
						&nbsp;Review
					</a>
				</div>` : '';
		const completeButton = (isInProgress && isUserAssigned) || (isUserAssigned && isClientApproved) || (isUserCompanyOwner) ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onCompleteActionItem">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.checkCircle} 
						</span>
						&nbsp;Complete
					</a>
				</div>` : '';
		const archiveButton = !isArchived ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-muted" data-id=${item.id} evt-click="onArchiveActionItem">
						<span class="svg-icon svg-icon-muted svg-icon-1x">
							${doutune.archive} 
						</span>
						&nbsp;Archive
					</a>
				</div>` : '';
		const deleteButton = `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-danger" data-id=${item.id} evt-click="onDeleteActionItem">
						<span class="svg-icon svg-icon-danger svg-icon-1x">
							${doutune.trash} 
						</span>
						&nbsp;Delete
					</a>
				</div>`;
		const assignButton = !isCompleted && !isArchived ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-warning" data-id=${item.id} evt-click="onAssignActionItem">
						<span class="svg-icon svg-icon-warning svg-icon-1x">
							${doutune.pencil} 
						</span>
						&nbsp;Assign
					</a>
				</div>` : '';
		const editButton = (isUserCreator && !isForReview && !isPendingApproval && !isCompleted && !isArchived) || (isUserCompanyOwner) ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onEditActionItem">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.pencil} 
						</span>
						&nbsp;Update
					</a>
				</div>` : '';
		const unArchiveButton = isArchived ? `<div class="menu-item px-3">
					<a href="javascript:" class="menu-link px-3 text-primary" data-id=${item.id} evt-click="onUnarchiveActionItem">
						<span class="svg-icon svg-icon-primary svg-icon-1x">
							${doutune.up} 
						</span>
						&nbsp;Unarchive
					</a>
				</div>` : '';

		const actionMenuItems = `
				${startButton}
				${reviewButton}
				${completeButton}
				${editButton}
				${assignButton}
				${archiveButton}
				${deleteButton}
				${unArchiveButton}
			`;

		return `<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
					Actions <i class="fas fa-chevron-down"></i>
				</a>
				<div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-200px py-4" data-kt-menu="true">
					${actionMenuItems}
				</div>`;
	}

	#getActionItemStatusBadge(item) {
		let css = '';
		let itemStatus = item.isArchived ? "Archived" : item.status;
		switch (itemStatus) {
			case "Not Started":
				css = 'badge-light';
				break;
			case "In Progress":
				css = 'badge-success';
				break;
			case "For Review":
				css = 'badge-info';
				break;
			case "Pending Client Response":
			case "Pending Client Acknowledgement":
				css = 'badge-danger';
				itemStatus = "Client Acknowledgement";
				break;
			case "Client Approved":
			case "Client Acknowledged":
				css = 'badge-warning';
				itemStatus = "Client Approved";
				break;
			case "Completed":
				css = 'badge-primary';
				break;
			case "Archived":
				css = 'badge-secondary';
				break;
		}

		let status = `<div class="badge ${css}">${itemStatus}</div>`;

		return status;
	}

	#actionItemRow(item) {

		const formatFullDate = (dateStr)=> {
			const date = new Date(dateStr);

			const day = date.getDate();
			const month = date.toLocaleString('en-US', { month: 'short' }); // e.g. "Aug"
			const year = date.getFullYear();

			let hours = date.getHours();
			const minutes = date.getMinutes().toString().padStart(2, '0');
			const ampm = hours >= 12 ? 'PM' : 'AM';

			hours = hours % 12 || 12; // convert to 12-hour format

			return `${month} ${day}, ${year} ${hours}:${minutes} ${ampm}`;

		}

		const createdBy = item.createdByUser ? `${item.createdByUser.firstName} ${item.createdByUser.lastName}` : '';

		const assignedTo = item.assignedSupervisors?.length
			? item.assignedSupervisors.map(s => `${s.firstName} ${s.lastName}`).join(', ')
			: 'N/A';
	

		return `<tr class="" data-action-item-row="${item.id}">
						<td class="py-1"><strong><a href="/action-items/${item.id}" class="text-dark" target="_blank">${item.title}</strong></a></td>
						<td class="py-1">${item.actionTypeName}</td>
						<td class="py-1">${createdBy}</td>
						<td class="py-1">${assignedTo}</td>
						<td class="py-1">${formatFullDate(item.dateCreated)}</td>
						<td class="py-1">${this.#getActionItemStatusBadge(item)}</td>
						<td class="text-center py-1">
							<div class="ms-auto">
								${this.#generateActionItemActionMenu(item)}
							</div>
						</td>
					</tr>`;
	}

	#sortActionItems(col, direction) {
		const actionitems = this.actionItems;
		switch (col) {
			case 'ACTION ITEM':
				actionitems.sort((a, b) => {
					if (a.title === b.title) return 0;
					return direction === 'desc' ? (a.title < b.title ? 1 : -1) : (a.title > b.title ? 1 : -1);
				});
				break;
			case 'STATUS':
				actionitems.sort((a, b) => {
					if (a.statusId === b.statusId) return 0;
					return direction === 'desc' ? (a.statusId < b.statusId ? 1 : -1) : (a.statusId > b.statusId ? 1 : -1);
				});
				break;
			case 'DATE':
				actionitems.sort((a, b) => {
					if (a.dateCreated === b.dateCreated) return 0;
					return direction === 'asc' ? (a.dateCreated < b.dateCreated ? 1 : -1) : (a.dateCreated > b.dateCreated ? 1 : -1);
				});
				break;
			case 'PROJECT NAME':
				actionitems.sort((a, b) => {
					if (a.projectName === b.projectName) return 0;
					return direction === 'desc' ? (a.projectName < b.projectName ? 1 : -1) : (a.projectName > b.projectName ? 1 : -1);
				});
				break;
		}

		actionitems.sort((a, b) => {
			if (a.statusId === 7) return 1; // Ensure items with statusId = 7 are at the bottom
			if (b.statusId === 7) return -1; // Ensure items with statusId = 7 are at the bottom
			return 0; // Keep original order for other items
		});

		this.renderActionItemsList(actionitems);
	}

	#initKtAppEventHandlers() {
		KTApp.initBootstrapTooltips();
		KTMenu.init();
		KTMenu.initGlobalHandlers();
	}

	#showCompleteActionItemEmailSender(actionItemId) {
		const actionItem = this.actionItems.find(ai => ai.id == actionItemId);
		const actionItemCreator = actionItem.createdByUser;

		var companySettings = Auth.getCompanySettings();

		this.emailSender.resetEmailFields();
		this.emailSender.clientId = actionItem.projectId;
		this.emailSender.clientName = actionItemCreator ? actionItemCreator.firstName : '';
		this.emailSender.senderName = Auth.currentUser().fullName;
		this.emailSender.companyName = companySettings.companyName;

		this.emailSender.setTo(actionItemCreator ? actionItemCreator.email : '');
		this.emailSender.setReplyTo(Auth.currentUser().email);

		this.emailSender.showGreetings = true;
		this.emailSender.setSubject(`${actionItem.title} - Completed`, true);
		this.emailSender.setContent(`This action item that you assigned to me has been completed. Below are the additional notes upon completion: `);
		this.emailSender.type = Enums.CompleteActionItem;
		this.emailSender.ref2Id = actionItemId;
		this.emailSender.onSentCallback = () => {
			actionItem.statusId = Enums.ActionItemStatus.Completed;
			actionItem.status = 'Completed';
			this.refreshActionItemListOnCurrentFilter();
		}

		this.emailSender.toggle();
	}

	#onAcceptActionItemSuccess(actionItem) {

		const actionItemId = actionItem.id;
		const actionItemIndex = this.actionItems.findIndex(item => item.id === actionItemId);
		if (actionItemIndex !== -1) {
			this.actionItems[actionItemIndex] = actionItem;
		}

		this.refreshActionItemListOnCurrentFilter();
	}

	#ArchiveActionItemActionSuccess(actionItemId, isArchived) {
		const actionItem = this.actionItems.find(item => item.id === actionItemId);
		actionItem.isArchived = isArchived;

		this.refreshActionItemListOnCurrentFilter();
	}

	onSelectActionItem(b, e) {
		e.preventDefault();
		
		const id = $(b).attr("data-action-item-row");
		alert(id);
	}

	renderActionItemsList(actionItems) {
		const actionItemsRows = actionItems.map(item => { return this.#actionItemRow(item) }).join('');
		this.actionItemsLv.renderRows(actionItemsRows);
		this.#initKtAppEventHandlers();
	}

	refreshActionItemListOnCurrentFilter() {
		$("#comp-action-items-list").find("span.badge.badge-light-primary").click();
	}

	triggerFirstFilter() {
		this.actionItemsLv.triggerFirstFilter();
	}

	onCompleteActionItem(b) {
		const actionItemId = parseInt(b.getAttribute('data-id'));
		const actionItem = this.actionItems.find(ai => ai.id == actionItemId);
		const actionItemCreator = actionItem.createdByUser;
		const currentUser = Auth.currentUser();

		if (actionItemCreator.id != currentUser.id) {
			this.#showCompleteActionItemEmailSender(actionItemId);
		}
		else {
			$(b).parents('.ms-auto').html("Completing <span class='spinner-border spinner-border-sm align-middle ms-2'></span>").addClass('text-primary');
			this.httpService.patch(`/api/action-items/${actionItemId}/complete`)
				.then((response) => {
					actionItem.statusId = Enums.ActionItemStatus.Completed;
					actionItem.status = 'Completed';
					this.refreshActionItemListOnCurrentFilter();
				});
		}
	}

	onReviewActionItem(b) {
		const actionItemId = parseInt(b.getAttribute('data-id'));
		const actionItem = this.actionItems.find(ai => ai.id == actionItemId);
		this.actionItemForm.onSaveActionItemCallback = (data) => {
			const actionItemIndex = this.actionItems.findIndex(item => item.id === data.id);
			if (actionItemIndex !== -1) {
				this.actionItems[actionItemIndex] = data;
			}
			else {
				this.actionItems.push(data);
			}

			this.refreshActionItemListOnCurrentFilter();
		}
		this.actionItemForm.review(actionItem);
	}

	onAcceptActionItem(b) {
		const actionItemId = parseInt(b.getAttribute('data-id'));
		$(b).parents('.ms-auto').html("Starting <span class='spinner-border spinner-border-sm align-middle ms-2'></span>").addClass('text-success');
		this.httpService.patch(`/api/action-items/${actionItemId}/accept`)
			.then((response) => {
				this.#onAcceptActionItemSuccess(response)
			});
	}

	onEditActionItem(b) {
		const actionItemId = parseInt(b.getAttribute('data-id'));
		const actionItem = this.actionItems.find(ai => ai.id == actionItemId);
		this.actionItemForm.onSaveActionItemCallback = (data) => {
			const actionItemIndex = this.actionItems.findIndex(item => item.id === data.id);
			if (actionItemIndex !== -1) {
				this.actionItems[actionItemIndex] = data;
				this.refreshActionItemListOnCurrentFilter();
			}
		}
		this.actionItemForm.edit(actionItem);

	}

	onAssignActionItem(b) {
		const actionItemId = parseInt(b.getAttribute('data-id'));
		const actionItem = this.actionItems.find(ai => ai.id == actionItemId);
		this.actionItemForm.onSaveActionItemCallback = (data) => {
			const actionItemIndex = this.actionItems.findIndex(item => item.id === data.id);
			if (actionItemIndex !== -1) {
				this.actionItems[actionItemIndex] = data;
				this.refreshActionItemListOnCurrentFilter();
			}
		}
		this.actionItemForm.assign(actionItem);
	}

	onDeleteActionItem(b) {
		this.swal.confirm('Are you sure you want to delete this action item?', () => {
			const actionItemId = parseInt(b.getAttribute('data-id'));
			$(b).parents('.ms-auto').html("Deleting <span class='spinner-border spinner-border-sm align-middle ms-2'></span>").addClass('text-danger');

			this.httpService.delete(`/api/action-items/${actionItemId}`)
				.then((response) => {
					if (response) {
						this.swal.alert('Action Item has been removed successfully!');
						$(`tr[data-action-item-row="${actionItemId}"]`).remove();
					}
				});
		});

	}

	onArchiveActionItem(b) {
		const actionItemId = parseInt(b.getAttribute('data-id'));
		this.#ArchiveActionItemActionSuccess(actionItemId, true);
		this.httpService.patch(`/api/action-items/${actionItemId}/archive`);
	}

	onUnarchiveActionItem(b) {
		const actionItemId = parseInt(b.getAttribute('data-id'));
		this.#ArchiveActionItemActionSuccess(actionItemId, false);
		this.httpService.patch(`/api/action-items/${actionItemId}/unarchive`);
	}

	setProjects(projects) {
		this.projects = projects;
		this.actionItemForm.populateProjects(projects);
	}

	setEmailRecipient(emailAddress) {
		this.emailSender.setTo(emailAddress);
	}

	loadProjects() {
		this.httpService.get('/api/projects')
			.then(response => {
				this.actionItemForm.populateProjects(response);
			});
	}

	getEmailSenderInstance() {
		return this.emailSender;
	}

	preload() {
		this.actionItemsLv.preload();
	}

	subscribe(name, callback) {
		this.eventSubscriptions.push({ name: name, callback: callback });
	}

	render() {
		const lvOptions = {
			title: 'Action Items',
			columnDefs: [
				{ class: 'w-lg-250px ps-1', title: 'Action Item', sortable: true, onsort: (colname, direction) => { this.#sortActionItems(colname, direction); } },
				{ class: 'w-lg-100px', title: 'Type', sortable: true, onsort: (colname, direction) => { this.#sortActionItems(colname, direction); } },
				{ class: 'w-lg-150px', title: 'Created By', sortable: true, onsort: (colname, direction) => { this.#sortActionItems(colname, direction); } },
				{ class: 'w-lg-150px', title: 'Assigned To', sortable: true, onsort: (colname, direction) => { this.#sortActionItems(colname, direction); } },
				{ class: 'w-lg-150px', title: 'Date', sortable: true, onsort: (colname, direction) => { this.#sortActionItems(colname, direction); } },
				{ class: 'w-lg-85px', title: 'Status', sortable: true, onsort: (colname, direction) => { this.#sortActionItems(colname, direction); } },
				{ class: 'w-150px text-center', title: 'Action' },
			],
			filters: {
				onclick: (status) => {
					this.preload();
					this.currentFilter = status;//status.toUpperCase();
					let urlParam = [status !== '0' ? `statusId=${status}` : '', this.projectId != null ? `projectId=${this.projectId}` : ''].filter(Boolean).join('&');
					urlParam = urlParam ? `?${urlParam}` : '';

					this.httpService.get('/api/dashboard/action-items' + urlParam)
						.then(response => {
							this.actionItems = response;
							this.renderActionItemsList(response);
						});
				},
				items: [
					{ value: 0, text: 'All' },
					{ value: 1, text: 'Not Started' },
					{ value: 3, text: 'For Review' },
					{ value: 2, text: 'In Progress' },
					{ value: 4, text: 'Pending Client Response' },
					{ value: 5, text: 'Client Approved / Acknowledged' },
					{ value: 6, text: 'Completed' },
					{ value: 7, text: 'Archived' }
				]
			},
			shadow: true
		}

		if (this.enableAdd) {
			lvOptions.onAdd = () => {
				this.actionItemForm.onSaveActionItemCallback = (data) => {
					this.actionItems.unshift(data);
					this.refreshActionItemListOnCurrentFilter(this.actionItems);

					if (data.actionTypeName == "Cost Change") {

						var companySettings = Auth.getCompanySettings();
						let emailBody = `Please approve the following Change Order:
										<p>Cost Change Item: ${data.costChangeItem}<br/>
										Current Amount: $${data.currentAmount}<br/>
										New Amount: $${data.revisedAmount}</p>
										<p>Attached you will find the change order form for your records.</p>`;

						const project = this.projects.find(p => p.id == data.projectId);
						console.log(project);
						const clientEmailAddress = project ? project.clientEmailAddress : '';
						const clientName = project ? project.clientName : '';
						this.emailSender.resetEmailFields();
						this.emailSender.ref2Id = data.id;
						this.emailSender.clientName = clientName;
						this.emailSender.senderName = Auth.currentUser().fullName;
						this.emailSender.companyName = companySettings.companyName;

						this.emailSender.setTo(clientEmailAddress);
						this.emailSender.setReplyTo(Auth.currentUser().email);

						this.emailSender.showGreetings = true;
						this.emailSender.setSubject(`Please approve the following change order`);
						this.emailSender.setContent(emailBody);
						this.emailSender.addDummyAttachmentItem(`${project.name}.change-order.pdf`);
						this.emailSender.type = Enums.ChangeOrder;
						this.emailSender.toggle();
					}
				};
				this.actionItemForm.projectId = this.projectId;
				this.actionItemForm.createNew();
			}
		}

		this.actionItemsLv = new ListView(lvOptions);
		this.actionItemsLv.render('comp-action-items-list');
	}
}