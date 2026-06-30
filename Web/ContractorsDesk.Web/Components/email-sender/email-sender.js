class EmailSender {
	constructor() {
		this.emailDrawer = null;
		this.sendEmailEditor = null;
		this.to = null;
		this.replyTo = null;
		this.subject = null;
		this.refId = null;
		this.ref2Id = null;
		this.invoice = null;
		this.type = null;
		this.emailId = Guid.new();
		this.sendEmailForm = document.getElementById('kt_inbox_compose_form');
		this.validator = this.#setValidator();
		this.httpService = new httpService();
		this.swal = new SwalUtil();
		this.onSentCallback = null;
		this.clientId = null;
		this.clientName = null;
		this.senderName = null;
		this.companyName = null;
		this.showGreetings = true;
		this.additionalRefId = null;
		this.revisionIds = null;

		this.emailToTagify = null;
		this.emailCcTagify = null;
		this.emailBccTagify = null;

		this.projects = [];
		this.selectedProjectId = null;
		this.isProjectRequired = false;

		this.render();
	}

	#setValidator() {

		let fields = {
			'send-email-component-to': {
				validators: {
					notEmpty: { message: 'To is required' }
				}
			},
			'send-email-component-reply-to': {
				validators: {
					notEmpty: { message: 'Reply To is required' }
				}
			},
			'send-email-component-subject': {
				validators: {
					notEmpty: { message: 'Subject is required' }
				}
			}
		};

		if (this.isProjectRequired) {
			fields['send-email-component-project'] = {
				validators: { notEmpty: { message: 'Project is required' } }
			};
		}

		var validator = new FormValidator(this.sendEmailForm, fields , 'fv-row');

		return validator.init();
		
	}

	setContent(content) {
		const greetings = this.clientName ? `Dear ${this.clientName},` : 'Dear Sir/Madam,';
		const body = `<p>${greetings}</p>${content}`;
		const emailContent = `${body}<p>&nbsp;</p><p>Best regards,<br>${this.senderName ?? ''}<br>${this.companyName ?? ''}</p>`;
		this.sendEmailEditor.setData(this.showGreetings ? emailContent : content);
	}

	getContent() {
		return this.sendEmailEditor.getData();
	}

	getTo() {
		if (this.emailToTagify && this.emailToTagify.value.length > 0) {
			return this.emailToTagify.value.map(recipient => recipient.value).join(",");
		}
		return '';
	}

	getCc() {
		if (this.emailCcTagify && this.emailCcTagify.value.length > 0) {
			return this.emailCcTagify.value.map(recipient => recipient.value).join(",");
		}
		return '';
	}

	getBcc() {
		if (this.emailBccTagify && this.emailBccTagify.value.length > 0) {
			return this.emailBccTagify.value.map(recipient => recipient.value).join(",");
		}
		return '';
	}

	getReplyTo() {
		return $("#send-email-component-reply-to").val();
	}

	getSubject() {
		return $("#send-email-component-subject").val();
	}

	setTo(to) {
		$("#send-email-component-to").val(to);
		this.to = to;
	}

	setCc(cc) {
		$("#send-email-component-cc").val(cc);
	}

	setBcc(bcc) {
		$("#send-email-component-bcc").val(bcc);
	}

	setReplyTo(replyTo) {
		$("#send-email-component-reply-to").val(replyTo);
		this.replyTo = replyTo;
	}

	setSubject(subject, skipDefaultValues =false) {
		let subjectLine = (this.companyName ? this.companyName + ' - ' : '') + subject;
		subjectLine = skipDefaultValues ? subject : subjectLine;
		$("#send-email-component-subject").val(subjectLine);
		this.subject = subject;
	}

	toggle() {
		this.emailDrawer.toggle();
	}

	setBodyHeight(height) {
		$("#send-email-component-editor-document").css("height", height);
	}

	getAttachmentListId() {
		return this.emailId;
	}

	clearAttachmentListId() {
		this.emailId = null;
		$("#send-email-component-files").html('');
	}

	resetEmailFields() {
		$("#send-email-component-cc").val('');
		$("#send-email-component-bcc").val('');
		$("#send-email-component-subject").val('');
		$("#send-email-component-file-input").val('');
		this.clearAttachmentListId();
		this.setContent('');
		this.emailId = Guid.new();
	}

	uploadFile(file) {
		const fileId = file.id;
		const fileUploadItem = this.fileUploadItem(file);
		$("#send-email-component-attachments").removeClass("d-none");

		// Send the file to the server
		const formData = new FormData();
		formData.append("file", file);
		formData.append("emailId", this.emailId);

		fetch("/api/email/attachment", {
			method: "POST",
			body: formData
		})
			.then(response => {
				if (response.ok) {
					$(`div.dropzone-item[file-upload-id='${fileId}']`).find('.dropzone-progress').removeClass('uploading').addClass('uploaded');
				}
			})
			.catch(error => console.error("Upload error", error));

		$("#send-email-component-files").prepend(fileUploadItem);
		document.querySelector("#send-email-component-drawer").scrollTop = document.querySelector("#send-email-component-drawer").scrollHeight;
	}

	fileUploadItem(file) {
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
				<div class="dropzone-progress text-center text-primary uploading">
					<span class="spinner-border spinner-border-sm align-middle ms-2 upload-progress"></span>
					<span class="upload-status svg-icon svg-icon-success svg-icon-2x">✔</span>
				</div>
				<div class="dropzone-toolbar">
					<span class="dropzone-delete" data-dz-remove="${file.name}" data-file-id="${file.id}">
						<i class="bi bi-x fs-1"></i>
					</span>
				</div>
			</div>`;
	}

	addDummyAttachmentItem(fileName) {
		const dummyId = Guid.new();
		const dummyItem = `
			<div class="dropzone-item dz-processing" file-upload-id="${dummyId}">
				<div class="dropzone-file">
					<div class="dropzone-filename" title="${fileName}">
						<span>${fileName}</span>
					</div>
					<div class="dropzone-error"></div>
				</div>
				<div class="dropzone-progress text-center text-primary">
					<span class="upload-status svg-icon svg-icon-success svg-icon-2x">✔</span>
				</div>
			</div>`;
		$("#send-email-component-attachments").removeClass("d-none");
		$("#send-email-component-files").prepend(dummyItem);
	}

	removeAttachment(fileName, fileId) {
		$(`div.dropzone-item[file-upload-id='${fileId}']`).remove();

		fetch(`/api/email/attachment?fileName=${fileName}&attachmentListId=${this.emailId}`, {
			method: "DELETE"
		});
	}

	requireProjectSelection(require) {
		const selector = `[data-dropdown-model="projects"]`;
		this.isProjectRequired = require;
		const projects = this.projects;
		const instance = this;

		if (projects && this.projects.length > 0) {
			$(selector).empty();
			$(selector).append(`<option value="" disabled selected>Select Project</option>`);
			projects.forEach(p => {
				$(selector).append(new Option(p.name, p.id));
			});

			$(selector).select2({ placeholder: 'Select Project' });

			$(selector).on('select2:select', function (e) {
				const selectedItem = {
					id: e.params.data.id,
					name: e.params.data.text
				};
				debugger;
				instance.clientId = selectedItem.id;
			});
		}

		$(selector).parent().parent().toggleClass('d-none', !require);
		this.validator = this.#setValidator();
	}

	onSendEmail(button) {
		this.validator.validate().then((status) => {
			if (status === 'Valid') {
				const emailModel = {
					to: this.getTo(),
					cc: this.getCc(),
					bcc: this.getBcc(),
					subject: this.getSubject(),
					body: this.getContent(),
					replyTo: this.getReplyTo(),
					type: this.type,
					refId: this.refId,
					additionalRefId: this.additionalRefId,
					revisionIds: this.revisionIds,
					attachmentListId: this.getAttachmentListId(),
					clientId: this.clientId,
					ref2Id: this.ref2Id,
					invoice: this.invoice
				};

				button.setAttribute('data-kt-indicator', 'on');
				button.disabled = true;

				this.httpService.post('/api/email/send/', emailModel)
					.then(() => {
						this.swal.alert("Your email is currently being sent.");
						this.toggle();
						this.clearAttachmentListId();
						if (this.onSentCallback) this.onSentCallback();
					})
					.finally(() => {
						button.setAttribute("data-kt-indicator", "off");
						button.disabled = false;
					});
			}
		});
	}

	initEventHandlers() {
		const instance = this;

		$("#send-email-component-button").on("click", (b) => {
			instance.onSendEmail(b.currentTarget);
		});

		$("#send-email-component-attach-file-button").on("click", () => {
			$("#send-email-component-file-input").click();
		});

		$("#send-email-component-file-input").on("change", (e) => {
			const files = e.target.files;
			if (files.length > 0) {
				$(files).each((_, file) => {
					file.id = Guid.new();
					instance.uploadFile(file);
				});
			}
		});

		$("html").on("click", ".dropzone-delete", (e) => {
			const fileName = $(e.currentTarget).data("dz-remove");
			const fileId = $(e.currentTarget).data("file-id");
			instance.removeAttachment(fileName, fileId);
		});

		$("#send-email-component-btn-cc").on("click", () => {
			$("div.send-email-component-cc").toggleClass("d-none");
		});

		$("#send-email-component-btn-bcc").on("click", () => {
			$("div.send-email-component-bcc").toggleClass("d-none");
		});
	}

	async fetchToEmailAddresses() {
		const response = await fetch('/api/email/to-email-addresses');
		if (!response.ok) return [];

		const result = await response.json();
		return result.success ? result.data : [];
	}

	async initializeTagify() {
		const emailAddresses = await this.fetchToEmailAddresses();
		const tagifyOptions = {
			enforceWhitelist: false,
			whitelist: emailAddresses,
			dropdown: {
				enabled: 1,
				maxItems: 5,
				searchKeys: ['value'],
				classname: "email-suggestions-list"
			},
			originalInputValueFormat: valuesArr => valuesArr.map(item => item.value).join(', ')
		};

		const toInput = document.querySelector("#send-email-component-to");
		if (toInput) this.emailToTagify = new Tagify(toInput, tagifyOptions);

		const ccInput = document.querySelector("#send-email-component-cc");
		if (ccInput) this.emailCcTagify = new Tagify(ccInput, tagifyOptions);

		const bccInput = document.querySelector("#send-email-component-bcc");
		if (bccInput) this.emailBccTagify = new Tagify(bccInput, tagifyOptions);
	}

	async render() {
		this.emailDrawer = new Drawer(`#send-email-component-drawer`);

		await DecoupledEditor
			.create(document.querySelector('#send-email-component-editor-document'))
			.then(editor => {
				document.querySelector('#send-email-component-editor-toolbar').appendChild(editor.ui.view.toolbar.element);
				this.sendEmailEditor = editor;
			})
			.catch(console.error);

		this.initEventHandlers();
		await this.initializeTagify();
	}
}