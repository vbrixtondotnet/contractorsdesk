class ActionItemView extends DomEventComponent {
    constructor() {
        super();
        this.actionItemId = document.URL.split('/').pop();
        this.actionItem = null;
        this.service = new ActionItemService();
        this.tabControl = new TabControl();
        this.swal = new SwalUtil();
        this.actionItemForm = new ActionItemForm();
        this.projects = [];
        this.emailSender = new EmailSender();
        this.assignedToDropdown = null;
        this.assignedToModal = null;
    }

    #populateActionItemData() {
        const userSession = Auth.currentUser();

        $("#action-item-details-container").removeClass('loading').addClass('loaded');
        var actionItem = this.actionItem;
        
        const formattedDescForEditor = actionItem.description
            .replace(/<br\s*\/?>/gi, '\n')         // Convert <br> to newline
            .replace(/<br\s*\/?>/gi, '\n')         // Convert <br> to newline
            .replace(/<\/?p>/gi, '\n')             // Convert <p> and </p> to newline
            .replace(/&nbsp;/gi, ' ')              // Convert non-breaking space
            .replace(/&lt;/g, '<')                 // Decode <
            .replace(/&gt;/g, '>')                 // Decode >
            .replace(/&amp;/g, '&')                // Decode &
            .replace(/\r?\n[\s\t]*/g, '\n')        // Normalize line breaks and trim
            .trim();      

        $("a[data-model='title']").html(actionItem.title);
        $("span[data-model='project']").html(actionItem.projectName);
        $("div[data-model='duedate']").html(DateUtils.toStandardDate(actionItem.dueDate));
        $(`div[data-model="createdBy"]`).attr("data-bs-original-title", `${actionItem.createdByUser.firstName} ${actionItem.createdByUser.lastName}`);
        $(`span[data-model="createdByInitials"]`).html(actionItem.createdByUser.initials);
        $(`div[data-model="description"]`).html(actionItem.description);
        $(`textarea[data-model="description"]`).val(formattedDescForEditor);

        if (userSession.id == actionItem.createdByUser.id) {
            $(`[data-toggle-owner="true"]`).removeClass('d-none');
            $(`a[evt-click="onDelete"]`).removeClass('d-none');
        }



        this.#populateActionItemType();
        this.#populateAssignedTo();
        this.#displayStatus();
        this.#toggleButtons();
        this.#populateComments();

        KTApp.initBootstrapTooltips();

        $("div.loading").removeClass('loading').addClass('loaded');
    }

    #populateComments() {
        const comments = this.actionItem.comments;
        let commentRows = comments.map(c => { return this.#commentHtml(c); }).join('');

        commentRows = commentRows != '' ? commentRows : `<div class="text-muted text-center d-flex empty-comment-section justify-content-center pb-10">No comment added.</div>`;
        $(`div[data-model="comments"]`).html(commentRows);
    }

    #populateActionItemType() {
        const actionItem = this.actionItem;

        const icon = actionItem.actionTypeName == 'Note' ? doutune.note : doutune.dollar;
        
        const template = `<span class="svg-icon svg-icon-light svg-icon-3hx mb-2">
					        ${icon}
				          </span>
				        <div class="fw-bold text-light fs-6 text-center text-uppercase">${actionItem.actionTypeName}</div>`;

        $(`div[data-model="action-type-icon"]`).html(template);
    }

    #populateAssignedTo() {
        const assignedTo = (a, i) => {
            const ii = i > 2 ? 0 : i;
            const bg = ['dark', 'warning', 'primary'];

            return `<div class="symbol symbol-35px symbol-circle me-3" data-bs-toggle="tooltip" title="" data-bs-original-title="${a.firstName} ${a.lastName}">
				<span class="symbol-label bg-${bg[ii]} text-inverse-${bg[ii]} fw-bolder">${a.initials}</span>
			</div>`;
        }

        const assignedSupervisors = this.actionItem.assignedSupervisors;
        const assignedToHtml = assignedSupervisors.map((a,i) => { return assignedTo(a, i) }).join('');
        $('div[data-model="assignedto"]').html(assignedToHtml);

        $("#slc-action-item-details-assignedto").empty();
        for (var i = 0; i < assignedSupervisors.length; i++) {
            let assignedSupervisor = assignedSupervisors[i];
            $("#slc-action-item-details-assignedto").append(`<option value="${assignedSupervisor.id}" selected>${assignedSupervisor.firstName} ${assignedSupervisor.lastName}</option>`); 
        }
    }

    #commentHtml(c) {
        const createdBy = c.createdBy;
        const fullName = `${createdBy.firstName} ${createdBy.lastName}`;
        const user = Auth.currentUser();
        const symbolClass = user.id == createdBy.id ? 'dark' : 'warning';

        return `<div class="d-flex mb-5">
							<!--begin::Avatar-->
							<div class="symbol symbol-35px symbol-circle me-3" data-bs-toggle="tooltip" title="" data-bs-original-title="${fullName}">
								<span class="symbol-label bg-${symbolClass} text-inverse-${symbolClass} fw-bolder">${createdBy.initials}</span>
							</div>
							<!--end::Avatar-->
							<!--begin::Info-->
							<div class="d-flex flex-column flex-row-fluid">
								<!--begin::Info-->
								<div class="d-flex align-items-center flex-wrap mb-1">
									<a href="#" class="text-gray-800 text-hover-primary fw-bolder me-2">${fullName}</a>
									<span class="text-gray-400 fw-bold fs-7">${DateUtils.getDateDurationFromAspNetDate(c.dateCreated)}</span>
								</div>
								<!--end::Info-->
								<!--begin::Post-->
								<span class="text-gray-800 fs-7 fw-normal pt-1">${c.comment}</span>
								<!--end::Post-->
							</div>
							<!--end::Info-->
						</div>`;
    }

    #toggleButtons() {
        const currentUser = Auth.currentUser();
        const actionItem = this.actionItem;
        const statusId = actionItem.statusId;
        const isUserAssigned = actionItem.assignedSupervisors.some(s => s.id === currentUser.id);
        const isForReview = statusId === Enums.ActionItemStatus.ForReview;
        const isInProgress = statusId == Enums.ActionItemStatus.InProgress;
        const isUserCreator = actionItem.createdById === currentUser.id;
        const isPendingApproval = actionItem.statusId == Enums.ActionItemStatus.PendingClientResponse;
        const isCompleted = statusId == Enums.ActionItemStatus.Completed;
        const isClientApproved = statusId == Enums.ActionItemStatus.ClientApproved;
        const isArchived = statusId == Enums.ActionItemStatus.Archived;
        const isNotStarted = statusId == Enums.ActionItemStatus.NotStarted;

        $("#btnEdit").toggle(isUserCreator && isNotStarted);
        $("#btnStart").toggle(isNotStarted && isUserAssigned);
        $("#btnReview").toggle(isForReview && isUserAssigned);
        $("#btnComplete").toggle((isInProgress && isUserAssigned) || (isUserAssigned && isClientApproved));
    }

    #displayStatus() {
        let css = '';
        let status = this.actionItem.status;
        switch (this.actionItem.status) {
            case "Not Started":
                css = 'badge-light';
                break;
            case "In Progress":
                css = 'badge-light-success';
                break;
            case "For Review":
                css = 'badge-light-info';
                break;
            case "Pending Client Response":
            case "Pending Client Acknowledgement":
                css = 'badge-light-danger';
                status = 'Client Review';
                break;
            case "Client Approved":
            case "Client Acknowledged":
                css = 'badge-light-warning';
                status = 'Client Confirmed';
                break;
            case "Completed":
                css = 'badge-light-primary';
                break;
            case "Archived":
                css = 'badge-secondary';
                break;
        }

        $("span[data-model='status']")
            .removeClass('badge-light')
            .removeClass('badge-light-success')
            .removeClass('badge-light-info')
            .removeClass('badge-light-danger')
            .removeClass('badge-light-warning')
            .removeClass('badge-light-primary')
            .removeClass('badge-secondary')
            .addClass(css).html(status);
        $(`[data-status-toggle="${status.toLowerCase()}"]`).removeClass('d-none')
    }
    
    #loadActionItem() {
        this.httpService.get(`/api/action-item/${this.actionItemId}/details`)
            .then((actionItem) => {
                this.actionItem = actionItem;
            })
            .then(() => {
                this.#populateActionItemData();
            })
            .catch((error) => {
                console.error('Error loading action item:', error);
            });
       
    }

    #toggleEditDescription(edit) {
        $(`div[data-model="description"]`).toggleClass('d-none', edit);
        $(`textarea[data-model="description"]`).toggleClass('d-none', !edit);
        $(`div[data-toggle-edit="description"]`).toggleClass('d-none', !edit);
    }

    #showCompleteActionItemEmailSender() {
        const actionItem = this.actionItem;
        const actionItemCreator = actionItem.createdByUser;

        var companySettings = Auth.getCompanySettings();

        this.emailSender.clientId = actionItem.projectId;
        this.emailSender.clientName = actionItemCreator ? actionItemCreator.firstName : '';
        this.emailSender.senderName = Auth.currentUser().fullName;
        this.emailSender.companyName = companySettings.companyName;

        this.emailSender.setTo(actionItemCreator ? actionItemCreator.email : '');
        this.emailSender.setReplyTo(Auth.currentUser().email);

        this.emailSender.resetEmailFields();
        this.emailSender.showGreetings = true;
        this.emailSender.setSubject(`${actionItem.title} - Completed`, true);
        this.emailSender.setContent(`This action item that you assigned to me has been completed. Below are the additional notes upon completion: `);
        this.emailSender.type = Enums.CompleteActionItem;
        this.emailSender.ref2Id = actionItem.id;
        this.emailSender.onSentCallback = () => {
            actionItem.statusId = Enums.ActionItemStatus.Completed;
            actionItem.status = 'Completed';
            this.#displayStatus();
            this.#toggleButtons();
        }

        this.emailSender.toggle();
    }

    #initAssignedToSearch() {
        const instance = this;
        this.assignedToDropdown = $("#slc-action-item-details-assignedto").select2({
            minimumInputLength: 0,
            dropdownParent: $('#kt_modal_1'),
            dropdownPosition: 'below',
            ajax: {
                url: '/api/users',
                data: params => ({ search: params.term }),
                processResults: response => {
                    return {
                        results: response.data.map(item => ({
                            id: item.id,
                            firstName: item.firstName,
                            lastName: item.lastName,
                            email: item.email,
                            text: `${item.firstName} ${item.lastName}` // fallback for selection
                        }))
                    };
                }
            },
            templateResult: function (item) {
                if (!item.id) return item.text; // for placeholder
                return $(`<div>
                            <div><strong>${item.firstName} ${item.lastName}</strong></div>
                            <div style="font-size: 0.9em; color: #888;">${item.email}</div>
                        </div>`);
            },
            templateSelection: function (item) {
                return item.text || '';
            }
        });
        $("#slc-action-item-form-assignedto").on('select2:select', function (e) {
            instance.assignedTo = e.params.data;
        });
    }

    #initTabControl() {

        this.tabControl.actions = [
            {
                tabid: 'tab-notes',
                callback: () => {
                    this.#initNotesTab();
                }
            },
            {
                tabid: 'tab-activities',
                callback: () => {
                    
                }
            }

        ];
    }

    #initNotesTab() {
    }

    #loadProjects() {
        this.httpService.get('/api/dashboard/projects')
            .then(response => {
                this.projects = response;
                if (this.projects.length == 1 && Auth.permissions().UserRole == 'Client') {
                    const project = projects[0];
                    location.href = `/project/${project.id}`;
                }
                else {
                    this.actionItemForm.populateProjects(this.projects);
                }
            })
            .catch((status) => {
                if (status === 401) {
                    $("#dvProjectList").remove();
                }
            });
    }

    onEditDescription(b) {
        this.#toggleEditDescription(true);
    }

    onCancelEditDescription(b) {
        this.#toggleEditDescription(false);
    }

    onCommentInput(i) {
        const comment = $(i).val();
        $(`button[evt-click="onAddComment"]`).prop("disabled", comment === '');
    }

    onSaveDescription(b) {
        const actionItemId = this.actionItem.id;
        const description = $(`textarea[data-model="description"]`).val();
        const formattedDesc = description
            .replace(/&/g, '&amp;')   // escape HTML
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/\n/g, '<br>');  // convert line breaks to <br>

        b.setAttribute('data-kt-indicator', 'on');
        b.disabled = true;

        const payload = {
            id: actionItemId,
            description: formattedDesc
        };

        this.httpService.patch(`/api/action-items/${actionItemId}`, payload)
            .then(resp => {
                this.actionItem.description = formattedDesc;
                $(`div[data-model="description"]`).html(formattedDesc);
                this.#toggleEditDescription(false);

            })
            .finally(() => {
                b.removeAttribute('data-kt-indicator');
                b.disabled = false;
            })
    }

    onAddComment(b) {
        const comment = $("#txtComment").val();
        b.setAttribute('data-kt-indicator', 'on');
        b.disabled = true;

        const payload = {
            actionItemId: this.actionItem.id,
            comment: comment
        };

        $(`[evt-input="onCommentInput"]`).prop("disabled", true);
        this.httpService.post('/api/action-items/comment', payload)
            .then(resp => {
                const newComment = this.#commentHtml(resp);
                $('div.empty-comment-section').remove();
                $(`div[data-model="comments"]`).prepend(newComment);
                $("#txtComment").val('');
            })
            .finally(() => {
                $(`[evt-input="onCommentInput"]`).prop("disabled", false);
                b.removeAttribute('data-kt-indicator');
                b.disabled = false;
            })

        //b.removeAttribute('data-kt-indicator');
        //b.disabled = false;
    }

    onStartActionItem(b) {
        b.setAttribute('data-kt-indicator', 'on');
        b.disabled = true;
        const actionItemId = this.actionItem.id;

        this.service.acceptActionItem(actionItemId)
            .then((response) => {
                this.actionItem = response;
                this.#displayStatus();
                this.#toggleButtons();
                this.swal.alert('Action Item has been accepted!');
            })
            .finally(() => {
                b.removeAttribute('data-kt-indicator');
                b.disabled = false;
            });
    }

    onReviewActionItem(b) {
        const actionItem = this.actionItem;
        this.actionItemForm.onSaveActionItemCallback = (response) => {
            this.actionItem = response;
            this.#displayStatus();
            this.#toggleButtons();
        }
        this.actionItemForm.review(actionItem);
    }

    onEditActionItem(b) {
        const actionItem = this.actionItem;
        this.actionItemForm.onSaveActionItemCallback = (data) => {
            debugger;
            this.actionItem = data;
            this.#populateActionItemData();

        }
        this.actionItemForm.edit(actionItem);
    }

    onEditAssignedTo(b) {
        this.assignedToModal = $("#kt_modal_1").modal({
            backdrop: 'static'
        });

        $(this.assignedToModal).modal('show');
    }

    onSaveAssignedTo(b) {
        const actionItemId = this.actionItem.id;

        b.setAttribute('data-kt-indicator', 'on');
        b.disabled = true;

        const payload = {
            id: actionItemId,
            assignedSupervisors: this.assignedToDropdown.val().map(Number)
        };

        this.httpService.patch(`/api/action-items/${actionItemId}`, payload)
            .then(resp => {
                this.actionItem = resp;
                this.#populateAssignedTo();

                KTApp.initBootstrapTooltips();
                $(this.assignedToModal).modal('hide');
            })
            .finally(() => {
                b.removeAttribute('data-kt-indicator');
                b.disabled = false;
            })
    }

    onCompleteActionItem(b) {
        const actionItem = this.actionItem;
        const actionItemCreator = actionItem.createdByUser;
        const currentUser = Auth.currentUser();

        if (actionItemCreator.id != currentUser.id) {
            this.#showCompleteActionItemEmailSender();
        }
        else {
            b.setAttribute('data-kt-indicator', 'on');
            b.disabled = true;
            this.service.completeActionItem(actionItem.id)
                .then((response) => {
                    this.actionItem = response;
                    this.#displayStatus();
                    this.#toggleButtons();
                })
                .finally(() => {
                    b.removeAttribute('data-kt-indicator');
                    b.disabled = false;
                });
        }
    }

    onStart(b) {
        const id = this.actionItem.id;
        this.actionItem.status = "In Progress";

        this.httpService.patch(`/api/action-items/${id}/accept`);
        this.#displayStatus();
    }

    onComplete(b) {
        const id = this.actionItem.id;
        this.actionItem.status = "Completed";

        this.httpService.patch(`/api/action-items/${id}/complete`);
        this.#displayStatus();
    }

    onArchive(b) {
        const id = this.actionItem.id;
        this.actionItem.status = "Archived";

        this.httpService.patch(`/api/action-items/${id}/archive`);
        this.#displayStatus();
    }

    init() {
        this.#initTabControl();
        this.#initAssignedToSearch();
        this.#loadActionItem();
        this.#loadProjects();
    }
}

$(document).ready(() => {
    const actionItemView = new ActionItemView();
    actionItemView.init();
});