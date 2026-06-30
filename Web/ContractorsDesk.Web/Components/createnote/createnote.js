class CreateNote extends DomEventComponent {
    constructor() {
        super();
        this.httpService = new httpService();
        this.actionItemNote = null;
		this.swal = new SwalUtil();
        this.assignedTo = null;
    }

	#initAssignedToControl() {
		const instance = this;
		$("#slcCreateNoteAssignedTo").select2({
			minimumInputLength: 0,
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
        $("#slcCreateNoteAssignedTo").on('select2:select', function (e) {
            instance.assignedTo = e.params.data;
        });
    }

    #isValidForm() {
        const form = document.getElementById('frmCreateActionItemNote');
        const select = document.getElementById('slcCreateNoteAssignedTo');
        const title = document.getElementById('txtNoteTitle');

        select.setCustomValidity('');
        title.setCustomValidity('');

        if (!select.value || select.value === "0") {
            select.setCustomValidity('This field is required.');
            form.reportValidity();

            $('#slcCreateNoteAssignedTo').select2('open');
            return false;
        }
        else if (title.value === "") {
            title.setCustomValidity('This field is required.');
            form.reportValidity();
            return false;
        }

        if (!form.checkValidity()) {
            form.reportValidity(); // Show native validation messages
            return false;
        }

        return true
    }
    onSaveNote(button) {

        if (this.#isValidForm()) {
            button.setAttribute('data-kt-indicator', 'on');
            button.disabled = true;

            const description = $("#txtNote").val().trim()
                .replace(/&/g, '&amp;')   // escape HTML
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;')
                .replace(/\n/g, '<br>');  // convert line breaks to <br>

            const payload = {
                title: $("#txtNoteTitle").val().trim(),
                description: description,
                assignedTo: parseInt(this.assignedTo.id)
            }
            
            fetch('/api/action-items/note', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            })
            .then(response => response.json())
            .then(data => {
                const form = document.getElementById('frmCreateActionItemNote');
                form.reset();
                $('#slcCreateNoteAssignedTo').val(0).trigger('change');
                $("#action-item-note-component-close").trigger('click');
                this.swal.alert('Note added successfully');

            })
            .finally(() => {
                button.removeAttribute('data-kt-indicator');
                button.disabled = false;
            });
        }

      
    }

	init() {
		this.#initAssignedToControl();
    }
}

$(document).ready(() => {
	createNote = new CreateNote();
	createNote.init();
});
