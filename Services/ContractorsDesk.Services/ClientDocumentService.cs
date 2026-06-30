using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.Core.Models;
using SignNow.Net.Model.ComplexTags;
using SignNow.Net.Model;
using ContractorsDesk.Core.ApiPayloadModels;
using DocuSign.eSign.Client;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using DocuSign.eSign.Model;
using System;
using SignNow.Net.Model.Requests.GetFolderQuery;
using Pipelines.Sockets.Unofficial.Arenas;
using Microsoft.Extensions.Primitives;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class ClientDocumentService : BaseService, IClientDocumentService
	{
		private readonly ISignNowService signNowService;
		private readonly IProposalService proposalService;
		private readonly IPDFService pDFService;
		private readonly ISysFolderService sysFolderService;
		private readonly IAzureStorageService azureStorageService;
		private readonly IPostMarkEmailService emailService;
		public ClientDocumentService(
			ISignNowService signNowService,
			IProposalService proposalService,
			IPDFService pDFService,
			ISysFolderService sysFolderService,
			IAzureStorageService azureStorageService,
			IPostMarkEmailService emailService,
			ClientDbContext clientDbContext,
			MasterDbContext masterDbContext,
			IMapper mapper)
			: base(mapper, clientDbContext, masterDbContext)
		{
			this.signNowService = signNowService;
			this.proposalService = proposalService;
			this.pDFService = pDFService;
			this.sysFolderService = sysFolderService;
			this.azureStorageService = azureStorageService;
			this.emailService = emailService;
		}

        #region Public
        public async Task<string> SendCostPlusDocument(Guid proposalId, ClientContractDetails clientContractDetails)
		{
			var proposal = await proposalService.GetProposalAsync(proposalId);

			if (proposal == null)
			{
				throw new Exception("Proposal not found");
			}

			var projectName = proposal.Project.Name;
			var fileName = $"{projectName.Replace(" ", "-")}-Cost-Plus.pdf";

			clientContractDetails.ProjectCost = proposal.Total;

			// Get PDF bytes (Cost Plus Document) from PDF service
			var pdfBytes = pDFService.GetCostPlusPDF(clientContractDetails);

			var eSignatureDocumentModel = new ESignatureDocumentModel
			{
				DocumentBytes = pdfBytes,
				FileName = fileName,
				CompanyName = "CH Anderson Construction",
				From = clientContractDetails.ContractorEmailAddress,
				FromName = clientContractDetails.Contractor,
				To = clientContractDetails.ClientEmail
			};

			var complexTags = new ComplexTextTags();
			complexTags.Properties.Add(new TextTag
			{
				TagName = "ownername",
				Role = "Signer 1",
				Required = true,
				Label = "Your Name Here",
				Width = 200,
				Height = 20
			});
			complexTags.Properties.Add(new TextTag
			{
				TagName = "owner1_fullname",
				Role = "Signer 1",
				Label = "Your Name Here",
				Required = true,
				Width = 200,
				Height = 20
			});
			complexTags.Properties.Add(new TextTag
			{
				TagName = "owner2_fullname",
				Role = "Signer 1",
				Label = "Your Partner's Name Here",
				Required = false,
				Width = 200,
				Height = 20
			});
			complexTags.Properties.Add(new SignatureTag
			{
				TagName = "owner1_signature",
				Role = "Signer 1",
				Required = true,
				Width = 200,
				Height = 20
			});
			complexTags.Properties.Add(new SignatureTag
			{
				TagName = "owner2_signature",
				Role = "Signer 1",
				Required = false,
				Width = 200,
				Height = 20
			});

			var documentId = await signNowService.UploadDocumentAsync(eSignatureDocumentModel.DocumentBytes, eSignatureDocumentModel.FileName, complexTags);

			var subject = $"{eSignatureDocumentModel.CompanyName}: Please review and sign this document";
			var message = $"{eSignatureDocumentModel.From} invited you to sign the document {eSignatureDocumentModel.FileName}";

			var response = await signNowService.SendSignatureInvite(documentId, eSignatureDocumentModel.From, eSignatureDocumentModel.To, subject, message);
			
			return documentId;
		}
        public async Task<List<SysFolderDto>> GetClientFoldersAsync(Guid projectId)
        {
            var retval = new List<SysFolderDto>();
            var sysFolders = await sysFolderService.GetSysFoldersAsync();
            var parentFolders = sysFolders.Where(x => x.ParentId == null).ToList();

            foreach (var folder in parentFolders)
            {
                var sysFolder = new SysFolderDto { Name = folder.Name, Id = folder.Id };

                if (sysFolder.Name.ToUpper() == "SUB-CONTRACTORS")
                {
					await AddSubcontractorFolders(projectId, sysFolder);
                }
                else
                {
                    await AddSubFoldersAndFiles(projectId, sysFolder);
                }

                retval.Add(sysFolder);
            }

            return retval;
        }
        public async Task<SysFolderDto> GetClientDocumentsAsync(Guid projectId, Guid folderId)
		{
			var sysFolder = await ClientDbContext.SysFolders.FirstOrDefaultAsync(f => f.Id == folderId) 
				?? throw new Exception("Folder not found");

            var folder = new SysFolderDto { Name = sysFolder.Name, Id = sysFolder.Id };
            await AddSubFoldersAndFiles(projectId, folder);

			return folder;
        }
		public async Task<int?> GetDocumentLatestVersionByFileNameAsync(Guid projectId, string fileName)
		{
			var clientDocument = await ClientDbContext.ClientDocuments
				.Where(cd => cd.ClientId == projectId && cd.FileName == fileName)
				.OrderByDescending(cd => cd.Version)
				.FirstOrDefaultAsync();

			if(clientDocument != null)
			{
				return clientDocument.Version;
			}

            return null;

        }
        public async Task SaveClientDocument(ClientDocumentDto clientDocumentDto, SysFolders sysFolder)
		{
			var folderName = sysFolder.GetStringValue();
			var dbSysFolder = await ClientDbContext.SysFolders.FirstOrDefaultAsync(f=> f.Name == folderName);

			if(dbSysFolder == null)
            {
                throw new Exception("Folder not found");
            }

            await this.SaveClientDocument(clientDocumentDto, dbSysFolder.Id);
        }
		public async Task<ClientDocumentDto> SaveClientDocument(ClientDocumentDto clientDocumentDto, Guid folderId)
		{
			clientDocumentDto.FolderId = folderId;
			var clientDocument = mapper.Map<ClientDocument>(clientDocumentDto);
			ClientDbContext.ClientDocuments.Add(clientDocument);

			await ClientDbContext.SaveChangesAsync();

			clientDocument = await ClientDbContext.ClientDocuments
				.AsNoTracking()
				.FirstOrDefaultAsync(cd => cd.Id == clientDocument.Id);

			return mapper.Map<ClientDocumentDto>(clientDocument);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientId">ID of the client</param>
		/// <param name="clientName">Name of the client</param>
		/// <param name="documentUrl">URL of the attachment file</param>
		/// <param name="sysFolder">Folder name where file will be stored</param>
		/// <param name="fileContextName">Example: revised-estimate</param>
		/// <param name="emailModel">Payload of the Email Details</param>
		/// <returns></returns>
		public async Task<bool> SendClientDocument(Guid clientId, string clientName, string attachmentUrl, Core.Enums.SysFolders sysFolder, string fileContextName, EmailPayloadModel emailModel)
		{
			//check latest version of the document

			var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
			var latestDocumentVersion = await this.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
			var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;
			var currentDate = DateTime.UtcNow.ToString("MM-dd-yyyy");
			var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.{currentDate}.pdf";

			// Upload the document to Azure Storage
			var uploadResult = await this.UploadClientDocument(attachmentUrl, sysFolder, fileName, clientName);

			var emailSent = await this.EmailClientDocument(fileName, uploadResult.Url, emailModel);

			if (emailSent)
			{
				//store to database 
				await this.SaveClientDocument(clientId, "pdf", baseFileName, uploadResult.Url, version, sysFolder);
			}

			return emailSent;
		}

		public async Task<bool> DeleteClientDocument(Guid id)
		{
			var clientDocument = await ClientDbContext.ClientDocuments
				.Where(ai => ai.Id == id).FirstOrDefaultAsync();
			if (clientDocument != null)
			{
				await this.azureStorageService.DeleteFileAsync(clientDocument.Url);

                ClientDbContext.ClientDocuments.Remove(clientDocument);
				ClientDbContext.SaveChanges();
				return true;
			}

			return false;
		}

		#endregion
		#region Private

		private SysFolders? GetSysFolder(string folderName)
		{
			var sysfolders = Enum.GetValues(typeof(SysFolders));

			foreach (SysFolders folder in sysfolders)
			{
				var fieldInfo = typeof(SysFolders).GetField(folder.ToString());
				var attributes = (StringValueAttribute[])fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false);

				if (attributes.Length > 0 && attributes[0].Value.ToUpper().Trim() == folderName.ToUpper().Trim())
				{
					return folder;
				}
			}

			return null;

		}
		private async Task GetAiActionItemsByDateDocuments(Guid projectId, Guid folderId, List<ClientDocumentDto> clientDocumentsDtos)
		{
			var actionItems = ClientDbContext.ActionItems.Where(ai=> ai.Source == (int)ActionItemSource.AI && ai.ProjectId == projectId)
				.Include(ai => ai.ActionItemsSupervisors)
				.Include(ai => ai.AcceptedByNavigation)
				.AsNoTracking()
				.ToList();

			foreach(var actionItem in actionItems)
			{
				clientDocumentsDtos.Add(new ClientDocumentDto
				{
					ClientId = projectId,
					FileName = $"{actionItem.Title}",
					Url = $"/pdf/ai-action-item/{actionItem.Id}",
					DateCreated = actionItem.DateCreated,
					FolderId = folderId,
					FileExtension = "link",
					Version = 1,
					Id = Guid.NewGuid()
				});
			}

			
		}
        private async Task<List<ClientDocumentDto>> GetClientDocumentsByFolder(Guid projectId, Guid folderId)
		{
			var clientDocuments = await ClientDbContext.ClientDocuments
				.Include(cd => cd.Folder)
				.Where(cd => cd.ClientId == projectId && cd.FolderId == folderId)
				.AsNoTracking()
				.ToListAsync();

			return mapper.Map<List<ClientDocumentDto>>(clientDocuments);
        }
		private async Task GetClientEmails(Guid projectId, Guid folderId, List<ClientDocumentDto> clientDocumentsDtos)
		{
			var clientEmails = await ClientDbContext.Emails.Where(e => e.ProjectId == projectId)
					.AsNoTracking()
					.ToListAsync();


			foreach (var email in clientEmails)
			{
				var clientDocument = new ClientDocumentDto
				{
					Id = email.Id,
					ClientId = projectId,
					FileName = $"{email.Subject}.pdf",
					Url = $"/pdf/client-email/{email.Id}",
					DateCreated = email.DateCreated,
					FolderId = folderId,
					FileExtension = "pdf",
					Version = 1
				};

				clientDocumentsDtos.Add(clientDocument);
			}
		}
		// Create a memory stream from the HTML bytes
		private Stream ConvertEmailDetailsToHtmlStream(EmailModel emailModel)
		{
			// Parse the HTML content to keep the formatting
			var htmlDoc = new HtmlDocument();
			var htmlContentBuilder = new StringBuilder();
			//htmlContentBuilder.Append($"From: {emailModel.ReplyTo}<br/>");
			htmlContentBuilder.Append($"Date: {DateTime.UtcNow.ToShortDateString()}<br/>");
			htmlContentBuilder.Append($"Subject: {emailModel.Subject.Trim()}<br/>");
			htmlContentBuilder.Append($"To: {emailModel.To.Trim()}<br/>");
			//htmlContentBuilder.Append($"Reply-To: {emailModel.ReplyTo?.Trim()}<br/>");
			htmlContentBuilder.Append($"<p>{emailModel.Body}</p>");

			htmlDoc.LoadHtml(htmlContentBuilder.ToString());
			// Convert the HTML document to a string
			string outerHtml = htmlDoc.DocumentNode.OuterHtml;

			// Convert the HTML string to bytes using UTF-8 encoding
			byte[] htmlBytes = Encoding.UTF8.GetBytes(outerHtml);

			// Create a memory stream from the HTML bytes
			return new MemoryStream(htmlBytes);
		}
		private async Task AddSubcontractorFolders(Guid projectId, SysFolderDto sysFolder)
        {
            var projectSubContractors = await ClientDbContext.ProjectSubContractors
				.Include(psc => psc.SubContractor)
				.Where(psc => psc.ProjectId == projectId)
				.AsNoTracking()
				.Select(p => p.SubContractor)
				.ToListAsync();

			foreach (var subcontractor in projectSubContractors)
			{
				var clientSubFolder = new SysFolderDto { Name = subcontractor.Name, Id = subcontractor.Id, ParentId = sysFolder.Id };

				var documents = await ClientDbContext.ClientDocuments
					.Where(cd => cd.SubcontractorId == subcontractor.Id)
					.ToListAsync();

				clientSubFolder.Files = mapper.Map<List<ClientDocumentDto>>(documents);

                sysFolder.SubFolders.Add(clientSubFolder);
			}
		}
		private async Task<(string Url, Stream fileStream)> UploadClientDocument(string attachmentUrl, SysFolders sysfolder, string fileName, string clientName)
		{
			//generate pdf stream
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);

			// Upload the document to Azure Storage
			var uploadedDocumentUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysfolder.GetStringValue()}");

			return (uploadedDocumentUrl, pdfStream);

		}
		private async Task<bool> EmailClientDocument(string fileName, Stream fileStream, EmailPayloadModel emailModel)
		{
			// Send Email to client
			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileStream = fileStream
			};

			emailModel.Attachments = new List<AttachmentModel> { attachment };
			return await this.emailService.SendEmailAsync(emailModel);
		}
		private async Task<bool> EmailClientDocument(string fileName, string fileUrl, EmailPayloadModel emailModel)
		{
			//if(emailModel.AttachmentLinks == null) emailModel.AttachmentLinks = new List<AttachmentLinkModel>();

			//emailModel.AttachmentLinks.Add(new AttachmentLinkModel
			//{
			//	FileName = fileName,
			//	FileUrl = fileUrl
			//});

			return await this.emailService.SendEmailAsync(emailModel);
		}
		public async Task SaveClientDocument(Guid clientId, string fileExtension, string baseFileName, string url, int version, SysFolders sysFolder)
		{
			// Store the uploaded document to the database
			var clientDocument = new ClientDocumentDto
			{
				ClientId = clientId,
				FileExtension = fileExtension,
				FileName = baseFileName,
				Url = url,
				Version = version
			};

			await this.SaveClientDocument(clientDocument, sysFolder);
		}
 
        private async Task AddSubFoldersAndFiles(Guid projectId, SysFolderDto sysFolder)
        {
            var subFolders = await ClientDbContext.SysFolders
                .Where(x => x.ParentId == sysFolder.Id)
                .ToListAsync();

			if (subFolders != null && subFolders.Any())
			{
                foreach (var subFolder in subFolders)
                {
                    var clientSubFolder = new SysFolderDto { Name = subFolder.Name, Id = subFolder.Id };

                    // add files here
                    var clientDocuments = await GetClientDocumentsByFolder(projectId, subFolder.Id);
                    clientSubFolder.Files = clientDocuments;

                    // Recursively add subfolders
                    await AddSubFoldersAndFiles(projectId, clientSubFolder);
					sysFolder.SubFolders.Add(clientSubFolder);
                }
            }
			else
			{
                // add files here
                var clientDocuments = await GetClientDocumentsByFolder(projectId, sysFolder.Id);
                sysFolder.Files = clientDocuments;
            }
           
        }

        public static string GetBlobNameFromUrl(string blobUrl)
        {
            var uri = new Uri(blobUrl);
            var fullPath = uri.AbsolutePath.TrimStart('/');

            // Remove the container name (first segment)
            var firstSlashIndex = fullPath.IndexOf('/');
            if (firstSlashIndex < 0)
                throw new ArgumentException("Invalid blob URL format.");

            var relativePath = fullPath.Substring(firstSlashIndex + 1);

            return Uri.UnescapeDataString(relativePath);

        }

        #endregion

    }
}
