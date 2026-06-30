class CompanySettingsView extends DomEventComponent {
    constructor() {
        super();
        this.service = new CompanySettingsService();
        this.emails = [];
        this.swal = new SwalUtil();
        this.setting = null;
        this.settingForm = document.getElementById('form-setting-details');
        this.settingUploadForm = document.getElementById('form-setting-upload');
        this.formValidator = this.#setFormValidator();
        this.uploadValidator = this.#setUploadValidator();
        this.fileToUpload = null;
    }

    #setFormValidator() {
        return new FormValidator(this.settingForm, {
            'companyName': {
                validators: {
                    notEmpty: {
                        message: 'Company Name is required.'
                    }
                }
            },
        }, 'input-group').init();
    }

    #setUploadValidator() {
        return new FormValidator(this.settingForm, {
            'companyLogoInput': {
                validators: {
                    notEmpty: {
                        message: 'Upload is required.'
                    }
                }
            },
        }, 'input-group').init();
    }

    bindEventHandlers() {
        const instance = this;
    }

    populateSettings() {
        this.companySettings = this.setting == null ? new CompanySettingsModel() : this.setting;

        const companySettingsForm = new Form();
        companySettingsForm.formId = "form-setting-details";
        companySettingsForm.model = this.companySettings;
        companySettingsForm.init();

        if (this.setting?.companyLogoUrl) {
            const preview = document.getElementById('companyLogoPreview');
            preview.src = this.setting?.companyLogoUrl;
            preview.style.display = 'block';
        }

        this.#renderState(
            '#slcState',
            this.companySettings.state,
            (selectedValue) => {
                this.companySettings.state = selectedValue;
            }
        );
    }


    #renderState(selector, state, onChangeCallback) {
        // Set the initial value and trigger the change event
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
    }

    updateData(button) {
        const validator = this.formValidator;
            validator.validate().then((status) => {
                if (status == 'Valid') {
                    const setting = this.setting;
                    button.setAttribute("data-kt-indicator", "on");
                    button.disabled = true;

                    this.service.saveCompanySettings(setting)
                        .then((data) => {
                            this.setting = data;
                            this.populateSettings();
                            this.swal.alert('Successfully saved company settings!');
                        }).finally(() => {
                            button.setAttribute("data-kt-indicator", "off");
                            button.disabled = false;
                        });
                }
        });
    }

    upload(event) {
        const file = event.files[0];
        this.fileToUpload = file;
        const preview = document.getElementById('companyLogoPreview');
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {

                preview.src = e.target.result;
                preview.style.display = 'block';
            };
            reader.readAsDataURL(file);
        }
        else {
            preview.src = '';
            preview.style.display = 'none';
        }
    }

    async saveUpload(button) {
        const validator = this.uploadValidator;
        const status = await validator.validate();

        if (status === 'Valid' && this.fileToUpload) {
            const formData = new FormData();
            formData.append("file", this.fileToUpload);

            button.setAttribute("data-kt-indicator", "on");
            button.disabled = true;

            const { success, data, error } = await this.uploadFile(formData);

            if (success) {
                this.setting = data;
                this.populateSettings();
            } else {
                console.error("Upload error:", error);
            }

            button.setAttribute("data-kt-indicator", "off");
            button.disabled = false;
        }
    }

    async uploadFile(formData) {
        const response = await fetch("/api/upload/company-logo", {
            method: "POST",
            body: formData,
        });

        if (!response.ok) {
            return { success: false, error: response.statusText };
        }

        const json = await response.json();
        return { success: true, data: json.data };
    }

    init() {
        debugger;
        this.setting = Auth.getCompanySettings();
        this.populateSettings();
        this.bindEventHandlers();
    }
}

$(document).ready(() => {
    const view = new CompanySettingsView();
    view.init();
});
