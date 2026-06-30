class DomEventComponent {
	constructor() {
		this.httpService = new httpService();
		this.swal = new SwalUtil();
		this.cacheService = new CacheService();
		this.initEventHandlers();
		this.hasChanges = false;
		this.enableUnloadProtection = false;
		this.saveChangesMethod = null;
		this.isInitializing = true;

		if (this.enableUnloadProtection) {
			this.initUnloadProtection();
		}
	}

	initEventHandlers() {
		const fireEvent = (e, name, trackChanges = false) => {
	
			var ignoreChangeFlag = $(e.target).attr('evt-ignore-change');

			//console.log(`${name} initiated ${new Date()}`);
			// Check if the clicked element or its ancestor has 'evt-click' attribute
			const targetElement = e.target.closest(`[${name}]`);
			//const targetElement = event.target;
			if (!targetElement) return true; // Exit if no matching element

			// Extract the method name from the 'evt-click' attribute
			const methodName = targetElement.getAttribute(name);

			// Check if the method exists in the class and call it
			if (typeof this[`${methodName}`] === 'function') {
				this[`${methodName}`](targetElement, e);
				if (trackChanges && !this.isInitializing && (!ignoreChangeFlag || ignoreChangeFlag != "true")) {
					this.hasChanges = true;
				}
				
			}
		}
	
		document.addEventListener('click', (event) => {
			fireEvent(event, 'evt-click');
		});
		
		document.addEventListener('input', (event) => {
			fireEvent(event, 'evt-input', true);
		});

		document.addEventListener('change', (event) => {
			fireEvent(event, 'evt-change', true);
		});

		document.addEventListener('keyup', (event) => {
			fireEvent(event, 'evt-keyup');
		});

		document.addEventListener('keydown', (event) => {
			fireEvent(event, 'evt-keydown');
		});

		$("html").on("focus", "input", (event) => {
			fireEvent(event, 'evt-focus');
		})

		$('form').on('input change', 'input, textarea, select', (e) => {
			if (!this.isInitializing) {
				//var ignoreChangeFlag = $(e.target).attr('evt-ignore-change');
				
				this.hasChanges = true;
			}
		});

		$(document).on('select2:select select2:unselect', 'select[data-control="schedule-select2"]', () => {
			if (!this.isInitializing) {
				this.hasChanges = true;
			}
		});
	}

	initUnloadProtection() {
		const shouldWarnUser = () => this.hasChanges;

		const confirmNavigation = (proceedCallback) => {
			this.swal?.confirm?.(
				'There are unsaved changes. Do you want to save before leaving?',
				() => {
					if (typeof this.saveChangesMethod === 'function') {
						this.saveChangesMethod(proceedCallback);
					} else {
						console.warn('No saveChangesMethod provided.');
						proceedCallback();
					}
				},
				() => {
					proceedCallback();
				}
			);
		};

		document.querySelectorAll('a').forEach(link => {
			link.addEventListener('click', (e) => {
				const href = link.getAttribute('href');
				if (
					link.classList.contains('tabcontrol-link') ||           
					href.startsWith('#') ||                                     
					href.startsWith('javascript:') ||                           
					link.getAttribute('data-bs-toggle') === 'modal' ||    
					link.hasAttribute('data-ignore-unsaved-check')            
				) {
					return;
				}
				
				//if (shouldWarnUser()) {
				//	e.preventDefault();
				//	confirmNavigation(() => {
				//		window.location.href = link.href;
				//	});
				//}
			});
		});

		//window.addEventListener('keydown', (e) => {
		//	const isReload = e.key === 'F5' || ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'r');
		//	if (isReload && shouldWarnUser()) {
		//		e.preventDefault();
		//		confirmNavigation(() => location.reload());
		//	}
		//});
	}
}