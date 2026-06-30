class EmailsService {
    constructor() {
        this.httpService = new httpService();
    }

    loadSentEmails() {
        return this.httpService.get('/api/email/sent-emails');
    }
    loadSentEmailById(id) {
        return this.httpService.get('/api/email/sent-email/' + id);
    }
    sendEmail(emailModel) {
        return this.httpService.post('/api/email/send/', emailModel);
    }
    markSentEmailAsRead(id) {
        return this.httpService.put('/api/email/sent-email-read/' + id);
    }
}
