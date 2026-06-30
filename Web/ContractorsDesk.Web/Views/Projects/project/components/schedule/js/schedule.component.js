class ScheduleComponent extends DomEventComponent {
	constructor() {
		super();
		this.projectId = null;
		this.proposalId = null;
		this.clientEmail = null;
		this.clientName = null;
		this.httpService = new httpService();
		this.service = new ProjectScheduleService();
		this.swal = new SwalUtil();
		this.printer = new printer();
		this.project = null;
		this.projectSchedule = null;
		this.schedule = null;
		this.constructionTask = [];
		this.startDate = '';
		this.delay = null;
		this.addDelayForm = document.getElementById('frmAddDelay');
		this.addItemForm = document.getElementById('frmAddItem');
		this.addDelayFormValidator = this.#setAddDelayValidator(this.addDelayForm);
		this.addItemFormValidator = this.#setAddItemValidator(this.addItemForm);
		this.addDelayDrawer = new Drawer('#drawer-add-delay');
		this.showDelayDrawer = new Drawer('#drawer-delay-list');
		this.addItemDrawer = new Drawer('#drawer-add-item');
		this.deleteItemDrawer = new Drawer('#drawer-delete-item');
		this.printer = new printer();
		this.unScheduled = [];
		this.status = null;
		this.enableUnloadProtection = true;
		this.saveChangesMethod = (callback) => this.saveSchedule(null, 'Final', callback);
		this.onSetLagTimeout = null;
		this.emailSender = null;
	}

	#setAddItemValidator(form) {
		return new FormValidator(form, {
			'startDate': {
				validators: {
					notEmpty: {
						name: 'Name is required'
					}
				}
			}
		}).init();
	}

	#setAddDelayValidator(form) {
		return new FormValidator(form, {
			'startDate': {
				validators: {
					notEmpty: {
						message: 'Start Date is required'
					}
				}
			},
			'reason': {
				validators: {
					notEmpty: {
						message: 'Reason is required'
					}
				}
			},
			'days': {
				validators: {
					notEmpty: {
						message: 'Number of Days is required'
					}
				}
			},
			'addDelayConstructionTask': {
				validators: {
					notEmpty: {
						message: 'Construction Task is required'
					}
				}
			}
		}).init();
	}

	#refreshTaskSequence() {
		this.schedule.map((s, index) => { s.sequence = index + 1; });
	}

	#renderProjectSchedule() {
		var scheduleStart = this.schedule[0];
		var lastTask = this.schedule[this.schedule.length - 1];

		$("#lblStartDate").html(scheduleStart.startDateFormatted);
		$("#lblEstimatedCompletionDate").html(lastTask.endDateFormatted);
		
		this.#refreshScheduleRows();
		$("#txtStartDate").removeAttr("disabled").flatpickr({
			dateFormat: "m/d/Y",
			onChange: (selectedDates, dateStr, instance) => {
				this.startDate = dateStr;
				this.#updateStartAndEndDates();
				
			},
			disable: [
				function (date) {
					// Disable Sundays (0) and Saturdays (6)
					return date.getDay() === 0 || date.getDay() === 6;
				}
			],
			defaultDate: this.startDate
		});

		$("#txtDelayStart").flatpickr({
			dateFormat: "m/d/Y",
			onChange: (selectedDates, dateStr, instance) => {
				//this.startDate = dateStr;
				//this.#updateStartAndEndDates(dateStr);
			},
			disable: [
				function (date) {
					// Disable Sundays (0) and Saturdays (6)
					return date.getDay() === 0 || date.getDay() === 6;
				}
			],

			defaultDate: new Date()
		});

		$("#btnAddDelay").removeAttr("disabled");
	
		if (this.status == 'Final') {
			$('[data-status-toggle="Draft"]').addClass('d-none');
			$('[data-status-toggle="Final"]').removeClass('d-none');
			$(".delay-controls").removeClass('d-none');
		}
		else {
			$(".delay-controls").addClass('d-none');
			$('[data-status-toggle="Draft"]').removeClass('d-none');
			$('[data-status-toggle="Final"]').addClass('d-none');
		}
	}

	renderDeleteItemsList() {
		const tbody = document.getElementById('delete-items-list');
		if (!tbody) return;
		tbody.innerHTML = (this.schedule || []).map(item => `
		 <tr data-id="${item.constructionTaskId}">
            <td class="ps-4">
                <input class="form-check-input chk-delete-item" type="checkbox"  value="${item.constructionTaskId}" data-id="${item.constructionTaskId}">
            </td>
            <td>
                <a href="javascript:" class="text-dark fw-bolder text-hover-primary d-block fs-7">${item.name}</a>
            </td>
        </tr>
    `).join('');
		const chkAll = document.getElementById('chkDeleteAll');
		if (chkAll) chkAll.checked = false;
 		this.#initDeleteDrawerEvents();
	}

	#initDeleteDrawerEvents() {
		const chkDeleteAll = document.getElementById('chkDeleteAll');
		if (chkDeleteAll) {
			chkDeleteAll.onchange = null;
			chkDeleteAll.addEventListener('change', function () {
				const checked = this.checked;
				document.querySelectorAll('.chk-delete-item').forEach(cb => cb.checked = checked);
			});
		}
	}

	#refreshScheduleRows() {
		const instance = this;
		const taskSelector = `select[data-control="schedule-select2"]`;

		$("#schedule-details-body").html('');
		let scheduleRows = this.schedule.map(schedule => `${this.#scheduleRow(schedule)}`).join('');

		//const rows = `${scheduleRows} ${this.projectSchedule.status == 'Draft' ? this.#addNewItemRow() : ''}`;
		
		$("#schedule-details-body").html(scheduleRows);

		$(taskSelector).select2({ width: '93%' });

		$(taskSelector).on('select2:select', (e) => {
			this.#onpredecessorselect(e);
		});

		this.#initItemDatesSelection();
		this.#initDurationEventHandler();
		this.#initLag1EventHandler();

		TextboxUtils.init();
	}

	#scrollToRow(selector) {
		const element = $(selector).get(0);
		if (element) {
			const elementPosition = element.getBoundingClientRect().top + window.scrollY;
			const offsetPosition = elementPosition - 200;

			window.scrollTo({
				top: offsetPosition,
				behavior: 'smooth'
			});

			element.classList.add('highlight-on-scroll');

			// Remove highlight after 3 seconds
			setTimeout(() => {
				element.classList.remove('highlight-on-scroll');
			}, 3000);

		} else {
			console.log("Element not found with data-id:", value);
		}
	}

	#animateScrollToBottom() {
		$('html, body').animate({
			scrollTop: $(document).height() - $(window).height()
		}, 'slow');
	}

	#populatePredecessorOptions() {
		const predecessorOptions = `<option value="${Guid.empty}">&nbsp;</option>
							${this.schedule.map(s => {
								return s.name != '' ? `<option value="${s.constructionTaskId}"}>${s.name}</option>` : '';
							}).join('')}`;

		const newItemPredSelector = `select[data-control="new-item-select2"]`;
		$(newItemPredSelector).each(function (){
			$(this).html(predecessorOptions);
		});

		$(newItemPredSelector).select2();
		$(newItemPredSelector).on('select2:open', function () {
			requestAnimationFrame(() => {
				let searchField = document.querySelector('.select2-container--open .select2-search__field');
				if (searchField) {
					searchField.focus();
				}
			});
		});
	}

	#refreshPredOptions() {
		const $selectPred1 = $('#slcAddItemPred1');
		const $selectPred2 = $('#slcAddItemPred2');
		// Clear existing options
		$selectPred1.empty();
		$selectPred2.empty();

		const predecessorOptions = `<option value="${Guid.empty}">&nbsp;</option>
							${this.schedule.map(s => {
			return s.name != '' ? `<option value="${s.constructionTaskId}"}>${s.name}</option>` : '';
		}).join('')}`;

	
		$selectPred1.html(predecessorOptions).trigger('change');
		$selectPred2.html(predecessorOptions).trigger('change');
	}

	#onpredecessorselect(e) {
		const target = $(e.currentTarget);
		const selectedData = e.params.data;
		const predId = target.attr("data-id");
		const predtype = target.attr("data-predtype");

		const schedule = this.schedule.find(s => s.constructionTaskId == predId);
		switch (predtype) {
			case 'pred-1':
				schedule.pred1 = selectedData.id;
				break;
			case 'pred-2':
				schedule.pred2 = selectedData.id;
				break;
			case 'pred-3':
				schedule.pred3 = selectedData.id;
				break;
		}
		
		this.#sortByParentSequence();
		this.#updateStartAndEndDates();
		//this.#refreshScheduleRows();
	}

	#updateStartAndEndDates(index = null) {
		if (!this.startDate || this.startDate == '') return;

		let startDate = this.startDate;
		let endDate = '';

		this.schedule.map((s, i) => {
			const id = s.constructionTaskId;
			const pred = this.schedule.find(item => item.constructionTaskId == s.pred1);

			if (i == 0 || !pred) {
				startDate = this.#addDaysExcludingWeekends(this.startDate, s.lag1);
			}

			endDate = this.#addDaysExcludingWeekends(startDate, s.duration - 1);
			
			s.startDate = DateUtils.formatDate(startDate);
			s.endDate = DateUtils.formatDate(endDate);

			s.startDateFormatted = startDate;
			s.endDateFormatted = endDate;

			startDate = this.#getNextStartDate(i, startDate, endDate);
			endDate = this.#getNextEndDate(s, i, startDate, endDate);
			
		});

		$(`[evt-click="onCancelChanges"]`).removeClass('d-none');
		this.#updateTable(index);
		this.#sortSchedule();
	}

	#getNextStartDate(currentIndex, startDate, endDate) {
		const i = currentIndex;

		let nextItem = i != (this.schedule.length - 1) ? this.schedule[i + 1] : null;

		if (nextItem) {
			if (!nextItem.pred1 || nextItem.pred1 == Guid.empty) {
				//return startDate;
				this.#addDaysExcludingWeekends(startDate, 1);
			}
			else {
				let furthestDateFromPred = this.#getFurthestPredecessorDate(nextItem.constructionTaskId);
				let nextStartDate = furthestDateFromPred ? furthestDateFromPred : endDate;

				return this.#addDaysExcludingWeekends(nextStartDate, nextItem.lag1 + 1);
			}
		}
	}

	#getNextEndDate(constructionTask, currentIndex, startDate, endDate) {
		const i = currentIndex;
		const s = constructionTask;

		let nextItem = i != (this.schedule.length - 1) ? this.schedule[i + 1] : null;

		if (nextItem) {
			if (!nextItem.pred1 || nextItem.pred1 == Guid.empty) {
				//return endDate;
				//return this.#addDaysExcludingWeekends(endDate, - 1);
			}
			else {
				let furthestDateFromPred = this.#getFurthestPredecessorDate(nextItem.constructionTaskId);
				let nextStartDate = furthestDateFromPred ? furthestDateFromPred : endDate;

				startDate = this.#addDaysExcludingWeekends(nextStartDate, nextItem.lag1);
				return this.#addDaysExcludingWeekends(startDate, s.duration - 1);
			}
		}
	}

	#updateTable(index = null) {
		this.schedule.map((s, i) => {
	
			const id = s.constructionTaskId;
			const startDate = s.startDateFormatted;
			const endDate = s.endDateFormatted;
			
			if (index) {
				if (index <= i) {
					$(`input.start-${id}`).val(startDate).trigger('change').parent().removeClass('bg-disabled-cell').addClass('bg-light-warning');
					$(`input.end-${id}`).val(endDate).trigger('change').parent().removeClass('bg-disabled-cell').addClass('bg-light-warning');
				}
			}
			else {
				$(`input.start-${id}`).val(startDate).trigger('change').parent().removeClass('bg-disabled-cell').addClass('bg-light-warning');
				$(`input.end-${id}`).val(endDate).trigger('change').parent().removeClass('bg-disabled-cell').addClass('bg-light-warning');
			}
		});

		this.#initItemDatesSelection();

	}
	
	#updateEndDatesFromDelay(delay) {
		$("#schedule-details-body").find("td.bg-light-warning").addClass("bg-disabled-cell").removeClass("bg-light-warning");
		const targetDate = new Date(delay.start);

		const inprogressTask = this.schedule.find(s => {
			const toUTCDateOnly = date =>
				new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));

			const check = toUTCDateOnly(targetDate);
			const start = toUTCDateOnly(new Date(s.startDate));
			const end = toUTCDateOnly(new Date(s.endDate));

			return check >= start && check <= end;

		});

		if (inprogressTask) {
			delay.taskId = inprogressTask.id;
			delay.taskName = inprogressTask.name;
			this.projectSchedule.delays.push(delay);
			const startIndex = this.schedule.findIndex(item => item.constructionTaskId === inprogressTask.constructionTaskId);
		
			inprogressTask.duration += delay.days;
			$(`input.duration`).filter(`[data-id="${inprogressTask.constructionTaskId}"]`).val(inprogressTask.duration);

			
			this.#updateStartAndEndDates(startIndex);

			var revision = {
				reason: delay.reason,
				description: delay.description,
				newDuration: inprogressTask.duration,
				newStartDate: DateUtils.formatDate(inprogressTask.startDate),
				newEndDate: inprogressTask.endDate,
				constructionTaskId: inprogressTask.constructionTaskId,
			}

			this.projectSchedule.scheduleRevisions.push(revision);

		}
		else {
			this.swal.alert("This delay does not affect any scheduled task.");
		}
	}

    #sortScheduleByPredecessor() {
		this.schedule.sort((a, b) => {
			if ((!a.pred1 || a.pred1 == Guid.empty) && (!b.pred1 || b.pred1 == Guid.empty)) {
				return a.sequence - b.sequence; // Both have no predecessor, sort by sequence
			}
			if (!a.pred1 || a.pred1 == Guid.empty) {
				return -1; // a comes first if it has no predecessor
			}
			if (!b.pred1 || b.pred1 == Guid.empty) {
				return 1; // b comes first if it has no predecessor
			}
			return a.sequence - b.sequence; // Sort by sequence if both have predecessors
		}
		);
    }

	#getFurthestPredecessorDate(id) {
		var endDates = [];
		let furthestDate = null;

		const contructionTask = this.schedule.find(s => s.constructionTaskId == id);
		if (contructionTask.pred1) {
			const pred1 = this.schedule.find(s => s.constructionTaskId == contructionTask.pred1);
			if (pred1) endDates.push(pred1.endDate);
		}
		if (contructionTask.pred2) {
			const pred2 = this.schedule.find(s => s.constructionTaskId == contructionTask.pred2);
			if (pred2) endDates.push(pred2.endDate);
		}
		if (contructionTask.pred3) {
			const pred3 = this.schedule.find(s => s.constructionTaskId == contructionTask.pred3);
			if (pred3) endDates.push(pred3.endDate);
		}

		const normalizeDate = (dateStr) => {
			const [month, day, year] = dateStr.split('/').map(Number);
			return `${year}-${month.toString().padStart(2, '0')}-${day.toString().padStart(2, '0')}`;
		}

		if (endDates.length > 0) {
			furthestDate = endDates.reduce((max, current) =>
				normalizeDate(current) > normalizeDate(max) ? current : max
			);
		}

		return furthestDate;
	}

	#addDaysExcludingWeekends(date1, daysToAdd) {
		
		let date2 = new Date(date1);
		let daysAdded = 0;

		while (daysAdded < daysToAdd) {
			date2.setDate(date2.getDate() + 1); //RJ: Start on the end date of the last task
			//date2.setDate(date2.getDate()); 
			const dayOfWeek = date2.getDay();

			// Skip weekends (Saturday and Sunday)
			if (dayOfWeek !== 0 && dayOfWeek !== 6) {
				daysAdded++;
			}
		}

		const formattedDate = date2.toLocaleDateString("en-US", {
			year: "numeric",
			month: "2-digit", // Ensures two-digit month
			day: "2-digit",   // Ensures two-digit day
		});
		return formattedDate;
	}

	#addRevision(constructionTaskId, reason, description, duration, startDate, endDate) {
		let revision = this.projectSchedule.scheduleRevisions.find(r => r.constructionTaskId == constructionTaskId);
		if (!revision) {
			revision = {
				reason: reason,
				description: description,
				newDuration: duration,
				newStartDate: DateUtils.formatDate(startDate),
				newEndDate: DateUtils.formatDate(endDate),
				constructionTaskId: constructionTaskId,
			}
			this.projectSchedule.scheduleRevisions.push(revision);
		}
		else {
			revision.reason = reason;
			revision.description = description;
			revision.newDuration = duration;
			revision.newStartDate = DateUtils.formatDate(startDate);
			revision.newEndDate = DateUtils.formatDate(endDate);
			revision.constructionTaskId = constructionTaskId;
		}
	}

	#getEndDate(date1, daysToAdd) {
	
		let date2 = new Date(date1);
		let daysAdded = 0;

		while (daysAdded < daysToAdd) {
			//date2.setDate(date2.getDate() + 1); //RJ: Start on the end date of the last task
			date2.setDate(date2.getDate() + 1); 
			const dayOfWeek = date2.getDay();

			// Skip weekends (Saturday and Sunday)
			if (dayOfWeek !== 0 && dayOfWeek !== 6) {
				daysAdded++;
			}
		}

		//date2.setDate(date2.getDate() + daysAdded);
		date2.setDate(date2.getDate() - 1); 
		const formattedDate = date2.toLocaleDateString("en-US", {
			year: "numeric",
			month: "2-digit", // Ensures two-digit month
			day: "2-digit",   // Ensures two-digit day
		});
		return formattedDate;
	}

	#scheduleRow(rowdata) {
		const isDraft = true;

		const bgclass = '';
		const textboxClass = 'form-control form-control-sm text-center border-0 bg-transparent';
		const textboxClassLeft = 'form-control form-control-sm text-left border-0 bg-transparent';
		const bgIsNewClass = rowdata.isNew ? 'bg-light-warning' : '';

		const nameCell = (rowdata)=> {
			if (!isDraft) {
				return `<span class="d-block pt-3">${rowdata.name}</span>`;
			}
			else {
				return `<input type="text" class="${textboxClassLeft}" evt-input="setTaskName" value="${rowdata.name}" data-id="${rowdata.constructionTaskId}" style="float:left;width:90%;">`;
			}
		}
		const predCell = (rowdata, predId, predtype) => {
			return `<select class="form-select form-select-transparent form-select-sm pred"
						data-control="schedule-select2"
						data-placeholder="Select an option"
						data-id="${rowdata.constructionTaskId}"
						data-predtype="${predtype}">
							<option value="${Guid.empty}">&nbsp;</option>
							${this.schedule.map(s => {
								return s.name != '' ? `<option value="${s.constructionTaskId}" ${s.constructionTaskId == predId ? 'selected' : ''}>${s.name}</option>` : '';
								}
							).join('')}
					</select>`;
		}

		return `<tr class="${rowdata.constructionTaskId}">
							<td class="ps-2 text-center ${bgclass} ${bgIsNewClass}">
								<input type="text" class="${textboxClass}" readonly value="${rowdata.sequence}">
							</td>
							<td class="${bgclass} ${bgIsNewClass}">
								${nameCell(rowdata)}
							</td>
							<td class="${bgIsNewClass}">
								<input type="text" class="${textboxClass} duration" data-type="int" value="${rowdata.duration}" data-id="${rowdata.constructionTaskId}">
							</td>
							<td class="${bgclass} ${bgIsNewClass}">
								<input type="text" class="${textboxClass} start-date start-${rowdata.constructionTaskId}" data-id="${rowdata.constructionTaskId}" value="${rowdata.startDateFormatted}"/>
							</td>
							<td class="${bgclass} ${bgIsNewClass}">
								<input type="text" class="${textboxClass} end-date end-${rowdata.constructionTaskId}" data-id="${rowdata.constructionTaskId}" value="${rowdata.endDateFormatted}"/>
							</td>
							<td class="${bgIsNewClass}">
								${predCell(rowdata, rowdata.pred1, 'pred-1')}
							</td>
							<td class="${bgIsNewClass}">
								<input type="text" data-type="int" class="min-w-50px ${textboxClass} lag1" evt-input="setLag" value="${rowdata.lag1}" data-id="${rowdata.constructionTaskId}"/>
							</td>
						</tr>`;
	}
	
	#sortByParentSequence() {
		this.schedule.map(s => {
			let parent = this.schedule.find(p => p.constructionTaskId == s.pred1);
			s.parentSequence = parent ? parent.sequence : 0; 
		});

		this.schedule.sort((a, b) => {
			if (a.parentSequence === b.parentSequence) {
				return a.sequence - b.sequence; // Sort by seq if parentSeq is the same
			}
			return a.parentSequence - b.parentSequence; // Sort by parentSeq
		});
	}

	#sortByStartDate() {
		this.schedule.sort((a, b) => {
			const dateA = a.startDate ? new Date(a.startDate) : null;
			const dateB = b.startDate ? new Date(b.startDate) : null;

			// Handle null cases: null dates should come last
			if (dateA === null && dateB === null) return 0;
			if (dateA === null) return 1;
			if (dateB === null) return -1;

			// Compare valid dates
			return dateA - dateB;
		});
	}

	#sortSchedule() {
		if (this.startDate != '') {
			this.#sortByStartDate();
		}
		else {
			this.#sortByParentSequence();
		}

		this.schedule.map((s, index) => {
			s.sequence = index + 1
		});

		this.schedule.sort((a, b) => {
			return a.sequence - b.sequence;
		});

		//this.#sortScheduleByPredecessor();
	}

	#loadProjectSchedule() {
		$("#dv-tab-schedule").addClass('loading').removeClass('loaded');
		this.service.loadSchedule(this.projectId)
			.then((projectSchedule) => {

				$("#dv-tab-schedule").removeClass('loading').addClass('loaded');
				
				this.originalSchedule = structuredClone(projectSchedule);

				this.projectSchedule = projectSchedule;
				this.projectSchedule.scheduleRevisions = [];
				this.schedule = this.projectSchedule.tasks;
				this.delays = this.projectSchedule.delays;
				this.startDate = DateUtils.fromAspNetDate(this.projectSchedule.startDate);
				this.status = projectSchedule.status;
				this.project = projectSchedule.projectDetails;
				this.proposalId = this.project.proposalId;
				this.clientEmail = this.project.clientEmailAddress;
				this.clientName = this.project.clientName;

				this.#initEmailSender();
				this.#sortSchedule();
				this.#buildStickyActions();
				//this.#buildStickyHeader();
				this.#renderProjectSchedule();
				this.#populatePredecessorOptions();

				$('#slcAddDelayConstructionTask').on('change', (e) => {
					const selectedValue = $(e.target).val();
					const scheduleMatched = this.schedule.find(p => p.constructionTaskId == selectedValue);

					$("#txtDelayStart").val(DateUtils.formatDate(scheduleMatched.endDate, 'm/d/Y'));
					//$("#txtDelayStart").val(scheduleMatched.endDate);
				});

				$("#unschedule-block").addClass('content-hidden');
				$("#schedule-block").removeClass('content-hidden');
			});
	}

	async printSchedule(button) {
		button.setAttribute('data-kt-indicator', 'on');
		button.disabled = true;
		debugger;

		try {
			// Fetch the PDF from the API
			const response = await fetch('/api/print/project-schedule/' + this.projectId);
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
		//this.printer.printCurrentPage('/print-project-schedule/' + this.projectId);
	}
	
	#buildStickyActions() {
		const isScheduleFinal = this.projectSchedule.status == "Final";
		window.addEventListener('scroll', function () {
			if (MAINVIEW.currentTab != 'tab-schedule') return;

			const scrollY = window.scrollY || window.pageYOffset;
			const totalHeight = document.documentElement.scrollHeight - window.innerHeight;
			const scrollPercent = (scrollY / totalHeight) * 100;
			const actionBar = document.getElementById('scroll-action-bar');

			if (scrollPercent > 13) {
				actionBar.style.bottom = '20px'; // slide up into view
			} else {
				actionBar.style.bottom = '-100px'; // slide down out of view
			}
		});
		//const width = isScheduleFinal ? 'min-w-lg-350px' : 'min-w-lg-650px';

		//this.stickyActions = new StickyActions(width);
		//let actions = isScheduleFinal ?
		//	[
		//		new StickyActionsButton('Save Schedule', null, '', null, true,
		//			(b) => {
		//				this.saveSchedule(b, null, 'Final');
		//			})
		//	] :
		//	[
		//		new StickyActionsButton('Delete Items', null, 'danger', null, false,
		//			(b) => { this.onDeleteItems && this.onDeleteItems(); },
		//			'btn-outline btn-outline-dashed btn-outline-danger btn-active-light-danger'),
		//		new StickyActionsButton('Add New Item', null, '', null, false,
		//			(b) => { this.onAddNewItem(); },
		//			'btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary'),
		//		new StickyActionsButton('Save as Draft', null, 'primary', null, true,
		//			(b) => {
		//				this.saveSchedule(b, null, 'Draft');
		//			}),
		//		new StickyActionsButton('Save and Finalize', null, 'success', null, true,
		//			(b) => {
		//				this.saveSchedule(b, null, 'Final');
		//			})
		//	];

		//actions.push(new StickyActionsButton('Cancel', null, 'light', null, false,
		//	(b) => {
		//		this.swal.confirm(`Are you sure to cancel all your changes?`,
		//			() => {
		//				location.reload();
		//			}
		//		);
		//	})
		//);

		//actions.push(new StickyActionsButton('Print', null, 'secondary', null, true,
		//	(b) => {
		//		if (this.projectId == Guid.empty) {
		//			this.swal.error("Please save this project schedule before printing.");
		//			b.removeAttribute('data-kt-indicator');
		//			b.disabled = false;
		//		}
		//		else {
		//			this.print(b);
		//		}
		//	})
		//);

		//this.stickyActions.actions = actions;
		//this.stickyActions.init();
	}

	#buildStickyHeader() {
		const options = {
			selector: ".sticky-header",
			top: '74px',
			showOnScrollTop: 466
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	removeTask(t) {
		const id = $(t).attr("data-id");

		const onconfirmDelete = (id) => {
			const constructionTask = this.schedule.find(c => c.constructionTaskId == id);
			const childTasks = this.schedule.filter(c => c.pred1 == id);

			childTasks.map(ct => {
				ct.pred1 = null;
			});

			this.schedule = this.schedule.filter(item => item !== constructionTask);
			this.#refreshTaskSequence();
			this.#refreshScheduleRows();
			this.#updateStartAndEndDates();
		}

		if (this.projectSchedule.status == 'Final') {
			this.swal.confirm("Remove this task from the schedule?", () => {
				onconfirmDelete(id);
			});
		}
		else {
			onconfirmDelete(id);
		}
		this.hasChanges = true;

	}

	saveSchedule(b, e, s) {
		let status = $(b).attr("data-status");
		status = status ?? s;

		if (this.startDate == '') {
			this.swal.error("Please select a start date.");
			return;
		}

		this.projectSchedule.startDate = DateUtils.formatDate(this.startDate);
		this.projectSchedule.status = status;
		this.projectSchedule.tasks = this.schedule;
		
		if (b) {
			b.setAttribute('data-kt-indicator', 'on');
			b.disabled = true;
		}

		this.service.saveSchedule(this.projectId, this.projectSchedule)
			.then((projectSchedule) => {
				const previousStatus = this.status;
				const newStatus = projectSchedule.status;
				this.projectSchedule = projectSchedule;
				this.projectSchedule.scheduleRevisions = [];
				this.schedule = this.projectSchedule.tasks;
				this.delays = this.projectSchedule.delays;
				this.startDate = DateUtils.fromAspNetDate(this.projectSchedule.startDate);
				this.status = projectSchedule.status;

				this.#sortSchedule();
				this.#renderProjectSchedule();
				this.#buildStickyActions();

				this.#populatePredecessorOptions();

				if (previousStatus == "Draft" && newStatus == "Final") {
					this.#showEmailFinalSchedule();
				}
				else {
					if (this.status == "Final") {
						Swal.fire({
							title: 'Schedule has been Updated',
							text: 'Final Schedule has been updated. Would you like to send an email notification to the client?',
							icon: 'question',
							showCancelButton: true,
							confirmButtonText: 'Yes',
							cancelButtonText: 'No',
							reverseButtons: true, // Optional: puts 'No' on the left
							allowOutsideClick: false
						}).then(result => {
							if (result.isConfirmed) {
								this.#showEmailScheduleRevision(); // Your custom function
							}
						});
					}
					else {
						this.swal.alert("Project Schedule has been updated successfully!");
					}
				}
				
				
			})
			.finally(() => {
				if (b) {
					b.removeAttribute('data-kt-indicator');
					b.disabled = false;
					this.hasChanges = false;
				}
			});
	}

	onAddNewItem() {
		this.addItemDrawer.toggle();
	}

	onCancelChanges(b) {
		this.swal.confirm("Are you sure you want to cancel all your changes?", () => {
			this.projectSchedule = this.originalSchedule;
			this.projectSchedule.scheduleRevisions = [];
			this.schedule = this.projectSchedule.tasks;
			this.delays = this.projectSchedule.delays;
			this.startDate = DateUtils.fromAspNetDate(this.projectSchedule.startDate);

			this.#sortSchedule();
			this.#renderProjectSchedule();
		});
	}

	onDeleteScheduleItems(b) {

		const checkedItems = document.querySelectorAll('.chk-delete-item:checked');

		const checkedIds = Array.from(checkedItems)
			.map(cb => cb.value);

		if (checkedIds.length === 0) {
			alert('Please select at least one item to delete.');
			return;
		}

		this.schedule = this.schedule.filter(item => !checkedIds.includes(item.constructionTaskId));

		// Remove checked items from the DOM
		checkedItems.forEach(item => {
			item.closest('tr').remove();
		});

		this.schedule.forEach(item => {
			if (checkedIds.includes(item.constructionTaskId)) {
				item.pred1 = null; // Set pred1 to null for items in checkedIds
			}
		});

		this.projectSchedule.tasks = this.schedule;

		this.#refreshTaskSequence();
		this.#refreshScheduleRows();
		this.#updateStartAndEndDates();
		this.deleteItemDrawer.toggle();
	}

	onDeleteItems() {
		this.renderDeleteItemsList();
		this.deleteItemDrawer.toggle();
	}

	onSaveItem() {
		const validator = this.addItemFormValidator;
		const addItemForm = this.addItemForm;
		validator.validate().then((status) => {
			if (status == 'Valid') {
				const lastTask = this.schedule[this.schedule.length - 1];
				const newItem = new TaskModel();
				const id = Guid.new();
				const duration = $("#txtAddItemDuration").val() == '' ? 0 : parseInt($("#txtAddItemDuration").val());
				const lag1 = $("#txtAddItemLag1").val() == '' ? 0 : parseInt($("#txtAddItemLag1").val());
				const lag2 = $("#txtAddItemLag2").val() == '' ? 0 : parseInt($("#txtAddItemLag2").val());

				newItem.id = id;
				newItem.constructionTaskId = id;
				newItem.name = $("#txtAddItemName").val();
				newItem.duration = duration;
				newItem.sequence = lastTask ? lastTask.sequence + 1 : 1;
				newItem.pred1 = $("#slcAddItemPred1").val();
				newItem.pred2 = $("#slcAddItemPred2").val();
				newItem.lag1 = lag1;
				newItem.lag2 = lag2;
				newItem.isNew = true;
				this.schedule.push(newItem);
				addItemForm.reset();
				this.addItemDrawer.toggle();
				this.#refreshPredOptions();
				//this.#animateScrollToBottom();
				this.#updateStartAndEndDates();

				this.#sortSchedule();
				this.#refreshScheduleRows();
				this.#scrollToRow(`tr.${id}`);
			}
		});
	}

	onSaveDelay() {
		const validator = this.addDelayFormValidator;
		const addDelayForm = this.addDelayForm;
		validator.validate().then((status) => {
			if (status == 'Valid') {
				const delay = new DelayModel();
				delay.start = DateUtils.formatDate($("#txtDelayStart").val());
				delay.description = $("#txtDelayDescription").val();
				delay.reason = $("#slcDelayReason").find('option:selected').text();
				delay.days = parseInt($("#txtDelayDays").val());
				delay.applyToOtherProjects = $("#chkApplyToOtherProjects").is(":checked");
				delay.new = true;
				this.#updateEndDatesFromDelay(delay);

				addDelayForm.reset();
				$("#txtDelayStart").val(DateUtils.formatDate(new Date(), 'm/d/Y'));
				this.addDelayDrawer.toggle();
			}
		});
	}

	onDeleteSelectAllItems(b) {
		const isChecked = $(b).is(":checked");
		$("input.chk-delete-item").prop("checked", isChecked);
	}

	onUpdateDelay(b) {
		const id = $(b).attr("data-id");
		const delay = this.delays.find(d => d.id == id);
		const constructionTask = this.schedule.find(s => s.id == delay.taskId);
		const oldNoOfDays = delay.days;

		Swal.fire({
			title: 'Update Delay',
			html: ScheduleView.PopupTemplates.UpdateDelay,
			focusConfirm: false,
			showCancelButton: true,
			confirmButtonText: 'Submit',
			allowOutsideClick: false,
			didOpen: () => {
				document.getElementById('onUpdateDelay-reasonField').value = '1';

				$("#onUpdateDelay-description").val(delay.description);
				$("#onUpdateDelay-duration").val(oldNoOfDays);

				if (constructionTask) {
					$("#onUpdateDelay-ConstructionTask").html(`<option selected>${constructionTask.name}</option>`);
				}
				
				$("#onUpdateDelay-reasonField option").each(function () {
					if ($(this).text().trim() === delay.reason) {
						$(this).prop("selected", true);
						return false; // break the loop once matched
					}
				});

				TextboxUtils.init();
			},
			preConfirm: () => {
				const reasonSelect = document.getElementById('onUpdateDelay-reasonField');
				const reason = reasonSelect.options[reasonSelect.selectedIndex].text;
				const description = document.getElementById('onUpdateDelay-description').value;
				const days = document.getElementById('onUpdateDelay-days').value;

				if (!reason || !description || !days) {
					Swal.showValidationMessage('Please fill in required fields');
					return false;
				}

				return { reason, description, days };
			},
			customClass: {
				popup: 'no-scroll-popup'
			}

		}).then(result => {
			if (result.isConfirmed) {
			
				const days = parseInt(result.value.days);
				const oldNoOfDays = delay.days;
				const updatedNoOfDays = days - oldNoOfDays;
				const currentDuration = constructionTask.duration;
				const duration = currentDuration + updatedNoOfDays;
				const index = this.schedule.findIndex(item => item.id === delay.taskId);
				const constructionTaskId = constructionTask.constructionTaskId;

				$(`input.duration[data-id='${constructionTaskId}']`).val(duration);
				constructionTask.duration = duration;
				delay.days = days;

				this.#updateStartAndEndDates(index);

				var revision = {
					reason: result.value.reason,
					description: result.value.description,
					newDuration: duration,
					newStartDate: DateUtils.formatDate(constructionTask.startDate),
					newEndDate: constructionTask.endDate,
					constructionTaskId: constructionTask.constructionTaskId,
				}

				var updatedDelay = {
					id: delay.id,
					reason: result.value.reason,
					description: result.value.description,
					days: days
				}

				this.projectSchedule.scheduleRevisions.push(revision);
				this.showDelayDrawer.toggle();
				this.projectSchedule.updatedDelays.push(updatedDelay);
			}
		});
	}

	showAddDelayForm() {
		this.addDelayDrawer.toggle();
	}

	showDelays() {
		if (this.delays.length > 0) {
			const delayRows = this.delays.map(d => `<tr>
													<td class="ps-2">${DateUtils.fromAspNetDate(d.start)}</td>
													<td class="ps-2">${d.taskName}</td>
													<td>${d.reason}</td>
													<td>${d.description}</td>
													<td class="text-center">${d.days}</td>
													<td class="text-center">
														<a href="javascript:" data-id="${d.id}" evt-click="onUpdateDelay" class="btn-delay btn-delay-update btn btn-sm btn-link btn-color-info p-0">Update</a>
													</td>
												</tr>`).join('');
			$("#tbody-delays").html(delayRows);
		}
		
		this.showDelayDrawer.toggle();
	}

	setTaskDate(dateStr, item) {
		
		const constructionTaskId = $(item.element).attr("data-id");
		const isStartDate = $(item.element).hasClass('start-date');
		const constructionTask = this.schedule.find(s => s.constructionTaskId == constructionTaskId);
		
		if (isStartDate) {
			if (this.status != "Draft") {
				this.#showStartDateChangeDetailsModal(constructionTask, dateStr);
			}
			else {
				this.#updateStartDate(constructionTask, dateStr);
			}
		}
		else {
			if (this.status != "Draft") {
				this.#showEndDateChangeDetailsModal(constructionTask, dateStr);
			}
			else {
				this.#updateEndDate(constructionTask, dateStr);
			}
		}
	}

	setLag(i) {
		clearTimeout(this.onSetLagTimeout);
		const $target = $(i);

		this.onSetLagTimeout = setTimeout(() => {
			$target.blur();
			const id = $target.attr("data-id");
			const lag = parseInt($target.val());
			const constructionTask = this.schedule.find(s => s.constructionTaskId == id);
			const index = this.schedule.findIndex(item => item.constructionTaskId === id);

			if (this.status != "Draft") {
				this.#showLagChangeDetailsModal(constructionTask, lag, index, i);
			}
			else {
				constructionTask.lag1 = lag;
				this.#updateStartAndEndDates(index);
			}
		}, 500);
	}

	setTaskName(i) {
		
		const $target = $(i);
		const id = $target.attr("data-id");
		const constructionTask = this.schedule.find(c => c.constructionTaskId == id);
		constructionTask.name = $target.val();
	}

	setDuration(i) {
		const $target = $(i);
		$target.blur();
		const id = $target.attr("data-id");
		const duration = parseInt($target.val());
		const constructionTask = this.schedule.find(s => s.constructionTaskId == id);
		const index = this.schedule.findIndex(item => item.constructionTaskId === constructionTask.constructionTaskId);

		if (this.status != "Draft") {
			this.#showDurationChangeDetailsModal(constructionTask, duration, index, i);
		}
		else {
			constructionTask.duration = duration;
			this.#updateStartAndEndDates(index);
		}
	}

	setReason(s) {
		let selectedOption = $(s).find('option:selected').text();
		if (selectedOption == "Weather") {
			$(".row-apply-other").removeClass('d-none');
		}
		else {
			$(".row-apply-other").addClass('d-none');
		}
	}

	onEmailScheduleToClientClick() {
		this.#showEmailFinalSchedule();
	}

	#updateStartDate(constructionTask, startDate) {
		debugger;
		const constructionTaskId = constructionTask.constructionTaskId;
		const index = this.schedule.findIndex(item => item.constructionTaskId === constructionTaskId);
		let duration = constructionTask.duration;
		constructionTask.startDate = DateUtils.formatDate(startDate);
		constructionTask.startDateFormatted = startDate;

		const newEndDate = this.#getEndDate(constructionTask.startDate, duration);
		constructionTask.endDate = DateUtils.formatDate(newEndDate);
		constructionTask.endDateFormatted = newEndDate;

		const pred1 = this.schedule.find(s => s.constructionTaskId == constructionTask.pred1);
		const prevTaskEndDate = pred1 ? pred1.endDateFormatted : this.startDate;

		let noOfDays = this.#getDateDifference(prevTaskEndDate, constructionTask.startDateFormatted);
		if (pred1) {
			noOfDays--;
		}

		constructionTask.lag1 = noOfDays < 0 ? 0 : noOfDays;
		$(`input.lag1`).filter(`[data-id="${constructionTaskId}"]`).val(constructionTask.lag1);

		this.#updateStartAndEndDates(index);
	}

	#updateEndDate(constructionTask, endDate) {
		const constructionTaskId = constructionTask.constructionTaskId;
		const index = this.schedule.findIndex(item => item.constructionTaskId === constructionTaskId);
		constructionTask.endDate = DateUtils.formatDate(endDate);
		constructionTask.endDateFormatted = endDate;

		const duration = this.#getDuration(constructionTask.startDateFormatted, constructionTask.endDateFormatted);
		constructionTask.duration = duration;

		$(`input.duration`).filter(`[data-id="${constructionTaskId}"]`).val(duration);
		this.#updateStartAndEndDates(index);
		//this.#onSetTaskDateCompleted(false, constructionTask, index);
	}

	#showLagChangeDetailsModal(constructionTask, lag, index, input) {
		const constructionTaskId = constructionTask.constructionTaskId;
		const onConfirm = () => {
			constructionTask.lag1 = lag;
			this.#updateStartAndEndDates(index);
		}
		const onCancel = () => {
			setTimeout(() => {
				$(input).val(constructionTask.lag1);
			}, 500);
		}

		this.#showRevisionModal(constructionTaskId, onConfirm, onCancel);
	}
	
	#showDurationChangeDetailsModal(constructionTask, duration, index, input) {
		const constructionTaskId = constructionTask.constructionTaskId;
		const onConfirm = () => {
			constructionTask.duration = duration;
			this.#updateStartAndEndDates(index);	
		}
		const onCancel = () => {
			setTimeout(() => {
				$(input).val(constructionTask.duration);
			}, 500);
		}

		this.#showRevisionModal(constructionTaskId, onConfirm, onCancel);
	}

	#showStartDateChangeDetailsModal(constructionTask, dateStr) {
		const constructionTaskId = constructionTask.constructionTaskId;
		const onConfirm = () => {
			this.#updateStartDate(constructionTask, dateStr);
		}
		const onCancel = () => {
			const startDate = constructionTask.startDateFormatted;
			$(`input.start-${constructionTaskId}`).val(startDate).trigger('change');
		}

		this.#showRevisionModal(constructionTaskId, onConfirm, onCancel);
	}

	#showEndDateChangeDetailsModal(constructionTask, dateStr) {
		const constructionTaskId = constructionTask.constructionTaskId;
		const onConfirm = () => {
			this.#updateEndDate(constructionTask, dateStr);
		}
		const onCancel = () => {
			const endDate = constructionTask.endDateFormatted;
			$(`input.end-${constructionTaskId}`).val(endDate).trigger('change');
		}

		this.#showRevisionModal(constructionTaskId, onConfirm, onCancel);
	}

	#showRevisionModal(constructionTaskId, onConfirm, onCancel) {
		Swal.fire({
			title: 'Revision Details',
			html: ScheduleView.PopupTemplates.SetDurationDetails,
			focusConfirm: false,
			showCancelButton: true,
			confirmButtonText: 'Submit',
			allowOutsideClick: false,
			preConfirm: () => {
				const reasonSelect = document.getElementById('onChangeDuration-reasonField');
				const reason = reasonSelect.options[reasonSelect.selectedIndex].text;
				const description = document.getElementById('onChangeDuration-description').value;

				if (!reason || !description) {
					Swal.showValidationMessage('Please fill in both fields');
					return false;
				}

				return { reason, description };
			}
		}).then(result => {
			if (result.isConfirmed && onConfirm) {
				onConfirm();

				const updatedTask = this.schedule.find(s => s.constructionTaskId == constructionTaskId);
				const reason = result.value.reason;
				const description = result.value.description;
				const duration = updatedTask.duration;
				const startDate = updatedTask.startDate;
				const endDate = updatedTask.endDate;

				this.#addRevision(constructionTaskId, reason, description, duration, startDate, endDate);
			}
			else {
				if (onCancel) onCancel();
			}

			this.#initItemDatesSelection();
		});
	}

	#getDateDifference(startStr, endStr) {
		
		const start = new Date(startStr);
		const end = new Date(endStr);
		let count = 0;
		let current = new Date(start);

		while (current < end) {
			const day = current.getDay();
			if (day !== 0 && day !== 6) {
				count++;
			}
			current.setDate(current.getDate() + 1);
		}

		return count;
	}

	#getDuration(startStr, endStr) {
		const start = new Date(startStr);
		const end = new Date(endStr);

		// Normalize to midnight
		start.setHours(0, 0, 0, 0);
		end.setHours(0, 0, 0, 0);

		// Ensure start is before end
		if (start > end) [start, end] = [end, start];

		let count = 0;
		const current = new Date(start);

		while (current <= end) {
			const day = current.getDay();
			if (day !== 0 && day !== 6) count++; // skip Sunday (0) and Saturday (6)
			current.setDate(current.getDate() + 1);
		}

		return count;


	}

	#showEmailScheduleRevision() {
		
		const clientEmail = this.project.clientEmailAddress;
		const clientName = this.project.clientName;
		const companySettings = Auth.getCompanySettings();

		this.emailSender.clientId = this.projectId;
		this.emailSender.clientName = clientName;
		this.emailSender.senderName = Auth.currentUser().fullName;
		this.emailSender.companyName = companySettings.companyName;
		this.emailSender.resetEmailFields();
		this.emailSender.showGreetings = true;
		this.emailSender.onSentCallback = null;

		const to = clientEmail ?? '';
		this.emailSender.setTo(to);
		this.emailSender.setReplyTo(Auth.currentUser().email);
		this.emailSender.setSubject('Notification of a change to your construction schedule');
		this.emailSender.setContent(`<p>Please acknowledge the following Schedule Revision. Attached you will find a copy of this notification&nbsp;</p>`);

		this.emailSender.type = Enums.ScheduleRevision;
		this.emailSender.refId = this.projectId;
		this.emailSender.additionalRefId = this.projectSchedule.scheduleRevision.id;

		this.emailSender.addDummyAttachmentItem(`${this.project.name}.schedule.pdf`);
		this.emailSender.addDummyAttachmentItem(`${this.project.name}.schedule-revision.pdf`);
		this.emailSender.toggle();
	}

	#showEmailFinalSchedule() {

		const clientEmail = this.project.clientEmailAddress;
		const clientName = this.project.clientName;
		const companySettings = Auth.getCompanySettings();

		this.emailSender.clientId = this.projectId;
		this.emailSender.clientName = clientName;
		this.emailSender.senderName = Auth.currentUser().fullName;
		this.emailSender.companyName = companySettings.companyName;
		this.emailSender.resetEmailFields();
		this.emailSender.showGreetings = true;
		this.emailSender.onSentCallback = null;

		const to = clientEmail ?? '';
		this.emailSender.setTo(to);
		this.emailSender.setReplyTo(Auth.currentUser().email);
		this.emailSender.setSubject('Final Construction Schedule');
		this.emailSender.setContent(`<p>Please find attached the updated construction schedule items for your review. Let me know if you have any questions or require further clarification. &nbsp;</p>`);

		this.emailSender.type = Enums.ScheduleReport;
		this.emailSender.refId = this.projectId;

		this.emailSender.addDummyAttachmentItem(`${this.project.name}.schedule.pdf`);
		this.emailSender.toggle();
	}

	#initEmailSender() {
		this.emailSender.type = Enums.ScheduleReport;
		this.emailSender.refId = this.projectId;
		this.emailSender.onSentCallback = () => { this.hasChanges = false; }
	}

	#initDurationEventHandler() {
		const instance = this;
		document.querySelectorAll('input.duration').forEach(input => {
			
			let previousValue = input.value;
			let debounceTimeout;

			input.addEventListener('focus', () => {
				input.value = '';
			});

			input.addEventListener('blur', () => {
				const $target = $(input);
				const id = $target.attr("data-id");
				const constructionTask = this.schedule.find(s => s.constructionTaskId == id);

				if (input.value.trim() === '') {
					input.value = constructionTask.duration;
				} else {
					previousValue = input.value;
				}
			});

			input.addEventListener('input', () => {
				clearTimeout(debounceTimeout); // Reset timer if user keeps typing
				
				debounceTimeout = setTimeout(() => {
					instance.setDuration(input); // Call your method after 500ms
				}, 500);
			});

		});
	}

	#initLag1EventHandler() {
		const instance = this;
		document.querySelectorAll('input.lag1').forEach(input => {
			let previousValue = input.value;
			let debounceTimeout;

			input.addEventListener('focus', () => {
				input.value = '';
			});

			input.addEventListener('blur', () => {
				if (input.value.trim() === '') {
					input.value = previousValue;
				} else {
					previousValue = input.value;
				}
			});
		});
	}

	#initItemDatesSelection() {

		const instance = this;
		this.schedule.map((s, i) => {
			const id = s.constructionTaskId;
			const lag = s.lag1 ?? 0;
			const pred = this.schedule.find(item => item.constructionTaskId == s.pred1);
			let minDate = this.startDate;
			if (pred) {
				minDate = this.#addDaysExcludingWeekends(pred.endDateFormatted, 1);
			}

			$(`input.start-date[data-id='${id}']`).flatpickr({
				dateFormat: "m/d/Y",
				minDate: minDate,
				onChange: (selectedDates, dateStr, item) => {
					item.input.blur();
					instance.setTaskDate(dateStr, item);
				},
				disable: [
					function (date) {
						// Disable Sundays (0) and Saturdays (6)
						return date.getDay() === 0 || date.getDay() === 6;
					}
				]

			});

			$(`input.end-date[data-id='${id}']`).flatpickr({
				dateFormat: "m/d/Y",
				onChange: (selectedDates, dateStr, item) => {
					item.input.blur();
					instance.setTaskDate(dateStr, item);
				},
				disable: [
					function (date) {
						// Disable Sundays (0) and Saturdays (6)
						return date.getDay() === 0 || date.getDay() === 6;
					}
				]

			});

		});

		//$("input.start-date, input.end-date").flatpickr({
		//	dateFormat: "m/d/Y",
		//	onChange: (selectedDates, dateStr, item) => {
		//		instance.setTaskDate(dateStr, item);
		//	}
		//});
	}

	init() {
		this.#loadProjectSchedule();
	}

}

const ScheduleView = {
	PopupTemplates: {
		UpdateDelay: `<div class="fv-row mb-2 fv-plugins-icon-container"  style="text-align:left;">
							<label class="fs-6 fw-bold required mb-2">Construction Task</label>
							<select class="form-select" disabled data-placeholder="Select a reason" id="onUpdateDelay-ConstructionTask">
								
							</select>
						<div class="fv-plugins-message-container invalid-feedback"></div>
						</div>
						<div class="fv-row mb-2 fv-plugins-icon-container"  style="text-align:left;">
							<label class="fs-6 fw-bold required mb-2">Reason</label>
							<select class="form-select" name="reason" data-placeholder="Select a reason" id="onUpdateDelay-reasonField">
								<option value=""></option>
								<option value="1">Weather</option>
								<option value="2">Sub-Contractor</option>
								<option value="3">Client</option>
								<option value="4">Delivery</option>
								<option value="5">Other</option>
							</select>
						<div class="fv-plugins-message-container invalid-feedback"></div>
						</div>
						<div class="fv-row mb-5" style="text-align:left;">
							<label class="fs-6 fw-bold required mb-2">Description</label>
							<textarea class="form-control form-control-solid" style="resize:none;font-size:13px;" id="onUpdateDelay-description"></textarea>
						</div>
						<div class="fv-row mb-5" style="text-align:left;">
							<label class="fs-6 fw-bold required mb-2">No. Of Days</label>
							<input type="text" data-type="int" required class="form-control form-control-solid required" id="onUpdateDelay-days" />
						</div>`,
		SetDurationDetails: `<div class="fv-row mb-2 fv-plugins-icon-container"  style="text-align:left;">
							<label class="fs-6 fw-bold required mb-2">Reason</label>
							<select class="form-select" name="reason" data-placeholder="Select a reason" id="onChangeDuration-reasonField">
								<option value=""></option>
								<option value="1">Weather</option>
								<option value="2">Sub-Contractor</option>
								<option value="3">Client</option>
								<option value="4">Delivery</option>
								<option value="5">Other</option>
							</select>
						<div class="fv-plugins-message-container invalid-feedback"></div></div>
						<div class="fv-row mb-5" style="text-align:left;">
							<label class="fs-6 fw-bold required mb-2">Description</label>
							<textarea class="form-control form-control-solid" style="resize:none;font-size:13px;" id="onChangeDuration-description"></textarea>
						</div>`

	}
}
