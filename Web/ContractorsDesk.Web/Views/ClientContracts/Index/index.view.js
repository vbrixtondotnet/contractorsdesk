class ClientContractView extends DomEventComponent {
    constructor() {
        super();
        this.service = new ClientContractService();
    }

    async generateContract(button) {
        button.setAttribute('data-kt-indicator', 'on');
        button.disabled = true;
        const payload = {
            proposalId: $("#txtProposalId").val(),
            clientName: $("#txtClientName").val(),
            clientEmail: $("#txtClientEmail").val(),
            clientAddress: $("#txtprojectAddress").val(),
            clientCity: 'Los Angeles',
            clientState: 'CA',
            contractorEmailAddress: "chacontruction@gmail.com",
            estStartDate: $("#txtestStartDate").val(),
            estCompletionDate: $("#txtestCompletionDate").val(),
            genContractorsFeePercentage: $("#txtgenContractorsFeePercentage").val(),
            genContractorsFeeAmount: $("#txtgenContractorsFeeAmount").val(),
            projectCost: $("#txtprojectCost").val(),
            initialDepositPercentage: $("#txtinitialDepositPercentage").val(),
            contractor: $("#txtcontractor").val()
        };


        const response = await fetch('/api/proposals/client-contract', {
            method: 'POST', // or 'POST', depending on your API
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error('Failed to fetch file');
        }
        //// Get the filename from the `Content-Disposition` header
        const disposition = response.headers.get('Content-Disposition');
        // Regular expression to extract the filename
        const matches = disposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);

        let filename = null;
        if (matches) {
            // Extract the value of the filename
            filename = matches[1].replace(/['"]/g, ''); // Remove any quotes around the filename
        }

        const blob = await response.blob(); // Get the file data as a Blob
        const url = window.URL.createObjectURL(blob); // Create a URL for the Blob
        // Create an anchor element to download the file
        const a = document.createElement('a');
        a.href = url;
        a.download = filename; // Replace with the desired file name
        document.body.appendChild(a); // Append the anchor to the DOM
        a.click(); // Trigger the download
        document.body.removeChild(a); // Remove the anchor element
        window.URL.revokeObjectURL(url); // Release the Blob URL
        button.removeAttribute('data-kt-indicator');
        button.disabled = false;
    }

    init() {

    }
}

$(document).ready(() => {
    const view = new ClientContractView();
    view.init();
});