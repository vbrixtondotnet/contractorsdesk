using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IDocuSignService : IBaseService
	{
		Task<string> SendEnvelope(ESignatureDocumentModel eSignatureDocumentModel);
	}
}
