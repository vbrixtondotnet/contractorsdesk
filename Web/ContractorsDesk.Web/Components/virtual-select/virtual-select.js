class VirtualSelect {
    constructor(options) {
        let def = {
            element: null,
            options: [],
            defaultValue: null, // Add defaultValue
            ajax: {
                type: 'query',
                key: '',
                url: ''
            },
            onselect: null
        };

        $.extend(def, options);

        this.options = def.options;
        this.element = def.element;
        this.defaultValue = def.defaultValue; // Store defaultValue
        this.onselect = def.onselect;
        this.listActive = false;
        this.link = null;
    }

    setWidth($link) {
        const $width = $link.outerWidth();
        const $list = $link.next();
        $list.css('width', `${$width}px`);
    }

    setDefaultValue() {
        if (this.defaultValue) {
            const defaultOption = this.options.find(option => option.id === this.defaultValue);
            if (defaultOption) {
                $(this.element).html(`${defaultOption.text} <span class="arrow"></span>`);
            }
        }
    }

    bindOnInput() {
        $('html').on('input', '.virtual-select-search', (e) => {
            const $textbox = $(e.currentTarget);
            const query = $textbox.val().trim().toLowerCase();
            $("div.virtual-select-item").each(function () {
                const text = $(this).text();
                if (text.trim().toLowerCase().indexOf(query) < 0) {
                    $(this).addClass('d-none');
                } else {
                    $(this).removeClass('d-none');
                }
            });
        });
    }

    bindOnOutsideClick() {
        $(document).off('click').on('click', (e) => {
            const $list = $(".virtual-select-container");
            const $current = $(".virtual-select.current");

            if (
                !$list.is(e.target) &&
                $list.has(e.target).length === 0 &&
                !$current.is(e.target) &&
                $current.has(e.target).length === 0
            ) {
                $list.remove();
                $current.removeClass('current');
            }
        });
    }

    bindOnSelect($link) {
        const $list = $link.next();
        const instance = this;
        $list.off('click').on('click', '.virtual-select-item', (e) => {
            const text = $(e.currentTarget).text();
            const id = $(e.currentTarget).attr("data-id");
            const style = $(e.currentTarget).attr("style");
            const selectedItem = {
                id: id,
                text: text
            };
            $link.attr('style', style);
            $link.html(`${text} <span class="arrow"></span>`);
            $list.remove();
            this.link = $link;

            $(".virtual-select.current").removeClass('current');

            if (instance.onselect) instance.onselect(selectedItem);
            e.stopImmediatePropagation();
        });
    }

    bindOnClickSearch($link) {
        $('html').on('focus click', '.virtual-select-search', (e) => {
            e.stopImmediatePropagation();
        });
        $('.virtual-select-search').focus();
    }

    setText(text) {
        this.link.html(`${text} <span class="arrow"></span>`);
    }

    #renderList(a) {
        const $link = $(a.currentTarget);
        $link.addClass('current');

        $link.after(`<div class="virtual-select-container"></div>`);
        const $list = $link.next();

        $list.append(`<div class="virtual-select-item-search">
                        <input type="text" class="form-control form-control-sm virtual-select-search" placeholder="Search" />
                    </div>`);
        this.options.forEach(item => {
            let style = item.style ?? '';
            $list.append(`<div class="virtual-select-item" style="${style}" data-id="${item.id}">${item.text}</div>`);
        });

        this.setWidth($link);
        this.bindOnClickSearch($link);
        this.bindOnSelect($link);
        this.bindOnOutsideClick();
        this.bindOnInput();

        this.listActive = true;

    }

    bindEventHandlers() {
        $(this.element).on("click", (a) => {
            const $link = $(a.currentTarget);
            if ($link.hasClass('current')) {
                $(".virtual-select-container").remove();
                $link.removeClass('current');
            } else {
                $(".virtual-select-container").remove();
                $(".virtual-select").removeClass('current');
                this.#renderList(a);
            }
            a.stopImmediatePropagation();
        });

        this.bindOnOutsideClick();
    }

    init() {
        this.setDefaultValue(); // Set the default value
        this.bindEventHandlers();
    }
}
