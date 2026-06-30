class NotificationView extends DomEventComponent {
    constructor() {
        super();
        this.proposalId = document.URL.split('/').pop();
        this.service = new NotificationService();
        this.notifications = [];
        this.openAccordion = null
	}

    bindEventHandlers() {
        const instance = this;
        // Handle accordion open/close
        $(document).on('click', '.notification-item .card-header', function (e) {
            // Prevent opening accordion if checkbox is clicked
            if ($(e.target).is('.notification-checkbox')) {
                return;
            }

            var notification = $(this).closest('.notification-item');
            var content = notification.find('.accordion-content');
            var icon = notification.find('.rotate-icon');
            var notificationId = notification.data('id');
            var cardTitle = notification.find('.card-title');

            // Mark as read if unread
            if (notification.data('read') === false) {
                notification.data('read', true);
                cardTitle.addClass('font-weight-200').removeClass('text-dark');
                instance.updateData(notificationId);
            }

            // Close any open accordion except the current one
            if (instance.openAccordion && instance.openAccordion[0] !== content[0]) {
                instance.openAccordion.slideUp(300);
                $('.rotate-icon').removeClass('rotate-down');
            }

            if (content.is(':visible')) {
                content.slideUp(300);
                icon.removeClass('rotate-down');
                instance.openAccordion = null;
            } else {
                content.slideDown(300);
                icon.addClass('rotate-down');
                instance.openAccordion = content;
            }
        });
	}

	loadData() {
        const currentUser = Auth.currentUser();
        this.service.loadUserNotification(currentUser.id)
            .then((notifications) => {
                this.notifications = notifications;
                this.renderNotifications(notifications);
                $(".card-custom").removeClass('loading').addClass('loaded');
            });
    }

    renderNotifications(notifications) {
        const notificationList = $('#notificationList');
        notificationList.empty();

        notifications.forEach((notification) => {
            const textClass = notification.isRead ? 'font-weight-200' : 'text-dark';
            const notificationItem = `
                <div class="notification-item card mb-1" data-id="${notification.id}" data-read="${notification.isRead}">
                    <div class="card-header">
                        <div class="d-flex align-items-center">
                            <input type="checkbox" class="notification-checkbox" data-id="${notification.id}" evt-change="showHideActionItems">
                            <span class="card-title ${textClass}">${notification.message.replace(/ /g, "&nbsp;")}</span>
                        </div>
                        <div class="d-flex align-items-center" style="gap: 10px;">
                            <span class="notification-date text-muted">${notification.dateCreated}</span>
                            <div class="icon-container">
                                <i class="fas fa-chevron-down rotate-icon"></i>
                            </div>
                        </div>
                    </div>
                    <div class="accordion-content">
                        <p><a href="${notification.relatedUrl}" target="_blank" class="notification-link text-dark"><strong>Details:</strong> ${notification.description}</a></p>
                    </div>
                </div>
            `;
            notificationList.append(notificationItem);
        });
    }

    updateData(id) {
        this.service.updateNotification(id).then((res) => {
            //this.service.notificationCheck();
        });

    }

    refresh() {
        location.reload();
    }

    unSelectAll() {
        $('.notification-checkbox').prop('checked', false);
        this.showHideActionItems();
    }

    selectAll() {
        $('.notification-checkbox').prop('checked', true);
        this.showHideActionItems();
    }

    showHideActionItems() {
        var selectedCount = $('.notification-checkbox:checked').length;
        if (selectedCount > 0) {
            $('#btnUnSelectAll').removeClass('d-none');
            $('#btnSelectAll').addClass('d-none');
        } else {
            $('#btnUnSelectAll').addClass('d-none');
            $('#btnSelectAll').removeClass('d-none');
        }
    }

    markedAsRead() {
        var selectedCount = $('.notification-checkbox:checked').length;
        if (selectedCount === 0)
            return;

        const instance = this;
        $('.notification-checkbox:checked').each(function () {
            var id = $(this).data('id');
            var notification = $('.notification-item[data-id="' + id + '"]');
            var cardTitle = notification.find('.card-title');

            if (notification.data('read') === false) {
                notification.data('read', true);
                cardTitle.addClass('font-weight-200').removeClass('text-dark');
                instance.updateData(id);
            }
        });
        $('.notification-checkbox').prop('checked', false); // Uncheck all after marking read
    }

	init() {
		this.loadData();
		this.bindEventHandlers();
    }
}

$(document).ready(() => {
	const view = new NotificationView();
    view.init();
});