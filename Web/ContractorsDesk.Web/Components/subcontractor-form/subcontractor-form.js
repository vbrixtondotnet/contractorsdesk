class SubcontractorForm extends DomEventComponent {
	constructor() {
		super();
		this.subcontractorFormDrawer = new Drawer(`#subcontractor-form-drawer`);
		this.mode = "new"; // or "edit
		this.form = document.getElementById('subcontractor-form-drawer-form');
		this.httpService = new httpService();
		this.swal = new SwalUtil();
		this.onSaveCallback = null;
		this.currentUser = Auth.currentUser();
		this.subcontractor = null; // This will hold the selected subcontractor object
		this.subContractors = [];
		this.subContractorTypes = [];
		this.projectId = Guid.empty;
		this.isNew = true;
	}

	populateProjects(projects) {
		this.projects = projects;
		this.#setProjectDropdown();
	}

	createNew() {
		this.mode = "new";
		this.subcontractorFormDrawer.toggle();
		this.#prepareForm();
	}

	edit(subcontractor) {
		this.subcontractor = subcontractor;
		this.mode = "edit";
		this.subcontractorFormDrawer.toggle();
		this.#prepareEditForm();
	}

	#setProjectDropdown() {
		const projects = this.projects;

		if (projects && this.projects.length > 0) {
			$("#slc-action-item-form-project").empty();
			$("#slc-action-item-form-project").append(`<option value="" disabled selected>Select Project</option>`);
			projects.forEach(p => {
				$("#slc-action-item-form-project").append(new Option(p.name, p.id));
			});

			$("#slc-action-item-form-project").select2({ placeholder: 'Select Project' });

			this.#fixSelect2Width();
		}
	}

	#preloadDropdown(id, disabledText, loadingText) {
		this.#resetDropdown(id, disabledText);
		$(id).select2({ placeholder: loadingText });
		this.#fixSelect2Width();
	}

	#resetDropdown(id,) {
		$(id).val("").trigger("change");
	}

	#initSubContractorSearch() {
		if (!this.projectId || this.projectId == Guid.empty) return;

		this.httpService.get("/api/sub-contractors?projectId=" + this.projectId+'&excludeExisting=true')
			.then((data) => {
				this.subContractors = data;
				this.#populateSubcontractors(data);
			});
	}

	#initSubContractorTypes() {
		this.httpService.get("/api/sub-contractors/types")
			.then((data) => {
				this.subContractorTypes = data;
				this.#populateSubcontractorCategories(data);
			});
	}

	#populateSubcontractorCategories(data) {
		$("#slc-subcontractor-category").empty();
		$("#slc-subcontractor-category").prop("disabled", false);
		$("#slc-subcontractor-category").append(`<option value="" disabled selected>Select Category</option>`);

		data.forEach(t => {
			$("#slc-subcontractor-category").append(new Option(t, t));
		});
		
		this.#fixSelect2Width();
	}

	#populateSubcontractors(data) {
		const instance = this;
		$("#slcSubcontractorSearch").empty();
		$("#slcSubcontractorSearch").prop("disabled", false);
		$("#slcSubcontractorSearch").append(`<option value="" disabled selected>Select Existing</option>`);

		data.forEach(t => {
			$("#slcSubcontractorSearch").append(new Option(t.name, t.id));
		});

		$("#slcSubcontractorSearch").on('select2:select', function (e) {
			const selectedId = $(e.currentTarget).val();
			instance.subcontractor = instance.subContractors.find(item => item.id === selectedId);
			$(`#slc-subcontractor-category`).val(`${instance.subcontractor.category}`).trigger("change");
			$(`#txt-subcontractor-name`).val(instance.subcontractor.name);
			$(`#txt-subcontractor-address`).val(instance.subcontractor.address);
			$(`#txt-subcontractor-city`).val(instance.subcontractor.city);
			$(`#txt-subcontractor-email`).val(instance.subcontractor.email);
			$(`#txt-subcontractor-phone`).val(instance.subcontractor.phone);
			$(`#slc-subcontractor-state`).val(instance.subcontractor.state).trigger('change');
			$(`#txt-subcontractor-company`).val(instance.subcontractor.company);
			$(`#txt-subcontractor-licenseno`).val(instance.subcontractor.licenseNo);
			$(`#txt-subcontractor-licenseexp`).val(instance.subcontractor.licenseExp);
			$(`#txt-subcontractor-liability-policyno`).val(instance.subcontractor.liabilityInsurancePolicyNo);
			$(`#txt-subcontractor-liability-policyexp`).val(instance.subcontractor.liabilityInsuranceExpiry);
			$(`#txt-subcontractor-comp-policyno`).val(instance.subcontractor.compInsurancePolicyNo);
			$(`#txt-subcontractor-comp-policyexp`).val(instance.subcontractor.compInsuranceExpiry);
			$(`#txt-subcontractor-bond-policyno`).val(instance.subcontractor.bondInsurancePolicyNo);
			$(`#txt-subcontractor-bond-policyexp`).val(instance.subcontractor.bondInsuranceExpiry);
			instance.isNew = false;
		});

		//$("#slc-subcontractor-type").select2({ placeholder: 'Select Type' });
		this.#fixSelect2Width();
	}
	
	#prepareForm() {
		const formTitle = this.mode == "new" ? "Add Sub Contractor" : "Edit Sub Contractor";
		$("#subcontractor-drawer-form-title").html(formTitle);

		$("#subcontractor-form-drawer-form div.input-group").removeClass('d-none');
		
		this.#resetDropdown("#slc-subcontractor-category", "Select a category");
		this.#resetDropdown("#slc-subcontractor-state", "Select a state");
		this.#resetDropdown("#slcSubcontractorSearch", "Select existing");

		$("#row-subcontractors-search-container").toggleClass('d-none', (!this.projectId || this.projectId == Guid.empty));
		this.isNew = true;
		this.form.reset();
	}

	#initDateFields() {
		$("#txt-subcontractor-licenseexp").flatpickr({
			dateFormat: "Y-m-d"
		});
		$("#txt-subcontractor-liability-policyexp").flatpickr({
			dateFormat: "Y-m-d"
		});
		$("#txt-subcontractor-comp-policyexp").flatpickr({
			dateFormat: "Y-m-d"
		});
		$("#txt-subcontractor-bond-policyexp").flatpickr({
			dateFormat: "Y-m-d"
		});
	}

	async #prepareEditForm() {
		const subcontractor = this.subcontractor;
		const formTitle = "Edit Sub Contractor";
		$("#subcontractor-drawer-form-title").html(formTitle);
		$(`#slc-subcontractor-category`).val(`${subcontractor.category}`).trigger("change");
		$(`#txt-subcontractor-name`).val(subcontractor.name);
		$(`#txt-subcontractor-address`).val(subcontractor.address);
		$(`#txt-subcontractor-city`).val(subcontractor.city);
		$(`#txt-subcontractor-zip`).val(subcontractor.zip);
		$(`#txt-subcontractor-email`).val(subcontractor.email);
		$(`#txt-subcontractor-phone`).val(subcontractor.phone);
		$(`#slc-subcontractor-state`).val(subcontractor.state).trigger('change');
		$(`#txt-subcontractor-company`).val(subcontractor.company);
		$(`#txt-subcontractor-licenseno`).val(subcontractor.licenseNo);
		$(`#txt-subcontractor-licenseexp`).val(subcontractor.licenseExp);
		$(`#txt-subcontractor-liability-policyno`).val(subcontractor.liabilityInsurancePolicyNo);
		$(`#txt-subcontractor-liability-policyexp`).val(subcontractor.liabilityInsuranceExpiry);
		$(`#txt-subcontractor-comp-policyno`).val(subcontractor.compInsurancePolicyNo);
		$(`#txt-subcontractor-comp-policyexp`).val(subcontractor.compInsuranceExpiry);
		$(`#txt-subcontractor-bond-policyno`).val(subcontractor.bondInsurancePolicyNo);
		$(`#txt-subcontractor-bond-policyexp`).val(subcontractor.bondInsuranceExpiry);
		this.isNew = false;
	}

    #fixSelect2Width() {
		$("#select2-slc-subcontractor-category-container").parent().parent().parent().css("width", "");
	}

	#isValidForm() {
		const form = this.form;

		const categorySelect = document.getElementById('slc-subcontractor-category');
		const categoryTextbox = document.getElementById('txt-subcontractor-category');
		const phoneInput = document.getElementById('txt-subcontractor-phone');
		const emailInput = document.getElementById('txt-subcontractor-email');

		const selectValue = categorySelect.value.trim();
		const textboxValue = categoryTextbox.value.trim();
		const phoneValue = phoneInput.value.trim();
		const emailValue = emailInput.value.trim();

		this.selectedCategory = selectValue;

		// Reset custom validity
		categorySelect.setCustomValidity('');
		categoryTextbox.setCustomValidity('');

		// Validate category
		const isCategoryValid = !!selectValue || !!textboxValue;
		if (!isCategoryValid) {
			categorySelect.setCustomValidity('Category is required.');
			categoryTextbox.setCustomValidity('Category is required.');
		} else {
			if (selectValue) {
				categoryTextbox.removeAttribute('required');
				categorySelect.setAttribute('required', 'required');
			}
			if (textboxValue) {
				categorySelect.removeAttribute('required');
				categoryTextbox.setAttribute('required', 'required');
			}
		}

		// Validate phone and email
		const isPhoneValid = !phoneValue || VALIDATIONS.isValidUSPhoneNumber(phoneValue);
		const isEmailValid = !emailValue || VALIDATIONS.isValidEmail(emailValue);

		// Final check
		const isFormValid = form.checkValidity() && isCategoryValid && isPhoneValid && isEmailValid;

		if (!isFormValid) {
			if (!isCategoryValid) {
				this.swal.info('Category is required.');
			} else if (!isPhoneValid) {
				this.swal.info('Phone Number is invalid');
			} else if (!isEmailValid) {
				this.swal.info('Email Address is invalid');
			} else {
				form.reportValidity();
			}
			return false;
		}

		return true;
	}

	#createSubcontractorPayload() {

		let categoryValue = $("#txt-subcontractor-category").val() || $("#slc-subcontractor-category").val() || null;

		return {
			projectId: this.projectId,
			name: $("#txt-subcontractor-name").val(),
			address: $("#txt-subcontractor-address").val(),
			city: $("#txt-subcontractor-city").val(),
			state: $("#slc-subcontractor-state").val(),
			email: $("#txt-subcontractor-email").val(),
			phone: $("#txt-subcontractor-phone").val(),
			zip: $("#txt-subcontractor-zip").val(),
			company: $("#txt-subcontractor-company").val(),
			licenseNo: $("#txt-subcontractor-licenseno").val(),
			licenseExp: $("#txt-subcontractor-licenseexp").val() || null,
			liabilityInsurancePolicyNo: $("#txt-subcontractor-liability-policyno").val(),
			liabilityInsuranceExpiry: $("#txt-subcontractor-liability-policyexp").val() || null,
			compInsurancePolicyNo: $("#txt-subcontractor-comp-policyno").val(),
			compInsuranceExpiry: $("#txt-subcontractor-comp-policyexp").val() || null,
			bondInsurancePolicyNo: $("#txt-subcontractor-bond-policyno").val(),
			bondInsuranceExpiry: $("#txt-subcontractor-bond-policyexp").val() || null,
			category: categoryValue,
			isNew: this.isNew,
			id: this.subcontractor ? this.subcontractor.id : null
		}
	}

	onSubContractorFormSave(b) {
	
		if (this.#isValidForm()) {
			b.setAttribute('data-kt-indicator', 'on');
			b.disabled = true;
			const message = this.mode === "new" ? "Sub Contractor created successfully." : "Sub Contractor updated successfully.";
			const payload = this.#createSubcontractorPayload();
			if (this.projectId != null) {
				this.httpService.post(`/api/projects/${this.projectId}/sub-contractor`, payload)
					.then((data) => {
						this.swal.alert(message);
						this.subcontractorFormDrawer.toggle();
						this.#initSubContractorTypes();
						this.form.reset();
						if (this.onSaveCallback)
							this.onSaveCallback(data);
					})
					.finally(() => {
						b.removeAttribute('data-kt-indicator');
						b.disabled = false;
					});
			}
		}
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
	
	init() {
		this.#initSubContractorSearch();
		this.#initSubContractorTypes();
		this.#prepareForm();
		this.#initDateFields();
		TextboxUtils.init();
	}
}