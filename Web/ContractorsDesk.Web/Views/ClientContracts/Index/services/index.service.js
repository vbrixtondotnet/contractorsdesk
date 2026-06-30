class ClientContractService {
    constructor() {
        this.httpService = new httpService();
    }

    generateContract() {
        return this.httpService.post('/api/proposals/client-contract');
    }
}
