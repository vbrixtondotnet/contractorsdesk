using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IClientDocumentService : IBaseService
	{
		Task<string> SendCostPlusDocument(Guid proposalId, ClientContractDetails clientContractDetails);
		Task<List<SysFolderDto>> GetClientFoldersAsync(Guid projectId);
        Task<SysFolderDto> GetClientDocumentsAsync(Guid projectId, Guid folderId);
        Task<int?> GetDocumentLatestVersionByFileNameAsync(Guid projectId, string fileName);
        Task SaveClientDocument(ClientDocumentDto clientDocumentDto, SysFolders sysFolder);
		Task<ClientDocumentDto> SaveClientDocument(ClientDocumentDto clientDocumentDto, Guid folderId);
		Task SaveClientDocument(Guid clientId, string fileExtension, string baseFileName, string url, int version, SysFolders sysFolder);
		Task<bool> SendClientDocument(Guid clientId, string clientName, string attachmentUrl, SysFolders sysFolder, string fileContextName, EmailPayloadModel emailModel);
		Task<bool> DeleteClientDocument(Guid id);
	}
}
