class BookmarksComponent extends DomEventComponent {
	#httpService;
	#swal;
	#addForm;
	constructor() {
		super();
		this.#httpService = new httpService();
		this.#swal = new SwalUtil();
		this.#addForm = document.getElementById('modal-form-bookmark');
		this.boorkmarks = [];
	}
	#getuserBookmarks() {
		this.#httpService.get('/api/user-bookmarks')
			.then((bookmarks) => {
				this.boorkmarks = bookmarks;
				this.#renderBookmarks();
			});
	}
	#showModal() {
		debugger;
		const modal = `<div class="modal fade" id="modalAddBookmark" tabindex="-1" aria-hidden="true" data-bs-backdrop="static" tabindex="-1" role="dialog">
						<!--begin::Modal dialog-->
						<div class="modal-dialog modal-dialog-centered">
							<!--begin::Modal content-->
							<div class="modal-content">
								<!--begin::Form-->
								<form class="form" action="#" id="modal-form-bookmark">
									<!--begin::Modal header-->
									<div class="modal-header">
										<!--begin::Modal title-->
										<h2 class="fw-bolder" data-kt-calendar="title">Bookmark this page</h2>
										<!--end::Modal title-->
										<!--begin::Close-->
										<div class="btn btn-icon btn-sm btn-active-icon-primary data-bs-dismiss">
											<!--begin::Svg Icon | path: icons/duotune/arrows/arr061.svg-->
											<span class="svg-icon svg-icon-1">
												${doutune.x}
											</span>
											<!--end::Svg Icon-->
										</div>
										<!--end::Close-->
									</div>
									<!--end::Modal header-->
									<!--begin::Modal body-->
									<div class="modal-body pb-0">
										<!--begin::Input group-->
										<div class="fv-row mb-5">
											<!--begin::Label-->
											<label class="fs-6 fw-bold required mb-2">Title</label>
											<input type="text" name="title" class="form-control form-control-solid" placeholder="" id="txtBookmarkTitle" />
										</div>
										<div class="fv-row mb-5">
											<label class="fs-6 fw-bold mb-2">Short Description</label>
											<input type="text" name="title" class="form-control form-control-solid" placeholder="" id="txtBookmarkShortDescription" />
										</div>
					
									</div>
									<!--end::Modal body-->
									<!--begin::Modal footer-->
									<div class="modal-footer flex-center">
										<!--begin::Button-->
										<button type="reset" id="btn-cancel-add-bookmark" class="btn btn-light me-3">Cancel</button>
										<!--end::Button-->
										<!--begin::Button-->
										<button type="button" id="btn-submit-add-bookmark" class="btn btn-primary">
											<span class="indicator-label">
												<!--begin::Svg Icon | path: assets/media/icons/duotune/abstract/abs024.svg-->
												<span class="svg-icon svg-icon-muted svg-icon-2">
													<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none">
														<path opacity="0.3" d="M7.16973 20.95C6.26973 21.55 5.16972 20.75 5.46972 19.75L7.36973 14.05L2.46972 10.55C1.56972 9.95005 2.06973 8.55005 3.06973 8.55005H20.8697C21.9697 8.55005 22.3697 9.95005 21.4697 10.55L7.16973 20.95Z" fill="black" />
														<path d="M11.0697 2.75L7.46973 13.95L16.9697 20.85C17.8697 21.45 18.9697 20.65 18.6697 19.65L13.1697 2.75C12.7697 1.75 11.3697 1.75 11.0697 2.75Z" fill="black" />
													</svg>
												</span>
												<!--end::Svg Icon-->
												Add to Bookmarks</span>
											<span class="indicator-progress">
												Please wait...
												<span class="spinner-border spinner-border-sm align-middle ms-2"></span>
											</span>
										</button>
										<!--end::Button-->
									</div>
									<!--end::Modal footer-->
								</form>
								<!--end::Form-->
							</div>
						</div>
					</div>`;
		let title = $("[data-bookmark-title='true']").html() || $("[data-bookmark-input-title='True']").val() || "";

		const preTitle = $("[data-bookmark-pre-title='true']").html() || "";
		const combinedTitle = title ? `${preTitle} - ${title}` : preTitle;
		$("body").append(modal);
		$("#modalAddBookmark").modal('show');
		$("#txtBookmarkTitle").val(combinedTitle);
	}
	#saveBookmark() {
		const bookMark = {
			title: $("#txtBookmarkTitle").val(),
			description: $("#txtBookmarkShortDescription").val(),
			url: document.URL
		}
		const submitButton = document.getElementById('btn-submit-add-bookmark');
		submitButton.setAttribute('data-kt-indicator', 'on');
		submitButton.disabled = true;

		this.addForm = document.getElementById('modal-form-bookmark');
		this.httpService.post('/api/user-bookmarks', bookMark)
			.then(() => {
				this.swal.alert('Successfully added to bookmarks!', () => {
					this.addForm.reset();
					submitButton.removeAttribute('data-kt-indicator');
					submitButton.disabled = false;
					$("#modalAddBookmark").modal('hide');
				});
			});
	}
	#removeBookmark(id) {
		this.httpService.delete('/api/user-bookmarks/' + id)
			.then(() => {
				$(`div[data-bookmark-id='${id}']`).remove();
			});
	}
	#checkBookmarkButtonState() {
		const title = document.title;
		const bookmarkItem = this.boorkmarks.find((bookmark) => bookmark.title === title);
		if (bookmarkItem) {
			$("button.add-to-bookmark").attr("disabled", "disabled");
		}
	}
	#renderBookmarks() {
		const bookmarkListHtml = this.boorkmarks.map((bookmark) => {
			return `<div class="menu-item px-5"><a href="${bookmark.url}" class="menu-link px-5">${bookmark.title}</a></div>`
		}).join('');

		$("#bookmarkslist_items").html(bookmarkListHtml);
		this.#checkBookmarkButtonState();

	}
	onAddToBookmarks(b) {
		const bookMark = {
			title: document.title,
			url: document.URL
		}
		
		b.setAttribute('data-kt-indicator', 'on');
		b.disabled = true;

		this.#httpService.post('/api/user-bookmarks', bookMark)
			.then((data) => {
				debugger;
				this.boorkmarks.push(data);
				this.#renderBookmarks();
				this.#swal.alert('Successfully added to bookmarks!', () => {
					b.removeAttribute('data-kt-indicator');
					b.disabled = false;
				});
			});

	}
	init() {
		const instance = this;
		//$("html").off("click", ".add-to-bookmark").on("click", ".add-to-bookmark", () => {
		//	this.#showModal();
		//});
		//$("html").off("click", "#btn-cancel-add-bookmark,.data-bs-dismiss").on("click", "#btn-cancel-add-bookmark,.data-bs-dismiss", () => {
		//	$("#modalAddBookmark").modal('hide');
		//	$("#modalAddBookmark").remove();
		//});
		//$("html").off("click", "#btn-submit-add-bookmark").on("click", "#btn-submit-add-bookmark", () => {
		//	this.#saveBookmark();
		//});
		//$("html").off("click", ".remove-bookmark").on("click", ".remove-bookmark", function() {
		//	const bookmarkId = $(this).attr("data-bookmark-id");
		//	instance.#removeBookmark(bookmarkId);
		//});
		this.#getuserBookmarks();
    }
}

const bookmarkcomponent = new BookmarksComponent();
bookmarkcomponent.init();