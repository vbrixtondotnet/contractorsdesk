class InvoiceDrawer extends DomEventComponent {
    constructor() {
        super();
        this.sendEmailEditor = null;
        this.invoiceDrawer = null;
        this.httpService = new httpService();
        this.billToOptions = null;
        this.clientId = null;
        this.invoiceNumber = null;
        this.invoiceItems = [];
        this.currentInvoiceItem = null;
        this.currentDate = new Date().toISOString().split('T')[0]; // Default to today's date
        this.invoice = null;
        this.emailSender = null;
        this.projectName = null;
        this.emailContent = '';
        this.subject = '';
        this.additionalAttachments = [];
        this.additionalRefId = null;
        this.type = null;
	}

	toggle() {
        this.invoiceDrawer.toggle();
    }

    #loadProjects() {
        this.httpService.get('/api/projects')
            .then(response => {
                this.billToOptions = response;
                const billToSelect = document.querySelector('#invoice-drawer-bill-to');
                if (billToSelect) {
                    billToSelect.innerHTML = ''; // Clear existing options
                    this.billToOptions.forEach(project => {
                        const option = document.createElement('option');
                        option.value = project.id;
                        option.textContent = project.name;
                        billToSelect.appendChild(option);
                    });
                }

                $("#invoice-drawer-bill-to").val(this.clientId ? this.clientId : this.billToOptions[0]?.id || '');

            });
    }

    disableBillTo() {
        const billToSelect = document.querySelector('#invoice-drawer-bill-to');
        if (billToSelect) {
            billToSelect.disabled = true;
        }
    }

    setCurrentDate() {
        const dateElement = document.querySelector('#invoice-drawer-date');
        const dateElementDue = document.querySelector('#invoice-drawer-due-date');
        
        dateElement.value = this.currentDate;
        dateElementDue.value = this.currentDate;
        $(".invoice-item-date").val(this.currentDate)

        this.invoice.dueDate = this.currentDate;
        this.invoice.invoiceDate = this.currentDate;
    }

    setInvoiceNumber(invoiceNumber) {
        this.invoiceNumber = invoiceNumber;
        const invoiceNumberElement = document.querySelector('#invoice-drawer-invoiceno');
        if (invoiceNumberElement) {
            invoiceNumberElement.value = this.invoiceNumber;
        }

        this.invoice.invoiceNumber = this.invoiceNumber;
    }

    setInvoice() {
        this.invoiceItems = [];
        this.invoice = {
            id: Guid.empty,
            clientId: this.clientId,
            invoiceNumber: this.invoiceNumber,
            totalAmount: 0,
            dueDate: this.currentDate,
            invoiceDate: this.currentDate,
            status: 'saved',
            items: this.invoiceItems
        }
        this.additionalAttachments = [];
        
    }

    addNewItem(item) {
        this.currentInvoiceItem = {
            id: Guid.new(),
            date: this.currentDate,
            description: '',
            quantity: '',
            rate: '',
            amount: ''
        }
        const newItem = (item && item.id) ? item : this.currentInvoiceItem;

        this.invoiceItems.push(newItem);
        this.#addInvoiceItemRow(newItem);
    }

    #addInvoiceItemRow(invoiceItem) {
        var invoiceItemsRow = this.#createInvoiceItemRow(invoiceItem);
                    
        const addButtonContainer = document.querySelector('.add-button-container');
        if (addButtonContainer) {
            addButtonContainer.insertAdjacentHTML('beforebegin', invoiceItemsRow);
        }

        TextboxUtils.init();

    }

    #createInvoiceItemRow(invoiceItem) {
        const it = invoiceItem;
        return `<tr class="invoice-item-row">
						<td class="w-200px ps-2">
							<input type="date" class="form-control form-control-sm invoice-item-date" evt-input="" value="${it.date}" />
						</td>
						<td class="w-400px">
							<textarea required class="form-control form-control-sm textarea-dynamic" data-id="${it.id}" evt-input="onActivityInput">${it.description}</textarea>
						</td>
						<td class="text-center">
							<input required type="text" class="form-control form-control-sm" data-type="int" data-id="${it.id}" value="${it.quantity}" evt-input="calculateAmount"/>
						</td>
						<td class="text-center">
							<input required type="text" class="form-control form-control-sm rate" data-type="money" data-id="${it.id}" value="${it.rate}" evt-input="calculateAmount"/>
						</td>
						<td class="text-center pe-2">
							<input type="text" class="form-control form-control-sm amount" data-type="money" data-id="${it.id}" readonly value="${it.amount}"/>
						</td>
						<td class="text-center pe-1 ps-1">
							<a href="javascript:" class="btn btn-sm btn-icon remove-item btn-light-danger" evt-click="onRemoveLineItem" title="Remove Item" data-id="${it.id}">
							<i class="bi bi-trash"></i></a>
						</td>
					</tr>`;
    }

    renderInvoiceItems() {
        var invoiceItemsRows = this.invoiceItems.map(it => {
            return this.#createInvoiceItemRow(it)
        }).join('');

        $("tr.invoice-item-row").remove();
        const addButtonContainer = document.querySelector('.add-button-container');
        if (addButtonContainer) {
            addButtonContainer.insertAdjacentHTML('beforebegin', invoiceItemsRows);
        }

        TextboxUtils.init();
    }

    onRemoveLineItem(e) {
        const itemId = e.getAttribute('data-id');
        this.invoiceItems = this.invoiceItems.filter(item => item.id !== itemId);
        const invoiceItemRow = document.querySelector(`tr.invoice-item-row:has(input[data-id="${itemId}"])`);
        if (invoiceItemRow) {
            invoiceItemRow.remove();
        }
    }

    onInvoiceNoChange(a) {
        this.invoice.invoiceNumber = a.value;
    }

    onInvoiceDateChange(a) {
        this.invoice.invoiceDate = a.value;
    }

    onInvoiceDueDateChange(a) {
        this.invoice.dueDate = a.value;
    }

    onInvoiceTermChange(a) { }

    onActivityInput(a) {
        const textarea = a;
        textarea.style.height = '37px'; // Reset height to initial value
        textarea.style.height = textarea.scrollHeight + 'px'; // Adjust height based on content
        const value = textarea.value;
        const itemId = textarea.getAttribute('data-id');
        const currentInvoiceItem = this.invoiceItems.find(item => item.id === itemId);
        if (!currentInvoiceItem) return; // Ensure the item exists

        const htmlValue = value.replace(/\n/g, '<br>');

        currentInvoiceItem.description = htmlValue;
    }

    onItemDateInput(e) {
        const value = e.value;
        const itemId = e.getAttribute('data-id');
        const currentInvoiceItem = this.invoiceItems.find(item => item.id === itemId);
        if (!currentInvoiceItem) return; // Ensure the item exists

        currentInvoiceItem.date = value;

        // Update the date in the invoice item row
        const dateInput = document.querySelector(`input.invoice-item-date[data-id="${itemId}"]`);
        if (dateInput) {
            dateInput.value = value;
        }
    }

    calculateAmount(e) {
        const value = e.value;
        const itemId = e.getAttribute('data-id');
        const currentInvoiceItem = this.invoiceItems.find(item => item.id === itemId);
        if (!currentInvoiceItem) return; // Ensure the item exists

        const amountInput = document.querySelector(`input.amount[data-id="${itemId}"]`);
        let quantity = 0;
        let rate = 0;
        let amount = 0;

        if (amountInput) {
            if (e.getAttribute('data-type') === 'int') { // Quantity input
                const rateInput = document.querySelector(`input.rate[data-id="${itemId}"]`);
                quantity = parseFloat(value.replace(/,/g, '')) || 0;
                rate = parseFloat(rateInput.value.replace(/,/g, '')) || 0;
                currentInvoiceItem.quantity = quantity;
            } else {
                const qtyInput = document.querySelector(`input[data-id="${itemId}"][data-type="int"]`);
                quantity = parseFloat(qtyInput.value.replace(/,/g, '')) || 0;
                rate = parseFloat(value.replace(/,/g, '')) || 0;
                currentInvoiceItem.rate = rate;
            }

            amount = (quantity * rate).toFixed(2);
            currentInvoiceItem.amount = parseFloat(amount);
            amountInput.value = amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }

        console.log(this.invoice);
    }

    onSaveInvoice(b) {
        var form = document.getElementById('frmCreateInvoiceForm');
        const isValid = form.reportValidity();

        if (isValid) {
            b.setAttribute('data-kt-indicator', 'on');
            b.disabled = true;
            this.httpService.post('/api/invoices', this.invoice)
                .then(response => {
                    this.invoice.id = response.id;
                    this.toggle();
                    this.#setEmailSender();
                    
                })
                .finally(() => {
                    b.removeAttribute('data-kt-indicator');
                    b.disabled = false;
                });
        }
    }

    #setEmailSender() {
        if (this.emailSender != null) {
            const subject = this.subject || `Invoice #${this.invoiceNumber}`;
            const emailContent = this.emailContent || `<p>Hope you're having a good week! Just sending over the invoice for the ${this.projectName} Construction Job.&nbsp;</p>
            <p>You'll find it attached. All the payment details are included in the invoice itself.&nbsp;</p>
            <p>Let me know if you have any questions at all.&nbsp;</p>
            <p>Thank you for your continued trust and partnership. We look forward to delivering excellent results for you.</p>`;

            this.emailSender.resetEmailFields();
            this.emailSender.showGreetings = true;
            this.emailSender.refId = this.invoice.id;
            this.emailSender.additionalRefId = this.additionalRefId;
            this.emailSender.type = this.type || Enums.Invoice;
            this.emailSender.setSubject(subject);
            this.emailSender.addDummyAttachmentItem(`${this.projectName}.invoice.${this.invoiceNumber}.pdf`);
            this.emailSender.setContent(emailContent);

            if (this.additionalAttachments.length > 0) {
                for (const attachment of this.additionalAttachments) {
                    this.emailSender.addDummyAttachmentItem(attachment);
                }
            }

            this.emailSender.toggle();
            this.emailSender.setBodyHeight('250px');
        }
    }

    setEmailContent(content) {
        this.emailContent = content;
    }

    setSubject(subject) {
        this.subject = subject;
    }

    setAdditionalAttachments(attachments) {
        this.additionalAttachments.push(attachments);
    }

    async render() {
        this.invoiceDrawer = new Drawer(`#create-invoice-component-drawer`);
        const instance = this;
        this.invoiceDrawer.onToggle = () => {
            if (this.invoiceDrawer.shown && this.billToOptions == null) {
                this.#loadProjects();
            }
        }
    }
}