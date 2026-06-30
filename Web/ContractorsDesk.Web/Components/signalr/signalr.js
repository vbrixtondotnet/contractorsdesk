class SignalR {
    constructor() {
        this.selector = 'div#kt_activities_notifications';
        this.httpService = new httpService();
        this.isNewMailReceivedPopupShown = false;
    }

    #buildNoNewNotification() {
        return `<div class="timeline-item">
					<div class="timeline-content mb-2 mt-n1">
						<div class="overflow-auto pe-3">
							<div class="fs-5 fw-bold mb-2 text-center text-muted">No new notifications</div>
						</div>
					</div>
				</div>`;
    }

    #buildNotification(notification) {
        const notificationContent = `<div class="timeline-content mb-2 mt-n1">
			                            <div class="overflow-auto pe-3">
                                            <div class="d-flex justify-content-end mt-1 fs-6">
					                            <div class="text-muted me-2 fs-7">${DateUtils.getDateDurationFromAspNetDate(notification.dateCreated)}</div>
				                            </div>
				                            <div class="fs-7 mb-2 text-dark">${notification.message}</div>
                                            <button class="remove-notification" data-id="${notification.id}">
                                                <i class="fas fa-times"></i>
                                            </button>
			                            </div>
		                            </div>`;

        let notificationWrapper = notificationContent;

        if (notification.relatedUrl) {
            notificationWrapper = `<a class="notification-link" href="javascript:" data-href="${notification.relatedUrl}" data-id="${notification.id}">
		                                ${notificationContent}
	                                </a>`
        }

        return `<div id="notif_${notification.id}" class="timeline-item notification-item">
	                ${notificationWrapper}
                </div>`;
    }

    #buildSeparator(notification) {
        return `<div id="notif_divider_${notification.id}" class="separator my-2"></div>`;
    }

    #promptBrowserNotification() {
        if (Notification.permission === "granted") {
            new Notification("🔔 New Notification!", { body: "You have a new message." });
        } else if (Notification.permission !== "denied") {
            Notification.requestPermission().then(permission => {
                if (permission === "granted") {
                    new Notification("🔔 New Notification!", { body: "You have a new message." });
                }
            });
        }
    }

    #promptNotificationCounter(counter) {
        const notificationCounter = $('div#kt_header').find('span#badge-notification-count');
        const notificationCounterInput = $('div#kt_header').find('input#hidden-notification-count');
        let hasNotification = counter == 0 ? false : true;

        if (hasNotification) {
            notificationCounter.removeClass('d-none');
            notificationCounter.text(`${counter}`);
            notificationCounterInput.val(`${counter}`);
        } else {
            notificationCounter.addClass('d-none');
            notificationCounter.text('');
            notificationCounterInput.val('');
        }
    }

    promptNotification(notification) {
        const notificationsParent = $('div#kt_activities_notifications');
        const notificationTimeline = notificationsParent.find('.timeline');
        let newNotification = notificationTimeline.find(`div#notif_${notification.id}`);

        if (newNotification.length == 0) {
            let notificationItems = notificationsParent.find('.timeline-item');
            let newNotificationElmnt = this.#buildNotification(notification);
            let newNotificationSeparator = this.#buildSeparator(notification);

            if (notificationItems.length == 1 && notificationItems[0].innerText.toLowerCase() == 'no new notifications') {
                $(this.selector).find('.timeline').html(newNotificationElmnt);
            }
            else {
                $(this.selector).find('.timeline').prepend(newNotificationSeparator);
                $(this.selector).find('.timeline').prepend(newNotificationElmnt);
            }
            
            notificationItems = notificationsParent.find('.timeline-item');
            this.#promptNotificationCounter(notificationItems.length);
            this.#promptBrowserNotification();
            //this.#playNotificationSound();
            //this.#promptFlashTitle();
        }
    }

    promptDataSyncStartedNotification(notification) {
        let notificationId = notification.id;
        $("#background-process-title").html("QuickBooks Data Sync Progress");
        const notificationsParent = $('div#kt_activities_notifications');
        const notificationTimeline = notificationsParent.find('.timeline');
        let newNotification = notificationTimeline.find(`div#notif_${notificationId}`);

        let notificationItems = notificationsParent.find('.timeline-item');

        if (newNotification.length == 0) {
            let newNotificationElmnt = `<div id="notif_${notificationId}" class="timeline-item notification-item">
	                    <a class="toggle-background-process-messages" href="javascript:" data-id="">
		                    <div class="timeline-content mb-2 mt-n1">
			                    <div class="overflow-auto pe-3">
				                    <div class="fs-7 mb-2 text-dark"><span class="mini-loader"></span> 
                                        <strong>QuickBooks Data Sync</strong> is in progress. Click here to view the details.</div>
				                    <div class="d-flex align-items-center mt-1 fs-6">
					                    <div class="text-muted me-2 fs-7">6/3/2025 7:42:35 PM</div>
				                    </div>
			                    </div>
		                    </div>
	                    </a>
                    </div>
                    <div id="notif_divider_${notificationId}" class="separator my-2"></div>`;
            //let newNotificationSeparator = this.#buildSeparator(notification);

            if (notificationItems.length == 1 && notificationItems[0].innerText.toLowerCase() == 'no new notifications') {
                $(this.selector).find('.timeline').html(newNotificationElmnt);
            }
            else {
                //$(this.selector).find('.timeline').prepend(newNotificationSeparator);
                $(this.selector).find('.timeline').prepend(newNotificationElmnt);
            }

            notificationItems = notificationsParent.find('.timeline-item');
            this.#promptNotificationCounter(notificationItems.length);
            this.#promptBrowserNotification();
            
            if (notification.description) {
                $(`div#background-process-messagescroll`).find("div.progress-content").find("pre").html(notification.description);
            }
            //this.#playNotificationSound();
            //this.#promptFlashTitle();
        }
    }

    markNotificationRead(e) {
        const notificationTimeline = $('div#kt_activities_notifications').find('.timeline');
        const notificationParent = e.currentTarget.offsetParent;
        //let notificationId = notificationParent.attributes.id.value.replace('notif_', '');
        //let notificationSeparator = $(notificationTimeline).find(`div#notif_divider_${notificationId}`);
        //let prevNotificationSeparator = $(notificationParent).prev();
      
        var notificationId = $(e.currentTarget).data('id');
        var instance = this;
        $("div#notif_" + notificationId).slideUp(function () {
            $(this).next('.separator').remove();
            $(this).remove();

            const notificationItems = notificationTimeline.find('.timeline-item');
            instance.#promptNotificationCounter(notificationItems.length);
     
            if (notificationItems.length == 0) {
                let noNewNotificationElmnt = instance.#buildNoNewNotification();
                $(instance.selector).find('.timeline').html(noNewNotificationElmnt);
            }

        });

        this.httpService.put('/api/notification-read/' + notificationId, null);
        
        e.preventDefault();
        e.stopPropagation();
    }
}

$(document).ready(() => {
    const signalr = new SignalR();

    $('html').on('click', 'button[id^="notif_btn"]', function (e) {
        signalr.markNotificationRead(e);
    });

    $('html').on('click', 'a.toggle-background-process-messages', function (e) {
        $("#kt-btn-notifications_close").trigger("click");
        $("#btnToggleBackgroundProcessMessages").click();
    });
    $('html').on('click', 'a.notification-link', function (e) {
        var href = $(this).data('href');
        var notificationId = $(this).data('id');
        signalr.httpService.put('/api/notification-read/' + notificationId, null)
            .then(() => {
                location.href = href;
            });
    });

    $('html').on('click', '.notification-item button.remove-notification', function (e) {
        signalr.markNotificationRead(e);
    });

    signalRConnection.on("DefaultGlobalNotification", function (title, message) {
        alert("New Notification: " + title + " - " + message);
    });

    signalRConnection.on("DefaultUserNotification", function (title, message) {
        alert("New Notification: " + title + " - " + message);
    });

    signalRConnection.on("GlobalNotification", function (notification) {
        signalr.promptNotification(notification);
    });

    signalRConnection.on("UserNotification", function (notification) {
        if (notification.message.toUpperCase() == "DATA SYNC STARTED")
            signalr.promptDataSyncStartedNotification(notification);
        else
            signalr.promptNotification(notification);
    });

    signalRConnection.on("DataSyncStarted", function (notification) {
        signalr.promptDataSyncStartedNotification(notification);
    });
    signalRConnection.on("DataSyncNotification", function (payload) {
        $(`${payload.containerId}`).find("div.progress-content").find("pre").html(payload.message);
    });

    if (window.location.pathname !== '/emails') {
        signalRConnection.on("OnNewEmailReceived", function (email) {
            if (!signalr.isNewMailReceivedPopupShown) {
                Swal.fire({
                    title: ' ',
                    html: `
                <div class="d-flex flex-column align-items-center">
                    <i class="bi bi-envelope-fill text-primary" style="font-size: 4rem;"></i>
                    <h3 class="mt-3 mb-4 fw-bold text-center">You've got mail!</h3>
                </div>
            `,
                    showCancelButton: true,
                    confirmButtonText: 'Open Mail',
                    cancelButtonText: 'View Later',
                    allowOutsideClick: false,
                    reverseButtons: true
                }).then((result) => {
                    signalr.isNewMailReceivedPopupShown = false;
                    if (result.isConfirmed) {
                        window.location.href = '/emails';
                    }
                });
                signalr.isNewMailReceivedPopupShown = true;
            }
        });
    }
   
});