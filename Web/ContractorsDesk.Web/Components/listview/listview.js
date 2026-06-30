class ListView {
	constructor(options) {
		let def = {
			elementId: '',
			shadow: '',
			title: '',
			filters: null,
			columnDefs: [],
			onAdd: null,
			addText: 'Add',
			minHeight: '300px',
			searchText: 'Search',
			isBookmarkTitle: false,
		}

		$.extend(def, options);

		this.elementId = def.elementId;
		this.shadow = def.shadow;
		this.title = def.title;
		this.filters = def.filters;
		this.columnDefs = def.columnDefs;
		this.onAdd = def.onAdd;
		this.addText = def.addText;
		this.minHeight = def.minHeight;
		this.searchText = def.searchText;
		this.id = 'lv-' + GenerateId(5);
		this.searchtimeout = null;
		this.isBookmarkTitle = def.isBookmarkTitle

	}

	loader() {
		const loadingLines = '<div class="loading-line"></div>'.repeat(15);
		return `<div class="preloader">
					${loadingLines}
				</div>`
	}

	filterButtons() {
		const filters = this.filters;
		if (filters) {
			const filterItems = filters.items;
			return `<div class="flex" style="margin-top: -5px;margin-left: 10px;">
					${filterItems.map((item, index) => {
						const isFirst = index === 0; // Check if the current item is the first index
						const cls = item.css ?? (isFirst ? 'light-primary' : 'secondary');

						return `<a href="javascript:" class="filter-button" data-filter-value="${item.value}">
										<span class="badge badge-${cls}">${item.text}</span>
									</a>`
					}).join('')}
				</div>`;
		}

		return '';
	}

	addButton() {
		if (this.onAdd) {
			return `<div class="d-flex align-items-center">
					<a href="javascript:" class="btn btn-sm btn-light-primary add-button">
						<i class="bi bi-plus"></i>
						${this.addText}
					</a>
				</div>`
		}
		return '';
	}

	#renderColumns() {
		return `<tr class="text-start text-gray-400 fw-bolder fs-7 text-uppercase gs-0">
					${this.columnDefs.map((col, index) =>
						`<th class="${col.class} ${col.sortable ? `sortable ${this.id} asc` : ''}" ${col.sortable ? `data-sort-index="${index}"` : ''}><strong>${col.title}</strong></th>`
					).join('')}
				</tr>`;
	}

	renderRows(rows) {
		let content = `<div class="notice d-flex bg-light-warning rounded border-warning border border-dashed rounded-3 p-6 mt-5">
								<div class="d-flex flex-stack flex-grow-1">
									<div class="fw-bold">
										<h4 class="text-gray-900 fw-bolder">No Item Found!</h4>
										<div class="fs-6 text-gray-700">There is no item found for this list.
										</div>
									</div>
								</div>
							</div>`;
		let rowcount = 0;
		if (rows != '') {
			content = `<table class="table table-row-bordered table-row-solid align-middle gs-0 gy-4">
						<!--begin::Table head-->
						<thead class="border-gray-200 fs-5 fw-bold fixed-header table-header">
							${this.#renderColumns()}
						</thead>
						<tbody>
							${rows}
						</tbody>
					</table>`;
			rowcount = (rows.match(/<tr\b[^>]*>/g) || []).length;
		}

		$(`#${this.id}`).find(".scroller-content").empty();
		$(`#${this.id}`).find(".scroller-content").html(content);
		$(`#${this.id}`).find(".row-count").html(rowcount.toString());
		$(`#${this.id}`).find(".listview-container").removeClass('loading').addClass('loaded');


	}

	body() {
		return `<div class="card-body pt-5 listview" id="${this.id}">
					<div class="d-flex flex-stack mb-2">
						<div class="d-flex align-items-center">
							<h2 data-bookmark-title=${this.isBookmarkTitle}>${this.title}</h2>
							${this.filterButtons()}
						</div>
						${this.addButton()}
					</div>
					<form class="w-100 position-relative" autocomplete="off">
						<span class="svg-icon svg-icon-2 svg-icon-lg-1 svg-icon-gray-500 position-absolute top-50 ms-5 translate-middle-y">
							<i class="bi bi-search"></i>
						</span>
						<input type="text" class="form-control form-control-solid px-15 search-control" placeholder="${this.searchText}">
					</form>
					<div class="listview-container loading">
						${this.loader()}
						<div class="listview-content">
							<div class="scroll-y me-n5 pe-5 h-200px h-lg-auto scroller-content" 
								data-kt-scroll="true" 
								data-kt-scroll-activate="{default: false, lg: true}" 
								data-kt-scroll-max-height="auto" data-kt-scroll-offset="5px"
								style="max-height: 400px;"
								/*style="max-height: 300px;min-height:${this.minHeight};*/">
							</div>
							<strong class="py-5" style="float:right;">Total Records: <span class="row-count"></strong>
						</div>
					</div>
				</div>`;
	}

	triggerFirstFilter() {
		const firstFilterBtn = $(`#${this.id} .filter-button`).first();
		if (firstFilterBtn.length) {
			firstFilterBtn.trigger('click');
		}
	}

	bindEventHandlers() {

		$(`#${this.id}`).find("a.add-button").on("click", () => { this.onAdd() });
		$(`#${this.id}`).find("a.filter-button").on("click", (a) => {
			$(`#${this.id}`).find("a.filter-button span.badge").removeClass('badge-light-primary').addClass('badge-secondary');
			$(a.currentTarget).find("span.badge").addClass('badge-light-primary').removeClass('badge-secondary');
			const onclick = this.filters.onclick;
			const value = $(a.currentTarget).attr("data-filter-value")
			if (onclick) {
				onclick(value);
			}

			this.onsearch(); 
		});

		$(`#${this.id}`).find("input.search-control").on("keyup", (a) => {
			clearTimeout(this.searchtimeout);
			this.searchtimeout = setTimeout(() => { this.onsearch(); }, 800)

		});
		$(`#${this.id}`).find("input.search-control").on("keydown", (a) => {
			clearTimeout(this.searchtimeout);
		});

		$('html').on("click", `th.sortable.${this.id}`,  (c) => {
			const sortingIndex = $(c.currentTarget).attr('data-sort-index');
			const colName = $(c.currentTarget).text().trim().toUpperCase();

			const col = this.columnDefs[sortingIndex];
			var sortDir = $(c.currentTarget).hasClass('asc') ? 'desc' : 'asc';
			col.onsort(colName,sortDir);

			var updatedCol = $(`th.sortable.${this.id}[data-sort-index="${sortingIndex}"]`);
			if (sortDir == 'desc') {
				updatedCol.removeClass('asc').addClass('desc');
			}
			else {
				updatedCol.removeClass('desc').addClass('asc');
			}
		});
	}

	onsearch() {
		const searchTerm = $(`#${this.id}`).find("input.search-control").val().toLowerCase();
		let visibleRowCount = 0;
		$(`#${this.id} table tbody tr`).each(function () {
			const rowText = $(this).text().toLowerCase();
			$(this).show();
			if (rowText.includes(searchTerm)) {
				visibleRowCount++; 
				$(this).show(); // Show row if it contains the search term
			} else {
				$(this).hide(); // Hide row if it does not contain the search term
			}
		});
		$(`#${this.id}`).find(".row-count").html(visibleRowCount.toString());
	}

	preload() {
		$(`#${this.id}`).find(".listview-container").removeClass('loaded').addClass('loading');
	}

	render(elementId) {
		this.elementId = elementId;
		const listView = $(`#${this.elementId}`);
		const shadowClass = this.shadow ? 'has-shadow' : '';
		listView.addClass('card').addClass('card-flush').addClass('h-lg-100').addClass(shadowClass);
		$(`#${this.elementId}`).html(this.body());
		this.bindEventHandlers();
    }
}