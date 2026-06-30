class SearchableDropdown {
	constructor() {
		this.elementId = '';
		this.data = [];
		this.optionSelected = false;
		this.optionText = null;
		this.disabled = false;
		this.iconText = null;
		this.width = '100%';
		this.onSelect = null;
    }

	init(elementId) {
		this.elementId = elementId;
		const onselect = this.onSelect;
		if (this.data.length > 0) {
				
			const options = this.data.map(o => {
				let isSelected = this.optionSelected ? this.optionSelected(o) : false;
				const selected = isSelected ? 'selected' : '';
				const iconText = this.iconText ? this.iconText(o) : '';
				const text = this.optionText ? this.optionText(o) : o.id;

				return `<option ${selected} value="${o.id}" title="${iconText}">${text}</option>`
			}
			).join('');

			if (this.disabled) {
				$(elementId).attr("disabled", true);
			}

			$(elementId).html(options).select2({
				templateResult: function (item) {
					if (!item.id) {
						return item.text;
					}
					var span = $("<span>");
					var icon = $("<span>", {
						class: "btn btn-sm btn-icon btn-dark btn-active-icon-primary rounded-circle",
						text: item.title
					});
					var text = $("<span>", {
						class: "ps-3",
						text: item.text
					});

					span.append(icon);
					span.append(text);
					return span;
				},
				width: this.width
			});

			$(elementId).on('select2:select', function (e) {
				if (onselect) onselect();
			});
		}
	}

	getSelectedValues() {
		return $(this.elementId).select2('data').map(item => parseInt(item.id))
	}
}

class SearchableDropdown2 {
	constructor() {
		this.elementId = '';
		this.data = [];
		this.optionSelected = false;
		this.optionText = null;
		this.disabled = false;
		this.iconText = null;
		this.width = '100%';
		this.onSelect = null;
		this.onDeselect = null;
	}

	init(elementId) {
		this.elementId = elementId;
		const onselect = this.onSelect;
		const ondeselect = this.onDeselect;

		if (this.data.length > 0) {
			const options = this.data.map(o => {
				let isSelected = this.optionSelected ? this.optionSelected(o) : false;
				const selected = isSelected ? 'selected' : '';
				const iconText = this.iconText ? this.iconText(o) : '';
				const text = this.optionText ? this.optionText(o) : o.id;

				return `<option ${selected} value="${o.id}" title="${iconText}">${text}</option>`;
			}).join('');

			if (this.disabled) {
				$(elementId).attr('disabled', true);
			}

			$(elementId).html(options).select2({
				templateResult: function (item) {
					if (!item.id) {
						return item.text;
					}
					const span = $('<span>');
					const icon = $('<span>', {
						class: 'btn btn-sm btn-icon btn-dark btn-active-icon-primary rounded-circle',
						text: item.title,
					});
					const text = $('<span>', {
						class: 'ps-3',
						text: item.text,
					});

					span.append(icon);
					span.append(text);
					return span;
				},
				width: this.width,
			});

			$(elementId).on('select2:select', function (e) {
				const selectedId = e.params.data.id;
				const selectedItem = this.data.find(o => o.id === selectedId);
				if (onselect) onselect(selectedItem);
			}.bind(this));

			$(elementId).on('select2:unselect', function (e) {
				const deselectedId = e.params.data.id;
				const deselectedItem = this.data.find(o => o.id === deselectedId);
				if (ondeselect) ondeselect(deselectedItem);
			}.bind(this));
		}
	}

	getSelectedValues() {
		return $(this.elementId).select2('data').map(item => parseInt(item.id));
	}
}

