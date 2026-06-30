class ProjectView extends DomEventComponent {
    constructor() {
        super();
        const pathSegments = window.location.pathname.split('/');
        const params = new URL(document.URL).searchParams;
        this.selectedTab = params.get('t');
        this.projectId = pathSegments.pop() || pathSegments.pop();
        this.proposalId = null;
        this.isAccepted = false;
        this.project = null;
        this.service = new ProjectsService();
        this.tabControl = new TabControl();
        this.journalEditor = null;
        this.notesEditor = null;
        this.onjournaleditingtimeout = null;
        this.onprojectNoteEditingTimeout = null;
        this.actionItems = [];
        this.tagifySelect = null;
        this.supervisors = [];
        this.selectedSupervisors = [];
        this.selectedAsstManagers = [];
        this.selectedOnsiteSupervisor = [];
        this.swal = new SwalUtil();
        this.client = null;
        this.projectSettings = null;
        this.templates = new ProjectTemplates();
        this.permissions = Auth.permissions();
        this.clientDocumentsRendered = false;
        this.clientDocsData = [];
        this.emailSender = null;
        this.emailType = "client emails";
        this.emailServiceType = 0;
        this.journalLoaded = false;
        this.clientForm = document.getElementById('form-client-details');
        this.clientFormValidator = this.#setClientFormValidator();
        this.refId = null;
        this.projectManagersDropdown = null;
        this.actionItemDetails = null;
        this.selectedSubContractor = null;
        this.invoiceDrawer = null;
        this.projects = [];
        this.constructionTasks = [];
        this.estimateCategories = [];
        this.clientSecondaryEmails = null;

        this.enableUnloadProtection = true;
        this.isInitializing = true;
        this.currentTab = null;
        this.actionItemList = new ActionItemList();
        this.actionItemList.enableAdd = true;
        this.actionItemList.render();
        this.actionItemList.projectId = this.projectId;
        this.subcontractorList = new SubContractorList();
        this.subcontractorList.enableAdd = true;
        this.subcontractorList.setProjectId(this.projectId);
        this.subcontractorList.onSaveCallback = (data) => { this.#loadSubContractors(data); };
        this.subcontractorList.render();
        this.emailTemplates = [];
        this.proposalView = new ProposalComponent();
        this.budgetToActualComponent = new BudgetToActualComponent();
        this.scheduleComponent = new ScheduleComponent();
    }
    #populateProjectData() {
        this.#filterActions();

        $("#project-view-header").removeClass("loading").addClass('loaded');

        $(`a[data-model="name"]`).html(this.project.name);
        const $statusLabel = $('span[data-model="status"]');
        const $statusText = this.project.isArchived ? 'Archived' : 'Active';
        const $statusClass = this.project.isArchived ? 'badge-secondary' : 'badge-primary';

        $statusLabel.html($statusText).addClass($statusClass);

        //$("#txt-project-openedDate").html(this.project.openedDateFormatted);
        const address = this.project.projectDetails.address;
        const city = this.project.projectDetails.city;
        const state = this.project.projectDetails.state;
        let fulLAddress = `No address provided`;

        if (address != null && address != '')
            fulLAddress = address;
        if (city != null && city != '')
            fulLAddress += `,${city}`;
        if (state != null && state != '')
            fulLAddress += `,${state}`;

        $(`div[data-model="startdate"]`).html(DateUtils.toStandardDate(this.project.startDate));
        $(`div[data-model="endDate"]`).html(DateUtils.toStandardDate(this.project.endDate));
        $('div[data-model="address"]').html(fulLAddress);
        $(`div[data-model="budget"]`).html(StringUtils.formatMoney(this.project.budget));
        $('img[data-model="photo-url"]').attr("src", this.project.photoUrl);

        if (this.project.proposalId != null)
            $("#btnManageEstimates").attr("href", `/revised-estimates/${this.project.proposalId}`);

        if (this.project.id != null)
            $("#btnManageSchedule").attr("href", `/project/${this.project.id}/schedule`);

        if (!this.project.hasSchedule) {
            $(`span[data-model="has-schedule"]`).removeClass('d-none');
            $(`a[data-tab-item="tab-action-schedule"]`).addClass('text-danger');
        }

        this.#populateSupervisors();
        this.#populateClientDetails();

        const projectNote = this.project.notes;
        if (projectNote != null)
            this.notesEditor.setData(projectNote);

        $("#dv-project-preloader").remove();
        $("#dv-project-main-container").show();
    }
    #setClientFormValidator() {
        return new FormValidator(this.clientForm, {
            'emailAddress': {
                validators: {
                    notEmpty: {
                        message: 'Email Address is required.'
                    },
                    emailAddress: {
                        message: 'Invalid Email Address.'
                    }
                }
            },
        }, 'input-group').init();
    }
    #filterActions() {
        const proposal = this.project.proposalId;
        if (proposal === null) {
            $("#sendProposalMenuItem").remove();
            $("#sendBudgetToActualReportMenuItem").remove();
        }

        if (!this.project.isActiveProjectSchedule)
            $("#sentScheduleReportMenuItem").remove();
    }
    #populateSupervisors() {
        const assignedTo = (a, i) => {
            const ii = i > 2 ? 0 : i;
            const bg = ['dark', 'warning', 'primary'];
            
            return `<div class="symbol symbol-30px symbol-circle me-3 mt-3" data-bs-toggle="tooltip" title="" data-bs-original-title="${a.firstName} ${a.lastName}">
				<span class="symbol-label bg-${bg[ii]} text-inverse-${bg[ii]} fw-bolder">${a.initials}</span>
			</div>`;
        }

        const projectManagers = this.project.projectManagers;
        const assignedToHtml = projectManagers.map((a, i) => { return assignedTo(a, i) }).join('');
        $('div[data-model="supervisors"]').html(assignedToHtml);

    }
    #populateClientDetails() {
        const instance = this;
        this.client = new ClientModel();

        if (this.project.clientDetails != null)
            this.client.map(this.project.clientDetails);
        
        const clientDataForm = new Form();
        clientDataForm.formId = "form-client-details";
        clientDataForm.model = this.client;

        var secondaryEmail = document.querySelector("#txtSecondaryEmail");
        this.clientSecondaryEmails = new Tagify(secondaryEmail);
        clientDataForm.init();

        $('#slcCustomer').select2({
            dropdownPosition: 'below',
            ajax: {
                url: '/api/customers',
                data: params => ({ key: params.term }),
                processResults: response => {
                    instance.customers = response.data;
                    return {
                        results: response.data.map(item => ({
                            id: item.id,
                            text: item.name
                        }))
                    };
                }
            }
        });

        $('#slcCustomer').on('select2:select', function (e) {
            const customer = e.params.data;
            const selectedCustomer = instance.customers.find(item => item.id === customer.id);

            if (selectedCustomer) {
                instance.client.id = selectedCustomer.id;
                $('#form-client-details').find(`input[data-model="name"]`).val(selectedCustomer.name).trigger('keyup');
                $('#form-client-details').find(`input[data-model="companyName"]`).val(selectedCustomer.companyName).trigger('keyup');
                $('#form-client-details').find(`input[data-model="address"]`).val(selectedCustomer.address).trigger('keyup');
                $('#form-client-details').find(`input[data-model="city"]`).val(selectedCustomer.city).trigger('keyup');
                $('#form-client-details').find(`select[data-model="state"]`).val(selectedCustomer.state).trigger('change');
                $('#form-client-details').find(`input[data-model="phone"]`).val(selectedCustomer.phone).trigger('keyup');
                $('#form-client-details').find(`input[data-model="emailAddress"]`).val(selectedCustomer.emailAddress).trigger('keyup');
            }
        });
    }
    #initProjectJournal() {
        DecoupledEditor
            .create(
                document.querySelector('#kt_docs_ckeditor_document')
            )
            .then(editor => {
                const toolbarContainer = document.querySelector('#kt_docs_ckeditor_document_toolbar');
                toolbarContainer.appendChild(editor.ui.view.toolbar.element);

                this.journalEditor = editor;
                // this.journalEditor.setData(data.journal);

                editor.isReadOnly = true;
                const editableDiv = document.getElementById('kt_docs_ckeditor_document');

                // Add event listener for the input event
                editableDiv.addEventListener('input', () => {
                    clearTimeout(this.onjournaleditingtimeout);
                    this.onjournaleditingtimeout = setTimeout(() => {
                        this.#onProjectJournalEditing();
                    }, 500);
                });
            })
            .catch(error => {
                console.error(error);
            });
    }
    #initProjectNotes() {
        DecoupledEditor
            .create(
                document.querySelector('#kt_docs_ckeditor_notes')
            )
            .then(editor => {
                const toolbarContainer = document.querySelector('#kt_docs_ckeditor_notes_toolbar');
                toolbarContainer.appendChild(editor.ui.view.toolbar.element);

                this.notesEditor = editor;
                // this.journalEditor.setData(data.journal);
                
                const editableDiv = document.getElementById('kt_docs_ckeditor_notes');

                 //Add event listener for the input event
                editableDiv.addEventListener('input', () => {
                    clearTimeout(this.onprojectNoteEditingTimeout);
                    this.onprojectNoteEditingTimeout = setTimeout(() => {
                        this.#onProjectNoteEditing();
                    }, 500);
                });
            })
            .catch(error => {
                console.error(error);
            });
    }
    #loadProjectJournal() {
        $("#tab-journal").addClass("loading").removeClass('loaded');
        this.service.loadProjectJournal(this.projectId)
            .then((data) => {
                const journal = data ? data.journal : '';
                this.journalEditor.setData(journal);
                this.journalLoaded = true;
                $("#tab-journal").removeClass("loading").addClass('loaded');
            });
    }
    #onProjectJournalEditing() {
        clearTimeout(this.onjournaleditingtimeout);
        const journalData = this.journalEditor.getData();
        const payload = { id: Guid.empty, journal: journalData };
        this.service.saveProjectJournal(this.projectId, payload);
    }
    #onProjectNoteEditing() {
        const notes = this.notesEditor.getData();
        const payload = { projectId: this.projectId, notes: notes };
        this.httpService.put(`/api/projects/${this.projectId}/project-note`, payload);
        //this.service.saveProjectJournal(this.projectId, payload);
    }
    #initProjectData(project) {
  
        this.project = project;
        this.proposalId = project.proposalId;

        this.proposalView.proposalId = this.proposalId;
        this.budgetToActualComponent.proposalId = this.proposalId;
        this.scheduleComponent.proposalId = this.proposalId;
        this.scheduleComponent.projectId = this.project.id;
 
        this.selectedSupervisors = this.project.projectManagers.map(supervisor => supervisor.id);
        this.selectedAsstManagers = this.project.assistantProjectManagers.map(supervisor => supervisor.id);
        this.selectedOnsiteSupervisor = this.project.onsiteSupervisors.map(supervisor => supervisor.id);
        
    }
    #loadProject() {
        const role = Auth.currentUser().role;
        this.service.loadProject(this.projectId)
            .then((project) => {
                this.#initProjectData(project);
            })
            .then(() => {
                this.#populateProjectData();
                this.#initTabControl();
                $("#tab-loading").removeClass('active');
                //$("#tab-action-items").addClass('active');
                this.#loadSupervisorUsers();
                this.#initEmailSender();
                this.#initInvoiceDrawer();
            })
            .catch((error) => {
                console.error('Error loading project:', error);
            });

    }
    #generateInvoiceNumber() {
        this.httpService.get('/api/invoices/invoiceno?clientId=' + this.projectId)
            .then(response => {
                this.invoiceDrawer.setInvoiceNumber(response);
                this.invoiceDrawer.toggle();
            });
    }
    #loadSupervisorUsers() {
        this.service.loadSupervisorUsers()
            .then((supervisors) => {
                this.supervisors = supervisors;
                return this.supervisors;
            })
            .then(() => {
                this.#initSupervisorControl();
            });
    }
    #initTabControl() {
        const baseCallBack = () => {
            APP.resetStickyActionBars();
        }

        this.tabControl.actions = [
            {
                tabid: 'tab-client-details',
                callback: () => {
                    this.currentTab = 'tab-client-details';
                    baseCallBack();
                }
            },
            {
                tabid: 'tab-action-items',
                callback: () => {
                    this.currentTab = 'tab-action-items';
                    if (!this.actionItemListLoaded) {
                        this.actionItemList.triggerFirstFilter();
                        this.actionItemListLoaded = true;
                    }
                    baseCallBack();
                }
            },
            {
                tabid: 'tab-journal',
                callback: () => {
                    this.currentTab = 'tab-journal';
                    this.#loadProjectJournal();
                    baseCallBack();
                }
            },
            {
                tabid: 'tab-sub-contractors',
                callback: () => {
                    this.currentTab = 'tab-sub-contractors';
                    this.#loadSubContractors();
                    baseCallBack();
                }
            },
            {
                tabid: 'tab-settings',
                callback: () => {
                    this.currentTab = 'tab-settings';
                    this.#initProjectSettings();
                    baseCallBack();

                    setTimeout(() => {
                        this.isInitializing = false;
                    }, 100); 
                }
            },
            {
                tabid: 'tab-client-documents',
                callback: () => {
                    this.currentTab = 'tab-client-documents';
                    this.#initClientDocuments();
                    baseCallBack();
                }
            },
            {
                tabid: 'tab-proposal',
                callback: () => {
                    this.currentTab = 'tab-proposal';
                    if (!this.proposalLoaded) {
                        this.proposalView.init();
                        this.proposalLoaded = true;
                    }
                    baseCallBack();
                }
            },
            {
                tabid: 'tab-budget-to-actual',
                callback: () => {
                    this.currentTab = 'tab-budget-to-actual';
                    if (!this.budgetToActualComponentLoaded) {
                        this.budgetToActualComponent.init();
                        this.budgetToActualComponentLoaded = true;
                    }
                    baseCallBack();
                }
            },
            {
                tabid: 'tab-schedule',
                callback: () => {
                    this.currentTab = 'tab-schedule';
                    if (!this.scheduleComponentLoaded) {
                        this.scheduleComponent.init();
                        this.scheduleComponentLoaded = true;
                    }
                    baseCallBack();
                }
            }

        ];

        switch (this.selectedTab) {
            case 'proposal':
                this.proposalView.init();
                this.proposalLoaded = true;
                break;
            case 'estimate-to-actual':
                this.budgetToActualComponent.init();
                this.budgetToActualComponentLoaded = true;
                break;
            case 'schedule':
                this.scheduleComponent.init();
                this.scheduleComponentLoaded = true;
                break;
            default:
                $("#tab-action-items").addClass('active');
                this.actionItemList.triggerFirstFilter();
                break;
        }
    }
    #initClientDocuments() {
        $("#tab-client-documents").removeClass("loaded").addClass('loading');
        this.service.getClientDocuments(this.projectId)
            .then((clientDocs) => {
                this.clientDocumentsRendered = true;
                const clientDocuments = new FileSystem(`div[data-content="client-documents"]`);
                clientDocuments.clientId = this.projectId;
                clientDocuments.storageName = "client-documents";
                clientDocuments.setData(clientDocs);
                clientDocuments.render();
                $("#tab-client-documents").removeClass("loading").addClass('loaded');
            });
    }
    #initProjectSettings() {
        this.#initSupervisorControl();
        this.projectSettings = new ProjectModel();
        this.projectSettings.jobTypeId = this.project.isSpecJob ? 2 : 1;
        this.projectSettings.statusId = this.project.isActive ? 2 : 1;

        if (this.project.projectDetails != null) {
            this.projectSettings.map(this.project.projectDetails);
        }

        this.projectSettingsForm = new Form();
        this.projectSettingsForm.formId = "form-project-settings";
        this.projectSettingsForm.model = this.projectSettings;
        this.projectSettingsForm.init();

        this.#renderStates();
        
    }
    #initSupervisorControl() {
        const initDropdown = (selector, selectedItems, supervisors) => {
            const sd = new SearchableDropdown();
            sd.data = supervisors;
            sd.iconText = (o) => o.initials;
            sd.optionText = (o) => `${o.firstName} ${o.lastName}`;
            sd.optionSelected = (o) => selectedItems.includes(o.id);
            sd.width = '1%';
            sd.disabled = !this.permissions.CanAssignJobs;
            sd.init(selector);
            return sd;
        };

        this.projectManagersDropdown = initDropdown("#slcSupervisor1", this.selectedSupervisors, this.supervisors);
        this.asstManagersDropdown = initDropdown("#slcSupervisor2", this.selectedAsstManagers, this.supervisors);
        this.onsiteSupervisorDropdown = initDropdown("#slcSupervisor3", this.selectedOnsiteSupervisor, this.supervisors);
    }
    #initEmailSender() {
        var emailSender = this.actionItemList.getEmailSenderInstance();
        var companySettings = Auth.getCompanySettings();
        
        emailSender.clientId = this.projectId;
        emailSender.clientName = this.client.name;
        emailSender.senderName = Auth.currentUser().fullName;
        emailSender.companyName = companySettings.companyName;

        emailSender.setTo(this.client.emailAddress);
        emailSender.setReplyTo(Auth.currentUser().email);
        
        this.emailSender = emailSender;

        this.budgetToActualComponent.emailSender = this.emailSender;
        this.scheduleComponent.emailSender = this.emailSender;
    }
    #initInvoiceDrawer() {
        this.invoiceDrawer = new InvoiceDrawer();
        this.invoiceDrawer.clientId = this.projectId;
        this.invoiceDrawer.projectName = this.project.name;
        this.invoiceDrawer.emailSender = this.emailSender;
        this.invoiceDrawer.render();
        this.invoiceDrawer.disableBillTo();
    }
    onCreateInvoiceClick(e) {
        this.invoiceDrawer.setInvoice();
        this.invoiceDrawer.setCurrentDate();
        this.invoiceDrawer.addNewItem();
        this.invoiceDrawer.renderInvoiceItems();
        this.#generateInvoiceNumber();
    }
    #loadSubContractors(data = null) {
        if (data == null) {
            this.subcontractorList.preload();
            this.subcontractorList.loadSubContractors();
        }
        else {
            this.subcontractorList.subContractors.unshift(data);
            this.subcontractorList.renderList();
        }
    }
    #initEventHandlers() {
       
        const $activeJobs = $('input[data-model="activeJobs"]');
        const $activeSpecJobs = $('input[data-model="activeSpecJobs"]');
        $activeJobs.on('change', function () {
            $activeSpecJobs.prop('checked', !this.checked);
        });
        $activeSpecJobs.on('change', function () {
            $activeJobs.prop('checked', !this.checked);
        });

        const $isActive = $('input[data-model="isActive"]');
        const $isArchived = $('input[data-model="isArchived"]');

        $isActive.on('change', function () {
            $isArchived.prop('checked', !this.checked);
        });

        $isArchived.on('change', function () {
            $isActive.prop('checked', !this.checked);
        });
    }
    #renderState(selector, state, onChangeCallback) {
        // Set the initial value and trigger the change event
        this.isInitializing = true;
        $(selector)
            .val(state)
            .trigger('change');

        // Handle the 'change' event to update the state
        $(selector).on('change', function () {
            const selectedValue = $(this).val();
            if (typeof onChangeCallback === 'function') {
                onChangeCallback(selectedValue);
            }
        });

        // Handle the 'select2:open' event to focus on the search box
        $(selector).on('select2:open', function () {
            $('.select2-search__field').focus();
        });
        this.isInitializing = false;
    }
    #renderStates() {
        const clientModel = this.client;
        const projectModel = this.projectSettings;

        this.#renderState(
            '#slcCustomerState',
            clientModel.state ?? '',
            (selectedValue) => {
                clientModel.state = selectedValue;
            }
        );

        this.#renderState(
            '#slcProjectState',
            projectModel.state ?? '',
            (selectedValue) => {
                projectModel.state = selectedValue;
            }
        );
    }
    #parser() {
        const html = this.journalEditor.getData();

        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = html;

        const h2s = tempDiv.querySelectorAll('h2');

        let firstWeeklyUpdateStart = null;

        for (let h2 of h2s) {
            const txt = h2.textContent || h2.innerText;
            if (txt.includes('Weekly Update')) {
                firstWeeklyUpdateStart = h2;
                break;
            }
        }

        let futureWeeksStart = null;

        if (firstWeeklyUpdateStart) {
            let pointer = firstWeeklyUpdateStart.nextElementSibling;

            while (pointer) {
                const txt = pointer.textContent || pointer.innerText;

                if (pointer.tagName === 'H2' && txt.includes('Future Weeks Work')) {
                    futureWeeksStart = pointer;
                    break;
                }

                pointer = pointer.nextElementSibling;
            }
        }

        let allItems = [];

        if (futureWeeksStart) {
            let pointer = futureWeeksStart.nextElementSibling;

            while (pointer) {
                // Stop at the next <h2> or if we've passed the relevant <ul>
                if (pointer.tagName === 'H2') break;

                // If we find the first <ul>, extract and break
                if (pointer.tagName === 'UL') {
                    allItems = Array.from(pointer.querySelectorAll('li'))
                        .map(li => li.innerHTML.trim())
                        .filter(item => item && item !== '&nbsp;');
                    break;
                }

                pointer = pointer.nextElementSibling;
            }
        }

        return allItems;
    }
    #parseAmount(value) {
        if (value != "")
            return parseFloat(value.replace(/[^0-9.-]+/g, "")) || 0;
    }
    #formatWithCommas(num) {
        if (isNaN(num)) return '0.00';


        const fixedNum = Number(num).toFixed(2);
        const parts = fixedNum.split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }
    formatPhone(e) {
        let value = $(e).val().replace(/\D/g, ''); // remove non-numeric characters

        if (value.length > 10) value = value.slice(0, 10); // limit to 10 digits

        let formatted = '';
        if (value.length > 0) formatted += '(' + value.slice(0, 3);
        if (value.length >= 4) formatted += ') ' + value.slice(3, 6);
        if (value.length >= 7) formatted += '-' + value.slice(6);

        $(e).val(formatted);
    }
    onChangePhoto(b) {
        $('input[data-model="photo"]').click();
    }
    onPhotoFileSelected(b, e) {
        //debugger;
        const files = e.target.files;
        if (files && files.length > 0) {
            Swal.fire({
                title: 'Uploading Photo...',
                text: 'Please wait while the photo is being processed.',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            debugger;
            const formData = new FormData();
            formData.append("file", files[0]);
            formData.append("projectId", this.projectId);
            formData.append("projectName", this.project.name);
            
            this.httpService.patch(`/api/projects/${this.projectId}/photo`, formData, true)
                .then(url => {
                    const $img = $('img[data-model="photo-url"]');

                    $img.one('load', function () {
                        Swal.close(); // Close the loading popup once image is fully rendered
                    }).attr('src', url).each(function () {
                        // Handle cached images that may not trigger 'load'
                        if (this.complete) $(this).trigger('load');
                    });


                    $(`img[data-model="photo-url"]`).attr("src", url);
                })
                .catch(() => {
                    Swal.close(); // Close the loading popup once image is fully rendered
                    Swal.fire({
                        title: 'Error',
                        text: 'Failed to upload the photo. Please try again or check the file.',
                        icon: 'error'
                    });
                })
        }

    }
    onUseClientAddress(b) {
        this.projectSettings.mapAddress(this.client);
        this.projectSettingsForm.updateFormFromModel();
    }
    onResetCustomer(b) {
        this.client.id = null;
        $('#form-client-details').find(`input[data-model="name"]`).val('').trigger('keyup');
        $('#form-client-details').find(`input[data-model="companyName"]`).val('').trigger('keyup');
        $('#form-client-details').find(`input[data-model="address"]`).val('').trigger('keyup');
        $('#form-client-details').find(`input[data-model="city"]`).val('').trigger('keyup');
        $('#form-client-details').find(`select[data-model="state"]`).val('').trigger('change');
        $('#form-client-details').find(`input[data-model="phone"]`).val('').trigger('keyup');
        $('#form-client-details').find(`input[data-model="emailAddress"]`).val('').trigger('keyup');
        $("#slcCustomer").val('0').trigger('change');
    }
    onUpdateProject(b) {
        const project = this.projectSettings;

        const mapToSupervisorData = (ids, supervisorTypeId) =>
            ids.map(id => ({ id, supervisorTypeId }));

        const supervisorsType1 = mapToSupervisorData(this.projectManagersDropdown.getSelectedValues(), 1);
        const supervisorsType2 = mapToSupervisorData(this.asstManagersDropdown.getSelectedValues(), 2);
        const supervisorsType3 = mapToSupervisorData(this.onsiteSupervisorDropdown.getSelectedValues(), 3);

        if (supervisorsType1.length === 0) {
            this.swal.error("Please select at least one Project Manager.");
            return;
        }

        const groupOfSupervisors = [
            ...supervisorsType1,
            ...supervisorsType2,
            ...supervisorsType3
        ];

        project.supervisors = groupOfSupervisors;
        project.id = this.projectId;
        project.jobTypeId = parseInt(project.jobTypeId);
        project.statusId = parseInt(project.statusId);

        b.setAttribute("data-kt-indicator", "on");
        b.disabled = true;

        this.httpService.put('/api/projects/' + this.projectId, project)
            .then((responseData) => {
                this.#initProjectData(responseData);
                this.swal.alert('Successfully saved project settings!');
            })
            .finally(() => {
                b.setAttribute("data-kt-indicator", "off");
                b.disabled = false;
            });
    }
    updateClientDetails(button) {
        const client = this.client;
        const secondaryEmails = this.clientSecondaryEmails.value.map(tag => tag.value);

        const buttonState = (loading) => {
            button.setAttribute("data-kt-indicator", loading ? "on" : "off");
            button.disabled = loading;
        };
        
        const showInfo = (message) => this.swal.info(message);
        const isValidPhone = !client.phone || VALIDATIONS.isValidUSPhoneNumber(client.phone);

        // Validate client fields
        if (!client.name?.trim()) return showInfo('Please enter client name.');
        if (!client.emailAddress?.trim()) return showInfo('Please enter client email address.');
        if (!VALIDATIONS.isValidEmail(client.emailAddress)) return showInfo('Client Email Address is invalid.');
        if (!isValidPhone) return showInfo('Phone Number is invalid.');

        buttonState(true);
        const payload = {
            id: client.id,
            name: client.name,
            companyName: client.companyName,
            address: client.address,
            city: client.city,
            state: client.state,
            phone: client.phone,
            emailAddress: client.emailAddress,
            secondaryEmailAddress: secondaryEmails.join(',')
        };

        this.service.updateClientDetails(payload, this.projectId)
            .then(data => {
                //this.client = data;
                $("#lblStatusEmailQuestionsClientName").html(`Questions for ${data.fullName}`);
                this.swal.alert('Successfully saved client details!');
                //this.#populateClientDetails(); 
            })
            .finally(() => buttonState(false));
        
    }
    sendStatusReport() {
        const emailTemplate = this.emailTemplates.find(e => e.emailType == 'Status Report');
        let listItem = this.#parser();
        let futureWeeksItems = listItem.length
            ? listItem.map(item => `<li>${item}</li>`).join('')
            : '';

        let emailTemplateBody = emailTemplate.body;
        emailTemplateBody = emailTemplateBody.replace(/\(Auto Fill Project Name - Don't Remove\)/g, this.project.name);
        emailTemplateBody = emailTemplateBody.replace(/\(Auto Fill Client Name - Don't Remove\)/g, this.client.fullName);
        emailTemplateBody = emailTemplateBody.replace(/<li>\s*\(Auto Fill Subs - Don't Remove\)\s*<\/li>/gi, futureWeeksItems);
        
        this.emailSender.resetEmailFields();
        this.emailSender.showGreetings = false;
        this.emailSender.setSubject(`Weekly Update - ${this.project.name}`);
        this.emailSender.setContent(`${emailTemplateBody}`);
        this.emailSender.type = Enums.StatusReport;
        this.emailSender.refId = this.project.id;
        this.emailSender.onSentCallback = () => {
            if (this.currentTab == 'tab-journal') {
                var statusReport = this.emailSender.getContent();
                const journal = this.journalEditor.getData();
                this.journalEditor.setData(statusReport + journal);
            }
        }
        this.emailSender.toggle();
    }
    sendClientEmail() {
        const emailTemplate = this.emailTemplates.find(e => e.emailType == 'Email To Client');

        this.emailSender.resetEmailFields();
        this.emailSender.showGreetings = true;
        this.emailSender.setSubject('');

        if (emailTemplate.body == '') {
            this.emailSender.setContent(`${emailTemplate.body}`);
        }
        else {
            this.emailSender.setContent(`${emailTemplate.body}`);
        }
        
        this.emailSender.type = Enums.ClientEmail;
        this.emailSender.refId = this.project.id;
        this.emailSender.onSentCallback = null;
        this.emailSender.toggle();
    }
    sendScheduleReport() {
        const emailTemplate = this.emailTemplates.find(e => e.emailType == 'Schedule Report');

        this.emailSender.resetEmailFields();
        this.emailSender.showGreetings = true;
        this.emailSender.onSentCallback = null;
        this.emailSender.addDummyAttachmentItem(`${this.project.name}.schedule.pdf`);
        this.emailSender.setSubject('Here is the Project Schedule Report you requested');
        this.emailSender.setContent(`${emailTemplate.body}`);
        this.emailSender.type = Enums.ScheduleReport;
        this.emailSender.refId = this.project.id;
        this.emailSender.toggle();
    }
    sendProposal() {
        const emailTemplate = this.emailTemplates.find(e => e.emailType == 'Proposal Report');

        this.emailSender.resetEmailFields();
        this.emailSender.showGreetings = true;
        this.emailSender.onSentCallback = null;
        this.emailSender.addDummyAttachmentItem(`${this.project.name}.proposal.pdf`);

        this.emailSender.setSubject('Here is the cost estimate you requested');
        this.emailSender.setContent(`${emailTemplate.body}`);
        this.emailSender.type = Enums.ProposalReport;
        this.emailSender.refId = this.project.proposalId;

        this.emailSender.toggle();
    }
    sendBudgetToActualReport() {
        const emailTemplate = this.emailTemplates.find(e => e.emailType == 'Budget To Actual Report');

        this.emailSender.resetEmailFields();
        this.emailSender.showGreetings = true;
        this.emailSender.onSentCallback = null;
        this.emailSender.addDummyAttachmentItem(`${this.project.name}.estimate-to-actual.pdf`);
        
        this.emailSender.setReplyTo(Auth.currentUser().email);
        this.emailSender.setSubject('Here is the Estimate To Actual Report you requested');
        this.emailSender.setContent(`${emailTemplate.body}`);

        this.emailSender.type = Enums.EstimateToActualReport;
        this.emailSender.refId = this.project.proposalId;
        this.emailSender.toggle();
    }
    toggleActionTypeFields(actionTypeId, actionItem) {
        // Hide all sections first
        $('#action-type-1-fields').addClass('d-none');
        $('#action-type-2-fields').addClass('d-none');

        if (actionTypeId == 1) {
            this.editActionItemFormValidator.addField('costChangeAmount', {
                validators: {
                    notEmpty: {
                        message: 'Cost Change Amount is required'
                    }
                }
            });
            // Show Estimate Category and Cost Change Amount
            $('#action-type-1-fields').removeClass('d-none');


            // Populate fields
            $('#estimateCategory-edit').val(actionItem.lineItemName || '');

            let editCostChangeAmount = actionItem.costChangeAmount || 0;
            let editCurrentAmount = actionItem.currentAmount || 0;

            $('#costChangeAmount-edit2').val(this.#formatWithCommas(editCostChangeAmount));
            $('#currentAmount-edit2').val(this.#formatWithCommas(editCurrentAmount));
            let revisedAmount = editCostChangeAmount + editCurrentAmount;
            $('#newRevisedTotal-edit2').val(this.#formatWithCommas(revisedAmount));
        } else if (actionTypeId == 2) {
            // Show Construction Task and No. of Days
            $('#action-type-2-fields').removeClass('d-none');

            // Populate fields
            $('#constructionTask-edit').val(actionItem.constructionTask || '');
            $('#noOfDays-edit').val(actionItem.noOfDaysChange || '');
        }
    }
    changeCostChangeAmount() {
        const currentAmount = this.#parseAmount($('#currentAmount-edit2').val() || 0);
        const newRevisedTotal = this.#parseAmount($('#newRevisedTotal-edit2').val() || 0);
        let costChangeAmount = 0;
        if (!isNaN(currentAmount) && !isNaN(newRevisedTotal)) {
            costChangeAmount = newRevisedTotal - currentAmount;
        }

        $('#costChangeAmount-edit2').val(this.#formatWithCommas(costChangeAmount));
    }
    changeNewRevisedTotal() {
        const currentAmount = this.#parseAmount($('#currentAmount-edit2').val() || 0);
        const costChangeAmount = this.#parseAmount($('#costChangeAmount-edit2').val() || 0);

        let newRevisedTotal = 0;
        if (!isNaN(currentAmount) && !isNaN(costChangeAmount)) {
            newRevisedTotal = currentAmount + costChangeAmount;
        }

        $('#newRevisedTotal-edit2').val(this.#formatWithCommas(newRevisedTotal));
    }
    #getEmailTemplates() {
        this.httpService.get("/api/email-template")
            .then(response => {
                this.emailTemplates = response;
            });
    }
    init() {
        const role = Auth.currentUser().role;
        this.#loadProject();
        this.#initProjectJournal();
        this.#initProjectNotes();
        this.#initEventHandlers();
        this.#getEmailTemplates();
        
        APP.subscribe('loadProjects', (response) => {
            this.projects = response;
            this.actionItemList.setProjects(this.projects);
        });


        KTApp.initBootstrapTooltips();
        KTMenu.init();
        KTMenu.initGlobalHandlers();
    }
}

class ProjectTemplates {
    constructor() {
    }
    options(arr, label) {
        let options = `<option value="">--Select Option--</option>`;
        options += arr.map(ar =>
            `<option value="${ar.id}">${ar[label]}</option>`
        ).join('');

        return options;
    }
}

MAINVIEW = null;
$(document).ready(() => {
    MAINVIEW = new ProjectView();
    MAINVIEW.init();
});