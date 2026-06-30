using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Tls;
using System.Text;

namespace ContractorsDesk.Services
{
	public class DocuSignService : BaseService, IDocuSignService
	{
		private readonly DocuSignClient docuSignClient;

		public DocuSignService(IConfiguration config)
        {
			configuration = config;

			var basePath = configuration["DocuSign:BaseUrl"];
			docuSignClient = new DocuSignClient(basePath);
		}

		public async Task<string> SendEnvelope(ESignatureDocumentModel eSignatureDocumentModel)
		{
			var token = GetAccessToken();

			if (!docuSignClient.Configuration.DefaultHeader.Select(name => "Authorization").Any())
			{
				docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + token);
			}

			var envelopesApi = new EnvelopesApi(docuSignClient);
			var accountId = configuration["DocuSign:AccountId"];


			var envelopeDefinition = new EnvelopeDefinition
			{
				EmailSubject = "CH Anderson Construction - Please review and sign this document",
				Documents = GenerateDocuSignDocuments(eSignatureDocumentModel.DocumentBytes, eSignatureDocumentModel.FileName, "pdf", "1"),
				Recipients = GenerateDocuSignRecipients(eSignatureDocumentModel.From, eSignatureDocumentModel.FromName, "1", "1"),
				Status = "sent"
			};

			var result = await envelopesApi.CreateEnvelopeAsync(accountId, envelopeDefinition);
			return result.EnvelopeId;
		}

		private Recipients GenerateDocuSignRecipients(string recipientEmail, string recepientName, string recipientId, string documentId)
		{
			var recipients = new Recipients();
			recipients.Signers = new List<Signer>();

			var signer = new Signer
			{
				Email = recipientEmail,
				Name = recepientName,
				RecipientId = recipientId, 
				Tabs = new Tabs
				{
					TextTabs = new List<Text>
					{
						new Text
						{
							DocumentId = documentId,
							PageNumber = "1",
							Bold = "true",
							FontSize = "Size11",
							Width = "350",
							XPosition = "88",
							YPosition = "255"
						},
						new Text
						{
							DocumentId = documentId,
							PageNumber = "4",
							Bold = "true",
							Required = "true",
							Width = "200",
							FontSize = "Size11",
							XPosition = "85",
							YPosition = "397"
						},
						new Text
						{
							DocumentId = documentId,
							PageNumber = "4",
							Bold = "true",
							Required = "false",
							Width = "200",
							FontSize = "Size11",
							XPosition = "85",
							YPosition = "467"
						}
					},
					SignHereTabs = new List<SignHere>
					{
						new SignHere
						{
							DocumentId = documentId,
							PageNumber = "4",
							XPosition = "93",
							YPosition = "362"
						},
						new SignHere
						{
							DocumentId = documentId,
							PageNumber = "4",
							Optional = "true",
							XPosition = "93",
							YPosition = "433"
						}
					}
				}
			};

			recipients.Signers.Add(signer);

			return recipients;
		}

		private List<Document> GenerateDocuSignDocuments(byte[] pdfFile, string documentName, string fileExtension, string documentId)
		{
			var documents = new List<Document>();

			if (!pdfFile.Any() || string.IsNullOrWhiteSpace(documentName) ||
				string.IsNullOrWhiteSpace(fileExtension) || string.IsNullOrWhiteSpace(documentId))
			{
				throw new InvalidOperationException($"{nameof(DocuSignService)} | GenerateDocuSignDocuments(): One or more required configuration values are missing.");
			}

			var document = new Document
			{
				DocumentBase64 = Convert.ToBase64String(pdfFile),
				Name = documentName,
				FileExtension = fileExtension,
				DocumentId = documentId
			};

			documents.Add(document);

			return documents;
		}

		private string GetAccessToken()
		{
			var clientId = configuration["DocuSign:ClientId"];
			var userId = configuration["DocuSign:UserId"];
			var authServer = configuration["DocuSign:AuthServer"];
			var privateKeyPath = configuration["DocuSign:PrivateKeyPath"];

			if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(userId) ||
				string.IsNullOrWhiteSpace(authServer) || string.IsNullOrWhiteSpace(privateKeyPath))
			{
				throw new InvalidOperationException($"{nameof(DocuSignService)} | GetAccessToken(): One or more required configuration values are missing.");
			}

			var privateKey = File.ReadAllText(privateKeyPath);
			var scopes = new List<string>
			{ 
				"signature impersonation"
			};

			var response = docuSignClient.RequestJWTUserToken(
				clientId,
				userId,
				authServer,
				Encoding.UTF8.GetBytes(privateKey),
				1,
				scopes
			);
			return response.access_token;
		}
	}
}
