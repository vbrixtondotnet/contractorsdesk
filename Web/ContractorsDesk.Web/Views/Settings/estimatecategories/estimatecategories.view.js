class EstimateCategoriesView extends DomEventComponent {
	constructor() {
		super();
		this.service = new EstimateCategoriesService();
		this.scheduleDataMappings = [];
		this.estimateCategories = [];
		this.parentEstimateCategories = [];
		this.swal = new SwalUtil();
		this.TasksDropdown = null;
		this.selectedSupervisors = [];
		this.dropdownInstances = [];
		this.categoryForm = document.getElementById('frmNewEstimateCategory');
		this.categoryFormDrawer = new Drawer('#new-estimate-category-drawer');
		this.categoryFormDrawer.onToggle = () => {
			this.categoryForm.reset();
		}
		this.categoryFormValidator = this.#setCategoryFormValidator(this.categoryForm);
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

	#estimateCategoryRow(data) {
		const id = data.id;
		const renderparentselectfield = (item) => {
			return `<a href="javascript:" class="virtual-select" data-id="${id}">${item.parent.name}<span class="arrow"></span></a>`;
		};
		return `<tr class="" style="text-transform:uppercase;padding:5px 0px 5px 0px" data-id="${id}">
                                <td class="fw-bolder ps-2" style="">
									<span class="d-none">${data.name}</span>
									<input type="text" 
										class="form-control form-control-sm border-0 item-name bg-transparent" 
										evt-input="onNameChange" 
										data-id="${id}"
										value="${data.name}">
								</td>
								<td>${renderparentselectfield(data)} </td>
								<td class="ps-0 text-center">
									<a href="javascript:"
									   evt-click="confirmDelete"
									   data-id="${id}" 
									   class="btn btn-sm btn-icon btn-light-danger" 
									   title="Delete">
										<span class="svg-icon svg-icon-muted svg-icon-2">
											${doutune.trash}
										</span>
									</a>
								</td>
                            </tr>`;
	}

	async #loadEstimateCategoriesAsync() {
		await this.service.loadEstimateCategories()
			.then((estimateCategories) => {
				this.estimateCategories = estimateCategories;
			});
	}

	async #loadParentEstimateCategories() {
		await this.service.loadParentEstimateCategories()
			.then((parentEstimateCategories) => {
				this.parentEstimateCategories = parentEstimateCategories;
			});
	}

	#renderEstimateCategoryList() {
		$("#dv-estimatecategories").removeClass("loading").addClass("loaded");
		let estimateCategories = [];
		this.estimateCategories.map(ec => {
			if (ec.parent != null) {
				estimateCategories.push(ec);
			}
		});
		const rows = estimateCategories.map(item => { return this.#estimateCategoryRow(item); }).join('');
		$("#body-estimatecategories").html(rows);
		this.#buildVirtualSelect();
	}

	#refreshEstimateCategoryList() {
		this.#loadEstimateCategoriesAsync()
			.then(() => {
				this.#renderEstimateCategoryList();
				this.#initAutoComplete();
			});
	}

	#buildStickyActions() {
		this.stickyActions = new StickyActions('min-w-lg-250px');
		this.stickyActions.actions = [
			new StickyActionsButton('Save', null, 'primary', null, false,
				(b) => {
					this.saveEstimateCategories(b);
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

	#buildStickyHeader() {
		const options = {
			selector: ".sticky-header",
			top: '74px',
			showOnScrollTop: 260
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	#buildVirtualSelect() {
		const parentEstimateCategories = [];
		this.parentEstimateCategories.map(p => { parentEstimateCategories.push({ id: p.id, text: p.name }); });

		$('.virtual-select').each((ind, obj) => {
			var virtualSelectOptions = {
				options: parentEstimateCategories,
				element: $(obj),
				onselect: (item) => {
					let id = $(obj).attr("data-id");
					const estimateCategory = this.estimateCategories.find(e => e.id == id);
					estimateCategory.parentEstimateCategoryId = item.id

					$(`tr[data-id='${id}']`).find('td').addClass('bg-light-warning');
					//$(`tr[data-account-id='${accountId}']`).find('td.parent').html(estimateDataParent.name);
					////console.log(`CATEGORYID: ${datamappingId}, ESTIMATECATEGORYID: ${item.id}`);
				}
			};
			const virtualSelect = new VirtualSelect(virtualSelectOptions);
			virtualSelect.init();
		});
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

	onNameChange(i) {
		const id = $(i).attr("data-id");
		const name = $(i).val();
		const estimateCategory = this.estimateCategories.find(e => e.id == id);
		estimateCategory.name = name.trim();
		$(`tr[data-id='${id}']`).find('td').addClass('bg-light-warning');
		$(i).prev().html(name);
	}

	confirmDelete(i) {
		const id = $(i).attr("data-id");
		this.swal.confirm(`Are you sure to delete this item?`,
			() => {
				$(`tr[data-id='${id}']`).remove();
				const estimateCategory = this.estimateCategories.find(e => e.id == id);
				estimateCategory.deleted = true;
			}
		);
	}

	onSaveNewEstimateCategory(b) {
		const validator = this.categoryFormValidator;
		validator.validate().then((status) => {
			if (status == 'Valid') {
				b.setAttribute('data-kt-indicator', 'on');
				b.disabled = true;
				const estimateCategory = {
                    name: $('#txtCategoryName').val(),
                    parentName: $('#txtParentCategory').val()
                };
				this.service.saveNewEstimateCategory(estimateCategory)
					.then(() => {
						this.swal.alert('New Estimate Category has been added successfully.', () => {
							this.#refreshEstimateCategoryList();
							this.categoryFormDrawer.toggle();
							this.categoryForm.reset();

						});
					}).finally(() => {
						b.removeAttribute('data-kt-indicator');
						b.disabled = false;
					});
			}
		});
	}

	saveEstimateCategories(b) {

		b.setAttribute('data-kt-indicator', 'on');
		b.disabled = true;

		this.service.saveEstimateCategories(this.estimateCategories)
			.then((estimateCategories) => {
				b.removeAttribute('data-kt-indicator');
				b.disabled = false;
				this.estimateCategories = estimateCategories;
				this.#renderEstimateCategoryList();
				this.swal.alert('Estimate Categories have been saved successfully.', () => {

				});
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
		var input = $(`input[evt-input="filterRows"]`);
		input.val('');
		this.filterRows(input);
	}

	filterRows(i) {
		const filter = $(i).val().toLowerCase().trim(); // Get the search value
		$('tbody#body-estimatecategories tr').each(function () {
			const rowText = $(this).text().toLowerCase(); // Get all text in the row
			if (rowText.includes(filter)) {
				$(this).show(); // Show rows that match the filter
			} else {
				$(this).hide(); // Hide rows that don't match
			}
			console.log(rowText);
		});
	}

	toggleCategoryForm() {
		this.categoryFormDrawer.toggle();
	}

	init() {
		this.#loadEstimateCategoriesAsync()
			.then(() => {
				this.#loadParentEstimateCategories()
					.then(() => {
						this.#renderEstimateCategoryList();
						this.#buildStickyActions();
						this.#buildStickyHeader();
						this.#initAutoComplete();
					});
			});
    }
}

$(document).ready(() => {
	const view = new EstimateCategoriesView();
    view.init();
});