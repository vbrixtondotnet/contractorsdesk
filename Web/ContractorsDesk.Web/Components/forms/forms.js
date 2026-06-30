class Form {
    constructor() {
        this.model = null;
        this.formId = null;
    }

    init() {
        const formId = `#${this.formId}`;
        const model = this.model;
        $(formId).find('input').each((ind, obj) => {

            var propertyName = $(obj).attr("data-model");

            if (propertyName) {
                $(obj).val(model[propertyName]);

                $(obj).on("keyup", function () {
                    model[propertyName] = $(this).val();
                    //if (onchangecallback) onchangecallback();
                });
            }
        });

        $(formId).find('input[type="checkbox"]').each((ind, obj) => {
            var propertyName = $(obj).attr("data-model");

            if (propertyName) {
                $(obj).prop('checked', model[propertyName]);
                $(obj).on("change", function () {
                    var propertyName = $(obj).attr("data-model");
                    var checked = $(this).is(':checked');
                    var dataCheckedValue = $(this).attr('data-checked') ?? true;
                    var dataUnCheckedValue = $(this).attr('data-unchecked') ?? false;
                    model[propertyName] = checked ? dataCheckedValue : dataUnCheckedValue;

                    //if (onchangecallback) onchangecallback();
                });
            }
        });

        $(formId).find('select').each((ind, obj) => {

            var propertyName = $(obj).attr("data-model");

            if (propertyName) {
                $(obj).val(model[propertyName]);

                $(obj).on("change", function () {
                    model[propertyName] = $(this).val();
                    //if (onchangecallback) onchangecallback();
                });
            }
        });

        $(formId).find('input[type="date"]').each((ind, obj) => {
            var propertyName = $(obj).attr("data-model");
            var propertyValue = model[propertyName];

            if (propertyName) {
                if (propertyValue) {
                    $(obj).val(propertyValue.substr(0, 10));
                    $(obj).val(propertyValue);
                }

                $(obj).on("change", function () {
                    model[propertyName] = $(this).val();
                    //if (onchangecallback) onchangecallback();
                });
            }
        });
    }

    updateFormFromModel() {
        const model = this.model;
        const formId = `#${this.formId}`;
        $(formId).find('input').each((ind, obj) => {

            var propertyName = $(obj).attr("data-model");

            if (propertyName) {
                $(obj).val(model[propertyName]);
            }
        });

        $(formId).find('input[type="checkbox"]').each((ind, obj) => {
            var propertyName = $(obj).attr("data-model");

            if (propertyName) {
                $(obj).prop('checked', model[propertyName]);
            }
        });

        $(formId).find('select').each((ind, obj) => {
            var propertyName = $(obj).attr("data-model");

            if (propertyName) {
                $(obj).val(model[propertyName]).trigger('change');
            }
        });

        $(formId).find('input[type="date"]').each((ind, obj) => {
            var propertyName = $(obj).attr("data-model");
            var propertyValue = model[propertyName];

            if (propertyName) {
                if (propertyValue) {
                    $(obj).val(propertyValue.substr(0, 10));
                    $(obj).val(propertyValue);
                }
            }
        });
    }
}

class FormValidator {
    constructor(formElement, fields, rowSelector ='fv-row') {
        this.formElement = formElement;
        this.fields = fields;
        this.rowSelector = rowSelector;
    }

    init() {
        return FormValidation.formValidation(
            this.formElement,
            {
                fields: this.fields,
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: `.${this.rowSelector}`,
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );
    }
}