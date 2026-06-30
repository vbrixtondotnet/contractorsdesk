using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces.@base;
using SignNow.Net.Interfaces;
using SignNow.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface ISignNowService
	{
		Task AuthenticateAsync();
		Task<string> UploadDocumentAsync(byte[] documentBytes, string fileName, ComplexTextTags? tags = null);
		Task<InviteResponse?> SendSignatureInvite(string documentId, string from, string to, string subject, string message);
	}
}
