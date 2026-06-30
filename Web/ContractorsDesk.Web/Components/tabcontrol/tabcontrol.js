class TabControl {
    constructor() {
        this.actions = [];
        this.#init();
    }

    #onSelectTab(dataTabItem) {
        const action = this.actions.find(item => item.tabid === dataTabItem);
        if (action && action.callback) {
            action.callback();
        }
    }

    #init() {
        const instance = this;
        $("html").on("click", "a.tabcontrol-link", function () {
            var dataTabItem = $(this).attr("data-tab-item");
            $('.tabcontrol-item').removeClass('active');
            $(`#${dataTabItem}`).addClass('active');
            $('.tabcontrol-link').removeClass('active');
            $(this).addClass('active');
            instance.#onSelectTab(dataTabItem);
        })
    }
}