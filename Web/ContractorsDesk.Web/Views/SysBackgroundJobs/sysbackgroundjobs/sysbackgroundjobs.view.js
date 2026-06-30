class SysBackgroundJobsView extends DomEventComponent {
	constructor() {
		super();
		this.service = new SysBackgroundJobsService();
		this.httpService = new httpService();
		this.sysBackgroundjobs = [];
		this.swal = new SwalUtil();
		this.addItemForm = document.getElementById('frmAddItem');
		this.addSysbackgroundjobsDrawer = new Drawer('#add-sysbackgroundjobs-drawer');
		this.addItemFormValidator = this.#setAddItemValidator(this.addItemForm);
		this.swal = new SwalUtil();
		this.triggeredJobId = null;
	}   
	#setAddItemValidator(form) {
		return new FormValidator(form, {
			'name': {
				validators: {
					notEmpty: {
						message: 'Job name is required'
					}
				}
			},
			'duration': {
				validators: {
					notEmpty: {
						message: 'Duration is required'
					}
				}
			}
		}).init();
	}
	async handleButtonClick(jobId) {
		try {

			const job = this.sysBackgroundjobs.find(j => j.sysJobsId === parseInt(jobId));

			if (!job) { return; }

			const containerId = "tr.sync-progress." + jobId;

			const payload = {
				containerId: containerId,
				notifyOnStart: false
			}

			this.httpService.post(`/api/sysbackgroundjobs${job.apiEndpoint}`, payload)
				.then(response => {
					this.triggeredJobId = jobId;
					$(containerId).removeClass("d-none");
				});

			
		}catch (error) {
			alert("Error executing job: " + error.message); // Show error message
				console.error("Error executing job:");
		}
	}
	async #loadAllSysBackgroundJobs() {
		await this.service.loadAllSysBackgroundJobs()
			.then((response) => {
				this.sysBackgroundjobs = response;
		    })
			.catch((error) =>
			{
				 console.error(error);
            });
	}
	#rendersysBackgroundjobsTaskRow(item) {
		const formattedDate = new Date(item.dateOfExecution).toLocaleDateString("en-US"); // Change 
		return `<tr class="${item.sysJobsId}">
							<td class="ps-2">
								<strong>${item.jobName}</strong>
							</td>
							 <td class="text-center">${formattedDate}</td>
							<td class="text-center">
								${item.lastStatus}
							</td>
							<td class="text-center p-1">
                                 <button class="btn btn-sm btn-secondary execute-btn w-100px" data-jobid="${item.sysJobsId}">
                                     Run
                                 </button>
                            </td>
				 </tr>
				 <tr class="sync-progress d-none ${item.sysJobsId}">
					<td colspan="4">
						<div class="w-100 highlight progress-content" style="color:white;">
							<span style="font-size:12px;">Progress Monitor</span>
							<button class="highlight-copy btn" data-bs-toggle="tooltip" title="" data-jobid="${item.sysJobsId}" evt-click="toggleProgress" data-bs-original-title="Copy code">Toggle Progress</button>
							<pre style="margin-top:10px;font-size:10px;"></pre>
						</div>
					</td>
				 </tr>

				 `;
	}
	#renderSysBackgroundJobsTasks() {
		$("#dv-sysbackgroundjobs-tasks").removeClass("loading").addClass("loaded");
		const rows = this.sysBackgroundjobs.map(item => { return this.#rendersysBackgroundjobsTaskRow(item); }).join('');
		$("#sysbackgroundjobs-tasks-body").html(rows);
	}
	onNewConstructionTaskSave() {
		const validator = this.addItemFormValidator;
		const addItemForm = this.addItemForm;
		let constructionTask = new SysBackgroundJobsModel();

		validator.validate().then((status) => {
			if (status == 'Valid') {
				const existingItem = this.constructionTasks.find(c => c.name.trim().toLowerCase() == addItemForm.name.value.trim().toLowerCase());
				if (existingItem != null) {
					$("#txtAddItemName").next().html(`<div data-field="name" data-validator="notEmpty">${addItemForm.name.value} already exists!</div>`);
					$("#txtAddItemName").parents('fv-row').removeClass('fv-plugins-bootstrap5-row-valid').addClass('fv-plugins-bootstrap5-row-invalid');
					return;
				}
				else {
					$("#txtAddItemName").next().html('');
					$("#txtAddItemName").parents('fv-row').removeClass('fv-plugins-bootstrap5-row-invalid').addClass('fv-plugins-bootstrap5-row-valid');

					const highestSequenceTask = this.constructionTasks.reduce((maxTask, task) =>
						task.sequence > maxTask.sequence ? task : maxTask
					);

					const selectedValue = $("#slcAddItemPred1").val();
					constructionTask.name = addItemForm.name.value;
					constructionTask.duration = addItemForm.duration.value;
					constructionTask.sequence = highestSequenceTask.sequence + 1;
					constructionTask.parentTaskId = selectedValue ? selectedValue : null;

					this.service.saveConstructionTask(constructionTask)
						.then((data) => {
							this.swal.alert('Construction Task has been submitted successfully!', () => {
								location.reload();
							});
						});
				}
			}
		});
	}
	toggleProgress(e) {
		const jobid = $(e).attr('data-jobid');
		var progress = $(`tr.sync-progress.${jobid}`).find("div.progress-content").find("pre");

		progress.toggleClass('d-none');
	}
	toggleAddDrawer() {
		this.addSysbackgroundjobsDrawer.toggle();
	}
	clearFilter() {
		var input = $(`input[evt-input="filterRows"]`);
		input.val('');
		this.filterRows(input);
	}
	filterRows(i) {
		const filter = $(i).val().toLowerCase().trim(); // Get the search value
		$('tbody#sysbackgroundjobs-tasks-body tr').each(function () {
			const rowText = $(this).text().toLowerCase(); // Get all text in the row
			if (rowText.includes(filter)) {
				$(this).show(); // Show rows that match the filter
			} else {
				$(this).hide(); // Hide rows that don't match
			}
		});
	}
	init() {

		this.#loadAllSysBackgroundJobs()
			.then(() => {
				this.#renderSysBackgroundJobsTasks();
				TextboxUtils.init();

				// Add event listener for button clicks
				document.addEventListener("click", (event) => {
					if (event.target.classList.contains("execute-btn")) {
						const jobId = event.target.getAttribute("data-jobid");
						this.handleButtonClick(jobId);
					}
				});
			});
    }
}

$(document).ready(() => {
	const view = new SysBackgroundJobsView();
    view.init();
});
