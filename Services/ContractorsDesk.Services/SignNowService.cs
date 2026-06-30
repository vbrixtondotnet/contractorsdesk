using ContractorsDesk.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using SignNow.Net.Interfaces;
using SignNow.Net.Service;
using SignNow.Net;
using SignNow.Net.Model;
using SignNow.Net.Model.FieldContents;
using SignNow.Net.Model.Requests;
using Org.BouncyCastle.Tls;
using SignNow.Net.Model.ComplexTags;
using DocuSign.eSign.Model;
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;

namespace ContractorsDesk.Services
{
	public class SignNowService : ISignNowService
	{
		private ISignNowContext _signNowContext;
		private readonly string clientId;
		private readonly string clientSecret;
		private readonly string username;
		private readonly string password;
		private readonly string apiUrl;
		private Token accessToken;
		private Uri baseApiUri;
		private DocumentService documentService;
		public SignNowService(IConfiguration configuration)
		{
			clientId = configuration["SignNow:ClientId"];
			clientSecret = configuration["SignNow:ClientSecret"];
			username = configuration["SignNow:Username"]; // SignNow account email
			password = configuration["SignNow:Password"]; // SignNow account password
			apiUrl = configuration["SignNow:ApiUrl"]; // SignNow account password
			this.baseApiUri = new Uri(apiUrl);
		}

		public async Task AuthenticateAsync()
		{
			// Authenticate and create SignNow context
			var auth = new OAuth2Service(this.baseApiUri, clientId, clientSecret);
			this.accessToken = auth.GetTokenAsync(username, password, new Scope()).Result;
			documentService = new DocumentService(this.baseApiUri, accessToken);
			_signNowContext = new SignNowContext(this.accessToken);
		}
		public async Task<string> UploadDocumentAsync(byte[] documentBytes, string fileName, ComplexTextTags? tags = null)
		{
			try
			{
				await this.AuthenticateAsync();
				using var stream = new MemoryStream(documentBytes);

				var document = await documentService.UploadDocumentWithFieldExtractAsync(stream, fileName, tags);
				return document.Id;
			}
			catch(Exception)
			{
				throw;
			}
		}
		public async Task<InviteResponse?> SendSignatureInvite(string documentId, string from, string to, string subject, string message)
		{
			try
			{
				var signNowDoc = await _signNowContext.Documents.GetDocumentAsync(documentId).ConfigureAwait(false);

				var invite = new RoleBasedInvite(signNowDoc)
				{
					Message = message, //$"{from} invited you to sign the document {signNowDoc.Name}",
					Subject = subject
				};

				var signer = new SignerOptions(to.Trim(), invite.DocumentRoles().First())
				{
					ExpirationDays = 15,
					RemindAfterDays = 7,
				};

				invite.AddCcRecipients(from);
				invite.AddRoleBasedInvite(signer);

				// Creating Invite request
				return await _signNowContext.Invites
					.CreateInviteAsync(signNowDoc.Id, invite)
					.ConfigureAwait(false);
			}
			catch (Exception)
			{
				throw;
			}
		}

	}
}
