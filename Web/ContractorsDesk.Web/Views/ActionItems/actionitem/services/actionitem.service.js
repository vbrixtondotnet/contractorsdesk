class ActionItemService {
    constructor() {
        this.httpService = new httpService();
    }

    loadActionItem(id) {
        return this.httpService.get('/api/action-items/' + id);
    }

    loadActionItemSummary(id) {
        return this.httpService.get('/api/action-item/summary/' + id);
    }
    acceptActionItem(id) {
        return this.httpService.patch(`/api/action-items/${id}/accept`);
    }
    completeActionItem(id) {
        return this.httpService.patch(`/api/action-items/${id}/complete`);
    }
}
