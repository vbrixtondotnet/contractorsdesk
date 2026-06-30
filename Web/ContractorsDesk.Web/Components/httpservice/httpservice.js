class httpService {
    // General method to handle any request type
    request(url, method = 'GET', data = null, isFormData = false) {
        const options = {
            method
        };

        if (!isFormData) {
            options.headers = {
                'Content-Type': 'application/json',
            }
        }

        if (data) {
            options.body = isFormData ? data : JSON.stringify(data);
        }

        return new Promise((resolve, reject) => {
            fetch(url, options)
                .then(resp => {
                    if (resp.status === 401) {
                        location.href = "/login";
                    }
                    else {
                        this.handleApiResponse(resp, resolve, reject);
                    }
                })
                .catch(error => this.handleError(error, reject));
        });

    }

    // Wrapper methods for specific HTTP methods
    get(url) {
        return this.request(url, 'GET');
    }

    post(url, data, isFormData = false) {
        return this.request(url, 'POST', data, isFormData);
    }

    put(url, data, isFormData = false) {
        return this.request(url, 'PUT', data, isFormData);
    }

    patch(url, data, isFormData = false) {
        return this.request(url, 'PATCH', data, isFormData);
    }

    delete(url, data, isFormData = false) {
        return this.request(url, 'DELETE', data, isFormData);
    }

    // Common response handling logic
    handleApiResponse(response, resolve, reject) {
        response.json()
            .then(body => {
                if (!response.ok) {
                    if (body.errorMessage == "401") {
                        Swal.fire({
                            text: "There are changes made in the server that require you to login again.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Close",
                            customClass: { confirmButton: "btn btn-primary" },
                        }).then(() => { location.href = "/signout"; });

                        return;
                    }
                    else if (body.status == 401) {
                        reject(body.status)
                        return;
                    }
                    Swal.fire({
                        text: body?.errorMessage || "Sorry, looks like there are some errors detected, please try again.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Close",
                        customClass: { confirmButton: "btn btn-primary" },
                    }).then(() => reject(body));
                } 
                else {
                    if (body.permissions) {
                        resolve({ data: body.data, permissions: body.permissions });
                    }
                    else {
                        resolve(body.data);
                    }
                }
            })
            .catch(error => this.handleError(error, reject));
    }

    // Common error handling logic
    handleError(error, reject) {
        Swal.fire({
            text: "An unexpected error occurred. Please try again.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Close",
            customClass: { confirmButton: "btn btn-primary" },
        }).then(() => reject(error));
    }
}