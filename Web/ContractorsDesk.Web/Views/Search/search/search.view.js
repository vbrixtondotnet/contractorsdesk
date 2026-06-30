class SearchView extends DomEventComponent {
	constructor() {
		super();
		this.proposalId = document.URL.split('/').pop();
		this.results = [
			{
				"type": "proposal",
				"title": "Result 1",
				"description": "Description here",
				"badge": "Proposal"
			},
			{
				"type": "action-item",
				"title": "Result 2",
				"description": "Some other details here",
				"badge": "Action Item"
			},
			{
				"type": "proposal",
				"title": "Another Result",
				"description": "Additional content",
				"badge": "Proposal"
			},
			{
				"type": "proposal",
				"title": "More Results",
				"description": "Some more content",
				"badge": "Proposal"
			}
		];

/*		this.results = [];*/
		this.service = new SearchService();
	}

	bindEventHandlers() {
		/*Event handlers here*/
	}

	loadData() {
		const url = new URL(window.location.href);

		// Use URLSearchParams to extract the 'query' parameter
		const query = url.searchParams.get("query");

		console.log(query);

		this.renderResults(this.results, query);
		$(".card-custom").removeClass('loading').addClass('loaded');
	}

	renderResults(results, query) {
	const resultList = $('.result-list');
	const resultTitle = $('#result-title');

	resultList.empty(); // Clear existing results

	if (results.length === 0) {
		// If no results found, update the title and show a message
		resultTitle.text(`Search Results (0 found)`);
		resultList.append('<li class="list-group-item text-muted text-center">No results found.</li>');
		return;
	}

	// Update title with total results count
	resultTitle.text(`Search Results (${results.length} found)`);

	results.forEach((result) => {
		// Highlight the matching query in title and description
		const highlightedTitle = result.title.replace(new RegExp(query, 'gi'), (match) => `<span class="highlight-keyword">${match}</span>`);
		const highlightedDescription = result.description.replace(new RegExp(query, 'gi'), (match) => `<span class="highlight-keyword">${match}</span>`);

		// Create result item
		const resultItem = `
            <li class="list-group-item" data-type="${result.type}">
                <span class="result-content">${result.badge}: ${highlightedTitle} - ${highlightedDescription}</span>
                <span class="redirect-arrow" evt-click="redirectTo('${result.type}', '${result.title}')">
                    <i class="fas fa-arrow-right"></i>
                </span>
                <span class="badge search-badge">${result.badge}</span>
            </li>
        `;

		resultList.append(resultItem);
	});
}



	saveData() {

	}

	updateData() {

	}

	redirectTo(type, title) {
		alert("Redirecting to " + type + ": " + title);
	}

	highlightResults(query) {
		const listItems = document.querySelectorAll('.list-group-item .result-content');

		listItems.forEach(item => {
			let content = item.innerHTML;

			// Use a dynamic regular expression to match the query in a case-insensitive way
			const regex = new RegExp(`(${query})`, 'gi');

			// Replace matching query with highlighted text
			let highlightedContent = content.replace(regex, (match) => `<span class="highlight-keyword">${match}</span>`);
			item.innerHTML = highlightedContent;
		});
	}

	init() {
		this.loadData();
		this.bindEventHandlers();
    }
}

$(document).ready(() => {
	const view = new SearchView();
    view.init();
});