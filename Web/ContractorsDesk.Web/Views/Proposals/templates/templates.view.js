class TemplateView {
    constructor() {
		this.proposalId = document.URL.split('/').pop();
		this.httpService = new httpService();
		this.service = new TemplateService();
		this.swal = new SwalUtil();
		this.proposalTemplates = [];
		this.templates = new ProposalTemplatesTemplates();
		this.proposalTemplatesUserDefault = [];
		this.proposalTemplateId = 0;
	}

	bindEventHandlers() {
		/*Event handlers here*/
		const instance = this;
		$("html").on("click", "a.proposal-template-action", function () {
			var dataAction = $(this).attr("data-action");
			var dataId = $(this).attr("data-id");
			instance.#setDefaultTemplate(dataId, dataAction);
		});
	}

	#setDefaultTemplate(id, action)
	{
		const onUpdateTemplate = () => {
			this.service.saveProposalTemplateUserDefault(id)
				.then((data) => {
					this.swal.alert('Proposal Template has been submitted successfully!');
					this.init();
				});
		}

		const onRemoveTemplate = () => {
			this.service.removeProposalTemplate(id)
				.then((data) => {
					this.swal.alert('Proposal Template has been removed successfully!');
					this.init();
				})
		}

		if (action === 'accept') {
			this.swal.confirm('Are you sure you want to set this template as default?', () => { onUpdateTemplate() });
		}
		else if (action === 'remove')
		{
			this.swal.confirm('Are you sure you want to remove this template?', () => { onRemoveTemplate() });
		}
		else {
			location.href = `proposals/templates/${id}`;
		}
	}

	loadData() {
		this.templatesLv.preload();
		this.service.loadTemplates()
			.then((templates) => {
				this.proposalTemplates = templates;
				return this.proposalTemplates;
			})
			.then((templates) => {
				this.#renderProposalTemplatesList(templates);
				KTApp.initBootstrapTooltips();
			})
			.catch((status) => {
				if (status === 401) {
					$("#dvProposalTemplatesList").remove();
				}
			});
	}

	#renderProposalTemplatesList(templates) {
		let proposalTemplatesRows = '';

		proposalTemplatesRows = templates.map(item => { return this.templates.proposalTemplatesRow(item, this.proposalTemplateId) }).join('');
		this.templatesLv.renderRows(proposalTemplatesRows);
		KTApp.initBootstrapTooltips();
	}

	async #renderProposalTemplateListView() {
		let colDefs = [
			{ class: 'ps-1', title: 'Template Name', sortable: true, onsort: (colname, direction) => { this.#sortProposalTemplate(colname, direction); } },
			{ class: 'ps-1 text-center', title: 'Owner', sortable: true, onsort: (colname, direction) => { this.#sortProposalTemplate(colname, direction); } },
			{ class: 'w-250px text-center', title: 'Actions' }
		]

		const lvOptions = {
			title: 'Proposal Templates',
			isBookmarkTitle : true,
			columnDefs: colDefs,
			shadow: true,
			minHeight: '400px',
			searchText: 'Search By Name',
			addText : 'Create New Template',
			onAdd: () => {
				location.href = '/proposals/templates/00000000-0000-0000-0000-000000000000';
			},
		}

		this.templatesLv = new ListView(lvOptions);
		this.templatesLv.render('templates-listview');
	}

	#sortProposalTemplate(col, direction) {
		const proposalTemplates = this.proposalTemplates;
		switch (col) {
			case 'TEMPLATE NAME':
				proposalTemplates.sort((a, b) => {
					if (a.name === b.name) return 0;
					return direction === 'desc' ? (a.name < b.name ? 1 : -1) : (a.name > b.name ? 1 : -1);
				});
			case 'OWNER':
				proposalTemplates.sort((a, b) => {
					const firstNameA = a.owner[0]?.firstName?.toLowerCase() || '';
					const firstNameB = b.owner[0]?.firstName?.toLowerCase() || '';

					if (firstNameA === firstNameB) return 0;
					return direction === 'desc'
						? (firstNameA < firstNameB ? 1 : -1)
						: (firstNameA > firstNameB ? 1 : -1);
				});
				break;
		}
		this.#renderProposalTemplatesList(proposalTemplates);
	}

	saveData() {

	}

	updateData() {

	}

	loadProposalTemplateDefaultUser()
	{
		this.service.getProposalTemplateUserDefault()
			.then((userDefault) => {
				this.proposalTemplatesUserDefault = userDefault;
				return this.proposalTemplatesUserDefault;
			})
			.then((userDefault) => {
				if (this.proposalTemplatesUserDefault)
					this.proposalTemplateId = this.proposalTemplatesUserDefault.templateId;
			})
			.catch((status) => {
				if (status === 401) {}
			});
	}

	async renderListViews() {
		await this.#renderProposalTemplateListView();
	}

	 init() {
		this.renderListViews().then(() => {
			this.loadProposalTemplateDefaultUser();
			this.loadData();
			this.bindEventHandlers();
		});
    }
}

class ProposalTemplatesTemplates {
	constructor() { }
	options(arr, label) {
		let options = `<option value="">--Select Option--</option>`;
		options += arr.map(ar =>
			`<option value="${ar.id}">${ar[label]}</option>`
		).join('');

		return options;
	}
	proposalTemplatesRow(item, proposalTemplateId) {
		const currentUser = Auth.currentUser();
		const addButton = (id, action, btnClass, title) => {
			return `<div class="d-flex align-items-center">
				<a href="javascript:"
					class="btn btn-sm proposal-template-action ${btnClass} ${action == 'accept' && id == proposalTemplateId ? 'disabled' : ''}"
					data-id="${id}" 
					data-action="${action}" 
				>
					${title}
				</a>
			</div>`;
		}

		const actionButtons = (item) => {
			const baseButtonTemplate = (id, action, btnClass, title, icon) => `
				<a href="javascript:" 
				   data-id="${id}" 
				   data-action="${action}" 
				   class="btn btn-sm btn-icon ${btnClass} proposal-template-action ${!item.canBeDeleted && action === 'remove' ? 'disabled' : ''}" 
				   data-bs-toggle="tooltip"
				   data-bs-placement="top" 
				   title="${title}">
					<span class="svg-icon svg-icon-muted svg-icon-2">
						${icon}
					</span>
				</a>`;
			const setDefaultButton = addButton(item.id, 'accept', 'btn-primary', 'Set as Default');
			const actionSpecificButton = baseButtonTemplate(item.id, 'edit', 'btn-warning', 'Edit', doutune.pencil);
			const actionSpecificButton2 = baseButtonTemplate(item.id, 'remove', 'btn-danger', 'Remove', doutune.trash);

			return `<div class="ms-auto d-flex gap-2">
                ${setDefaultButton}
                ${actionSpecificButton}
				${item?.owner.some(owner => owner.id === currentUser.id) ? actionSpecificButton2 : ''}
            </div>`;
		};
		return `<tr>
					<td class="py-1">
						<a href="proposals/templates/${item.id}" class="fs-5 fw-bolder text-gray-900 text-hover-primary">${item.name}</a>
					</td>
					<td class="py-1 text-center">
						${ item?.owner?.[0]?.firstName || '' } ${ item?.owner?.[0]?.lastName || '' }
					</td>
					<td class="w-250px text-center">
						${actionButtons(item)}
					</td>
				</tr>`;
	}
}

$(document).ready(() => {
	const view = new TemplateView();
    view.init();
});