class AutoComplete {
	constructor(options) {
		let def = {
			element: null,
			options:[],
			ajax: {
				type: 'query',
				key: '',
				url: ''
			},
			onselect: null
		}

		$.extend(def, options);

		this.options = def.options;
		this.element = def.element;
		this.ajax = def.ajax;
		this.ajaxTimeout = null;
		this.onselect = def.onselect;
	}

	setWidth() {
		const $textbox = $(this.element);
		const $width = $textbox.outerWidth();
		const $list = $textbox.next();
		$list.css('width', `${$width}px`);
	}

	bindOnOutsideClick() {
		const $textbox = $(this.element);
		const $list = $textbox.next();
		$(document).on('click', function (e) {
			if (!$(e.target).closest('#autocomplete, #autocomplete-list').length) {
				$list.addClass('d-none');
			}
		});
	}

	bindOnClick() {
		const $textbox = $(this.element);
		const $list = $textbox.next();
		const instance = this;
		$list.off('click').on('click', '.autocomplete-item', function () {
			const text = $(this).text();
			const id = $(this).attr("data-item-id");
			const selectedItem = {
				id: id,
				text: text
			};

			$textbox.val(text).trigger('change');
			$list.addClass('d-none');
			if (instance.onselect) instance.onselect(selectedItem);
		});
	}

	bindOnKeyDown() {
		$(this.element).on("keydown", () => {
			if (this.ajaxTimeout) {
				clearTimeout(this.ajaxTimeout);
			}
		});
	}

	bindOnInput() {
		const $textbox = $(this.element);
		const $list = $textbox.next();
		const query = $textbox.val().trim().toLowerCase();
		$list.empty();

		if (!query) {
			this.#hideList();
			return;
		}

		this.#showList();

		if (this.options?.length) {
			this.#handleLocalOptions(query, this.options);
		} else if (this.ajax?.url) {
			this.#handleAjaxSearch(query);
		}
	}

	#hideList() {
		const $list = $(this.element).next();
		$list.addClass('d-none');
	}

	#showList() {
		const $list = $(this.element).next();
		$list.removeClass('d-none');
	}

	#handleLocalOptions(query, options) {
		const $list = $(this.element).next();
		const filteredItems = options.filter(item => item.name.toLowerCase().includes(query));
		if (filteredItems.length) {
			this.#renderListItems(filteredItems);
		} else {
			this.#hideList();
		}
	}

	#handleAjaxSearch(query) {
		this.#renderLoadingState();
		this.onajaxsearch(query); // Assuming `this.onajaxsearch` is bound appropriately
	}

	#renderListItems(items) {
		const $list = $(this.element).next();
		const itemsHtml = items.map(item => `<div class="autocomplete-item" data-item-id="${item.id}">${item.name}</div>`).join('');
		$list.html(itemsHtml);
	}

	#renderLoadingState() {
		const $list = $(this.element).next();
		$list.html(`<div class="autocomplete-item-searching">Searching...</div>`);
	}

	bindEventHandlers() {
		$(this.element).on("input", () => {
			this.setWidth();
			this.bindOnClick();
			this.bindOnOutsideClick();
			this.bindOnInput();
		});

		$(window).on('resize', () => {
			this.setWidth();
		});
	}

	onajaxsearch(searchValue) {
		clearTimeout(this.ajaxTimeout);
		this.ajaxTimeout = setTimeout(() => {
			const $list = $(this.element).next();
			const url = this.ajax.type == 'query' ? `${this.ajax.url}?${this.ajax.key}=${searchValue}` : `${this.ajax.url}/${key}`;
			$.ajax({
				url: url,
				type: 'GET',
				dataType: 'json',
				success: function (response) {
					$list.find('.autocomplete-item-searching').remove();
					if (response && response.data && response.data.length > 0) {
						response.data.forEach(item => {
							$list.append(`<div class="autocomplete-item" data-item-id="${item.id}">${item.name}</div>`);
						});
					}
				},
				error: function (xhr, status, error) {
					console.error('Error:', error);
				}
			});
		}, 500);
	}

	init() {
		
		$(this.element).after(`<div class="autocomplete-list d-none"></div>`);
		const $list = $(this.element).next();
		
		this.options.forEach(item => {
			$list.append(`<div class="autocomplete-item">${item}</div>`);
		});

		this.bindEventHandlers();
		this.setWidth();
    }
}