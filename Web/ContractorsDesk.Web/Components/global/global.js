
const DateUtils = {
    toStandardDate: (dateString) => {
        if (dateString == null) return ``;

        //const apiDate = "2025-09-29";
        const date = new Date(dateString);

        // Override year and day
        //date.setFullYear(2029);
        //date.setDate(9); // sets day to 09

        const formatter = new Intl.DateTimeFormat('en-US', {
            month: 'short',
            day: '2-digit',
            year: 'numeric'
        });

        return formatter.format(date); // → "September 09, 2029"
    },
    formatDate : (dateString, format = '') => {
        const date = new Date(dateString);
        if (!isNaN(date.getTime())) {
            const formattedDate = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
            const [year, month, day] = formattedDate.split('-');

            if (format == '') {
                return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
            }
            else {
                return `${month.padStart(2, '0')}/${day.padStart(2, '0') }/${year}`;
            }
        } else {
            return '';
        }
    },
    toAspNetDate : (dateString) => {
        const dateParts = dateString.split('/');
        return `${dateParts[2]}-${dateParts[0]}-${dateParts[1]}`;
    },
    fromAspNetDate: (dateString) => {
        if (dateString == null) return '';

        const dateParts = dateString.split('T')[0].split('-');
        return `${dateParts[1]}/${dateParts[2]}/${dateParts[0]}`;
    },
    formatDateTime: (dateString) => {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return '';

        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');

        let hours = date.getHours();
        const minutes = date.getMinutes().toString().padStart(2, '0');
        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12 || 12;
        hours = hours.toString().padStart(2, '0');

        return `${year}-${month}-${day} ${hours}:${minutes} ${ampm}`;
    },
    formatDateWithTimeDifference: (dateString) => {
        //dateString -> "2025-07-30T06:06:20.1099536"
        const inputDate = new Date(dateString);
        const now = new Date();
        const nowInCA = new Date(
            now.toLocaleString('en-US', { timeZone: 'America/Los_Angeles' })
        );

        const diffMs = nowInCA - inputDate;
        const diffSeconds = diffMs / 1000;
        const diffMinutes = diffSeconds / 60;
        const diffHours = diffMinutes / 60;

        if (diffMinutes < 60) {
            const rounded = Math.floor(diffMinutes);
            return `${rounded < 1 ? 'less than a' : rounded} minute${rounded > 1 ? 's' : ''} ago`;
        }

        if (diffHours < 24) {
            const rounded = Math.floor(diffHours);
            return `${rounded < 1 ? 'less than an' : rounded} hour${rounded > 1 ? 's' : ''} ago`;
        }

        // Format as "Sep 13, 2025 • 11:34 PM"
        const formatter = new Intl.DateTimeFormat('en-US', {
            timeZone: 'America/Los_Angeles',
            month: 'short',
            day: 'numeric',
            year: 'numeric',
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        });

        const formatted = formatter.format(inputDate);
        const parts = formatted.split(', ');
        return `${parts[0]}, ${parts[1]} • ${parts[2]}`;
    },
    getDateDurationFromAspNetDate: (apiDateStr) => {
        const inputDate = new Date(apiDateStr);
        const now = new Date();

        const diffMs = now - inputDate;
        const diffSeconds = diffMs / 1000;
        const diffMinutes = diffSeconds / 60;
        const diffHours = diffMinutes / 60;
        const diffDays = diffHours / 24;

        if (diffSeconds < 60) {
            const rounded = Math.floor(diffSeconds);
            return `${rounded === 0 ? '<1' : rounded} second${rounded !== 1 ? 's' : ''} ago`;
        }

        if (diffMinutes < 60) {
            const rounded = Math.floor(diffMinutes);
            return `${rounded} minute${rounded !== 1 ? 's' : ''} ago`;
        }

        if (diffHours < 24) {
            const rounded = Math.floor(diffHours);
            return `${rounded} hour${rounded !== 1 ? 's' : ''} ago`;
        }

        // Format as "Oct 16, 2025 5:00 PM"
        const formatter = new Intl.DateTimeFormat('en-US', {
            month: 'short',
            day: '2-digit',
            year: 'numeric',
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        });

        return formatter.format(inputDate);

    }
}

const StringUtils = {
    formatMoney: (value, addSymbol = true) => {
        const hasDecimals = value % 1 !== 0; // Check if there are decimal places
        if (addSymbol) {
            return new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD',
                minimumFractionDigits: hasDecimals ? 2 : 0,
                maximumFractionDigits: hasDecimals ? 2 : 0
            }).format(value);
        }
        else {
            return new Intl.NumberFormat('en-US', {
                minimumFractionDigits: hasDecimals ? 2 : 0,
                maximumFractionDigits: hasDecimals ? 2 : 0
            }).format(value);
        }
    }
}

const TextboxUtils = {
    handleMoneyFormat: () => {
        $('input[data-type="money"]').on('keyup', function (e) {
            let rawValue = $(this).val().replace(/,/g, '');

            // Split into integer and decimal parts
            const [integerPart, decimalPart] = rawValue.split('.');

            // Format the integer part with commas
            const formattedInteger = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

            // Recombine with decimal if present
            const formattedValue = decimalPart !== undefined
                ? `${formattedInteger}.${decimalPart}`
                : formattedInteger;

            $(this).val(formattedValue);

        });

        $('input[data-type="money"]').on('keypress', function (e) {
            const charCode = (e.which) ? e.which : e.keyCode;
            const charStr = String.fromCharCode(charCode);

            // Allow "-" character if data-allow-negative is true and it is the first character
            const allowNegative = $(this).data('allow-negative') || $(this).hasClass('allow-negative');
            if (allowNegative && e.key === '-' && $(this).val().length === 0) {
                return;
            }

            // Allow only numbers, '.', and control keys (like backspace)
            if (!charStr.match(/[0-9.]/) || (charStr === '.' && $(this).val().includes('.'))) {
                e.preventDefault();
            }
        });

    },
    handleIntFormat: () => {
        $('input[data-type="int"]').on('keydown', function (e) {
            
            const allowNegative = $(this).data('allow-negative');
            if (allowNegative && e.key === '-' && $(this).val().length === 0) {
                return;
            }

            // Allow backspace, delete, tab, escape, enter, and arrow keys
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 37, 38, 39, 40]) !== -1 ||
                // Allow Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
                (e.keyCode === 65 && e.ctrlKey === true) ||
                (e.keyCode === 67 && e.ctrlKey === true) ||
                (e.keyCode === 86 && e.ctrlKey === true) ||
                (e.keyCode === 88 && e.ctrlKey === true) ||
                // Allow home, end, and other navigational keys
                (e.keyCode >= 35 && e.keyCode <= 39)) {
                return;
            }

            // Ensure it's a number and prevent if otherwise
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }

            
        });
    },
    handleDecimalFormat: () => {
        $('input[data-type="decimal"]').on('keypress', function (e) {
            const charCode = (e.which) ? e.which : e.keyCode;
            const charStr = String.fromCharCode(charCode);
            const currentValue = $(this).val();

            // Allow backspace, delete, and other control keys
            if (e.ctrlKey || e.metaKey || charCode === 8 || charCode === 46) {
                return true;
            }

            // Allow only numbers and one decimal point
            if (!charStr.match(/[0-9.]/) || (charStr === '.' && currentValue.includes('.'))) {
                e.preventDefault();
            }
        });

        $('input[data-type="decimal"]').on('input', function () {
            const value = $(this).val();

            // Remove any invalid characters and allow only one decimal point
            const sanitizedValue = value
                .replace(/[^0-9.]/g, '') // Remove all non-numeric characters except '.'
                .replace(/(\..*?)\..*/g, '$1'); // Ensure only one decimal point

            $(this).val(sanitizedValue);
        });

    },
    getFloatValue: (strval) => {
        const value = strval.replace(/,/g, '');
        const floatValue = parseFloat(value);
        return isNaN(floatValue) ? 0 : floatValue;
    },
    init: () => {
        TextboxUtils.handleMoneyFormat();
        TextboxUtils.handleIntFormat();
        TextboxUtils.handleDecimalFormat();
    }
}

const Guid = {
    empty: '00000000-0000-0000-0000-000000000000',
    new: () => {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (char) {
            const random = Math.random() * 16 | 0;
            const value = char === 'x' ? random : (random & 0x3 | 0x8);
            return value.toString(16);
        });
    }
}

const GenerateId = (length = 10)=> {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
}

const toCamelCaseKeys = (obj) => {
    if (Array.isArray(obj)) {
        return obj.map(toCamelCaseKeys);
    } else if (obj !== null && typeof obj === 'object') {
        return Object.fromEntries(
            Object.entries(obj).map(([key, value]) => [
                key.charAt(0).toLowerCase() + key.slice(1),
                toCamelCaseKeys(value)
            ])
        );
    }
    return obj;
}

const Auth = {
    currentUser: () => {
        var currentUser = localStorage.getItem('user');
        return JSON.parse(currentUser);
    },
    permissions: () => {
        var permissions = localStorage.getItem('permissions');
        return JSON.parse(permissions);
    },
    getCompanySettings: ()=> {
        var companySettings = localStorage.getItem('companySettings');
        const parsed = JSON.parse(companySettings);
        return toCamelCaseKeys(parsed);
    }
}

const printFile = async (response) => {
    const blob = await response.blob();
    // Create a URL for the Blob
    const url = URL.createObjectURL(blob);
    
    const printWindow = window.open(url);

    // Trigger the print dialog when the content is loaded
    printWindow.addEventListener('load', () => {
    	printWindow.print();
    });
}

const downloadFile = async (response)=>{
    // Get the filename from the `Content-Disposition` header
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
}

const signalRConnection = new signalR.HubConnectionBuilder()
    .withUrl("/contractorDeskHub", {
        accessTokenFactory: () => localStorage.getItem("authToken")
    })
    .configureLogging(signalR.LogLevel.None)
    .build();

signalRConnection.start()
    .then(function () {
        signalRConnection.invoke("PageLoaded");
    })
    .catch(err => console.error("SignalR Error: " + err));

const SETBREADCRUMB = (id, pageName, active = false) => {
    $("#header-breadcrumbs").removeClass("d-none");
    const breadcrumbItem = $("#header-breadcrumbs").find(`a[data-page-name="${pageName}"]`);
    const url = breadcrumbItem.attr('data-nav-url');
    breadcrumbItem.attr('href', url.replace('{id}', id)).removeClass("disabled").removeAttr("disabled");

    if (active) {
        breadcrumbItem.addClass("text-primary").removeClass('text-muted');
    }
    else {
        breadcrumbItem.addClass("text-dark").removeClass('text-muted');
    }
}

const SCROLLTOTOP = () => {
    window.scrollTo({
        top: 0,
        behavior: 'smooth' // optional: adds smooth animation
    });
}

const VALIDATIONS = {
    isValidUSPhoneNumber: (input) => {
        const regex = /^(\+1\s?)?(\(?\d{3}\)?[\s.-]?)?\d{3}[\s.-]?\d{4}$/;
        return regex.test(input.trim());
    },
    isValidEmail: (email)=> {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email.trim());
    }
}
String.prototype.padLeft = function (length, char) {
    const str = this.toString();
    return str.length >= length ? str : char.repeat(length - str.length) + str;
};

