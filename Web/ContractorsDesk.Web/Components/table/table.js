

var AddForm = (function () {
    return {
        init: function (tbl) {
            const tableid = tbl.tableId;
            const onsavecallback = tbl.onsavecallback;
            const formdata = tbl.formFieldsCopy;
            const apiUrl = tbl.apiUrl;
            const httpSvc = new httpService();
            const swal = new SwalUtil();
            const   t = document.getElementById(`kt_modal_add_${tableid}`),
                    e = t.querySelector(`#kt_modal_add_${tableid}_form`),
                    n = new bootstrap.Modal(t);

            var submitButton = `[data-kt-${tableid}-modal-action="submit"]`;
            const i = t.querySelector(submitButton);

            (() => {
                var closeButtonId = `[data-kt-${tableid}-modal-action="close"]`;
                var cancelButton = `[data-kt-${tableid}-modal-action="cancel"]`;

                const onHideModal = (n, t)=>{
                    n.hide();
                    t.isConfirmed;
                    $(`#kt_modal_add_${tableid}`).remove();
                    $(".modal-backdrop.fade.show").remove();
                }

                const onPost = (payload) => {
                    httpSvc.post(apiUrl, payload)
                        .then((data) => {
                            swal.alert("Form has been successfully submitted!", function () {
                                e.reset();
                                t.isConfirmed;
                                onHideModal(n, t);
                                tbl.formFieldsCopy = tbl.formFields;
                                onsavecallback(data), (onsavecallback)
                            })
                        })
                        .then(() => {
                            i.setAttribute("data-kt-indicator", "on"), (i.disabled = !0);
                            i.removeAttribute("data-kt-indicator"), (i.disabled = !1);
                        });
                }
                const onPut = (payload) => {
                    httpSvc.put(apiUrl, payload)
                        .then((data) => {
                            swal.alert("Form has been successfully submitted!", function () {
                                e.reset();
                                t.isConfirmed;
                                onHideModal(n, t);
                                tbl.formFieldsCopy = tbl.formFields;
                                onsavecallback(data), (onsavecallback)
                            })
                        })
                        .then(() => {
                            i.setAttribute("data-kt-indicator", "on"), (i.disabled = !0);
                            i.removeAttribute("data-kt-indicator"), (i.disabled = !1);
                        });
                }
                const onCancel = (e, n, t) => {
                    swal.confirm("Are you sure you would like to cancel?",
                        function () {
                            e.reset();
                            onHideModal(n,t);
                        },
                        function () {
                            t.dismiss
                        }
                    );
                }
                const onSave = (i, e, n, t) => {
                    let f = formdata;
                    const payload = tbl.formMode == "ADD" ? f.reduce((acc, field) => {
                        acc[field.data] = field.value; // Set the data property as key and value as its value
                        return acc;
                    }, {}) : tbl.currentDataCopy;

                    if (tbl.formMode === "ADD")
                        onPost(payload);
                    else
                        onPut(payload);
                    
                }
                var o = FormValidation.formValidation(e, {
                    fields: {
                        //description: {
                        //    validators: {
                        //        notEmpty: { message: "Full name is required" }
                        //    }
                        //},
                        //user_email: {
                        //    validators: {
                        //        notEmpty: { message: "Valid email address is required" }
                        //    }
                        //}
                    },
                    plugins: { trigger: new FormValidation.plugins.Trigger(), bootstrap: new FormValidation.plugins.Bootstrap5({ rowSelector: ".fv-row", eleInvalidClass: "", eleValidClass: "" }) },
                });

                i.addEventListener("click", (t) => {
                    t.preventDefault(),
                        o &&
                    o.validate().then(function (t) {
                        debugger;
                            if ("Valid" == t) onSave(i, e, n, t);
                        });
                }),
                t.querySelector(cancelButton).addEventListener("click", (t) => {
                    t.preventDefault(), onCancel(e, n, t);
                }),
                t.querySelector(closeButtonId).addEventListener("click", (t) => {
                    t.preventDefault(), onCancel(e, n, t);
                });
            })();
        },
    };
})();
class Table {
    constructor(tableDef) {
        this.header = tableDef.header ?? true;
        this.recordName = tableDef.recordName ?? "";
        this.label = tableDef.label ?? "";
        this.dataSet = tableDef.dataSet ?? [];
        this.colDefs = tableDef.colDefs ?? [];
        this.filters = tableDef.filters ?? [];
        this.searchbar = tableDef.searchbar ?? false;
        this.searchLabel = tableDef.searchLabel ? `Search ${tableDef.recordName}` : "Search";
        this.filter = tableDef.filter ?? false;
        this.addButton = tableDef.addButton ?? false;
        this.selectedFilters = [];
        this.onApplyFilter = tableDef.onApplyFilter ?? undefined;
        this.tableId = '';
        this.formFields = tableDef.formFields ?? [];
        this.formFieldsCopy = tableDef.formFields ? structuredClone(tableDef.formFields) : [];
        this.currentData = null;
        this.currentDataCopy = null;
        this.onsavecallback = tableDef.onsavecallback ?? undefined;
        this.apiUrl = tableDef.apiUrl ?? '';
        this.cellSize = tableDef.cellSize ?? 'large';
        this.pageSize = tableDef.pageSize ? tableDef.pageSize : this.dataSet.length;
        this.actions = tableDef.actions ?? null;
        this.onEdit = tableDef.onEdit ?? null;
        this.onDelete = tableDef.onDelete ?? null;
        this.currentPage = 1;
        this.formMode = 'ADD';
        this.swal = new SwalUtil();
        this.httpSvc = new httpService();
    }

    #applyFilter() {
        // CHANGE LOGIC: create a  filtered dataset and call buildTable method
       

        const className = this.selectedFilters[0].value;
        const table = document.getElementById('table-' + this.tableId); // Get the table element
        const rows = table.querySelectorAll('tr.datarow'); // Select all table rows

        // Loop through all rows and show/hide based on the specified class
        rows.forEach(row => {
            if (row.classList.contains(className)) {
                row.style.display = ''; // Show the row if it contains the class
            } else {
                row.style.display = 'none'; // Hide the row if it does not contain the class
            }
        });
    } 
    #generateTableId() {
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        const charactersLength = characters.length;
        let result = '';

        for (let i = 0; i < 22; i++) {
            const randomIndex = Math.floor(Math.random() * charactersLength);
            result += characters.charAt(randomIndex);
        }

        return result;
    }
    #hasFilters = () => this.filters.length > 0;
    #selectFilter(sender) {
     
        var filterControl = $(sender.currentTarget);
        var filterName = filterControl.attr("data-filter-label");
        this.selectedFilters = this.selectedFilters.filter(item => item['name'] !== filterName);
        this.selectedFilters.push({ name: filterName, value: filterControl.val() });
    }
    #buildTitle() {
        return this.searchbar ? 
             `<div class="d-flex align-items-center position-relative my-1">
                <!--begin::Svg Icon | path: icons/duotune/general/gen021.svg-->
                <span class="svg-icon svg-icon-1 position-absolute ms-6">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
                        <rect opacity="0.5" x="17.0365" y="15.1223" width="8.15546" height="2" rx="1" transform="rotate(45 17.0365 15.1223)" fill="black" />
                        <path d="M11 19C6.55556 19 3 15.4444 3 11C3 6.55556 6.55556 3 11 3C15.4444 3 19 6.55556 19 11C19 15.4444 15.4444 19 11 19ZM11 5C7.53333 5 5 7.53333 5 11C5 14.4667 7.53333 17 11 17C14.4667 17 17 14.4667 17 11C17 7.53333 14.4667 5 11 5Z" fill="black" />
                    </svg>
                </span>
                <!--end::Svg Icon-->
                <input type="text" data-kt-user-table-filter="search" class="form-control form-control-solid w-250px ps-14" placeholder="${this.searchLabel}" />
            </div>` : '';
    }
    #buildFilterOptions() {
        // Check if filters and dataSet are not empty
        if (this.filters.length === 0 || this.dataSet.length === 0) {
            return ''; 
        }

        // Create filters using map and filter
        const filters = this.filters
            .map(({ label: filterName, dataCol }) => {
                if (!dataCol) return null; // Skip if dataCol is not defined

                // Create a Set to filter unique values and convert it to an array
                const uniqueValues = [...new Set(this.dataSet.map(item => item[dataCol]))];

                // Create options array using map
                const options = uniqueValues.map(value => ({ value, text: value }));

                return { label: filterName, options }; // Return the filter object
            })
            .filter(Boolean); // Filter out any null values

        // Generate HTML for each filter and return the result
        return filters.map(filter => `
                            <div class="mb-10">
                                <label class="form-label fs-6 fw-bold">${filter.label}:</label>
                                <select class="form-select form-select-solid fw-bolder filter-${this.tableId}" data-filter-label="${filter.label}" data-placeholder="Select option" data-allow-clear="true" data-kt-user-table-filter="role" data-hide-search="true">
                                    <option></option>
                                    ${filter.options.map(option => `
                                        <option value="${option.value}">${option.text}</option>
                                    `).join('')}
                                </select>
                            </div>
                        `).join('');
    }
    #buildFilter() {
        return this.#hasFilters() ? `
                        <button type="button" class="btn btn-light-primary me-3" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                            <!--begin::Svg Icon | path: icons/duotune/general/gen031.svg-->
                            <span class="svg-icon svg-icon-2">
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
                                    <path d="M19.0759 3H4.72777C3.95892 3 3.47768 3.83148 3.86067 4.49814L8.56967 12.6949C9.17923 13.7559 9.5 14.9582 9.5 16.1819V19.5072C9.5 20.2189 10.2223 20.7028 10.8805 20.432L13.8805 19.1977C14.2553 19.0435 14.5 18.6783 14.5 18.273V13.8372C14.5 12.8089 14.8171 11.8056 15.408 10.964L19.8943 4.57465C20.3596 3.912 19.8856 3 19.0759 3Z" fill="black" />
                                </svg>
                            </span>
                            <!--end::Svg Icon-->Filter
                        </button>
                        <!--begin::Menu 1-->
                        <div class="menu menu-sub menu-sub-dropdown w-300px w-md-325px" data-kt-menu="true">
                            <!--begin::Header-->
                            <div class="px-7 py-5">
                                <div class="fs-5 text-dark fw-bolder">Filter Options</div>
                            </div>
                            <!--end::Header-->
                            <!--begin::Separator-->
                            <div class="separator border-gray-200"></div>
                            <!--end::Separator-->
                            <!--begin::Content-->
                            <div class="px-7 py-5" data-kt-user-table-filter="form">
                                ${this.#buildFilterOptions()}
                                <div class="d-flex justify-content-end">
                                    <button type="reset" class="btn btn-light btn-active-light-primary fw-bold me-2 px-6" data-kt-menu-dismiss="true" data-kt-user-table-filter="reset">Reset</button>
                                    <button type="button" class="btn btn-primary fw-bold px-6 filter-${this.tableId}" data-kt-menu-dismiss="true" data-kt-user-table-filter="filter">Apply</button>
                                </div>
                                <!--end::Actions-->
                            </div>
                            <!--end::Content-->
                        </div>` : '';
    }
    #buildAddButton() {
        return this.addButton ? `<!--begin::Add user-->
                        <button type="button" class="btn btn-primary btn-add-record" >
                            <!--begin::Svg Icon | path: icons/duotune/arrows/arr075.svg-->
                            <span class="svg-icon svg-icon-2">
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
                                    <rect opacity="0.5" x="11.364" y="20.364" width="16" height="2" rx="1" transform="rotate(-90 11.364 20.364)" fill="black" />
                                    <rect x="4.36396" y="11.364" width="16" height="2" rx="1" fill="black" />
                                </svg>
                            </span>
                            <!--end::Svg Icon-->Add ${this.recordName}
                        </button>
                        <!--end::Add user-->` : '';
    }
    #buildToolbar() {
        return`
            <div class="d-flex justify-content-end" data-kt-user-table-toolbar="base">
                ${this.#buildFilter()}
                ${this.#buildAddButton()}
            </div>`
    }
    #buildColumnHeaders() {
        return `
              <thead>
                <tr class="text-start text-muted fw-bolder fs-7 text-uppercase gs-0">
                  ${this.colDefs.map(colDef => `<th class="${colDef.cellStyle} ${colDef.sortable ? `sorting sorting_asc` : ``}" data-sort="${colDef.data}">${colDef.label}</th>`).join('')}
                  ${this.actions ? `<th class="text-center">Actions</th>` : ''}
                </tr>
              </thead>
            `;
    }
    #buildHeader() {
        debugger;
        return (this.searchbar || this.addButton || this.#hasFilters()) ?
                `<div class="card-header border-0 pt-6 mb-5">
                    <div class="card-title">
                        ${this.label}
                        ${this.#buildTitle()}
                    </div>
                    <div class="card-toolbar">
                        ${this.#buildToolbar()}
                    </div>
                </div>` : '';
    }
    #buildActions() {
        return `<td class="text-center">
                    <a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                        Actions
                        <i class="fas fa-chevron-down"></i>
                    </a>
                    <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-bold fs-7 w-125px py-4" data-kt-menu="true">
                        ${this.actions.map(action => `
                        <div class="menu-item px-3">
                            <a href="javascript:" class="menu-link px-3 table-row-action" data-action="${action}">${action}</a>
                        </div>`).join('')}
                    </div>
                </td>
            `;
    }
    #buildRows(page) {
        return this.dataSet[page].map(item => {
            // Initialize filterClass for the current row
            const filterClass = this.filters
                .map(filter => {
                    const dataCol = filter.dataCol ?? undefined;
                    return dataCol ? item[dataCol] : ''; // If dataCol is defined, return its value; otherwise, return an empty string
                })
                .join(' '); // Join all filter values with a space

            // Generate the HTML for the row
            let rowHtml = this.colDefs.map(col => {
                const tdStyle = col.tdStyle ? col.tdStyle : ''; // Get the td style if it exists
                // Determine the content for the table cell
                let dataHtml = col.html ? this.#replaceProperties(col.html, item) : item[col.data];
                dataHtml = col.onrenderHtml ? col.onrenderHtml(dataHtml, item) : dataHtml;

                // Return the table cell HTML
                let cell = `<td class="${tdStyle}">${dataHtml}</td>`;
                return cell;
            }).join(''); // Join all cells to form a row

            if (this.actions != null) {
                rowHtml += this.#buildActions();
            }

            // Return the complete row HTML
            return `<tr class="datarow ${filterClass}" data-row-id="${item.id}">${rowHtml}</tr>`;
        }).join(''); // Join all rows to form the complete table HTML
        
    }
    #buildTable() {
        const cellSizeClass = this.cellSize == 'small' ? 'gy-2' : 'gy-5';
        return `
            <table class="table align-middle table-row-dashed fs-6 ${cellSizeClass} dataTable" id="table-${this.tableId}">
                ${this.#buildColumnHeaders()}
                <tbody class="text-gray-600 fw-bold" id="tbody-${this.tableId}">
                    ${this.#buildRows(0)}
                </tbody>
            </table>
            `;
    }
    #buildFormFields(edit) {
        const formFields = edit ? this.formFields.filter(f => f.edit === true) : this.formFields;

        const formFieldHtml = formFields.map(formField => {
            const type = formField.type ?? "text";
            const required = formField.required ? "required" : "";
            const value = edit ? this.currentDataCopy[formField.data] : "";

            let fieldHtml = `<div class="fv-row mb-7 fv-plugins-icon-container">
                                <label class="${required} fw-bold fs-6 mb-2">${formField.label}</label>`;

            if (type === "text" || type === "password") {
                if (type === "password") {
                    fieldHtml += `<div class="position-relative mb-3">
									<input type="${type}" name="${formField.data}" data-model="${formField.data}" placeholder="Enter ${formField.label}" class="form-control form-control-solid mb-3 mb-lg-0" value="${value}" autocomplete="off" />
									<span class="btn btn-sm btn-icon position-absolute translate-middle top-50 end-0 me-n2 show-password">
										<i class="bi bi-eye-slash fs-2"></i>
										<i class="bi bi-eye fs-2 d-none"></i>
									</span>
								</div>`;
                }
                else {
                    fieldHtml += `<input type="${type}" name="${formField.data}" data-model="${formField.data}" placeholder="Enter ${formField.label}" class="form-control form-control-solid mb-3 mb-lg-0" value="${value}" autocomplete="off">`;
                }
            } else if (type === "select") {
                fieldHtml += `<select class="form-select" data-model="${formField.data}" aria-label="Select example" name="${formField.data}" value="${value}">
                                <option value="0">Select ${formField.label}</option>`;

                if (formField.options) {
                    debugger;
                    fieldHtml += formField.options.map(option => `
                                <option value="${option.id}" ${value == option.id ? `selected` : ``}>${option.text}</option>`).join('');
                }

                fieldHtml += `</select>`;
            }

                fieldHtml += `
                            <div class="fv-plugins-message-container invalid-feedback"></div>
                        </div>`;

            return fieldHtml;
        }).join('');

        return formFieldHtml;
    }
    #buildPagination(pageCount, currentPage) {
        if (pageCount === 1) return '';

        const pagination = `
            <div class="col-sm-12 d-flex align-items-center justify-content-center mt-5">
                <div class="dataTables_paginate paging_simple_numbers" id="kt_inbox_listing_paginate">
                    <ul class="pagination pagination-circle">
                        ${[...Array(pageCount)].map((_, i) => {
                    const pageNo = i + 1;
                    const active = currentPage === pageNo ? 'active' : '';
                    return `<li class="paginate_button ${this.tableId}-pager page-item ${active}">
                                <a href="javascript:" data-dt-idx="${pageNo}" tabindex="0" class="${this.tableId}-pager page-link">${pageNo}</a>
                            </li>`;
                }).join('')}
                    </ul>
                </div>
            </div>
        `;

        return pagination;
    }
    #replaceProperties(str, obj) {
        // Use a regular expression to find all instances of {property}
        return str.replace(/\{(\w+)\}/g, (match, key) => {
            // Check if the object has the key and return its value, otherwise return the original match
            return obj.hasOwnProperty(key) ? obj[key] : match;
        });
    }
    #showModalForm(edit = false) {
        this.formMode = edit ? 'EDIT' : 'ADD';
        const addModalHtml = this.buildModalForm(edit);
        $(`#dv-table-container-${this.tableId}`).append(addModalHtml);
        AddForm.init(this);
        this.initFormEventHandlers();
        $(`#kt_modal_add_${this.tableId}`).modal('show');
    }
    #onFormEdit(id) {
        this.currentData = this.dataSet.flat().find(i => i.id == id);
        this.currentDataCopy = { ...this.currentData };
        this.#showModalForm(true);
    }
    #onFormDelete(id) {
        this.currentData = this.dataSet.flat().find(i => i.id == id);
        const currentDataCopy = { ...this.currentData };
        this.swal.confirm("Would you like to delete this record?",
            () => {
                const onsuccess = (data) => {
                    this.onsavecallback(data), (this.onsavecallback);
                }
                const onbegin = (data) => { }
                const oncomplete = (data) => { }
                this.httpSvc.delete(this.apiUrl, currentDataCopy, onsuccess, onbegin, oncomplete);
            }
        );
    }
    #initEventHandlers() {
        const instance = this;
        const tableId = instance.tableId;
        $("a." + tableId + '-pager').off('click').on('click', function () {
            var pageNo = $(this).attr('data-dt-idx');
            const sender = $(this);
            instance.setPage(parseInt(pageNo), sender);
        });

        $("button.btn-add-record").off("click").on("click", () => {
            this.#showModalForm();
        });

        $("a.table-row-action").off('click').on("click", function () {
            const row = $(this).parents('tr')[0];
            const action = $(this).attr('data-action');
            const id = $(row).attr("data-row-id");
            debugger;
            switch (action) {
                case "Edit":
                    if (instance.onEdit) {
                        instance.onEdit(id);
                    }
                    else {
                        instance.#onFormEdit(id);
                    }
                    break;
                case "Delete":
                    instance.#onFormDelete(id);
                    break;
            }
        });

        $("th.sorting").off("click").on("click", function () {
          
            const cell = $(this);
            const sortData = cell.attr("data-sort");
          
            const allData = instance.dataSet.flat();
            const sortedData = cell.hasClass('sorting_asc') ? allData.sort((a, b) => a[sortData].localeCompare(b[sortData]))
                : allData.sort((a, b) => b[sortData].localeCompare(a[sortData]));

            instance.dataSet = sortedData.chunk(instance.pageSize);
            let pageRows = instance.#buildRows(0);

            $(`#tbody-${instance.tableId}`).html(pageRows);

            cell.toggleClass('sorting_asc sorting_desc');
            KTMenu.init();
            KTMenu.initGlobalHandlers();
            instance.#initEventHandlers();
        });

        $("html").on("click", ".show-password", function () {
            $(this).find("i").toggleClass('d-none');
            const pwField = $(this).prev();
            pwField.attr("type", (pwField.attr("type") == "text" ? "password" : "text"));
        });

    }
    initFormEventHandlers() {
        const instance = this;
        $(`#kt_modal_add_${this.tableId}_form`).find('input[type="text"],input[type="password"]').on('keyup', function () {
           
            const formFieldData = $(this).attr("data-model");

            if (instance.formMode == "ADD") {
                const formField = instance.formFieldsCopy.find(item => item.data === formFieldData);
                formField.value = $(this).val();
            }
            else {
                instance.currentDataCopy[formFieldData] = $(this).val();
            }

        });
        $(`#kt_modal_add_${this.tableId}_form`).find('select').on('change', function () {
            const formFieldData = $(this).attr("data-model");

            if (instance.formMode == "ADD") {
                const formField = instance.formFieldsCopy.find(item => item.data === formFieldData);
                formField.value = $(this).val();
            }
            else {
                instance.currentDataCopy[formFieldData] = $(this).val();
            }
        });
    }
    buildModalForm(edit = false) {
        const modalMode = edit ? 'Edit' : 'Add';

        if (this.formFields.length == 0) {
            throw new Error('Table Definition: formFields parameter is required.');
        }
        return `<div class="modal fade" id="kt_modal_add_${this.tableId}" data-bs-backdrop="static" tabindex="-1" role="dialog">
				    <!--begin::Modal dialog-->
				    <div class="modal-dialog modal-dialog-centered mw-650px">
					    <!--begin::Modal content-->
					    <div class="modal-content">
						    <!--begin::Modal header-->
						    <div class="modal-header" id="kt_modal_add_user_header">
							    <!--begin::Modal title-->
							    <h2 class="fw-bolder">${modalMode} ${this.recordName}</h2>
							    <!--end::Modal title-->
							    <!--begin::Close-->
							    <div class="btn btn-icon btn-sm btn-active-icon-primary" data-kt-${this.tableId}-modal-action="close">
								    <!--begin::Svg Icon | path: icons/duotune/arrows/arr061.svg-->
								    <span class="svg-icon svg-icon-1">
									    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
										    <rect opacity="0.5" x="6" y="17.3137" width="16" height="2" rx="1" transform="rotate(-45 6 17.3137)" fill="black"></rect>
										    <rect x="7.41422" y="6" width="16" height="2" rx="1" transform="rotate(45 7.41422 6)" fill="black"></rect>
									    </svg>
								    </span>
								    <!--end::Svg Icon-->
							    </div>
							    <!--end::Close-->
						    </div>
						    <!--end::Modal header-->
						    <!--begin::Modal body-->
						    <div class="modal-body">
							    <!--begin::Form-->
							    <form id="kt_modal_add_${this.tableId}_form" class="form fv-plugins-bootstrap5 fv-plugins-framework" action="#">
								    <!--begin::Scroll-->
                                    ${this.#buildFormFields(edit)}
								    <!--end::Scroll-->
								    <!--begin::Actions-->
								    <div class="text-center pt-10">
									    <button type="reset" class="btn btn-light me-3" data-kt-${this.tableId}-modal-action="cancel">Discard</button>
									    <button type="submit" class="btn btn-primary" data-kt-${this.tableId}-modal-action="submit">
										    <span class="indicator-label">Submit</span>
										    <span class="indicator-progress">Please wait...
										    <span class="spinner-border spinner-border-sm align-middle ms-2"></span></span>
									    </button>
								    </div>
								    <!--end::Actions-->
							    <div></div></form>
							    <!--end::Form-->
						    </div>
						    <!--end::Modal body-->
					    </div>
					    <!--end::Modal content-->
				    </div>
				    <!--end::Modal dialog-->
			    </div>`;
    }
    setPage(pageNo, sender) {
        let pageRows = this.#buildRows(pageNo - 1);
        $(`#tbody-${this.tableId}`).html(pageRows);
        $(`li.${this.tableId}-pager`).removeClass('active');
        sender.parent().addClass('active');
        this.currentPage = pageNo;

        KTMenu.init();
        KTMenu.initGlobalHandlers();
        this.#initEventHandlers();
    }
    render(container) {
        this.tableId = this.#generateTableId();
        this.dataSet = this.dataSet.chunk(this.pageSize);

        const pageCount = this.dataSet.length;
        const tableId = this.tableId;


        var content = `<div class="card card-xl-stretch" id="dv-table-container-${this.tableId}">
                            ${this.#buildHeader()}
                            <div class="card-body pt-0 card-xl-stretch">
                                ${this.#buildTable()}
                                ${this.#buildPagination(pageCount, 1)}
                            </div>
                        </div>`;
        //if (this.addButton) {
        //    content += this.#buildAddModalForm();
        //}

        $(container).html(content);
       
        //filter selection eventhandler
        $("select.filter-" + tableId).on("change", (sender) => { this.#selectFilter(sender); });
        //filter apply button eventhandler
        $("button.filter-" + tableId).on("click", () => {
            if (this.onApplyFilter)
                this.onApplyFilter(this.selectedFilters, this);
            else
                this.#applyFilter();
        });

        this.#initEventHandlers();

        KTMenu.init();
        KTMenu.initGlobalHandlers();
    }
}

