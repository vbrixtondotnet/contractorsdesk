class EstimateDataMappingView extends DomEventComponent {
	constructor() {
		super();
		this.service = new EstimateDataMappingService();
		this.groups = [];
		this.accounts = [];
		this.swal = new SwalUtil();
		this.filterRowsTimeout = null;
		this.filterRowsDelayMs = 300;
	}

	async #loadEstimateDataMappings() {
		const page = await this.service.loadEstimateDataMappings();
		this.groups = page?.groups || [];
		this.accounts = page?.accounts || [];
	}

	#sameId(a, b) {
		return String(a || '').toLowerCase() === String(b || '').toLowerCase();
	}

	#escapeHtml(value) {
		return String(value ?? '')
			.replace(/&/g, '&amp;')
			.replace(/</g, '&lt;')
			.replace(/>/g, '&gt;')
			.replace(/"/g, '&quot;');
	}

	#activeAccounts(item) {
		return (item.accounts || []).filter(account => !account.removed);
	}

	#itemMatches(item, query) {
		return (item.name || '').toLowerCase().includes(query);
	}

	#filterGroups(filter) {
		if (!filter) {
			return this.groups;
		}

		const query = filter.toLowerCase();
		return this.groups
			.map(group => {
				const parentMatches = (group.name || '').toLowerCase().includes(query);
				const items = parentMatches
					? (group.items || [])
					: (group.items || []).filter(item => this.#itemMatches(item, query));

				if (!parentMatches && items.length === 0) {
					return null;
				}

				return {
					id: group.id,
					name: group.name,
					sequence: group.sequence,
					items
				};
			})
			.filter(group => group != null);
	}

	#renderGroup(group) {
		if (!(group.items || []).length) {
			return '';
		}

		const header = `<tr class="estimate-mapping-category">
							<td colspan="2" class="fw-bolder ps-2 text-uppercase">${this.#escapeHtml(group.name)}</td>
						</tr>`;
		const rows = (group.items || []).map(item => this.#renderMappingRow(item)).join('');
		return header + rows;
	}

	#renderMappingRow(item) {
		const dirty = (item.accounts || []).some(account => account.added || account.removed);
		const rowClass = dirty ? 'row-changed' : '';
		return `<tr class="estimate-mapping-item ${rowClass}" data-category-id="${item.id}">
					<td class="fw-bolder ps-8 text-uppercase">${this.#escapeHtml(item.name)}</td>
					<td>
						<div class="input-group input-group-sm">
							<select class="form-select" data-control="select2" multiple="multiple" data-placeholder="Select QuickBooks accounts" id="qb-map-${item.id}"></select>
						</div>
					</td>
				</tr>`;
	}

	#findItem(categoryId) {
		for (const group of this.groups) {
			const item = (group.items || []).find(candidate => this.#sameId(candidate.id, categoryId));
			if (item) {
				return item;
			}
		}
		return null;
	}

	#markRow(categoryId) {
		const item = this.#findItem(categoryId);
		const dirty = !!item && (item.accounts || []).some(account => account.added || account.removed);
		$(`tr[data-category-id="${categoryId}"]`).toggleClass('row-changed', dirty);
	}

	#releaseAccount(currentCategoryId, accountId) {
		this.groups.forEach(group => {
			(group.items || []).forEach(item => {
				if (this.#sameId(item.id, currentCategoryId)) {
					return;
				}

				const existing = (item.accounts || []).find(account =>
					this.#sameId(account.accountId, accountId) && !account.removed);
				if (!existing) {
					return;
				}

				if (existing.mappingId) {
					existing.removed = true;
					existing.added = false;
				} else {
					item.accounts = item.accounts.filter(account => !this.#sameId(account.accountId, accountId));
				}

				const $select = $(`#qb-map-${item.id}`);
				if ($select.length && $select.hasClass('select2-hidden-accessible')) {
					const values = ($select.val() || []).filter(id => !this.#sameId(id, accountId));
					$select.val(values).trigger('change');
				}

				this.#markRow(item.id);
			});
		});
	}

	#onAccountSelected(item, account) {
		if (!account) {
			return;
		}

		this.#releaseAccount(item.id, account.id);
		item.accounts = item.accounts || [];
		const existing = item.accounts.find(mapped => this.#sameId(mapped.accountId, account.id));
		if (existing) {
			existing.removed = false;
			existing.added = !existing.mappingId;
			existing.fullyQualifiedName = account.fullyQualifiedName || existing.fullyQualifiedName;
		} else {
			item.accounts.push({
				mappingId: null,
				accountId: account.id,
				estimateCategoryId: item.id,
				fullyQualifiedName: account.fullyQualifiedName || '',
				added: true,
				removed: false
			});
		}

		this.#markRow(item.id);
	}

	#onAccountDeselected(item, account) {
		if (!account) {
			return;
		}

		const existing = (item.accounts || []).find(mapped => this.#sameId(mapped.accountId, account.id));
		if (!existing || existing.removed) {
			return;
		}

		if (existing.mappingId) {
			existing.removed = true;
			existing.added = false;
		} else {
			item.accounts = item.accounts.filter(mapped => !this.#sameId(mapped.accountId, account.id));
		}

		this.#markRow(item.id);
	}

	#initAccountDropdown(item) {
		const elementId = `#qb-map-${item.id}`;
		if ($(elementId).length === 0) {
			return;
		}

		const dropdown = new SearchableDropdown2();
		dropdown.data = this.accounts;
		dropdown.placeholder = 'Select QuickBooks accounts';
		dropdown.width = '100%';
		dropdown.optionText = (account) => this.#escapeHtml(account.fullyQualifiedName || '');
		dropdown.iconText = (account) => {
			const name = (account.fullyQualifiedName || '').trim();
			const initial = name.charAt(0);
			return /[a-z0-9]/i.test(initial) ? initial.toUpperCase() : '';
		};
		dropdown.optionSelected = (account) =>
			this.#activeAccounts(item).some(mapped => this.#sameId(mapped.accountId, account.id));
		dropdown.onSelect = (account) => this.#onAccountSelected(item, account);
		dropdown.onDeselect = (account) => this.#onAccountDeselected(item, account);
		dropdown.init(elementId);
	}

	#destroyAccountDropdowns() {
		$('#body-data-mappings select[data-control="select2"]').each(function () {
			if ($(this).hasClass('select2-hidden-accessible')) {
				$(this).select2('destroy');
			}
		});
	}

	#renderGroups(filter) {
		this.#destroyAccountDropdowns();
		$("#dv-datamappings").removeClass("loading").addClass("loaded");

		const groups = this.#filterGroups(filter);
		const rows = groups.map(group => this.#renderGroup(group)).join('');
		$("#body-data-mappings").html(rows || `<tr><td colspan="2" class="text-muted ps-2">No estimate categories found.</td></tr>`);

		groups.forEach(group => {
			(group.items || []).forEach(item => this.#initAccountDropdown(item));
		});
	}

	#renderEstimateDataMappings() {
		const filter = ($(`input[evt-keyup="filterRows"]`).val() || '').toLowerCase().trim();
		this.#renderGroups(filter);
	}

	#mappingChanges() {
		const changes = [];
		this.groups.forEach(group => {
			(group.items || []).forEach(item => {
				(item.accounts || []).forEach(account => {
					if (!account.added && !account.removed) {
						return;
					}

					changes.push({
						mappingId: account.mappingId,
						accountId: account.accountId,
						estimateCategoryId: item.id,
						fullyQualifiedName: account.fullyQualifiedName || '',
						added: !!account.added,
						removed: !!account.removed
					});
				});
			});
		});
		return changes;
	}

	saveEstimateMappings(b) {
		b.setAttribute('data-kt-indicator', 'on');
		b.disabled = true;

		this.service.saveEstimateDataMappings(this.#mappingChanges())
			.then((page) => {
				this.groups = page?.groups || [];
				this.accounts = page?.accounts || [];
				this.#renderEstimateDataMappings();
			})
			.finally(() => {
				b.removeAttribute('data-kt-indicator');
				b.disabled = false;
			});
	}

	cancelChanges() {
		this.swal.confirm(`Are you sure to cancel all your changes?`,
			() => {
				location.reload();
			}
		);
	}

	clearFilter() {
		if (this.filterRowsTimeout) {
			clearTimeout(this.filterRowsTimeout);
			this.filterRowsTimeout = null;
		}

		const input = $(`input[evt-keyup="filterRows"]`);
		input.val('');
		this.#renderGroups('');
	}

	filterRows(i) {
		if (this.filterRowsTimeout) {
			clearTimeout(this.filterRowsTimeout);
		}

		this.filterRowsTimeout = setTimeout(() => {
			const filter = $(i).val().toLowerCase().trim();
			this.#renderGroups(filter);
			this.filterRowsTimeout = null;
		}, this.filterRowsDelayMs);
	}

	#buildStickyActions() {
		this.stickyActions = new StickyActions('min-w-lg-250px');
		this.stickyActions.actions = [
			new StickyActionsButton('Save', null, 'primary', null, false,
				(b) => {
					this.saveEstimateMappings(b);
				}
			),
			new StickyActionsButton('Cancel', null, 'light', null, false,
				(b) => {
					this.cancelChanges();
				}
			)
		];
		this.stickyActions.init();
	}

	init() {
		this.#loadEstimateDataMappings()
			.then(() => {
				this.#renderEstimateDataMappings();
				this.#buildStickyActions();
			});
	}
}

$(document).ready(() => {
	const view = new EstimateDataMappingView();
	view.init();
});
