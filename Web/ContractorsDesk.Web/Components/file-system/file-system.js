class FileSystem{
	constructor(selector) {
		this.data = [];
		this.selector = selector;
		this.fileUploads = [];
		this.storageName = 'client-documents';
		this.directory = '';
		this.folderId = 0;
		this.uploadInterval = null;
		this.clientId = Guid.empty;
		this.contextMenu = null;
		this.swal = new SwalUtil();
		this.selectedFiles = [];
		this.subContractors = [];
		this.subcontractor = null;
		this.httpService = new httpService();
		this.isSubConractorFolder = false;
	}

	setData(data) {
		this.data = data;
	}

	#findFolder(folders, targetId) {
		for (const folder of folders) {
			if (folder.id === targetId) {
				return folder; // Return the folder if the ID matches
			}
			if (folder.subFolders.length > 0) {
				const result = this.#findFolder(folder.subFolders, targetId);
				if (result) return result; // Return found folder
			}
		}
		return null; // Return null if not found
	}

	#getIcon(extension) {
		const icons = {
			pdf: svg.fileTypes.pdf,
			docx: svg.fileTypes.docx,
			html: svg.fileTypes.html,
			txt: svg.fileTypes.txt,
			jpg: svg.fileTypes.jpg,
			png: svg.fileTypes.png,
		};
		return icons[extension] || svg.fileTypes.txt;
	}

	#buildUploadButton(folderId) {
		// return empty if sub-contractors folder is selected
		return folderId == 'de9cbd8a-c1bf-4a46-9b5d-d9bb4c890810' ? '' : `<table class="table">
                    <tr class="upload-row">
                        <td colspan="5" class="text-center">
                            <input type="file" data-folder-id="${folderId}" style="display: none;">
                            <a href="javascript:" class="upload-file" data-folder-id="${folderId}" btn btn-outline-primary">
                                ${svg.fileTypes.upload} Upload File
                            </a>
                        </td>
                    </tr>
                </table>`;
	}

	#buildFileContainer(folderId) {
		return `<div class="file-list-container mt-0" id="uploadDiv" data-div-folder-id="${folderId}">
				             
				               <table class="table table-hover file-list-table">
								   <thead>
										  <tr>
											  <th></th>
											  <th>Name</th>
											  <th>Type</th>
											  <th>Date Created</th>
											  <th></th>
										  </tr>
									  </thead>
				                      <tbody id="file-list">
											<tr>
												<td colspan="5">
													<div class="preloader">
															<div class="loading-line"></div>
															<div class="loading-line"></div>
															<div class="loading-line"></div>
															<div class="loading-line"></div>
															<div class="loading-line"></div>
															<div class="loading-line"></div>
															<div class="loading-line"></div>
															<div class="loading-line"></div>
															<div class="loading-line"></div>
														</div>
												</td>
											</tr>
				                      </tbody>
				                  </table>
				              ${this.#buildUploadButton(folderId)}
				          </div>`;
	}

	#buildFolders(folders) {
		if (folders.length === 0) {
			return ""; // Return empty string when no folders
		}

		return `<div class="file-list-container mt-0" id="folderDiv">
       
         <table class="table table-hover file-list-table">
		  <thead>
                <tr>
                    <th></th>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Files</th>
                    <th>Subfolders</th>
                </tr>
            </thead>
                <tbody id="folder-list">
                    ${folders.map(folder => `
                        <tr class="folder-row">
                            <td class="file-icon">
                                <a href="javascript:" class="text-gray-800 text-hover-primary folder" data-filesystem-folder-id="${folder.id}">
                                    <div class="symbol symbol-35px">
                                        <svg width="" height="" viewBox="0 0 80 62" fill="none" xmlns="http://www.w3.org/2000/svg">
										  <path d="M43.124 8.04706H72.9412C76.851 8.08588 80.0002 11.2665 80 15.1765V54.1961C80.0002 58.1061 76.851 61.2869 72.9412 61.3255H7.05883C3.14902 61.2869 -0.000191243 58.1061 4.83581e-06 54.1961V7.13726C-0.00450497 3.22412 3.14589 0.0388235 7.05883 0H30.0861C32.0684 0 33.9577 0.840475 35.2851 2.31277L38.6677 6.06468C39.8054 7.32665 41.4249 8.04706 43.124 8.04706Z" fill="#f9d423"/>
										</svg>
                                    </div>
                                </a>
                            </td>
                            <td class="ps-0">
                                <a href="javascript:" class="text-gray-800 text-hover-primary folder" data-filesystem-folder-id="${folder.id}">
                                    ${folder.name}
                                </a>
                            </td>
                            <td>Folder</td>
                            <td>${folder.fileCount} File(s)</td>
                            <td>${folder.subFolders.length} Subfolder(s)</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
    </div>`;
	}

	#buildHtml() {
		return `<div class="card card-flush h-lg-100 bg-transparent">
					<div class="px-10 pb-2 pt-5">
						<div class="input-group input-group-sm mb-5">
							<span class="input-group-text">Search</span>
							<input type="text" class="form-control" evt-input="filterRows">
							<span class="input-group-text p-0">
								<button type="button" class="btn btn-sm bg-light text-inverse-light">
									CLEAR
								</button>
							</span>
						</div>
					</div>
					<div class="card-body file-system bg-transparent">
						<div class="row g-6 g-xl-9 mb-6 mb-xl-9 folders-container">
							${this.#buildFolders(this.data)}
						</div>
					</div>
				</div>`;
	}

	#getBreadcrumbItems(folders, targetId, path = []) {
		for (const folder of folders) {
			const newPath = [...path, folder]; // Append current folder name to the path

			if (folder.id === targetId) {
				return newPath;// Return breadcrumbs as a string
			}

			if (folder.subFolders.length > 0) {
				const result = this.#getBreadcrumbItems(folder.subFolders, targetId, newPath);
				if (result) return result; // Return found breadcrumbs
			}
		}
		return null; // Return null if not found
	}

	#createBreadcrumbs(folderId) {
		const folders = this.data;
		const breadcrumbs = this.#getBreadcrumbItems(folders, folderId);
		this.directory = breadcrumbs.map(item => item.name).join('/');

		const arrowIcon = `<span class="svg-icon svg-icon-2 svg-icon-primary mx-1">${doutune.arrowRight}</span>`;
		let links = breadcrumbs.map(item => `<a href="javascript:" data-filesystem-folder-id="${item.id}" class="breadcrumb-item">${item.name}</a>`);
		links.unshift(`<a href="javascript:" data-filesystem-folder-id="0" class="breadcrumb-item">Home</a>`);

		links = links.join(arrowIcon);
		return `<div class="d-flex flex-stack mt-0">
									<div class="badge badge-lg badge-light-primary">
										<div class="d-flex align-items-center flex-wrap">
										${links}
										</div>
									</div>
								</div>`;
	}

	#showAllFolders() {
		let html = this.#buildFolders(this.data);
		$(this.selector).find('.folders-container').html(html);
		this.#bindEventHandlers();
	}

	#openFolder(folderId) {
		if(folderId === 0) {
			this.#showAllFolders();
			return;
		}
		const folders = this.data;
		const currentFolder = this.#findFolder(folders, folderId);
		const subFolders = currentFolder.subFolders;
		const subContractorsFolderId = 'de9cbd8a-c1bf-4a46-9b5d-d9bb4c890810';
		this.isSubConractorFolder = folderId == subContractorsFolderId;
		this.isSubConractorSubFolder = currentFolder.parentId == subContractorsFolderId;

		let html = this.#createBreadcrumbs(folderId) + this.#buildFolders(subFolders);
		if (subFolders.length == 0) {
			html += this.#buildFileContainer(folderId);
		}

		$(this.selector).find('.folders-container').html(html);

		if (subFolders.length == 0) {
			this.#renderFiles(folderId);
		}

		this.#bindEventHandlers();
	}

	async deleteFile(fileId) {
		const removeFile = (folders) => {
			for (const folder of folders) {
				folder.files = folder.files.filter(file => file.id !== fileId);
				if (folder.subFolders.length) {
					removeFile(folder.subFolders);
				}
			}
		};

		removeFile(this.data);

		const response = await fetch(`/api/client-documents/${fileId}`, {
			method: "DELETE",
		});

		const result = await response.json();
		if (response.ok && result.success) {
			return;
		}
		else {
			console.log("Delete is not working");
		}
	}
		
	#bindEventHandlers() {
		const instance = this;
		$(document).off("click", ".client-file-delete").on("click", ".client-file-delete", function (d) {
			d.preventDefault();
			const deleteBtn = $(this);
			const fileId = deleteBtn.attr("data-id");
			const fileName = deleteBtn.attr("data-name");

			const originalIcon = deleteBtn.html();

			deleteBtn.html('<span class="spinner-border spinner-border-sm text-danger" role="status"></span>')
				.prop("disabled", true);

			instance.swal.confirm(`Are you sure you want to delete this file ${fileName}?`,
				() => {
					const fileRow = deleteBtn.closest("tr");

					fileRow.fadeOut(300, () => {
						deleteBtn.html(originalIcon).prop("disabled", false);
						fileRow.remove();
						instance.deleteFile(fileId);
					});
				},
				() => {
					deleteBtn.html(originalIcon).prop("disabled", false);
				}
			);
		});

		$("div.file-system a.folder, div.file-system a.breadcrumb-item").click((folder) => {
			const folderId = $(folder.currentTarget).data('filesystem-folder-id');
			this.#openFolder(folderId);
		});

		$("html").on("click", '.client-document-file', function (e) {
			e.preventDefault();
			debugger;
			const fileUrl = $(this).attr("href");
			const fileName = fileUrl.split('/').pop();
			const extension = $(this).attr("data-file-extension");//fileUrl.split('.').pop().toLowerCase();
			const clientDocumentModal = $("#kt_modal_client_documents");
			const modalHeader = $(clientDocumentModal).find(".modal-header");
			const modalHeaderNewTab = $(modalHeader).find("a");

			const modalBodyViewer = $(clientDocumentModal).find(".modal-body");
			const modalBodyViewerHeight = $('body').height() - 200;

			modalBodyViewer.empty();

			modalHeaderNewTab.attr("href", `${fileUrl}`);
			modalHeaderNewTab.html(`<h2>${fileName} <i class="bi bi-box-arrow-up-right" width="16" height="16"></i></h2>`);

			if (["pdf", "txt", "doc", "docx, odt", "pages", "rtf"].includes(extension)) {
				modalBodyViewer.html(`<iframe src="${fileUrl}" style="width:100%; height:${modalBodyViewerHeight}px;" frameborder="0"></iframe>`);
			} else {
				modalBodyViewer.html(`<p>File preview not supported. <a href="${fileUrl}" target="_blank">View here</a></p>`);
			}

			// For future use.......................
			//......................................
			// else if (["jpg", "jpeg", "png", "gif"].includes(extension)) {
			//	modalBodyViewer.html(`<img src="${fileUrl}" style="max-width:100%; height:${modalBodyViewerHeight}px;" alt="Image Preview">`);
			//} else if (["mp4", "webm", "wav"].includes(extension)) {
			//	modalBodyViewer.html(`<video src="${fileUrl}" controls style="width:100%; height:${modalBodyViewerHeight}px;"></video>`);
			//}

			$(clientDocumentModal).modal("show");
		});

		$("div.file-system a.upload-file").click((e) => {
			this.folderId = $(e.currentTarget).data("folder-id");
            //const folder = this.data.folders.find(f => f.id === this.folderId || f.subFolders.some(sf => sf.id === this.folderId));
			//const folderName = folder.name.toUpperCase();
			$("#fileUploadModal").modal("show");
			//if (folderName == "SUB-CONTRACTORS") {
			//	this.#initSubcontractorsSearch();
			//}
			//else {
			//	$("#fileUploadModal").modal("show");
			//}
		});

		$("#chooseFileBtn").off("click").on("click", () => {
			$("#fileInput").click();
		});

		$("#dropArea")
			.off("dragover dragleave drop")
			.on("dragover", (e) => {
				e.preventDefault();
				$("#dropArea").addClass("dragging");
			})
			.on("dragleave", () => {
				$("#dropArea").removeClass("dragging");
			})
			.on("drop", (e) => {
				e.preventDefault();
				$("#dropArea").removeClass("dragging");

				const files = Array.from(e.originalEvent.dataTransfer.files);

				files.forEach(f => {
					if (!instance.selectedFiles.some(existing => existing.name === f.name && existing.size === f.size)) {
						instance.selectedFiles.push(f);
						instance.fileUploads.push(f);
					}
				});

				instance.renderFileList();
			});

		$("#uploadBtn").off("click").on("click", async () => {
			await this.#upload();
		});

		$("#fileUploadModal").off("hidden.bs.modal").on("hidden.bs.modal", () => {
			instance.resetUploadModal();
		});

		$("#fileInput").off("change").on("change", (e) => {
			const files = Array.from(e.target.files);

			files.forEach(f => {
				if (!instance.selectedFiles.some(existing => existing.name === f.name && existing.size === f.size)) {
					instance.selectedFiles.push(f);
					instance.fileUploads.push(f);
				}
			});

			instance.renderFileList();
		});

		$("#fileListBody").on("click", ".btn-remove-file", function () {
			const fileName = $(this).data("filename");

			const fileIndex = instance.selectedFiles.findIndex(file => file.name === fileName);

			if (fileIndex !== -1) {
				instance.selectedFiles.splice(fileIndex, 1);
				instance.fileUploads.splice(fileIndex, 1);
				instance.renderFileList();
			}
		});

		$("#slcSubContractors").select2({
			dropdownPosition: 'below',
			dropdownParent: $('#mdlSubContractorSearch'),
			ajax: {
				url: '/api/sub-contractors',
				data: params => ({ key: params.term }),
				processResults: response => {
					instance.subContractors = response.data;
					
					return {
						results: response.data.map(item => ({
							id: item.id,
							text: item.name
						}))
					};
				}
			}
		});

		$("#slcSubContractors").on('select2:select', function (e) {
			instance.subcontractor = instance.subContractors.find(item => item.id === e.params.data.id);
			$(`#txtSearchSubContractorName`).val(`${instance.subcontractor.name}`);
			$(`#txtSearchSubContractorCompany`).val(instance.subcontractor.company);
			$(`#txtSearchSubContractorAddress`).val(instance.subcontractor.address);
			$(`#txtSearchSubContractorPhone`).val(instance.subcontractor.phone);
			$(`#txtSearchSubContractorEmail`).val(instance.subcontractor.email);
			$(`#txtSearchSubContractorCity`).val(instance.subcontractor.city);
			$(`#txtSearchSubContractorState`).val(instance.subcontractor.state).trigger('change');
			$(`#txtSearchSubContractorLicenseNo`).val(instance.subcontractor.licenseNo);
			$(`#txtSearchSubContractorLicenseExpiry`).val(instance.subcontractor.licenseExp);
		});

		$("#btnSearchSubContractorNext").on("click", () => {

			var form = document.getElementById('frmSubcontractorSearch');
			const isValid = form.reportValidity();

			if (isValid) {
				$("#mdlSubContractorSearch").modal("hide");
				$("#fileUploadModal").modal("show");
			} 

		});
		$(".subcontractor-search-input").on("input", function () {
			const val = $(this).val();
			const model = $(this).attr("data-model");
			debugger;
			if (instance.subcontractor != null) {
				instance.subcontractor[model] = val;
			}
			else {
				instance.subcontractor = {};
				instance.subcontractor.id = Guid.empty;
				instance.subcontractor[model] = val;
			}
		});
	}

	resetUploadModal() {
		this.selectedFiles = [];
		this.fileUploads = [];
		this.folderId = 0;

		$("#fileInput").val("");
		$("#fileUploadStatus").text("No file selected").hide();
		$("#dropArea").removeClass("dragging");
		$("#uploadBtn").prop("disabled", false).text("UPLOAD");
		$("#fileListBody").empty();
		$("#fileListContainer").hide();
		$('[data-bs-toggle="tooltip"]').tooltip('dispose');
	}

	renderFileList() {
		const $fileListBody = $("#fileListBody");
		$fileListBody.empty();

		if (this.selectedFiles.length === 0) {
			$("#fileListContainer").hide();
			return;
		}

		this.selectedFiles.forEach((file) => {
			const row = `
			<tr class="align-middle">
				<td class="py-2">${file.name}</td>

				<td class="text-center py-2">
					<i class="bi bi-check-circle-fill text-success fs-5"></i>
				</td>

				<td class="text-center py-2">
					<a href="javascript:;"
						class="btn btn-icon btn-sm btn-remove-file"
						data-filename="${file.name}"
						data-bs-toggle="tooltip"
						data-bs-placement="top"
						title="Delete File">
						<span class="svg-icon svg-icon-danger svg-icon-2">${doutune.trash}</span>
					</a>
				</td>
			</tr>
		`;
			$fileListBody.append(row);
		});

		$("#fileListContainer").show();
	}

	animateProgress(progressElement) {
		if (progressElement.data("isAnimating")) return;
		progressElement.data("isAnimating", true);

		let start = 0;
		let end = 100;
		let duration = 3000;
		let interval = duration / end;

		let counter = setInterval(() => {
			start++;
			progressElement.text(start + "%");

			if (start >= 100) {
				clearInterval(counter);
				progressElement.removeData("isAnimating");
				progressElement.addClass("text-success").text("90%");
			}
		}, interval);

		progressElement.data("interval", counter);
	}

	completeProgress(progressElement, fileUploaded) {
		// Stop any existing progress animation
		let existingInterval = progressElement.data("interval");
		if (existingInterval) {
			clearInterval(existingInterval);
			progressElement.removeData("interval");
		}

		progressElement.addClass("text-success").text("100%");

		setTimeout(() => {
			let deleteBtn = $(`
				<a href="javascript:" class="btn btn-sm btn-icon client-file-delete"
					data-bs-toggle="tooltip"
					data-bs-placement="top"
					title="Delete File" 
					data-url="${fileUploaded.url}"
					data-id="${fileUploaded.id}"
					data-name="${fileUploaded.fileName}"
				>
					<span class="svg-icon svg-icon-danger svg-icon-2">${doutune.trash}</span>
				</a>
			`);

			progressElement.replaceWith(deleteBtn);

		}, 300);
	}

	#renderFiles(folderId) {
		const fileListContainer = $("div.file-list-container[data-div-folder-id='" + folderId + "'] #file-list");
		const getFileNameAndVersion = (filename, version, fileExtension) => {
			const result = filename.replace(/\.[^/.]+$/, "");
			return `${result}.v${version}.${fileExtension}`;
		}
		const currentFolder = this.#findFolder(this.data, folderId);
		const files = currentFolder.files;
		let emptyMessage = this.isSubConractorFolder ? `No sub-contractors have been assigned to this project yet. To add a sub-contractor, click the Sub-Contractors tab and select one from the list.` : `This folder is empty.`;
		let fileList = `<tr class="file-row"><td colspan="5" class="text-center">${emptyMessage}</td> </tr>`;

		if (files.length > 0) {
			fileList = files.map(file => `
				                              <tr class="file-row">
				                                  <td class="file-icon">${this.#getIcon(file.fileExtension)}</td>
				                                  <td>
				                                      <a href="${file.url}" data-file-extension=${file.fileExtension} class="client-document-file text-gray-800 text-hover-primary" target="_blank">
				                                         ${getFileNameAndVersion(file.fileName, file.version, file.fileExtension)}
				                                      </a>
				                                  </td>
				                                  <td>${file.fileExtension.toUpperCase()}</td>
				                                  <td>${DateUtils.formatDateTime(file.dateCreated)}</td>
				                                  <td>
				                                      <a href="javascript:" class="btn btn-sm btn-icon client-file-delete"
				                                          data-bs-toggle="tooltip"
				                                          data-bs-placement="top"
				                                          title="Delete File"
														  data-file-extension=${file.fileExtension}
				                                          data-url=${file.url}
				                                          data-id=${file.id}
				                                          data-name=${file.fileName}
				                                      >
				                                          <span class="svg-icon svg-icon-danger svg-icon-2">${doutune.trash}</span>
				                                      </a>
				                                  </td>
				                              </tr>
				                          `).join('');
		}
		fileListContainer.html(fileList);
	}

	async #upload() {
		if (!this.fileUploads || this.fileUploads.length == 0) {
			this.swal.error("No file selected for upload!");
			return;
		}

		const files = this.fileUploads;
		const formattedDate = DateUtils.formatDateTime(new Date());
		const folderId = this.folderId;

		$("#fileUploadModal").modal("hide");

		for (const file of files) {
			const extension = file.name.split('.').pop();

			const progressRow = $(`
				<tr class="file-row uploading">
					<td class="file-icon">${this.#getIcon(extension)}</td>
					<td><span class="text-muted uploading-text">Uploading File. Please Wait... </span></td>
					<td>${extension.toUpperCase()}</td>
					<td>${formattedDate}</td>
					<td><span class="mini-loader"></span></td>
				</tr>
			`);

			$(".file-list-table tbody").prepend(progressRow);

			progressRow[0].scrollIntoView({ behavior: "smooth", block: "center" });

			//const progressElement = progressRow.find(".upload-progress");
			//this.animateProgress(progressElement);

			const formData = new FormData();

			formData.append("file", file);
			formData.append("storageName", this.storageName);
			formData.append("directory", this.directory);
			formData.append("folderId", this.isSubConractorFolder || this.isSubConractorSubFolder ? 'DE9CBD8A-C1BF-4A46-9B5D-D9BB4C890810' : folderId);
			formData.append("clientId", this.clientId);
			if (this.isSubConractorSubFolder) {
				formData.append("subcontractorId", this.folderId);
			}
			
			const response = await fetch("/api/upload/file", {
				method: "POST",
				body: formData,
			});

			const result = await response.json();

			if (response.ok) {
				const resultData = result.data;
				this.#addFileToFolder(resultData, folderId);
				this.#renderFiles(folderId);
				
			} else {
				let uploadingRow = $(".file-row.uploading");
				uploadingRow.find("span.uploading-text").html("File Upload Failed!").removeClass("text-muted").addClass('text-danger');
				uploadingRow.find("span.mini-loader").remove();
			}
		}
	}

	#addFileToFolder(newFile, targetFolderId) {			
		const currentFolder = this.#findFolder(this.data, targetFolderId);
		currentFolder.files.push(newFile);
	}

	render() {
		const html = this.#buildHtml();
		$(this.selector).html(html);

		this.#bindEventHandlers();
	}
}

