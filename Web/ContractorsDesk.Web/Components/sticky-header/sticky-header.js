class StickyHeader {
	constructor(options) {
		let def = {
			selector: null,
			top: '74px',
			showOnScrollTop: 100,
			debug: false
		}
		$.extend(def, options);
		this.element = $(def.selector);
		this.top = def.top;
		this.showOnScrollTop = def.showOnScrollTop;
		this.clone = null;
		this.debug = def.debug;
	}

	#create() {
		this.clone = this.element.clone();
		this.element.after(this.clone);
		
		this.clone.hide();
	}

	#render(scrollTop) {
		const updateWidths = () => {
			const $originalRows = this.element.find("tr");
			const $originalHeaders = this.element.find("th");

			this.clone.find("tr").css("width", $originalRows.outerWidth(true));

			this.clone.find("th").each((index, obj) => {
				const $header = $(obj);
				$header.css("width", $originalHeaders.eq(index).outerWidth(true));
				$header.removeClass('min-w-150px mw-150px mw-250px w-200px w-300px');
			});
		};

		const updateCloneStyles = () => {
			this.clone.css({
				position: "fixed",
				zIndex: 100,
				top: `${this.top}`,
				width: `${this.element.outerWidth(true)}`,
			}).toggle(scrollTop >= this.showOnScrollTop);
		};

		updateWidths();
		updateCloneStyles();

		if (this.debug) {
			console.log(`scrollTop: ${scrollTop}, this.offSetTop: ${this.offSetTop}`);
		}
	}


	#bindEventHandlers() {
		const instance = this;
		const handleScrollOrResize = () => {
			const scrollTop = $(window).scrollTop();
			instance.#render(scrollTop);
		};

		$(window).on('scroll resize', handleScrollOrResize);
	}

	init() {
		this.#create();
		this.#bindEventHandlers();
	}
}