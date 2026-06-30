class EmailsView extends DomEventComponent {
    constructor() {
        super();
        this.service = new EmailsService();
        this.emails = [];
        this.searchQuery = "";
        this.sortBy = "latest"; // or "oldest"
        this.statusFilter = ""; // "read", "unread", or ""
        this.sendEmailForm = document.getElementById('kt_inbox_compose_form');
        this.sendEmailFormValidator = this.#setSendEmailFormValidator();
        this.swal = new SwalUtil();
        this.projects = [];
		this.inbox = null;
		this.showAllProjectInbox = true;
		this.currentFilterProject = null;
		this.emailComposerContent = null;
		this.currentMessage = null;
		this.userEmails = [];
		this.emailSender = null;
		this.onSearchEmailTimeOut = null;
		this.onSearchProjectTimeOut = null;
		this.replyMessageId = null;
    }

    #setSendEmailFormValidator() {
        return new FormValidator(this.sendEmailForm, {
            'send-email-component-to': {
                validators: {
                    notEmpty: {
                        message: 'To is required'
                    }
                }
            },
            'send-email-component-reply-to': {
                validators: {
                    notEmpty: {
                        message: 'Reply To is required'
                    }
                }
            },
            'send-email-component-subject': {
                validators: {
                    notEmpty: {
                        message: 'Subject is required'
                    }
                }
            }
        }, 'fv-row').init();
    }

    bindEventHandlers() {
        const instance = this;
        // Search input
        $('[data-kt-inbox-listing-filter="search"]').on('input', function () {
            instance.searchQuery = $(this).val().toLowerCase();
            instance.applyFilters();
        });
		
    }

    countUnread() {
        const unreadCount = this.emails.filter(email => !email.isRead).length;
        if (unreadCount > 0) {
            $('#sentCount').text(`Unread (${unreadCount})`).show();
        } else {
            $('#sentCount').text('').hide();
        }
    }

    applyFilters() {
        let filtered = [...this.emails];

        // Filter by status
        if (this.statusFilter === 'read') {
            filtered = filtered.filter(m => m.isRead);
        } else if (this.statusFilter === 'unread') {
            filtered = filtered.filter(m => !m.isRead);
        }

        // Filter by search query
        if (this.searchQuery) {
            filtered = filtered.filter(m =>
                m.subject.toLowerCase().includes(this.searchQuery) ||
                m.to.toLowerCase().includes(this.searchQuery) ||
                m.body.toLowerCase().includes(this.searchQuery)
            );
        }

        // Sort by time
        filtered.sort((a, b) => {
            const timeA = new Date(a.dateCreated);
            const timeB = new Date(b.dateCreated);
            return this.sortBy === 'latest' ? timeB - timeA : timeA - timeB;
        });

        this.renderEmailContent(filtered);
    }

    renderEmailContent(list = []) {
        const tbody = $('#kt_inbox_listing tbody');
        tbody.empty();

        if (list.length === 0) {
            const noDataRow = `
            <tr>
                <td colspan="4" class="text-center text-muted py-10">
                    <span>No emails found.</span>
                </td>
            </tr>
        `;
            tbody.append(noDataRow);
            return;
        }

        this.countUnread();

        list.forEach(email => {
            const fontTitle = email.isRead ? "text-muted" : "fw-bold text-dark";
            const row = `
               <tr class="email-row" data-email-id="${email.id}">
                    <td class="ps-9 w-50px">
                        <div class="form-check form-check-sm form-check-custom form-check-solid mt-3">
                            <input class="form-check-input" type="checkbox" value="${email.id}" />
                        </div>
                    </td>
                    <td class="flex-grow-1">
                        <div class="d-flex justify-content-between align-items-center">
                            <div class="truncate-multiline-2">
                                <span class="email-subject ${fontTitle} fs-6">
                                 ${email.projectName ? `${email.projectName} - ${email.subject}` : email.subject}
                                </span>
                            </div>
                        </div>
                    </td>
                   <td class="text-end fs-7 pe-9 text-nowrap align-middle">
                        <span class="fw-bold">${email.sentAt}</span>
                    </td>
                </tr>

                <tr id="email-details-${email.id}" class="email-details-row" style="display: none;">
                    <td colspan="4" class="p-0 border-0">
                         <div class="email-preview d-flex gap-6 px-10 py-5">
                            <!-- Details -->
                            <div class="flex-grow-1">
                                <div class="text-muted fs-7 mb-3">${email.body}</div>
                                <div class="fs-7 text-gray-600">
                                    ${email.to ? `<div><strong>To:</strong> ${email.to}</div>` : ""}
                                    ${email.cc ? `<div><strong>CC:</strong> ${email.cc}</div>` : ""}
                                    ${email.bcc ? `<div><strong>BCC:</strong> ${email.bcc}</div>` : ""}
                                    ${email.replyTo ? `<div><strong>Reply To:</strong> ${email.replyTo}</div>` : ""}
                                    <div><strong>Sent:</strong> ${DateUtils.formatDateTime(email.dateCreated)}</div>
                                </div>
                                ${email.attachments.length > 0 ? `
                                    <div class="mt-3 fs-7 text-gray-600">
                                        <strong>Attachments:</strong>
                                            <ul class="list-unstyled">
                                                ${email.attachments.map(att => `
                                                    <li>
                                                        <a href="${att.fileUrl}" target="_blank" class="text-primary d-flex align-items-center">
                                                            <i class="fa fa-paperclip me-2"></i> ${att.fileName}
                                                        </a>
                                                    </li>
                                                `).join('')}
                                            </ul>
                                        </div>
                                ` : ''}
                            </div>
                        </div>
                    </td>
                </tr>
                `;
           
            tbody.append(row);
        });

        setTimeout(() => {
            $('[data-bs-toggle="tooltip"]').each(function () {
                // Initialize the Bootstrap tooltip for each element
                new bootstrap.Tooltip(this);
            });
        }, 0);
    }

    reinitializeTooltips() {
        setTimeout(() => {
            $('[data-bs-toggle="tooltip"]').tooltip('dispose');
            $('[data-bs-toggle="tooltip"]').tooltip({ container: 'body' });
        }, 0);
    }

    refresh() {
        $('[data-bs-toggle="tooltip"]').tooltip('dispose');
        $('[data-kt-check="true"]').prop('checked', false);
        $('#kt_inbox_listing .form-check-input').prop('checked', false);
        $('#searchInbox').val('');
        this.searchQuery = "";
        this.sortBy = "latest";
        this.statusFilter = ""; 
        this.renderEmailContent(this.emails);
    }

	onNewMessageClicked() {
		this.emailSender.toggle();
	}

	onSelectMessage(r) {
	
        const id = $(r).attr("data-id");
		const message = this.inbox.messages.find(m => m.messageId == id);
		message.isRead = true;
		this.replyMessageId = Guid.new();

		this.#renderSelectedMessage(message);

		APP.initKtAppEventHandlers();
        $("#dv-inbox-message-list").addClass('d-none');
		$("#dv-inbox-message-content").removeClass('d-none');

		this.markMessageAsRead(id);
		
	}

	onBackToList() {
		if (this.showAllProjectInbox)
			this.renderAllProjectsInbox();
        else if(this.currentFilterProject != null)
            this.renderProjectInbox(this.currentFilterProject);
        else
            this.renderAllProjectsInbox();
	}

	onShowFullMessage(b) {
		const id = $(b).attr("data-id");
		const message = this.inbox.messages.find(m => m.messageId == id);
		$(b).next().toggleClass('show');

		if (message.isRead == 0)
			this.markMessageAsRead(id);
	}

	onAddCC() {
		$(`div[data-kt-inbox-form="cc"]`).toggleClass('d-none').toggleClass('d-flex');
	}

	onAddBCC() {
		$(`div[data-kt-inbox-form="bcc"]`).toggleClass('d-none').toggleClass('d-flex');
	}

	onSendReply(b) {
		const replyText = this.emailComposerContent.getText();
		if (replyText.trim(`\n`) == '') {
			this.swal.info('Reply message cannot be empty.');
			return;

		}
		const messageBody = $("#dv-inbox-compose-form").find('div.ql-editor').html();
		
		b.setAttribute('data-kt-indicator', 'on');
		b.disabled = true;

		// change logic here. Reply should always go to the latest message in the thread from the original replier
		// do not reply to own message
		
		var latestMessageFromReplier = this.inbox.messages.find(
			msg => !msg.from?.includes('@inbound.postmarkapp.com') && msg.projectId === this.currentMessage.projectId
		);

		const emailModel = {
			messageId: latestMessageFromReplier.messageId,
			to: latestMessageFromReplier.from,
			subject: latestMessageFromReplier.subject,
			cc: this.#getCc(),
			bcc: this.#getBcc(),
			body: messageBody,
			type: Enums.ClientEmail,
			clientId: latestMessageFromReplier.projectId,
			refId: latestMessageFromReplier.projectId,
			attachmentListId: this.replyMessageId
		};

		this.httpService.post('/api/email/reply', emailModel)
			.then(() => {
				this.swal.alert("Your reply is currently being sent.", () => {
					const currentMessage = latestMessageFromReplier;//this.currentMessage;
					const user = Auth.currentUser();

					const reply = JSON.parse(JSON.stringify(this.currentMessage));
					reply.messageId = Guid.new();
					reply.senderName = user.fullName;
					reply.from = `${user.fullName} <${user.initials.toLowerCase()}${user.id}@inbound.postmarkapp.com>`;
					reply.to = currentMessage.from;
					reply.body = emailModel.body;
					reply.senderInitials = user.initials;
					reply.isRead = false;
					this.inbox.messages.unshift(reply);
					this.#renderSelectedMessage(reply);
				});

			})
			.finally(() => {
				b.removeAttribute('data-kt-indicator');
				b.disabled = false;
			});
	}

	onSearchEmail() {
		clearTimeout(this.onSearchEmailTimeOut);
		this.onSearchEmailTimeOut = setTimeout(() => {
			this.#searchEmail();
		}, 500);
	}

	onSearchProject() {
		clearTimeout(this.onSearchProjectTimeOut);
		this.onSearchProjectTimeOut = setTimeout(() => {
			this.#searchProject();
		}, 500);
	}

	onArchive(b) {
		const selectedIds = $('#kt_inbox_listing .form-check-input:checked')
			.map(function () {
				return $(this).data('id');
			})
			.get();

		this.inbox.messages = this.inbox.messages.filter(m => !selectedIds.includes(m.messageId));
		this.onBackToList();

		this.httpService.patch('/api/email/inbox/bulk-archive', selectedIds);
	}

	onArchiveCurrentMessage(b) {
		const messageId = $(b).attr("data-id");

		this.inbox.messages = this.inbox.messages.filter(m => m.messageId != messageId);
		this.onBackToList();

		this.httpService.patch(`/api/email/inbox/${messageId}/archive`);
	}

	onDelete(b) {
		const selectedIds = $('#kt_inbox_listing .form-check-input:checked')
			.map(function () {
				return $(this).data('id');
			})
			.get();

		this.inbox.messages = this.inbox.messages.filter(m => !selectedIds.includes(m.messageId));
		this.onBackToList();

		this.httpService.delete('/api/email/inbox', selectedIds);
	}

	onDeleteCurrentMessage(b) {
		const messageId = $(b).attr("data-id");

		this.inbox.messages = this.inbox.messages.filter(m => m.messageId != messageId);
		this.onBackToList();

		this.httpService.delete(`/api/email/inbox/${messageId}`);
	}

	onDeleteCurrentMessage(b) {
		const messageId = $(b).attr("data-id");

		this.inbox.messages = this.inbox.messages.filter(m => m.messageId != messageId);
		this.onBackToList();

		this.httpService.delete(`/api/email/inbox/${messageId}`);
	}

	onMarkAsRead(b) {
		const selectedIds = $('#kt_inbox_listing .form-check-input:checked')
			.map(function () {
				return $(this).data('id');
			})
			.get();

		selectedIds.forEach(id => {
			const message = this.inbox.messages.find(m => m.messageId == id);
			const project = this.inbox.projects.find(p => p.id == message.projectId);
			message.isRead = 1;
			project.unreadCount = this.inbox.messages.filter(m => m.projectId == message.projectId && m.isRead == 0).length;
		});

		this.#renderProjectsList();
		this.onBackToList();
		this.httpService.patch('/api/email/inbox/bulk-mark-read', selectedIds);
	}

	markMessageAsRead(id) {
		const message = this.inbox.messages.find(m => m.messageId == id);
		const project = this.inbox.projects.find(p => p.id == message.projectId);
		message.isRead = 1;
		project.unreadCount = this.inbox.messages.filter(m => m.projectId == message.projectId && m.isRead == 0).length;
		this.#renderProjectsList();

		this.httpService.patch(`/api/email/inbox/mark-read/${id}`);
	}

	renderAllProjectsInbox() {
		this.showAllProjectInbox = true;
		this.currentFilterProject = null;

        const inboxMessages = this.inbox.messages.filter(m => m.replyToMessageId != null);
		const latestByReplyTo = inboxMessages.reduce((acc, msg) => {
            const key = msg.replyToMessageId;
            if (!key) return acc; // skip root messages

            const existing = acc[key];
            const currentDate = new Date(msg.dateCreated);

            if (!existing || new Date(existing.dateCreated) < currentDate) {
                acc[key] = msg;
            }

            return acc;
        }, {});

		const result = Object.values(latestByReplyTo);
		//const result = inboxMessages;

        $(".menu-link.project-filter").removeClass('active');
        $(".menu-link.project-filter.all").addClass('active');
		this.#renderInboxMessages(result);
    }

	renderProjectInbox(b) {
		this.showAllProjectInbox = false;
		this.currentFilterProject = b;

        var id = $(b).attr("data-id");

		const inboxMessages = this.inbox.messages.filter(m => m.projectId == id && m.replyToMessageId != null);

		const latestByReplyTo = inboxMessages.reduce((acc, msg) => {
			const key = msg.replyToMessageId;
			if (!key) return acc; // skip root messages

			const existing = acc[key];
			const currentDate = new Date(msg.dateCreated);

			if (!existing || new Date(existing.dateCreated) < currentDate) {
				acc[key] = msg;
			}

			return acc;
		}, {});

		const result = Object.values(latestByReplyTo);
        
        $(".menu-link.project-filter").removeClass('active');
        $(b).addClass('active');
		this.#renderInboxMessages(result);
	}

	onReplyAttachmentClick(b) {
		$("#reply-attachment").click();
	}

	onReplyAttachmentFileSelect(b, e) {
		const instance = this;
		const files = e.target.files;
		if (files.length > 0) {
			$(files).each((_, file) => {
				file.id = Guid.new();
				instance.#uploadFile(file);
			});
		}
	}

	onRemoveAttachment(b) {
		const fileName = $(b).attr('data-dz-remove');
		const fileId = $(b).attr('data-file-id');

		$(`div.dropzone-item[file-upload-id='${fileId}']`).remove();

		this.httpService.delete(`/api/email/attachment?fileName=${fileName}&attachmentListId=${this.replyMessageId}`);
	}

	loadInbox() {
		APP.setLoadingIndicator('[data-loading-indicator="true"]', true);
        this.httpService.get('/api/email/inbox')
            .then(response => {
                this.inbox = response;
                this.#renderProjectsList();
				this.renderAllProjectsInbox();

				APP.setLoadingIndicator('[data-loading-indicator="true"]', false);
            })
            .catch((status) => {
                
            });
	}

	#getRelativeTime(dateStr) {
		const rtf = new Intl.RelativeTimeFormat('en', { numeric: 'auto', style: 'long' });

		const inputDate = new Date(dateStr.replace(' ', 'T')); // fix format for parsing
		const now = new Date();
		const diffMs = now - inputDate;

		const units = [
			{ unit: 'year', ms: 1000 * 60 * 60 * 24 * 365 },
			{ unit: 'month', ms: 1000 * 60 * 60 * 24 * 30 },
			{ unit: 'week', ms: 1000 * 60 * 60 * 24 * 7 },
			{ unit: 'day', ms: 1000 * 60 * 60 * 24 },
			{ unit: 'hour', ms: 1000 * 60 * 60 },
			{ unit: 'minute', ms: 1000 * 60 },
			{ unit: 'second', ms: 1000 }
		];

		for (const { unit, ms } of units) {
			const diff = Math.floor(diffMs / ms);
			if (Math.abs(diff) >= 1) {
				return rtf.format(-diff, unit); // negative = "ago"
			}
		}

		return 'just now';
	}

	#formatToReadableDate(dateStr) {
		const date = new Date(dateStr);

		const day = date.getDate();
		const month = date.toLocaleString('en-US', { month: 'short' });
		const year = date.getFullYear();

		let hours = date.getHours();
		const minutes = date.getMinutes().toString().padStart(2, '0');
		const ampm = hours >= 12 ? 'pm' : 'am';

		hours = hours % 12 || 12; // convert to 12-hour format

		return `${day} ${month} ${year}, ${hours}:${minutes} ${ampm}`;
	}

	#initEmailSender(projects) {
		var emailSender = new EmailSender();
		//emailSender.onSendEmail = (b) => {
		//    this.#onSendEmail(b);
		//};

		emailSender.projects = projects;
		emailSender.requireProjectSelection(true);
		this.emailSender = emailSender;
	}

	#searchEmail() {
		const searchText = $(`[data-action="search"]`).val();
		let emails = this.inbox.messages.filter(m => m.replyToMessageId != null);

		if (!this.showAllProjectInbox) {
			const selectedProjectId = $(this.currentFilterProject).attr("data-id");
			emails = this.inbox.messages.filter(m => m.projectId == selectedProjectId);
		}

		if (searchText && searchText.trim() != '') {
			emails = emails.filter(m => m.subject?.toLowerCase().includes(searchText.toLowerCase())
				|| m.senderName?.toLowerCase().includes(searchText.toLowerCase())
				|| m.to?.toLowerCase().includes(searchText.toLowerCase()));

		}

		const latestByReplyTo = emails.reduce((acc, msg) => {
			const key = msg.replyToMessageId;
			if (!key) return acc; // skip root messages

			const existing = acc[key];
			const currentDate = new Date(msg.dateCreated);

			if (!existing || new Date(existing.dateCreated) < currentDate) {
				acc[key] = msg;
			}

			return acc;
		}, {});

		const result = Object.values(latestByReplyTo);

		this.#renderInboxMessages(result);
	}

	#searchProject() {
		const projects = this.inbox.projects;
		let filteredProjects = projects;
		const searchText = $(`[data-action="search-project"]`).val();

		if (searchText && searchText.trim() != '') {
			filteredProjects = filteredProjects.filter(m => m.name?.toLowerCase().includes(searchText.toLowerCase()));
		}

		var allProjects = filteredProjects.map(p => {
			return this.#renderProjectItem(p);
		}).join('');

		$('#menu-projects-list').html(allProjects);
	}

	#getCc() {
		if (this.emailCcTagify && this.emailCcTagify.value.length > 0) {
			return this.emailCcTagify.value.map(recipient => recipient.value).join(",");
		}
		return '';
	}

	#getBcc() {
		if (this.emailBccTagify && this.emailBccTagify.value.length > 0) {
			return this.emailBccTagify.value.map(recipient => recipient.value).join(",");
		}
		return '';
	}

	#renderProjectItem(p) {
		const renderTotalMessages = (p) => {
			return `<span class="badge badge-secondary">${p.totalCount}</span>`;
		}
		const renderTotalUnread = (p) => {
			const totalUnread = p.unreadCount;
			return totalUnread > 0 ? `<span class="badge badge-danger badge-circle">${totalUnread}</span>` : '';
		}

		return `<div class="menu-item mb-3">
				    <!--begin::Custom work-->
				    <span class="menu-link project-filter" data-id="${p.id}" evt-click="renderProjectInbox">
					    <span class="menu-title fw-bold">${p.name}</span>
					  
					   ${renderTotalUnread(p)}
				    </span>
				    <!--end::Custom work-->
			    </div>`;
	}

	#renderProjectsList() {
		const projects = this.inbox.projects;
		const totalUnread = projects.length > 0 ? projects.reduce((sum, pr) => sum + (pr.unreadCount || 0), 0) : 0;
		const totalUnreadHtml = totalUnread > 0 ? `<span class="badge badge-warning badge-circle">${totalUnread}</span>` : '';

		var allProjects = projects.map(p => {
			return this.#renderProjectItem(p);
		}).join('');

		$('#menu-projects-list').html(allProjects);
		$('#lbl-all-projects-list-filter').parent().find('span.badge').remove();
		$('#lbl-all-projects-list-filter').after(totalUnreadHtml);
	}

	#uploadFile(file) {
		const fileId = file.id;
		const fileUploadItem = this.#fileUploadItem(file);
		$("#kt_inbox_reply_attachments").removeClass("d-none");
		$("#kt_inbox_reply_attachments .dropzone .dropzone-items").prepend(fileUploadItem);

		// Send the file to the server
		const formData = new FormData();
		formData.append("file", file);
		formData.append("emailId", this.replyMessageId);

		this.httpService.post(`/api/email/attachment`, formData, true)
			.then(() => { $(`div.dropzone-item[file-upload-id='${fileId}']`).find('.dropzone-progress').removeClass('uploading').addClass('uploaded'); })
			.catch(error => this.swal.error("Upload error", error));
	}

	#fileUploadItem(file) {
		const size = (file.size / 1000).toFixed(2);
		return `
			<div class="dropzone-item dz-processing" file-upload-id="${file.id}">
				<div class="dropzone-file">
					<div class="dropzone-filename" title="${file.name}">
						<span>${file.name}</span>
						<strong>(<span><strong>${size}</strong> MB</span>)</strong>
					</div>
					<div class="dropzone-error"></div>
				</div>
				<div class="d-flex justify-content-center dropzone-progress text-primary uploading">
					<span class="spinner-border spinner-border-sm align-middle ms-2 upload-progress"></span>
					<span class="upload-status svg-icon svg-icon-success svg-icon-2x">✔</span>
				</div>
				<div class="dropzone-toolbar">
					<span class="" data-dz-remove="${file.name}" data-file-id="${file.id}" evt-click="onRemoveAttachment">
						<i class="bi bi-x fs-1"></i>
					</span>
				</div>
			</div>`;
	}

	#renderInboxMessages(inboxItems) {

		const formatDate = (dateStr) => {
			const date = new Date(dateStr);
			const now = new Date();

			const isSameYear = date.getFullYear() === now.getFullYear();

			const options = {
				month: 'short',
				day: 'numeric',
				...(isSameYear ? {} : { year: 'numeric' })
			};

			return new Intl.DateTimeFormat('en-US', options).format(date);


		}

		const renderTitle = (m) => {
			const isRead = m.isRead ? '' : 'fw-bolder';
			return `<td>
						<div class="text-dark mb-1">
							<!--begin::Heading-->
							<a href="javascript:" evt-click="onSelectMessage" data-id="${m.messageId}" class="text-dark">
								<span class="${isRead}">${m.subject} -</span>
                                <span class="text-muted">${m.message}</span>
							</a>
							<div class="d-flex mt-2 justify-content-start">
								<div class="badge badge-light-danger">${m.projectName}</div>
							</div>
							<!--end::Heading-->
						</div>
					</td>`;
		}

		let inboxMessagesHtml = inboxItems.map(m => {
			return `<tr>
						<td class="ps-9">
							<!--begin::Checkbox-->
							<div class="form-check form-check-sm form-check-custom form-check-solid mt-3">
								<input class="form-check-input" data-id="${m.messageId}" type="checkbox" value="1" />
							</div>
							<!--end::Checkbox-->
						</td>
						
						<td class="w-175px">
							<a href="javascript:" evt-click="onSelectMessage" data-id="${m.messageId}" class="d-flex align-items-center text-dark">
								<span class="fw-bold">${m.senderName}</span>
								<!--end::Name-->
							</a>
						</td>
						<!--end::Author-->
						<!--begin::Title-->
						${renderTitle(m)}
						<!--end::Title-->
						<!--begin::Date-->
						<td class="w-100px text-end fs-7 pe-9">
							<span class="fw-bold text-muted">${formatDate(m.dateCreated)}</span>
						</td>
						<!--end::Date-->
					</tr>` }).join('');

		inboxMessagesHtml = inboxMessagesHtml != '' ? inboxMessagesHtml : `<tr><td colspan="5" class="ps-9 text-center"><span class="text-muted">No mail! You're all caught up.</span></tr>`;

		$("#tbody-inbox-mails").html(inboxMessagesHtml);

		$("#dv-inbox-message-list").removeClass('d-none');
		$("#dv-inbox-message-content").addClass('d-none');
	}

	#renderSelectedMessage(message) {

		this.currentMessage = message;

		const header = (m) => {
			return `<div class="card-header align-items-center py-5 gap-5">
				<!--begin::Actions-->
				<div class="d-flex">
					<!--begin::Back-->
					<a href="javascript:" evt-click="onBackToList" class="btn btn-sm btn-icon btn-clear btn-active-light-primary me-3" data-bs-toggle="tooltip" data-bs-placement="top" title="" data-bs-original-title="Back">
						<!--begin::Svg Icon | path: icons/duotune/arrows/arr063.svg-->
						<span class="svg-icon svg-icon-1 m-0">
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
								<rect opacity="0.5" x="6" y="11" width="13" height="2" rx="1" fill="black"></rect>
								<path d="M8.56569 11.4343L12.75 7.25C13.1642 6.83579 13.1642 6.16421 12.75 5.75C12.3358 5.33579 11.6642 5.33579 11.25 5.75L5.70711 11.2929C5.31658 11.6834 5.31658 12.3166 5.70711 12.7071L11.25 18.25C11.6642 18.6642 12.3358 18.6642 12.75 18.25C13.1642 17.8358 13.1642 17.1642 12.75 16.75L8.56569 12.5657C8.25327 12.2533 8.25327 11.7467 8.56569 11.4343Z" fill="black"></path>
							</svg>
						</span>
						<!--end::Svg Icon-->
					</a>
					<!--end::Back-->
					<!--begin::Archive-->
					<a href="javascript:" data-id="${m.messageId}" evt-click="onArchiveCurrentMessage" class="btn btn-sm btn-icon btn-light btn-active-light-primary me-2" data-bs-toggle="tooltip" data-bs-placement="top" title="" data-bs-original-title="Archive">
						<!--begin::Svg Icon | path: icons/duotune/communication/com010.svg-->
						<span class="svg-icon svg-icon-2 m-0">
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
								<path d="M6 8.725C6 8.125 6.4 7.725 7 7.725H14L18 11.725V12.925L22 9.725L12.6 2.225C12.2 1.925 11.7 1.925 11.4 2.225L2 9.725L6 12.925V8.725Z" fill="black"></path>
								<path opacity="0.3" d="M22 9.72498V20.725C22 21.325 21.6 21.725 21 21.725H3C2.4 21.725 2 21.325 2 20.725V9.72498L11.4 17.225C11.8 17.525 12.3 17.525 12.6 17.225L22 9.72498ZM15 11.725H18L14 7.72498V10.725C14 11.325 14.4 11.725 15 11.725Z" fill="black"></path>
							</svg>
						</span>
						<!--end::Svg Icon-->
					</a>
					<!--end::Archive-->
					<!--begin::Delete-->
					<a href="javascript:" data-id="${m.messageId}" evt-click="onDeleteCurrentMessage"  class="btn btn-sm btn-icon btn-light btn-active-light-primary me-2" data-bs-toggle="tooltip" data-bs-placement="top" title="" data-bs-original-title="Delete">
						<!--begin::Svg Icon | path: icons/duotune/general/gen027.svg-->
						<span class="svg-icon svg-icon-2 m-0">
							<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
								<path d="M5 9C5 8.44772 5.44772 8 6 8H18C18.5523 8 19 8.44772 19 9V18C19 19.6569 17.6569 21 16 21H8C6.34315 21 5 19.6569 5 18V9Z" fill="black"></path>
								<path opacity="0.5" d="M5 5C5 4.44772 5.44772 4 6 4H18C18.5523 4 19 4.44772 19 5V5C19 5.55228 18.5523 6 18 6H6C5.44772 6 5 5.55228 5 5V5Z" fill="black"></path>
								<path opacity="0.5" d="M9 4C9 3.44772 9.44772 3 10 3H14C14.5523 3 15 3.44772 15 4V4H9V4Z" fill="black"></path>
							</svg>
						</span>
						<!--end::Svg Icon-->
					</a>
				</div>
				<!--end::Actions-->
				
			</div>`
		}

		const title = (m) => {
			return `<div class="d-flex flex-wrap gap-2 justify-content-between mb-8">
					    <div class="d-flex align-items-center flex-wrap gap-2">
						    <!--begin::Heading-->
						    <h2 class="fw-bold me-3 my-1">${m.subject}</h2>
						    <!--begin::Heading-->
						    <!--begin::Badges-->
						     <!--<span class="badge badge-light-primary my-1 me-2">inbox</span>-->
						    <span class="badge badge-light-danger my-1">${m.projectName}</span>
						    <!--end::Badges-->
					    </div>
				    </div>`;
		}

		const previousReply = (m, r) => {
			
			const renderMessage = (me) => {
				return `<div class="separator my-6">
						</div>
						<!--begin::Message accordion-->
						<div data-kt-inbox-message="message_wrapper">
							<!--begin::Message header-->
							<div class="d-flex flex-wrap gap-2 flex-stack cursor-pointer" data-kt-inbox-message="header" data-id="${me.messageId}" evt-click="onShowFullMessage">
								<!--begin::Author-->
								<div class="d-flex align-items-center">
									<!--begin::Avatar-->
									<div class="symbol symbol-100 me-4">
										<span class="symbol-label bg-muted text-dark fs-2">${me.senderInitials}</span>
									</div>
									<!--end::Avatar-->
									<div class="pe-5">
										<!--begin::Author details-->
										<div class="d-flex align-items-center flex-wrap gap-1">
											<a href="#" class="fw-bolder text-dark text-hover-primary">${me.senderName}</a>
											<!--begin::Svg Icon | path: icons/duotune/abstract/abs050.svg-->
											<span class="svg-icon svg-icon-7 svg-icon-success mx-3">
												<svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">
													<circle fill="#000000" cx="12" cy="12" r="8" />
												</svg>
											</span>
											<!--end::Svg Icon-->
											<span class="text-muted fw-bolder">${this.#getRelativeTime(me.dateCreated)}</span>
										</div>
										<!--end::Author details-->
										<div class="text-muted fw-bold mw-450px" data-kt-inbox-message="preview">
											${me.message}
										</div>
										<!--end::Preview message-->
									</div>
								</div>
								<!--end::Author-->
								<!--begin::Actions-->
								<div class="d-flex align-items-center flex-wrap gap-2">
									<!--begin::Date-->
									<span class="fw-bold text-muted text-end me-3">${this.#formatToReadableDate(me.dateCreated)}</span>
									<!--end::Date-->
									<div class="d-flex">
										
									</div>
								</div>
								<!--end::Actions-->
							</div>
							<!--end::Message header-->
							<!--begin::Message content-->
							<div class="collapse fade" data-kt-inbox-message="message">
								<div class="py-5">
									${me.body}
								</div>
								${attachments(me)}
							</div>
							<!--end::Message content-->
						</div>`;
			}

			var reply = this.inbox.messages.filter(i =>
				i.projectId == m.projectId &&
				i.messageId != m.messageId &&
				(i.replyToMessageId == m.replyToMessageId) &&
				!r.includes(i.messageId))[0];

			if (reply) {
				var retval = renderMessage(reply);
				r.push(reply.messageId);
				retval += previousReply(reply, r); // look for next reply in thread
				return retval;
			}
			//get the source email message sent initially
			else {
				var sourceEmail = this.inbox.messages.find(i => i.messageId == m.replyToMessageId);
				return renderMessage(sourceEmail);
			}
			return ``;
		}

		const attachments = (m) => {
			if (m.attachments.length > 0) {
				return `<div class="py-5">
							<span class="svg-icon svg-icon-2 m-0">
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										<path opacity="0.3" d="M4.425 20.525C2.525 18.625 2.525 15.525 4.425 13.525L14.825 3.125C16.325 1.625 18.825 1.625 20.425 3.125C20.825 3.525 20.825 4.12502 20.425 4.52502C20.025 4.92502 19.425 4.92502 19.025 4.52502C18.225 3.72502 17.025 3.72502 16.225 4.52502L5.82499 14.925C4.62499 16.125 4.62499 17.925 5.82499 19.125C7.02499 20.325 8.82501 20.325 10.025 19.125L18.425 10.725C18.825 10.325 19.425 10.325 19.825 10.725C20.225 11.125 20.225 11.725 19.825 12.125L11.425 20.525C9.525 22.425 6.425 22.425 4.425 20.525Z" fill="black"></path>
										<path d="M9.32499 15.625C8.12499 14.425 8.12499 12.625 9.32499 11.425L14.225 6.52498C14.625 6.12498 15.225 6.12498 15.625 6.52498C16.025 6.92498 16.025 7.525 15.625 7.925L10.725 12.8249C10.325 13.2249 10.325 13.8249 10.725 14.2249C11.125 14.6249 11.725 14.6249 12.125 14.2249L19.125 7.22493C19.525 6.82493 19.725 6.425 19.725 5.925C19.725 5.325 19.525 4.825 19.125 4.425C18.725 4.025 18.725 3.42498 19.125 3.02498C19.525 2.62498 20.125 2.62498 20.525 3.02498C21.325 3.82498 21.725 4.825 21.725 5.925C21.725 6.925 21.325 7.82498 20.525 8.52498L13.525 15.525C12.325 16.725 10.525 16.725 9.32499 15.625Z" fill="black"></path>
									</svg>
								</span> Attachments: <br/>
							${m.attachments.map(a => { return `<a href="${a.fileUrl}" target="_blank"><span class="badge badge-light my-1 me-1">${a.fileName}</span></a>` }).join('')}
						</div>`;
			}
			return ``;
		}

		const messageContent = (m) => {
			//var fm = m[0];
			var retval = `<div data-kt-inbox-message="message_wrapper">
					<!--begin::Message header-->
					<div class="d-flex flex-wrap gap-2 flex-stack cursor-pointer" data-kt-inbox-message="header">
						<!--begin::Author-->
						<div class="d-flex align-items-center">
							<!--begin::Avatar-->
							<div class="symbol symbol-100 me-4">
								<span class="symbol-label bg-dark text-light fs-2">${m.senderInitials}</span>
							</div>
							<!--end::Avatar-->
							<div class="pe-5">
								<!--begin::Author details-->
								<div class="d-flex align-items-center flex-wrap gap-1">
									<a href="#" class="fw-bolder text-dark text-hover-primary">${m.senderName}</a>
									<!--begin::Svg Icon | path: icons/duotune/abstract/abs050.svg-->
									<span class="svg-icon svg-icon-7 svg-icon-success mx-3">
										<svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">
											<circle fill="#000000" cx="12" cy="12" r="8"></circle>
										</svg>
									</span>
									<!--end::Svg Icon-->
									<span class="text-muted fw-bolder">${this.#getRelativeTime(m.dateCreated)}</span>
								</div>
								<!--end::Author details-->
								<!--begin::Message details-->
								<div data-kt-inbox-message="details">
									<span class="text-muted fw-bold">to me</span>
								</div>
								<!--end::Message details-->
								<!--begin::Preview message-->
								<div class="text-muted fw-bold mw-450px d-none" data-kt-inbox-message="preview">With resrpect, i must disagree with Mr.Zinsser. We all know the most part of important part....</div>
								<!--end::Preview message-->
							</div>
						</div>
						<!--end::Author-->
						<!--begin::Actions-->
						<div class="d-flex align-items-center flex-wrap gap-2">
							<!--begin::Date-->
							<span class="fw-bold text-muted text-end me-3">${this.#formatToReadableDate(m.dateCreated)}</span>
							<!--end::Date-->
							<div class="d-flex"></div>
						</div>
						<!--end::Actions-->
					</div>
					<!--end::Message header-->
					<!--begin::Message content-->
					<div class="collapse fade show" data-kt-inbox-message="message">
						<div class="py-5">
							${m.body}
						</div>
						${attachments(m)}
					</div>
					<!--end::Message content-->
				</div>`
			retval += previousReply(m, [m.messageId]);

			return retval;
		}

		const replyForm = (message) => {
			return `<form id="kt_inbox_reply_form" class="rounded border mt-10">
					<!--begin::Body-->
					<div class="d-block">
						<!--begin::To-->
						<div class="d-flex align-items-center border-bottom px-8 min-h-50px">
							<!--begin::Label-->
							<div class="text-dark fw-bolder w-75px">To:</div>
							<!--end::Label-->
							<!--begin::Input-->
							<tags class="tagify form-control border-0" tabindex="-1">
							   <tag title="e.smith@kpmg.com.au" contenteditable="false" spellcheck="false" tabindex="-1" class="tagify__tag tagify--noAnim" value="1" name="Emma Smith" avatar="avatars/150-1.jpg" email="e.smith@kpmg.com.au">
								  <x title="" class="tagify__tag__removeBtn" role="button" aria-label="remove tag"></x>
								  <div class="d-flex align-items-center">
									 <span class="tagify__tag-text">${message.senderName}</span>
								  </div>
							   </tag>
							  
							</tags>
							
							<!--end::Input-->
							<!--begin::CC & BCC buttons-->
							<div class="ms-auto w-75px text-end">
								<span class="text-muted fs-bold cursor-pointer text-hover-primary me-2" data-kt-inbox-form="cc_button" evt-click="onAddCC">Cc</span>
								<span class="text-muted fs-bold cursor-pointer text-hover-primary" data-kt-inbox-form="bcc_button" evt-click="onAddBCC">Bcc</span>
							</div>
							<!--end::CC & BCC buttons-->
						</div>
						<!--end::To-->
						<!--begin::CC-->
						<div class="d-none align-items-center border-bottom ps-8 pe-5 min-h-50px" data-kt-inbox-form="cc">
							<!--begin::Label-->
							<div class="text-dark fw-bolder w-75px">Cc:</div>
							<!--end::Label-->
							<input type="text" class="form-control border-0" id="send-email-component-cc" name="send-email-component-cc" value="">
							<!--begin::Close-->
							<span class="btn btn-clean btn-xs btn-icon" data-kt-inbox-form="cc_close" evt-click="onAddCC">
								<i class="la la-close"></i>
							</span>
							<!--end::Close-->
						</div>
						<!--end::CC-->
						<!--begin::BCC-->
						<div class="d-none align-items-center border-bottom inbox-to-bcc ps-8 pe-5 min-h-50px" data-kt-inbox-form="bcc">
							<!--begin::Label-->
							<div class="text-dark fw-bolder w-75px">Bcc:</div>
							<!--end::Label-->
							<input type="text" class="form-control border-0" id="send-email-component-bcc" name="send-email-component-bcc" value="">
							<!--begin::Close-->
							<span class="btn btn-clean btn-xs btn-icon" data-kt-inbox-form="bcc_close" evt-click="onAddBCC">
								<i class="la la-close"></i>
							</span>
							<!--end::Close-->
						</div>
						<!--end::BCC-->
						<!--begin::Subject-->
						<div class="border-bottom">
							<input class="form-control border-0 px-8 min-h-45px" name="compose_subject" placeholder="Subject" value="${message.subject}" />
						</div>
						<!--end::Subject-->
						<div id="dv-inbox-compose-form" class="border-0 h-250px px-3"></div>
					</div>
					<!--end::Body-->
					<!--begin::Footer-->
					
					<div class="d-flex flex-column border-top">
						<div class="w-100">
							<div class="d-flex d-none p-3" id="kt_inbox_reply_attachments">
									<div class="dropzone dropzone-queue w-100">
										<div class="dropzone-items"></div>
									</div>
							</div>
						</div>
						<!--begin::Actions-->
						<div class="w-100 flex-stack flex-wrap gap-2 py-5 ps-8 pe-5 align-items-center me-3">
							<!--begin::Send-->
							<div class="btn-group me-4">
								<button type="button" class="btn btn-sm btn-primary fs-bold px-6" evt-click="onSendReply">
									<span class="svg-icon svg-icon-1">
										${doutune.sendEmail}
									</span>
									<span class="indicator-label">Send</span>
									<span class="indicator-progress">
										Please wait...
										<span class="spinner-border spinner-border-sm align-middle ms-2"></span>
									</span>
								</button>
							</div>
							<!--end::Send-->
							<!--begin::Upload attachement-->
							<input type="file" id="reply-attachment" class="d-none" multiple evt-change="onReplyAttachmentFileSelect" />
							<button type="button" class="btn btn-sm btn-light" evt-click="onReplyAttachmentClick">
								<span class="svg-icon svg-icon-2 m-0">
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										<path opacity="0.3" d="M4.425 20.525C2.525 18.625 2.525 15.525 4.425 13.525L14.825 3.125C16.325 1.625 18.825 1.625 20.425 3.125C20.825 3.525 20.825 4.12502 20.425 4.52502C20.025 4.92502 19.425 4.92502 19.025 4.52502C18.225 3.72502 17.025 3.72502 16.225 4.52502L5.82499 14.925C4.62499 16.125 4.62499 17.925 5.82499 19.125C7.02499 20.325 8.82501 20.325 10.025 19.125L18.425 10.725C18.825 10.325 19.425 10.325 19.825 10.725C20.225 11.125 20.225 11.725 19.825 12.125L11.425 20.525C9.525 22.425 6.425 22.425 4.425 20.525Z" fill="black"></path>
										<path d="M9.32499 15.625C8.12499 14.425 8.12499 12.625 9.32499 11.425L14.225 6.52498C14.625 6.12498 15.225 6.12498 15.625 6.52498C16.025 6.92498 16.025 7.525 15.625 7.925L10.725 12.8249C10.325 13.2249 10.325 13.8249 10.725 14.2249C11.125 14.6249 11.725 14.6249 12.125 14.2249L19.125 7.22493C19.525 6.82493 19.725 6.425 19.725 5.925C19.725 5.325 19.525 4.825 19.125 4.425C18.725 4.025 18.725 3.42498 19.125 3.02498C19.525 2.62498 20.125 2.62498 20.525 3.02498C21.325 3.82498 21.725 4.825 21.725 5.925C21.725 6.925 21.325 7.82498 20.525 8.52498L13.525 15.525C12.325 16.725 10.525 16.725 9.32499 15.625Z" fill="black"></path>
									</svg>
								</span>
								Attach File(s)
							</button>
							
							<!--end::Upload attachement-->
						</div>
						<!--end::Actions-->
						<!--begin::Toolbar-->
						<div class="d-flex align-items-center">
						
						</div>
						<!--end::Toolbar-->
					</div>
					<!--end::Footer-->
				</form>`;
		}

		const messageDetailsHtml = `<div class="card">
                                        ${header(message)}
                                        <div class="card-body">
                                            ${title(message)}
											${messageContent(message)}
											${replyForm(message)}
                                        </div>
                                    </div>`;

		$("#dv-inbox-message-content").html(messageDetailsHtml);

		this.emailComposerContent = new Quill("#dv-inbox-compose-form", { modules: { toolbar: [[{ header: [1, 2, !1] }], ["bold", "italic", "underline"], ["image", "code-block"]] }, placeholder: "Type your text here...", theme: "snow" });
		this.#initializeTagify();
		APP.initKtAppEventHandlers();

		this.httpService.put('/api/email-notification-read/' + message.id, {});
	}

	#initializeTagify() {
		const emailAddresses = this.userEmails;
		const tagifyOptions = {
			enforceWhitelist: false,
			whitelist: emailAddresses,
			dropdown: {
				enabled: 1,
				maxItems: 5,
				searchKeys: ['value'],
				classname: "email-suggestions-list"
			},
			originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(', '),
			delimiters: " ", // comma and space trigger tag creation
			addTagOnBlur: true // optional: creates tag when input loses focus

		};

		const ccInput = document.querySelector("#send-email-component-cc");
		if (ccInput) this.emailCcTagify = new Tagify(ccInput, tagifyOptions);

		const bccInput = document.querySelector("#send-email-component-bcc");
		if (bccInput) this.emailBccTagify = new Tagify(bccInput, tagifyOptions);
	}

	#loadUserEmails() {
		this.httpService.get('/api/email/to-email-addresses')
			.then(response => {
				this.userEmails = response;
			});
	}

	#subscribeToNewMessageReceived() {
		const instance = this;
		signalRConnection.on("OnNewEmailReceived", function (email) {
			debugger;
			const project = instance.inbox.projects.find(p => p.id == email.projectId);
			if (project) {
                project.unreadCount = (project.unreadCount || 0) + 1;
            }
			instance.inbox.messages.unshift(email);
			instance.#renderProjectsList();
		});
	}

    init() {
        //this.loadData();
		this.loadInbox();
		this.#loadUserEmails();
		this.bindEventHandlers();
		this.#subscribeToNewMessageReceived();
		APP.subscribe('loadProjects', (response) => {
			this.#initEmailSender(response);
		});
		
    }
}

$(document).ready(() => {
    emailsView = new EmailsView();
	emailsView.init();
});
