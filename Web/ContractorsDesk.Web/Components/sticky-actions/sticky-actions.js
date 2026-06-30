class StickyActions{
	constructor(wclass){
		this.actions = [];
		this.wclass = wclass ?? 'min-w-lg-400px';
	}

	mapActionButtons() {
		const buttonHtml = (a) => {
			return a.preloadonclick ? `<span class="indicator-label">${a.icon ?? ''} ${a.title}</span>
										<span class="indicator-progress">
											Please wait...
											<span class="spinner-border spinner-border-sm align-middle ms-2"></span>
										</span>` : a.icon ? `<i class="bi ${a.icon}"></i>${a.title}` : a.title;
		}

		return this.actions.map(actionButton => `
								<button type="button" class="btn btn-sm btn-${actionButton.color} ${actionButton.customClasses} me-2 ${actionButton.widthclass} sticky-action-button" data-id="${actionButton.id}">
									${buttonHtml(actionButton)}
								</button>`).join('');
	}
	bindEventHandlers() {
		this.actions.map(ab => {
			var id = ab.id;
			if (ab.onactionclick) {
				$(`button[data-id="${id}"]`).on("click", (e) => {
					e.preventDefault();
					const button = e.currentTarget;
					if (ab.preloadonclick) {
						//button.setAttribute('data-kt-indicator', 'on');
						//button.disabled = true;
					}
					e.stopImmediatePropagation();
					ab.onactionclick(button); 
				});
			}
		});
	}

	init = () => {
		$('.sticky-actions').remove();
		$("body").append(`<div id="kt_scrolltop" class="scrolltop ${this.wclass} h-50px bg-white sticky-actions" data-kt-scrolltop="true">
							<div class="d-flex flex-stack">
								<div class="d-flex align-items-center">
									${this.mapActionButtons()}
								</div>
							</div>
						</div>`);

		this.bindEventHandlers();
		KTScrolltop.init();
    }
}

class StickyActionsButton{
	constructor(title, icon, color, widthclass, preloadonclick, onactionclick, customClasses = '') {
		this.id = GenerateId();
		this.title = title;
		this.preloadonclick = preloadonclick;
		this.onactionclick = onactionclick;
		this.color = color ?? 'light';
		this.icon = icon;
		this.widthclass = widthclass ?? '';
		this.customClasses = customClasses
	}
}

