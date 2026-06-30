class RevisedEstimateView extends DomEventComponent {
	constructor() {
		super();
		this.proposalId = document.URL.split('/').pop();
		this.httpService = new httpService();
		this.service = new ReviseEstimateService();
		this.projectDetails = null;
		this.swal = new SwalUtil();
		this.categories = [];
		this.summary = null;
		this.unmappedTransactions = [];
		this.reconciledItems = [];
		this.printer = new printer();
		this.reTableRowRenderer = null;
		this.formatter = new Formatter(this.proposalId);
		this.newEstimateForm = document.getElementById('frmNewEstimateCategory');
		this.estimateMappingDrawer = new Drawer('#estimate-mapping-drawer');
		this.newEstimateCategoryDrawer = new Drawer('#new-estimate-category-drawer');
		this.newEstimateCategoryDrawer.onToggle = () => { this.#onNewEstimateCategoryDrawerToggle(); }
		this.categoryFormValidator = this.#setCategoryFormValidator(this.newEstimateForm);
		this.sendEmailForm = document.getElementById('kt_inbox_compose_form');
		this.sendEmailFormValidator = FormValidation.formValidation(
			this.sendEmailForm,
			{
				fields: {
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
				},

				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					bootstrap: new FormValidation.plugins.Bootstrap5({
						rowSelector: '.fv-row',
						eleInvalidClass: '',
						eleValidClass: ''
					})
				}
			}
		);
		this.OnMinimumRequestAmountInputTimeout = null;
		this.costRevision = null;
		this.estimateCategories = [];
		this.mappings = [];
		this.currentMappingAccountId = null;
		this.virtualSelects = [];
		this.invoiceDrawer = null;

		this.enableUnloadProtection = true;
		this.saveChangesMethod = (callback) => this.saveEstimate(null, callback);
		this.companySettings = Auth.getCompanySettings();
	}

	#setCategoryFormValidator(form) {
		return new FormValidator(form, {
			'categoryName': {
				validators: {
					notEmpty: {
						message: 'Category Name is required'
					}
				}
			},
			'parentCategoryName': {
				validators: {
					notEmpty: {
						message: 'Parent is required'
					}
				}
			}
		}, 'fv-row').init();
	}

	#bindEventHandlers() {
		var instance = this;
		$("html").on("keyup", '.revised-amount', function () {
			var itemId = $(this).attr("data-item-id");
			if (itemId) {
				var value = $(this).val() == '' ? '0' : $(this).val();
				value = value.replace(',', '');

				var item = instance.categories.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;

				item.revised = parseFloat(value);

				instance.#calculateRowTotals(item);
				this.hasChanges = true;
			}
		});

		$("html").on("keyup", '.original-amount', function () {
			var itemId = $(this).attr("data-item-id");
			if (itemId) {
				var value = $(this).val() == '' ? '0' : $(this).val();
				value = value.replace(',', '');

				var item = instance.categories.flatMap(item => item.lineItems).find(line => line.id === itemId) || null;
				item.original = parseFloat(value);

				instance.#calculateOriginalColumnTotals(item);
				this.hasChanges = true;
			}
		});
		
		$("html").on("focus", 'input.revised-amount', (e) => {
			const currentValue = parseInt($(e.currentTarget).val());

			if (currentValue <= 0) $(e.currentTarget).val('');

			this.hasChanges = true;
		});

		$("html").on("click", "a.reconcile", (a) => { this.#onReconcile(a); })

		$("#btnPrint").on("click", () => {
			//this.printer.printCurrentPage('/printproposalForm/' + this.proposalId);
		});
	}

	#onReconcile(a) {
		const parentCell = $(a.currentTarget).parent();
		let id = $(a.currentTarget).attr("data-id");
		var item = this.categories.flatMap(item => item.lineItems).find(line => line.id === id) || null;

		item.revised = item.costToDate;
		item.balance = 0;
		item.percentage = 100;
		this.#calculateRowTotals(item);

		//const val = $(i.currentTarget).val().replace(',', '');
		const revisedEstimateCell = $(parentCell).siblings('.revised-amount');
		const updatedValue = StringUtils.formatMoney(item.revised);

		const span = revisedEstimateCell.find('span');
		const input = revisedEstimateCell.find('input');

		span.html(updatedValue);
		input.val(item.revised);
		revisedEstimateCell.addClass('updated');
	}

	#calculateRowTotals(item) {
		const estimateAmount = item.revised;//item.revised > 0 ? item.revised : item.original;
		//if (estimateAmount == 0) {
		//	$(`tr.${item.id} td.item-percentage`).html(`N/A`);
		//	return;
		//}
		var percentage = this.#getPercentage(estimateAmount, item.costToDate);
		item.balance = parseFloat(estimateAmount - item.costToDate);
		item.percentage = percentage ?? item.percentage;

		const percentageText = percentage == null ? 'N/A' : `${item.percentage}%`;
		
		$(`tr.${item.id} td.item-balance`).html(`${this.formatter.formatBalance(item.balance)}`);
		$(`tr.${item.id} td.item-percentage`).html(`${percentageText}`);

		this.#calculateCategoryTotals(item);
	}

	#calculateOriginalColumnTotals() {
		let totalProject = 0;
		this.categories.map(c => {
			const lineItems = c.lineItems;
			let totalOriginal = 0;
			lineItems.map(item => {
				totalOriginal += item.original;
			});

			$(`tr.${c.id} td.total-original`).html(`${StringUtils.formatMoney(totalOriginal)}`);
			totalProject += totalOriginal;
		});

		$(`tr.project-totals td.total-original`).html(`${this.formatter.formatBalance(totalProject)}`);
		//$(`tr.${item.id} td.item-percentage`).html(`${item.percentage.toFixed(2)}%`);
	}

	#calculateCategoryTotals(item) {
		//debugger;
		const category = this.categories.find(c => c.id === item.parentId);
		if (!category) return;

		const { totalRevised, totalBalance } = category.lineItems.reduce(
			(totals, i) => {
				totals.totalRevised += i.revised;
				totals.totalBalance += i.balance;
				return totals;
			},
			{ totalRevised: 0, totalBalance: 0 }
		);

		let totalPercent = category.lineItems.reduce((sum, item) => sum + item.percentage, 0);
		totalPercent = totalPercent / category.lineItems.length;
		totalPercent = Math.round(totalPercent * 100) / 100;

		const formatPercent = (value) => {
			return value < 0 ? `<span class="text-danger">${value.toFixed(2)}%</span>` : `${value}%`;
		}

		category.totalBalance = totalBalance;
		category.totalRevised = totalRevised;
		category.totalPercentage = Math.round(totalPercent);

		$(`tr.${item.parentId} td.total-revised`).html(`${StringUtils.formatMoney(totalRevised)}`);
		$(`tr.${item.parentId} td.total-balance`).html(`${this.formatter.formatBalance(totalBalance)}`);
		$(`tr.${item.parentId} td.total-percent`).html(`${formatPercent(category.totalPercentage)}`);
		this.#calculateProjectTotals();
	}

	#calculateProjectTotals() {
		let totalOriginal = 0;
		let totalRevised = 0;
		let totalCostToDate = 0;

		//this.categories.map(c => {
		//	totalOriginal += c.totalOriginal;
		//	totalRevised += c.totalRevised;
		//});

		const categories = this.categories;
		categories.map(c => {
			totalOriginal += c.totalOriginal;
			totalRevised += c.totalRevised;
			if (c.name != "JOB BALANCE" && c.name != "OWNER DEPOSITS" && c.name != "TOTAL COST TO DATE") {
				totalCostToDate += c.totalCostToDate == null ? 0 : c.totalCostToDate;
			}
		});

		const formatPercent = (value) => {
			return value < 0 ? `<span class="text-danger">${value.toFixed(2)}%</span>` : `${value}%`;
		}

		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';

		if (totalRevised == 0) {
			$(".project-totals td.total-percent").html(`N/A`).attr('style', blackBorderStyle);
		}
		else {
			let totalPercentage = parseFloat(((totalCostToDate / totalRevised) * 100));
			totalPercentage = Math.round(totalPercentage);
			$(".project-totals td.total-percent").html(`${formatPercent(totalPercentage)}`).attr('style', blackBorderStyle);
		}

		$(".project-totals td.total-original").html(`${StringUtils.formatMoney(totalOriginal)}`).attr('style', blackBorderStyle);
		$(".project-totals td.total-revised").html(`${StringUtils.formatMoney(totalRevised)}`).attr('style', blackBorderStyle);
		$(".project-totals td.total-balance").html(`${this.formatter.formatBalance(totalRevised - totalCostToDate)}`).attr('style', blackBorderStyle);
		$(".project-totals td.reconcile-cell").attr('style', blackBorderStyle);
	}

	#getPercentage(estimateAmount, costToDate) {
		if (estimateAmount == costToDate) {
			return 100;
		}
		else if (estimateAmount == 0) {
			return null;
		}
		else {
			let percentage = parseFloat(((costToDate / estimateAmount) * 100));
			return Math.round(percentage);
		}
	}
	
	#renderProposalDetails() {
		const project = this.projectDetails;
	 
		if (project != null) {
			$("#lblProjectName").html(project.name);
			$("#lblStartDate").html(project.startDate);
			$("#lblEstimatedCompletionDate").html(project.endDate);
		}

		$(".proposal-details").removeClass('loading').addClass('loaded');
	}

	#renderRevisedEstimateRows() {
		$(".tooltip").remove();
		$("#dv-proposal-categories").removeClass('loading').addClass('loaded');

		//this.reTableRowRenderer = new RevisedEstimateTableRowRenderer(this.categories, this.summary, this.proposalId, this.proposal.locked);
		this.reTableRowRenderer = new RevisedEstimateTableRowRenderer(this.categories, this.summary, this.proposalId, this.projectDetails.locked);
		const estimateRows = this.reTableRowRenderer.generateRows();
		$("#body-categories").html(estimateRows);
		
		this.categories.map(c => {
			const lineItems = c.lineItems;
			lineItems.map(item => {
				this.#calculateRowTotals(item);
			})
		});

		this.#calculateProjectTotals();
		TextboxUtils.init();
		KTApp.initBootstrapTooltips();

	}

	#checkUnmappedTransactions() {
		if (this.unmappedTransactions != null) {
			this.#loadEstimateCategories()
				.then(() => {
					const unmappedRows = this.unmappedTransactions.map(t => {
						return `<tr class="" style="">
								<td class="ps-10" style="">${t.name}</td>
								<td class="text-center" style="">${StringUtils.formatMoney(t.costToDate)}</td>
								<td class="" style="">
									<a href="javascript:" data-account-id="${t.accountId}" class="virtual-select">&nbsp;<span class="arrow"></span></a>
								</td>
							</tr>` }).join('');

					$("#body-missing-transactions").html(unmappedRows);
					//debugger;
					const estimateCategories = [];
					this.estimateCategories.map(e => {
						if (e.parent != null) {
							estimateCategories.push({ id: e.id, text: `${e.parent.name}>${e.name}` });
						}
					});
					estimateCategories.unshift({ id: Guid.empty, text: "Add a New Estimate Category", style: "color:#009ef7;" });

					$('.virtual-select').each((ind, obj) => {
						let accountId = $(obj).attr("data-account-id");

						var virtualSelectOptions = {
							options: estimateCategories,
							element: $(obj),
							onselect: (item) => {
								this.mappings = this.mappings.filter(mapping => mapping.accountId !== accountId);

								if (item.id != Guid.empty) {
									let name = this.unmappedTransactions.find(e => e.accountId == accountId).name;
									let amount = this.unmappedTransactions.find(e => e.accountId == accountId).amount;

									this.mappings.push({ accountId: accountId, estimateCategoryId: item.id, name: name, amount: amount });
								}
								else {
									this.currentMappingAccountId = accountId;
									this.estimateMappingDrawer.toggle();
									this.newEstimateCategoryDrawer.toggle();
								}
							}
						};
						const virtualSelect = new VirtualSelect(virtualSelectOptions);
						virtualSelect.init();
						this.virtualSelects.push({ id: accountId, control: virtualSelect });
					});

					this.#initAutoComplete();

					$("#div-unmapped-items").removeClass('d-none');
				});
			
		}
		else {
			$("#div-unmapped-items").remove();
		}
	}

	#initAutoComplete() {
		const estimateCategoryOptions = [];
		const parentCategoryOptions = [];

		this.estimateCategories.map(ec => {
			if (ec.parentEstimateCategoryId != null) {
				estimateCategoryOptions.push({ id: ec.id, name: ec.name });
			}
			else {
				parentCategoryOptions.push({ id: ec.id, name: ec.name });
			}
		});

		//debugger;
		const acOptions = {
			element: $("#txtCategoryName"),
			options: estimateCategoryOptions
		}

		const parentOptions = {
			element: $("#txtParentCategory"),
			options: parentCategoryOptions
		}

		const ac = new AutoComplete(acOptions);
		ac.init();

		const parentAc = new AutoComplete(parentOptions);
		parentAc.init();
	}

	#hasQuickBooksAccountConnected() {
		this.service.hasQuickBooksAccountConnected().then((res) => {
			if (res === false) {
				$("#div-quickbooks-account").removeClass('d-none');
			}
			else {
				$("#div-quickbooks-account").remove();
			}
		});
	}

	async #loadEstimateCategories() {
		await this.service.loadEstimateCategories()
			.then((estimateCategories) => {
				this.estimateCategories = estimateCategories;
			});
	}

	loadRevisedEstimate() {
		const id = this.proposalId;
		this.service.loadRevisedEstimate(id)
			.then((data) => {
				this.categories = data.estimateCategories;
				this.summary = data.summary;
				this.unmappedTransactions = data.unmappedTransactions;
				this.projectDetails = data.projectDetails;
			})
			.then(() => {
				this.#renderProposalDetails();
				this.#initEmailSender();
				this.#initInvoiceDrawer();
				this.#bindEventHandlers();
				this.#buildStickyActions();
				this.#buildStickyHeader();
				this.#hasQuickBooksAccountConnected();
				this.#checkUnmappedTransactions();
				this.#renderRevisedEstimateRows();
				this.initUnloadProtection();
				this.isInitializing = false;
			});
	}

	async print(button) {
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;
		try {
			// Fetch the PDF from the API
			const response = await fetch('/api/print/revised-estimate/' + this.proposalId);
			if (!response.ok) {
				throw new Error('Failed to fetch the PDF.');
			}
			//downloadFile(response);
			printFile(response);
		} catch (error) {
			console.error("Error:", error);
			alert("Failed to load the PDF for printing.");
		}
		finally {
			button.removeAttribute('data-kt-indicator');
			button.disabled = false;
		}
	}

	onSaveNewEstimateCategory(b) {
		const validator = this.categoryFormValidator;
		validator.validate().then((status) => {
			if (status == 'Valid') {
				const accountId = this.currentMappingAccountId;
				const categoryName = $("#txtCategoryName").val();
				const parentCategory = $("#txtParentCategory").val();

				this.mappings = this.mappings.filter(mapping => mapping.accountId !== accountId);
				this.mappings.push({ accountId: accountId, estimateCategoryId: Guid.empty, name: categoryName, parent: parentCategory, amount: 0 });

				this.newEstimateForm.reset();
				const virtualSelect = this.virtualSelects.find(vs => vs.id === accountId).control;
				virtualSelect.setText(`${parentCategory}>${categoryName}`);
				this.newEstimateCategoryDrawer.toggle();
			}
		});
		
	}

	onSaveMapping(b) {
		if (this.mappings.length > 0) {
			b.setAttribute('data-kt-indicator', 'on');
			b.disabled = true;

			const payload = { proposalId: this.proposalId, mappings: this.mappings };

			this.service.saveRevisedEstimatesMapping(payload)
				.then(() => {
					this.swal.alert('Mapping has been saved successfully!', () => {
						location.reload();
					});
				}).finally(() => {
					b.removeAttribute('data-kt-indicator');
					b.disabled = false;
				});
		}
	}

	toggleMappingDrawer() {
		this.estimateMappingDrawer.toggle();
	}

	#onNewEstimateCategoryDrawerToggle() {
		let shown = this.newEstimateCategoryDrawer.shown;
		if (!shown) {
			this.estimateMappingDrawer.toggle();
			this.newEstimateForm.reset();
		}
	}

	#showEmailCostRevision() {
		const projectName = this.projectDetails.name;

		this.emailSender.resetEmailFields();
		this.emailSender.showGreetings = true;
		this.emailSender.onSentCallback = null;
		this.emailSender.setSubject('Notification of a change to your construction cost');
		this.emailSender.setContent(`<p>Please acknowledge the following Cost Revision. Attached you will find a copy of this notification&nbsp;</p>`);

		this.emailSender.type = Enums.CostRevision;
		this.emailSender.additionalRefId = this.costRevision.id;
		this.emailSender.addDummyAttachmentItem(`${projectName}.estimate-to-actual.pdf`);
		this.emailSender.addDummyAttachmentItem(`${projectName}.cost-revision.pdf`);
		this.emailSender.toggle();
	}

	saveEstimate(button = null, proceedCallback = null)
	{
		if (button) {
			button.setAttribute('data-kt-indicator', 'on');
			button.disabled = true;
		}

		const nonCategoryItemNames = ['OWNER DEPOSITS', 'JOB BALANCE', 'MINIMUM REQUESTED AMOUNT', 'PROJECT TOTALS', 'TOTAL COST TO DATE'];
		const categories = this.categories.filter(item => !nonCategoryItemNames.includes(item.name));

		this.service.saveRevisedEstimate(this.proposalId, categories)
			.then((response) => {
				this.costRevision = response;
				Swal.fire({
					title: 'Estimate Updated',
					text: 'Estimate to Actual has been updated. Would you like to send an email notification to the client?',
					icon: 'question',
					showCancelButton: true,
					confirmButtonText: 'Yes',
					cancelButtonText: 'No',
					reverseButtons: true, // Optional: puts 'No' on the left
					allowOutsideClick: false
				}).then(result => {
					if (result.isConfirmed) {
						this.#showEmailCostRevision(); // Your custom function
					}
				});

				$("td.revised-amount").removeClass('updated');
			})
			.finally(() => {
				if (button) {
					button.removeAttribute('data-kt-indicator');
					button.disabled = false;
				}
			});
	}

	#buildStickyActions() {
		this.stickyActions = new StickyActions('min-w-lg-350px');
		this.stickyActions.actions = [
			new StickyActionsButton('Save', null, 'primary', null, true,
				(b) => {
					this.saveEstimate(b);
				}
			),
			new StickyActionsButton('Cancel', null, 'light', null, false,
				(b) => {
					this.swal.confirm(`Are you sure to cancel all your changes?`,
						() => {
							$("#dv-proposal-categories").addClass('loading').removeClass('loaded');
							this.loadRevisedEstimate();
						}
					);
				}
			),
			new StickyActionsButton('Print', null, 'secondary', null, true,
				(b) => {
					this.print(b);
				}
			)
		];

		this.stickyActions.init();
	}

	#buildStickyHeader() {
		const options = {
			selector: ".sticky-header",
			top: '74px',
			showOnScrollTop: 336
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	#initEmailSender() {
		var emailSender = new EmailSender();
		emailSender.type = Enums.EstimateToActualReport;
		emailSender.refId = this.projectDetails.proposalId;
		emailSender.clientId = this.projectDetails.id;
		emailSender.clientName = this.projectDetails.clientName;
		emailSender.senderName = Auth.currentUser().fullName;
		emailSender.companyName = this.companySettings.companyName;

		emailSender.setTo(this.projectDetails.clientEmailAddress);
		emailSender.setReplyTo(Auth.currentUser().email);
		this.emailSender = emailSender;
	}

	#initInvoiceDrawer() {
		this.invoiceDrawer = new InvoiceDrawer();
		this.invoiceDrawer.clientId = this.projectDetails.id;
		this.invoiceDrawer.projectName = this.projectDetails.name;
		this.invoiceDrawer.emailSender = this.emailSender;
		this.invoiceDrawer.type = Enums.RequestDeposit;
		this.invoiceDrawer.additionalRefId = this.projectDetails.proposalId;
		this.invoiceDrawer.render();
		this.invoiceDrawer.disableBillTo();
	}

	onEmailToClientClick() {
		const to = this.projectDetails.clientEmailAddress;
		const projectName = this.projectDetails.name;

		this.emailSender.resetEmailFields();
		this.emailSender.showGreetings = true;
		this.emailSender.type = Enums.EstimateToActualReport;
		this.emailSender.refId = this.proposalId;
		this.emailSender.addDummyAttachmentItem(`${projectName}.estimate-to-actual.pdf`);
		this.emailSender.setTo(to);
		this.emailSender.setReplyTo(Auth.currentUser().email);
		this.emailSender.setSubject('Here is the Estimate To Actual Report you requested');
		this.emailSender.setContent(`
								<p>Attached is a copy of your Estimate To Actual Report.&nbsp;</p>
								<p>Please review and let's meet or call to discuss.&nbsp;</p>
								`);


		this.emailSender.toggle();
	}

	#setInvoiceEmailSender(invoice) {
		const projectName = this.projectDetails.name;
		if (this.emailSender != null) {
			const subject = 'Request for an Additional Deposit';
			const emailContent = `
							<p>As part of our agreement and to ensure the continued smooth progress of your project, we monitor the balance of the project funds. We have noticed that the current job balance has fallen below 20% of the original deposit.</p>
							<p>To maintain momentum and avoid any interruptions, we kindly request an additional deposit at your earliest convenience. Please let us know if you would like us to send an updated invoice for your reference.</p>
							<p>Attached is a copy of your Estimate To Actual Report and the Invoice for additional deposit request.&nbsp;</p>
							<p>Thank you for your continued trust and partnership. We look forward to delivering excellent results for you.</p>`;

			this.emailSender.resetEmailFields();
			this.emailSender.showGreetings = true;
			this.emailSender.refId = this.projectDetails.proposalId;
			this.emailSender.invoice = invoice;
			this.emailSender.type = 6;
			this.emailSender.setSubject(subject);
			this.emailSender.addDummyAttachmentItem(`${projectName}.invoice.${invoice.invoiceNumber}.pdf`);
			this.emailSender.addDummyAttachmentItem(`${projectName}.transaction-details.pdf`);
			this.emailSender.addDummyAttachmentItem(`${projectName}.estimate-to-actual.pdf`);
			this.emailSender.setContent(emailContent);

			this.emailSender.toggle();
			this.emailSender.setBodyHeight('250px');
		}
	}

	onRequestDepositClick(b) {
		const clientEmailAddress = this.projectDetails.clientEmailAddress;
		const clientName = this.projectDetails.clientName;
		const currentDate = new Date().toISOString().split('T')[0];
		const amount = parseFloat($("input.minimum-requested-amount").val().replace(/,/g, ''));

		if (clientName == null || clientName == '') {
			this.swal.error('Client Name is required to request a deposit. Please fill in the client details from the project settings.');
			return;
		}
		else if (clientEmailAddress == null || clientEmailAddress == '') {
			this.swal.error('Client Email Address is required to request a deposit. Please fill in the client details from the project settings.');
			return;
		}

		if (amount <= 0) {
			this.swal.error('Please enter a request deposit amount greater than 0.');
			return;
		}
		
		const invoice = {
			id: Guid.empty,
			clientId: this.projectDetails.id,
			invoiceNumber: '',
			totalAmount: amount,
			dueDate: currentDate,
			invoiceDate: currentDate,
			status: 'saved',
			items: [{
				id: Guid.new(),
				date: currentDate,
				description: 'Request for progress payment',
				quantity: 1,
				rate: amount,
				amount: amount
			}]
		};

		this.#setInvoiceEmailSender(invoice);
	}

	OnMinimumRequestAmountInput(e) {
		clearTimeout(this.OnMinimumRequestAmountInputTimeout);
		const value = e.value;
		const projectId = this.projectDetails.id;
		this.OnMinimumRequestAmountInputTimeout = setTimeout(() => {
            if (value != null && value != '') {
                const amount = parseFloat(value.replace(/,/g, ''));
				if (amount > 0) {

					const jobBalance = this.summary.jobBalance;
					const canRequestDeposit = jobBalance < amount;
					$(".btn-request-deposit").toggleClass('d-none', !canRequestDeposit);

					this.httpService.patch('/api/revised-estimates/minimum-requested-amount', { projectId: projectId, amount: amount })
						.then((res) => {
							this.summary.minimumRequestedAmount = amount;
						});
					//console.log(amount + projectId);
                }
            }
        }, 500);
		
	}

	init() {
		this.loadRevisedEstimate();
    }
}
class RevisedEstimateTableRowRenderer{
	constructor(categories, summary, proposalId, locked) {
		this.categories = categories;
		this.summary = summary;
		this.locked = locked;
		this.textboxStyle = 'height: 30px !IMPORTANT;min-height: 30px !important;';
		this.nonCategoryItemNames = ['OWNER DEPOSITS', 'JOB BALANCE', 'MINIMUM REQUESTED AMOUNT', 'TOTAL COST TO DATE'];
		this.formatter = new Formatter(proposalId);
		this.formatter.summary = this.summary;
	}

	#createCells(columns, index = 0, len = 0) {
		//border-top: 2px solid black !important;border-bottom: 5px double black !important
		if (index == len - 1) {
			return columns.map(col => `<td${col.class ? ` class="${col.class}"` : ''} style="border-bottom: 2px solid black !important;">${col.content}</td>`).join('');
		}
		else {
			return columns.map(col => `<td${col.class ? ` class="${col.class}"` : ''} style="${col.style ?? ''}">${col.content}</td>`).join('');
		}
	}

	#createRow(columns, classes = '', styles = '', index = 0, len = 0) {
		return `<tr class="${classes}" style="${styles}">
				${this.#createCells(columns, index, len)}
			</tr>`;
	}

	#categoryRow(c) {
		return this.#createRow(
			[{ content: c.name, class: 'fw-bolder py-2 ps-2' }, ...Array(6).fill({ content: '&nbsp;' })],
			'',
			'text-transform:uppercase;padding:5px 0px 5px 0px'
		);
	}

	#totalCategoryRow(c){
		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';
		const totalColumns = [
			{ content: `Total ${c.name}`, class: 'fw-bolder py-2 ps-2', style: `text-transform:uppercase;${blackBorderStyle}` },
			{ content: `${this.formatter.formatBalance(c.totalOriginal)}`, class: 'text-center fw-bold total-original py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatBalance(c.totalRevised)}`, class: 'text-center fw-bold total-revised py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatCostToDate(null, c.id, c.totalCostToDate)}`, class: 'text-center fw-bold py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatBalance(c.totalBalance)}`, class: 'text-center fw-bold total-balance py-2', style: `${blackBorderStyle}` },
			{ content: `${this.formatter.formatPercent(c.totalPercentage)}`, class: 'text-center fw-bold py-2 total-percent', style: `${blackBorderStyle}` },
			{ content: '&nbsp;', class: 'text-center py-2', style: `${blackBorderStyle}` }
		];

		const emptyRow = Array(7).fill({ content: '&nbsp;', class: 'text-center' });
		return this.#createRow(totalColumns, `${c.id} total-category-row`);
	}

	#reconcileButton(c) {
		const isEnabled = c.balance != 0;// > 0;
		const buttonClass = isEnabled ? "btn-secondary reconcile" : "btn-light-secondary disabled-link";
		const tooltip = isEnabled ? 'data-bs-toggle="tooltip" data-bs-placement="top" title="Reconcile"' : '';

		return `<a href="javascript:" 
                data-id="${c.id}" 
                class="btn btn-sm btn-icon ${buttonClass}" 
				style="height:30px; width:30px;"
                ${tooltip}>
                <span class="svg-icon svg-icon-muted svg-icon-2">${doutune.arrows}</span>
            </a>`;
	}

	#itemRow(c, index, len) {
		const revisedAmount = (item) => {
			return `<span>${this.formatter.formatBalance(item.revised)}</span>
					<input type="text"
							class="form-control form-control-sm revised-amount"
							data-type="money"
							data-item-id="${item.id}" 
							value="${this.formatter.formatDecimal(item.revised)}" 
							style="${this.textboxStyle}">`;
		}
		const originalAmountCell = (c) => {
			return this.formatter.formatBalance(c.original);
		}

		const formatName = (name) => {
			if (name.toUpperCase() == 'UNMAPPED TRANSACTIONS') {
				return '<span class="text-danger">Unmapped Transactions</span>';
			}
			else {
				return name;
			}
		}

		return this.#createRow(
			[
				{ content: formatName(c.name), style: '', class: "ps-10" },
				{ content: originalAmountCell(c), class: 'text-center' },
				{ content: revisedAmount(c), class: 'text-center revised-amount' },
				{ content: this.formatter.formatCostToDate(c.estimateCategoryId, null, c.costToDate), class: 'text-center' },
				{ content: this.formatter.formatBalance(c.balance), class: 'text-center item-balance' },
				{ content: this.formatter.formatPercent(c.percentage), class: 'text-center item-percentage' },
				{ content: this.#reconcileButton(c), class: 'text-center' }
			],
			`${c.id}`,
			'',
			index,
			len
		);
	} 

	#nonCategoryRow (name, cls = '') {
		const costToDate = (name) => {
			switch (name) {
				case 'OWNER DEPOSITS':
					return this.formatter.formatCostToDate(null, '36fde9f1-dce7-4c5b-8fb8-4c23e1fe3f38', this.summary.ownerDeposits, name);
				case 'MINIMUM REQUESTED AMOUNT':
					return this.formatter.formatCostToDate(null, '36fde9f1-dce7-4c5b-8fb8-4c23e1fe3f38', this.summary.minimumRequestedAmount, name);
				case 'JOB BALANCE':
					return this.formatter.formatBalance(this.summary.jobBalance);
			}
		}

		const requestDepositButton = (name) => {
			if (name == 'MINIMUM REQUESTED AMOUNT') {
				const threshold = this.summary.minimumRequestedAmount * 20 / 100;
				const jobBalance = this.summary.jobBalance;
				const canRequestDeposit = jobBalance < threshold;
				const dClass = canRequestDeposit ? '' : 'd-none';

				return `<button type="button" class="btn btn-sm btn-light-primary btn-request-deposit" evt-click="onRequestDepositClick"> 
							<span class="indicator-label">
								<span class="svg-icon svg-icon-muted">${doutune.sendEmail}</span> REQUEST DEPOSIT
							</span>
							<span class="indicator-progress">
								Please Wait...
								<span class="spinner-border spinner-border-sm align-middle ms-2"></span>
							</span>
						</button>`;
			}

			return ``;
		}

		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';
		return `<tr class="${cls}" style="">
					   <td class="fw-bolder py-2 ps-2" style="text-transform:uppercase;${blackBorderStyle}">${name}</td>
					   <td class="text-center fw-bold py-3 total-original"></td>
					   <td class="text-center fw-bold total-revised py-3" style=""></td>
					   <td class="text-center fw-bold py-3 total-costtodate" style="${blackBorderStyle}">${costToDate(name)}</td>
					   <td class="text-center fw-bold total-balance" style="" colspan="3">${requestDepositButton(name)}</td>
					</tr>`
	}

	#projectTotalRow(name, cls = '') {

		const totalCostToDate = () => {
			const costToDate = this.summary.totalCostToDate;

			return this.formatter.formatCostToDate(null, '0934406a-64db-432b-a01c-5d7573a0f3ce', costToDate, name);
		}

		const blackBorderStyle = 'border-top: 2px solid black !important;border-bottom: 5px double black !important;';
		return `<tr class="${cls}" style="">
					   <td class="fw-bolder py-2 ps-2" style="text-transform:uppercase;${blackBorderStyle}">${name}</td>
					   <td class="text-center fw-bold py-3 total-original"></td>
					   <td class="text-center fw-bold total-revised py-3" style=""></td>
					   <td class="text-center fw-bold py-3 total-costtodate" style="${blackBorderStyle}">${totalCostToDate()}</td>
					   <td class="text-center fw-bold total-balance py-3" style=""></td>
					   <td class="text-center fw-bold py-3 total-percent" style=""></td>
					   <td class="text-center py-2 reconcile-cell" style="">&nbsp;</td>
					</tr>`
	}

	#bindEventHandlers() {
		$("html").on("click", "td.revised-amount span", (e) => { this.#onRevisedAmountClick(e); });
		$("html").on("click", "input.original-amount", (e) => {
			$(e.currentTarget).select();
		});
	}

	#onRevisedAmountClick(e) {
		const span = $(e.currentTarget);
		const cell = span.parent();
		const input = cell.find("input");

		cell.addClass('editing');
		cell.removeClass('updated');

		input.select();
		const initialValue = span.html();
		let updatedValue = span.html();

		$(input).on("blur", (i) => {
			$(i.currentTarget).parent().removeClass('editing');

			if (initialValue != updatedValue)
				cell.addClass('updated');
		});

		$(input).on("keyup", (i) => {
			const val = $(i.currentTarget).val().replace(',', '');
			updatedValue = StringUtils.formatMoney(val);
			span.html(updatedValue);

		});
		$(input).on('keypress', function (e) {
			if (e.which === 13) {
				$(e.currentTarget).parent().removeClass('editing');

				if (initialValue != updatedValue)
					cell.addClass('updated');
			}
		});
	}

	generateRows() {
		this.#bindEventHandlers();
		const categories = this.categories.filter(item => !this.nonCategoryItemNames.includes(item.name));

		const categoryRows = categories.map(c => {
			let rowHtml = this.#categoryRow(c);
			rowHtml += c.lineItems.map((i, index) => this.#itemRow(i, index, c.lineItems.length)).join('');
			rowHtml += this.#totalCategoryRow(c);
			return rowHtml;
		}).join('');

		return `
			${categoryRows}
			${this.#projectTotalRow('PROJECT TOTALS', 'project-totals')}
			${this.#nonCategoryRow(this.nonCategoryItemNames[0])}
			${this.#nonCategoryRow(this.nonCategoryItemNames[1])}
			${this.#nonCategoryRow(this.nonCategoryItemNames[2])}
		`
	}
}
class Formatter {
	constructor(proposalId){
		this.proposalId = proposalId;
		this.summary = null;
	}

	formatDecimal(value) {
		return value ? value.toFixed(2) : '';
	}

	formatPercent(value) {
		let val = value == null ? 0 : value;
		return value < 0 ? `<span class="text-danger">${val}%</span>` : `${val}%`;
	}

	formatBalance(value) {
		const formattedValue = StringUtils.formatMoney(value);
		return value < 0 ? `<span class="text-danger">(${formattedValue.replace('-','')})</span>` : `${formattedValue}`;
	}

	formatCostToDate(estimateCategoryId, parentEstimateCategoryId, value, name = '') {
		const formattedValue = StringUtils.formatMoney(value);
		const fontColor = value < 0 ? 'text-danger' : '';

		//Unmapped Transactions
		if (estimateCategoryId && estimateCategoryId.toUpperCase() == "821DBDD6-A6FE-4ED3-8F98-2AA63DABFEBA") {
			return `<a href="javascript:" evt-click="toggleMappingDrawer">${formattedValue}</a>`;
		}
		else if (name == 'MINIMUM REQUESTED AMOUNT') {
			const threshold = this.summary.minimumRequestedAmount * 20 / 100;
			const jobBalance = this.summary.jobBalance;
			const aboveThreshold = jobBalance >= threshold;
			const minimumRequestedAmount = aboveThreshold ? 0 : value;
			return `<input type="text" data-type="money" evt-input="OnMinimumRequestAmountInput" class="form-control form-control-sm minimum-requested-amount text-center" value="${minimumRequestedAmount}"/>`;
		}

		return value != 0 ? `<a href="/transaction-detail-report/${this.proposalId}/${estimateCategoryId}/${parentEstimateCategoryId}" target="_blank" class="${fontColor}">${formattedValue}</a>` : `${formattedValue}`;
		
	}
}

$(document).ready(() => {
	const view = new RevisedEstimateView();
	view.init();
});