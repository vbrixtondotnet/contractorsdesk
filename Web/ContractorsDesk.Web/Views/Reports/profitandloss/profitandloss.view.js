

class ProfitAndLossReportView extends DomEventComponent {
	constructor() {
		super();
		const parts = document.URL.split('/');
		this.service = new ProfitAndLossReportService();
		this.swal = new SwalUtil();
        this.printer = new printer();
        this.tableWrapper = document.getElementById('report-table-wrapper'); // Get the existing wrapper div
        this.sectionCounter = 0; // Initialize section ID counter


	}
	onSelectProject(e) {
		console.log(e);
	}

	#buildStickyHeader() {
		const options = {
			selector: ".sticky-header",
			top: '74px',
			showOnScrollTop: 240
		}
		const stickyHeader = new StickyHeader(options);
		stickyHeader.init();
	}

	#renderReport(listIds) {
		$('.report-container').addClass('loading').removeClass('loaded');
		this.service.loadReport(listIds)
            .then((response) => {
                this.handleGenerateReport(response);
			});
	}

	#initEventHandlers() {
        const instance = this;
        

        // Initial check for placeholder visibility (if page loads with empty textarea)
        this.updatePlaceholderVisibility();

		$('#slcActiveJobs').select2();

		// Capture the change event
		$('#slcActiveJobs').on('change', function (e) {
			// Get the selected value
			var selectedValue = $(this).val();
			var selectedIds = Array.isArray(selectedValue) ? selectedValue.join(',') : selectedValue.toString();
			instance.#renderReport(selectedIds);
		});
	}

    handleGenerateReport(jsonString) {

        this.clearReport(); // Clear the previous report
        this.sectionCounter = 0; // Reset counter for a new report

        try {
            const reportData = this.parseJsonInput(jsonString); // Call static method

            if (!this.validateReportStructure(reportData, this.reportBodyDiv)) { // Call static method
                return; // Stop processing if structure is invalid
            }

            // Build the table
            const table = this.buildReportTable(reportData.Columns.Column, reportData.Rows.Row);

            // Append the table to the *existing* wrapper
            this.tableWrapper.appendChild(table);

            // Attach event listeners after the table is in the DOM
            const tableElement = this.tableWrapper.querySelector('#report-table');
            if (tableElement) {
                this.attachExpandCollapseListeners(tableElement.querySelector('tbody'));
            } else {
                console.error("Report table not found inside table wrapper after building.");
                this.displayError("Internal error: Could not find the report table after creation.");
            }

        } catch (error) {
            console.error('JSON parsing or rendering error:', error);
            // Check if it's a validation error message we already displayed
            if (!this.reportBodyDiv.querySelector('p[style*="color: red"]')) {
                this.displayError(`Failed to process JSON data: ${error.message}`);
            }
        }

        // Update placeholder visibility
        this.updatePlaceholderVisibility();
    }

    clearReport() {
        // Clear the content of the existing table wrapper
        this.tableWrapper.innerHTML = '';
        //// Clear any previous error messages in reportBodyDiv
        //const existingError = this.reportBodyDiv.querySelector('p[style*="color: red"]');
        //if (existingError) {
        //    existingError.remove(); // Use remove() instead of clearing innerHTML
        //}
    }

    parseJsonInput(jsonString) {
        if (!jsonString.trim()) return null; // Indicate empty input
        try {
            return JSON.parse(jsonString);
        } catch (e) {
            throw new Error("Invalid JSON format. Please check your syntax.");
        }
    }

    validateReportStructure(reportData, reportBodyDiv) {
        let isValid = true;
        let message = '';

        if (reportData === null) { // Handle the case of empty trimmed input
            message = 'JSON input is empty.';
            isValid = false;
        } else if (!reportData || typeof reportData !== 'object') {
            message = 'Invalid JSON data: Root is not an object.';
            isValid = false;
        } else if (!reportData.Columns || typeof reportData.Columns !== 'object' || !reportData.Columns.Column || !Array.isArray(reportData.Columns.Column)) {
            message = 'Invalid JSON data structure: "Columns.Column" array not found or is incorrect.';
            isValid = false;
        } else if (!reportData.Rows || typeof reportData.Rows !== 'object' || !reportData.Rows.Row || !Array.isArray(reportData.Rows.Row)) {
            message = 'Invalid JSON data structure: "Rows.Row" array not found or is incorrect.';
            isValid = false;
        }

        if (!isValid) {
            console.warn("Validation failed:", message);
            this.displayError(message); // Display error using the instance method
        }
        return isValid;
    }

    // buildReportTable now only creates and returns the table element
    buildReportTable(columnsArray, mainRowsArray) {
        const table = document.createElement('table');
        table.id = 'report-table';

        const thead = this.createTableHeader(columnsArray);
        table.appendChild(thead);

        const tbody = document.createElement('tbody');
        const columnCount = columnsArray.length;

        // Store totals as an array for each section ID for this specific report generation
        let sectionTotals = {};

        this.renderRowsRecursive(mainRowsArray, columnsArray, tbody, null, null, 0, columnCount, sectionTotals);

        table.appendChild(tbody);

        return table; // Return only the table element
    }

    createTableHeader(columnsArray) {
        const thead = document.createElement('thead');
        thead.classList.add('sticky-header');
        const headerRow = document.createElement('tr');
        columnsArray.forEach(col => {
            const th = document.createElement('th');
            th.textContent = col.ColTitle || '';
            headerRow.appendChild(th);
        });
        thead.appendChild(headerRow);
        return thead;
    }

    // Recursive function to render rows/sections and calculate totals
    renderRowsRecursive(rowsArray, columnsDefinitions, tbodyElement, parentId, grandParentId, level, columnCount, sectionTotals) {
        if (!rowsArray || !Array.isArray(rowsArray)) {
            return;
        }

        rowsArray.forEach((rowData, index) => {
            if (!rowData || typeof rowData !== 'object') {
                console.warn(`renderRowsRecursive: Skipping invalid row data at index ${index}.`, rowData);
                return;
            }

            const hasNestedRows = rowData.Rows && rowData.Rows.Row && Array.isArray(rowData.Rows.Row) && rowData.Rows.Row.length > 0;
            const hasHeader = rowData.Header?.ColData?.length > 0 || rowData.group;
            const hasSummary = rowData.Summary && rowData.Summary.ColData && Array.isArray(rowData.Summary.ColData);

            const isExpandableGroupHeader = rowData.type === 'Section' && hasNestedRows && hasHeader;
            const isSectionWithSummary = rowData.type === 'Section' && hasSummary;
            const isDataRow = rowData.type === 'Data';

            const firstSummaryValue = isSectionWithSummary ? rowData.Summary.ColData?.[0]?.value : null;
            const useRawValueForSummary = firstSummaryValue === 'Gross Profit' ||
                firstSummaryValue === 'Net Operating Income' ||
                firstSummaryValue === 'Net Income' ||
                firstSummaryValue === 'Net Other Income' ||
                firstSummaryValue === 'Total Cost of Goods Sold' ||
                firstSummaryValue === 'Total Income';


            let thisSectionExpandableId = null;
            if (isExpandableGroupHeader) {
                thisSectionExpandableId = `section-${++this.sectionCounter}`; // Use instance property
                sectionTotals[thisSectionExpandableId] = new Array(columnCount).fill(0);
            }

            const idWhereChildrenSumInto = isExpandableGroupHeader ? thisSectionExpandableId : parentId;

            if (isExpandableGroupHeader) {
                const trHeader = this.createGroupHeaderRow(rowData, columnsDefinitions, level, parentId, thisSectionExpandableId, columnCount);
                tbodyElement.appendChild(trHeader);
            }

            if (hasNestedRows) {
                this.renderRowsRecursive(rowData.Rows.Row, columnsDefinitions, tbodyElement, idWhereChildrenSumInto, isExpandableGroupHeader ? thisSectionExpandableId : grandParentId, level + 1, columnCount, sectionTotals);
            }

            if (isDataRow) {
                if (rowData.ColData && Array.isArray(rowData.ColData)) {
                    const trData = this.createDataRow(rowData, columnsDefinitions, level, parentId, sectionTotals, columnCount);
                    tbodyElement.appendChild(trData);
                } else {
                    console.warn(`renderRowsRecursive: Skipping invalid data row at index ${index}: ColData missing or not array.`, rowData);
                }
            } else if (isSectionWithSummary) {
                const trSummary = this.createSummaryRow(rowData, columnsDefinitions, level, parentId, grandParentId, sectionTotals, columnCount);
                tbodyElement.appendChild(trSummary);
            } else if (rowData.type === 'Section' && !hasHeader && !hasNestedRows && !hasSummary) {
                // console.warn(`renderRowsRecursive: Skipping Section at index ${index} with no header, rows, or summary.`, rowData);
            }
        });
    }

    createGroupHeaderRow(rowData, columnsDefinitions, level, parentId, thisSectionExpandableId, columnCount) {
        const trHeader = document.createElement('tr');
        trHeader.classList.add('group-header-row', `level-${level}`);
        trHeader.classList.add('expandable-header');
        trHeader.dataset.targetId = thisSectionExpandableId;

        if (parentId) {
            trHeader.classList.add(`child-of-${parentId}`);
            trHeader.style.display = 'none';
        }

        const firstHeaderCell = document.createElement('td');
        const togglerSpan = document.createElement('span');
        togglerSpan.classList.add('toggler');
        togglerSpan.textContent = '▶';
        firstHeaderCell.appendChild(togglerSpan);
        const headerText = rowData.Header?.ColData?.[0]?.value ?? rowData.group ?? 'Group Header';
        firstHeaderCell.appendChild(document.createTextNode(headerText));
        trHeader.appendChild(firstHeaderCell);

        for (let i = 1; i < columnCount; i++) {
            trHeader.appendChild(document.createElement('td'));
        }

        return trHeader;
    }

    createDataRow(rowData, columnsDefinitions, level, parentId, sectionTotals, columnCount) {
        const trData = document.createElement('tr');
        trData.classList.add('data-row', `level-${level}`);

        if (parentId) {
            trData.classList.add(`child-of-${parentId}`);
            trData.style.display = 'none';
        }

        const dataCells = rowData.ColData;
        for (let i = 0; i < columnCount; i++) {
            const td = document.createElement('td');
            const cellValue = dataCells[i]?.value !== undefined && dataCells[i]?.value !== null ? dataCells[i].value : '';
            td.textContent = cellValue;

            const cellTextClean = String(cellValue).trim();

            if (i > 0 && parentId && sectionTotals.hasOwnProperty(parentId)) {
                const numericValue = parseFloat(cellTextClean.replace(/,/g, ''));
                if (!isNaN(numericValue)) {
                    sectionTotals[parentId][i] += numericValue;
                    if (numericValue < 0) {
                        td.classList.add('negative-value');
                    }
                }
            }
            if (i > 0) td.style.whiteSpace = 'nowrap';
            trData.appendChild(td);
        }
        while (trData.cells.length < columnCount) {
            trData.appendChild(document.createElement('td'));
        }

        return trData;
    }

    createSummaryRow(rowData, columnsDefinitions, level, parentId, grandParentId, sectionTotals, columnCount) {
        const trSummary = document.createElement('tr');
        trSummary.classList.add('summary-row', `level-${level}`);

        const firstSummaryValue = rowData.Summary.ColData?.[0]?.value;

        if (firstSummaryValue === 'Net Income') {
            trSummary.classList.add('net-income-row');
        } else if (firstSummaryValue === 'Gross Profit') {
            trSummary.classList.add('gross-profit-row');
        } else if (firstSummaryValue === 'Net Operating Income') {
            trSummary.classList.add('net-operating-income-row');
        } else if (firstSummaryValue === 'Net Other Income') {
            trSummary.classList.add('net-other-income-row');
        } else if (firstSummaryValue && typeof firstSummaryValue === 'string' && firstSummaryValue.toLowerCase().includes('total')) {
            trSummary.classList.add('generic-total-row');
        }

        const actualExpandableParentId = parentId;

        if (actualExpandableParentId) {
            trSummary.classList.add(`child-of-${actualExpandableParentId}`);
            if (level > 0) {
                trSummary.style.display = 'none';
            }
        }

        const idOfGroupBeingSummarized = parentId;
        const totalForSummarizedContent = (idOfGroupBeingSummarized && sectionTotals.hasOwnProperty(idOfGroupBeingSummarized))
            ? sectionTotals[idOfGroupBeingSummarized]
            : new Array(columnCount).fill(0);

        const useRawValueForSummary = firstSummaryValue === 'Gross Profit' ||
            firstSummaryValue === 'Net Operating Income' ||
            firstSummaryValue === 'Net Income' ||
            firstSummaryValue === 'Net Other Income' ||
            firstSummaryValue === 'Total Cost of Goods Sold' ||
            firstSummaryValue === 'Total Income';

        if (grandParentId && sectionTotals.hasOwnProperty(grandParentId) && !useRawValueForSummary) {
            for (let i = 1; i < columnCount; i++) {
                const totalValueToAddUp = totalForSummarizedContent[i];
                if (!isNaN(totalValueToAddUp)) {
                    sectionTotals[grandParentId][i] += totalValueToAddUp;
                }
            }
        }

        for (let i = 0; i < columnCount; i++) {
            const td = document.createElement('td');

            if (i === 0) {
                td.textContent = firstSummaryValue ?? '';
                td.style.textAlign = 'left';
            } else {
                let cellDisplayValue;
                let numericValueForCheck = 0;

                if (useRawValueForSummary) {
                    const cellData = rowData.Summary.ColData[i];
                    const rawValue = cellData?.value;
                    const rawNumericValue = parseFloat(String(rawValue).replace(/,/g, ''));
                    numericValueForCheck = rawNumericValue;

                    cellDisplayValue = rawValue !== undefined && rawValue !== null ? String(rawValue).trim() : '';
                    if (!isNaN(rawNumericValue)) {
                        const formattedAbsValue = Math.abs(rawNumericValue).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                        cellDisplayValue = rawNumericValue < 0 ? `(${formattedAbsValue})` : formattedAbsValue;
                    }
                } else {
                    const calculatedTotal = totalForSummarizedContent[i];
                    numericValueForCheck = calculatedTotal;

                    cellDisplayValue = calculatedTotal !== undefined && !isNaN(calculatedTotal)
                        ? calculatedTotal.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",")
                        : '';

                    if (calculatedTotal !== undefined && !isNaN(calculatedTotal) && calculatedTotal < 0) {
                        const formattedAbsValue = Math.abs(calculatedTotal).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                        cellDisplayValue = `(${formattedAbsValue})`;
                    }
                }

                td.textContent = cellDisplayValue;

                if (!isNaN(numericValueForCheck) && numericValueForCheck < 0) {
                    td.classList.add('negative-value');
                }

                if (i > 0) td.style.whiteSpace = 'nowrap';
            }
            trSummary.appendChild(td);
        }
        while (trSummary.cells.length < columnCount) {
            trSummary.appendChild(document.createElement('td'));
        }

        return trSummary;
    }

    attachExpandCollapseListeners(tbodyElement) {
        if (!tbodyElement) return;

        const expandableHeaders = tbodyElement.querySelectorAll('.expandable-header');
        expandableHeaders.forEach(headerRow => {
            headerRow.addEventListener('click', () => {
                const targetId = headerRow.dataset.targetId;
                if (!targetId) {
                    console.warn("Expandable header missing target ID:", headerRow);
                    return;
                }

                const isExpanded = headerRow.classList.toggle('expanded');

                const toggler = headerRow.querySelector('.toggler');
                if (toggler) {
                    toggler.textContent = isExpanded ? '▼' : '▶';
                }

                const children = tbodyElement.querySelectorAll(`.child-of-${targetId}`);

                children.forEach(childRow => {
                    if (isExpanded) {
                        childRow.style.display = 'table-row';
                    } else {
                        this.collapseAndResetDescendants(targetId, tbodyElement); // Use 'this'
                    }
                });
            });
            const toggler = headerRow.querySelector('.toggler');
            if (toggler) {
                toggler.textContent = '▶';
            }
        });
    }

    // Recursive Collapse Function
    collapseAndResetDescendants(parentId, tbodyElement) { // Now a class method
        if (!tbodyElement) return;

        const children = tbodyElement.querySelectorAll(`.child-of-${parentId}`);

        children.forEach(childRow => {
            childRow.style.display = 'none';

            if (childRow.classList.contains('expandable-header')) {
                const childId = childRow.dataset.targetId;
                if (childId) {
                    childRow.classList.remove('expanded');
                    const childToggler = childRow.querySelector('.toggler');
                    if (childToggler) {
                        childToggler.textContent = '▶'; // Collapsed state
                    }
                    this.collapseAndResetDescendants(childId, tbodyElement); // Use 'this' for recursive call
                } else {
                    console.warn("Expandable child header found without target ID:", childRow);
                }
            }
        });
    }

    // Adjusted displayError to work with the tableWrapper
    displayError(message) {
        if (this.tableWrapper) this.tableWrapper.innerHTML = ''; // Clear table area
        // Clear any previous error messages in reportBodyDiv before adding a new one
        const existingError = this.reportBodyDiv.querySelector('p[style*="color: red"]');
        if (existingError) {
            existingError.textContent = message;
        } else {
            const errorP = document.createElement('p');
            errorP.style.color = 'red';
            errorP.textContent = message;
            this.reportBodyDiv.appendChild(errorP);
        }

        this.reportBodyDiv.style.display = 'block'; // Ensure report body is visible to show the error
    }

    updatePlaceholderVisibility() {
        const hasContent = this.tableWrapper && this.tableWrapper.innerHTML.trim() !== '';
        const hasError = false;//this.reportBodyDiv.querySelector('p[style*="color: red"]');

        //if (hasContent || hasError) {

        //    $('.report-container').removeClass('loading').addClass('loaded');
        //} else {
        //    this.reportContainer.classList.add('empty');
        //}
        var columnCount = $("#report-table thead").eq(0).find("tr th").length;
        if (columnCount > 8) {
            const containerWidth = $(".report-container").width();
            if (columnCount < 12) {
                var newWidth = containerWidth * 1.2;
            }
            else if (columnCount < 15) {
                var newWidth = containerWidth * 1.5;
            }
            else if (columnCount < 18) {
                var newWidth = containerWidth * 1.7;
            }
            else if (columnCount < 21) {
                var newWidth = containerWidth * 1.9;
            }
           
            $("#report-table").css("width", `${newWidth}px`);
        }

        $('.report-container').removeClass('loading').addClass('loaded');
        this.#buildStickyHeader();
    }

	init() {
		this.#initEventHandlers();
    }
}

$(document).ready(() => {
	const view = new ProfitAndLossReportView();
	view.init();
});