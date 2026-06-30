class NotificationService {
    constructor() {
        this.httpService = new httpService();
    }

    loadUserNotification(id) {
        return this.httpService.get('/api/notification-user/' + id);
    }

    updateNotification(notificationId) {
        return this.httpService.put("/api/notification-read/" + notificationId);
    }

    notificationCheck() {
        return this.httpService.get("/api/notification-check");
    }
    
}
