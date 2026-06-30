class printer {
    constructor() {

    }

    printCurrentPage(url) {
        const printWindow = window.open(url, '', 'height=700,width=1000');
        printWindow.print();
    }
}