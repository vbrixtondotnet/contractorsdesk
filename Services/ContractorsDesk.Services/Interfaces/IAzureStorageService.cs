using ContractorsDesk.Services.Interfaces.@base;
using Microsoft.AspNetCore.Http;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IAzureStorageService : IBaseService
	{
		Task<string> UploadFile(IFormFile file, string containerName, string fileName, string directoryName = "");
        Task<string> UploadFileFromStream(Stream fileStream, string containerName, string fileName, string directoryName = "");
		Task<string> UploadFileFromBase64(string base64Str, string containerName, string fileName, string directoryName = "");
		Task<bool> DeleteFileAsync(string blobUrl);

    }
}
