
var webAppIdle = {
    idleTimer: null,
    idleLimit: 60 * 60 * 1000,
    resetIdleTimer: function () {
        clearTimeout(this.idleTimer);
        this.idleTimer = setTimeout(() => this.showUserIdleNotif(), this.idleLimit);
    },
    showUserIdleNotif: function () {
        Swal.fire({
            title: "You've been idle!",
            text: "To ensure you are seeing the latest data, please reload the page or sign in again.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Reload Page",
            cancelButtonText: "Sign Out",
            buttonsStyling: false,
            allowOutsideClick: false,
            allowEscapeKey: false,
            customClass: {
                confirmButton: "btn btn-success me-3",
                cancelButton: "btn btn-danger"
            }
        }).then(function (result) {
            if (result.isConfirmed) {
                location.reload();
            } else {
                location.href = "/signout";
            }
        });
    },
    eventHandler: function () {
        this.resetIdleTimer();

        const events = ['mousemove', 'keydown', 'scroll', 'touchstart'];
        events.forEach(event => {
            window.addEventListener(event, this.resetIdleTimer.bind(this), true);
        });
    },
    init: function () {
        this.eventHandler();
    }
};

$(document).ready(() => {
    webAppIdle.init();
});