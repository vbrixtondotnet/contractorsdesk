class ProfitAndLossReportService {
    constructor() {
        this.httpService = new httpService();
    }

    loadReport(ids) {
        return this.httpService.get(`/api/quickbooks/reports/profitandloss?classListIds=${ids}`);
    }
}
