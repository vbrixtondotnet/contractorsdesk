const config = {
    baseUrl: {
        local: 'http://localhost:5000',
        dev: 'https://testcontractorsdesk.azurewebsites.net/',
        prod: 'https://chaconstruction.contractors-desk.com/'
    },
    getBaseUrl: () => {
        if (window.location.href.includes('localhost')) {
            return config.baseUrl.local;
        } else if (window.location.href.includes('testcontractorsdesk')) {
            return config.baseUrl.dev;
        } else {
            return config.baseUrl.prod;
        }
    }
}