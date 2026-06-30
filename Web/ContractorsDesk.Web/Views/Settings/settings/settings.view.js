class SettingsView extends DomEventComponent {
	constructor() {
		super();
		this.swal = new SwalUtil();
	}

    uploadCompanyLogo(e) {
        var fileInput = document.getElementById('imageUpload');
        var file = fileInput.files[0];
        var formData = new FormData();
        formData.append('file', file);

        fetch('/api/upload/company-logo', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                this.swal.alert('Upload Success!');
            })
            .catch(error => {
                // Handle the error
            });
	}
	
	init() {
	
    }
}

$(document).ready(() => {
    const view = new SettingsView();
    view.init();
});