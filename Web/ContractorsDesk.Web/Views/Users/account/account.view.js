class AccountView extends DomEventComponent {
    constructor() {
        super();
        const pathSegments = window.location.pathname.split('/');
        const params = new URL(document.URL).searchParams;
        this.profileForm = new Form();
        this.changeEmailValidator = this.#setChangeEmailFormValidator();
        this.changePasswordValidator = this.#setChangePasswordFormValidator();
        this.updateProfileFormValidator = this.#setUpdateProfileFormValidator();
        this.avatarBase64 = null;
        this.avatarFileName = null;
        this.removeAvatar = false;
       
    }
    #setChangeEmailFormValidator() {
        return new FormValidator(document.getElementById('change-email-form'), {
            'email': {
                validators: {
                    notEmpty: {
                        message: 'Email Address is required.'
                    },
                    emailAddress: {
                        message: 'Invalid Email Address.'
                    }
                }
            },
            'password': {
                validators: {
                    notEmpty: {
                        message: 'Password is required.'
                    }
                }
            }
        }, 'fv-row').init();
    }
    #setChangePasswordFormValidator() {
        return new FormValidator(document.getElementById('change-password-form'), {
            'password': {
                validators: {
                    notEmpty: {
                        message: 'Password is required.'
                    },
                    stringLength: {
                        min: 8,
                        message: 'Password must be at least 8 characters long.'
                    }
                }
            },
            'confirmPassword': {
                validators: {
                    notEmpty: {
                        message: 'Please confirm password.'
                    }
                }
            }
        }, 'fv-row').init();
    }
    #setUpdateProfileFormValidator() {
        return new FormValidator(document.getElementById('update-profile-form'), {
            'firstName': {
                validators: {
                    notEmpty: {
                        message: 'Password is required.'
                    }
                }
            },
            'lastName': {
                validators: {
                    notEmpty: {
                        message: 'Please confirm password.'
                    }
                }
            }
        }, 'input-group').init();
    }
    #initAccountForm() {
        const formSelector = $("#form-profile");

        this.profileForm.formId = "form-profile";
        this.profileForm.model = {
            firstName: formSelector.find("input[data-model='firstName']").val(),
            lastName: formSelector.find("input[data-model='lastName']").val(),
            email: formSelector.find("input[data-model='email']").val(),
            phone: formSelector.find("input[data-model='phone']").val()
        };
        this.profileForm.init();
    }
    #loadAccount() {
        this.#initAccountForm();
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
    onAvatarSelect(e) {
        const instance = this;
        const input = document.querySelector('input[name="avatar"]');

        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = function (e) {
                instance.avatarBase64 = e.target.result.split(',')[1];
                instance.avatarFileName = input.files[0].name;
            };

            reader.readAsDataURL(input.files[0]);
        }
    }
    onAvatarCancel(e) {
        $("input[name='avatar']").val("");
        this.avatarBase64 = null;
        this.avatarFileName = null;
    }
    onAvatarRemove(e) {
        const blankAvatarUrl = `url('/assets/media/avatars/blank.png')`;
        $("input[name='avatar']").val("");
        this.avatarBase64 = null;
        this.avatarFileName = null;
        this.removeAvatar = true;
        $("div.image-input-wrapper").css("background-image", blankAvatarUrl);
        $("div.image-input-outline").css("background-image", blankAvatarUrl);
    }
    onCancelChangeEmail(e) {
        const email = $("#hdnEmail").val();
        $(`[data-text-model="email"]`).html(email);
        $(`[data-model="email"]`).val(email);
        this.onToggleChangeEmail();
    }
    onToggleChangeEmail(e) {
        $("[data-toggle='change-email-shown']").toggleClass("d-none");
        $("[data-toggle='change-email-hidden']").toggleClass("d-none");
    }
    onCancelChangePassword(e) {
        const form = $("#change-password-form");
        form.find("input[name='password']").val('');
        form.find("input[name='confirmPassword']").val('');
        this.onToggleChangePassword();
    }
    onToggleChangePassword(e) {
        $("[data-toggle='change-password-shown']").toggleClass("d-none");
        $("[data-toggle='change-password-hidden']").toggleClass("d-none");
    }
    onSubmitChangeEmail(b) {
        const email = $("input[data-model='email']").val();
        const password = $("input[data-model='change-email-password']").val();

        this.changeEmailValidator.validate().then((status) => {
            if (status === 'Valid') {
                b.setAttribute('data-kt-indicator', 'on');
                b.disabled = true;

                const payload = {
                    newEmail: email,
                    password: password
                };

                this.httpService.patch('/api/users/account/change-email', payload)
                    .then(() => {
                        $("#hdnEmail").val(email);
                        this.onCancelChangeEmail();
                        this.swal.alert("Your email address has been changed successfully!");
                    })
                    .finally(() => {
                        b.setAttribute("data-kt-indicator", "off");
                        b.disabled = false;
                    });
            }
        });
    }
    onSubmitChangePassword(b) {
        const form = $("#change-password-form");
        const password = form.find("input[name='password']").val();
        const confirmPassword = form.find("input[name='confirmPassword']").val();

        this.changePasswordValidator.validate().then((status) => {
            if (status === 'Valid') {
                b.setAttribute('data-kt-indicator', 'on');
                b.disabled = true;

                const payload = {
                    password: password,
                    confirmPassword: confirmPassword
                };

                this.httpService.patch('/api/users/account/change-password', payload)
                    .then(() => {
                        this.onCancelChangePassword();
                        this.swal.alert("Your password has been changed successfully!");
                    })
                    .finally(() => {
                        b.setAttribute("data-kt-indicator", "off");
                        b.disabled = false;
                    });
            }
        });
    }
    onSubmitProfile(b) {
        const form = $("#update-profile-form");
        const firstName = form.find("input[name='firstName']").val();
        const lastName = form.find("input[name='lastName']").val();
        const phone = form.find("input[name='phone']").val();
        const avatarUrl = $("#hdnAvatarUrl").val();

        this.updateProfileFormValidator.validate().then((status) => {
            if (status === 'Valid') {
                b.setAttribute('data-kt-indicator', 'on');
                b.disabled = true;

                const payload = {
                    firstName: firstName,
                    lastName: lastName,
                    phone: phone,
                    avatarFileName: this.avatarFileName,
                    avatarBase64: this.avatarBase64,
                    removeAvatar: this.removeAvatar,
                    avatarUrl: avatarUrl
                };

                this.httpService.put('/api/users/account', payload)
                    .then((data) => {
                        $("#hdnAvatarUrl").val(data.avatarUrl);
                        this.removeAvatar = false;
                        $(`img[data-model="avatarUrl"]`).attr("src", data.avatarUrl);
                        this.swal.alert("Your profile has been updated successfully!");
                    })
                    .finally(() => {
                        b.setAttribute("data-kt-indicator", "off");
                        b.disabled = false;
                    });
            }
        });
    }
    onShowPassword(e) {
        $(e).prev().attr("type", "text");
    }
    init() {
        const role = Auth.currentUser().role;
        this.#loadAccount();
        APP.initKtAppEventHandlers();
    }
    
}

ACCOUNTVIEW = null;
$(document).ready(() => {
    ACCOUNTVIEW = new AccountView();
    ACCOUNTVIEW.init();
});