class Drawer {
    constructor(selector) {
        this.selector = selector;
        this.onToggle = null;
        this.shown = false;
        this.closeSelector = $(selector).attr("data-kt-drawer-close");
        $(this.closeSelector).on("click", () => {
            this.toggle();
        });
    }

    toggle() {
        const instance = this;

        $(this.selector).toggleClass('drawer-on');
        const overlayClass = 'drawer-overlay';
        const overlayStyle = 'z-index: 109;';

        // Check if the element already exists in the body
        let overlay = document.querySelector(`.${overlayClass}`);

        if (overlay) {
            overlay.remove();
            
        } else {
            overlay = document.createElement('div');
            overlay.className = overlayClass;
            overlay.style.cssText = overlayStyle;
            document.body.appendChild(overlay);
            overlay.addEventListener('click', () => { instance.toggle() });
        }

        this.shown = $(this.selector).hasClass('drawer-on');
        if (this.onToggle) {
            this.onToggle();
        }
    }
}